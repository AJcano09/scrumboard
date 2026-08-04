import {Component, inject, Input, OnDestroy, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {DragDropModule} from "primeng/dragdrop";
import {CdkDrag, CdkDropList, CdkDragDrop, moveItemInArray, transferArrayItem} from "@angular/cdk/drag-drop";
import {ReportService} from "../../reports/services/report.service";
import {finalize, Subscription} from "rxjs";
import {BoardHubService} from "../../../core/realtime/board-hub.service";
import {BoardColumn, BoardTask} from "../../board/models/board.model";
import {BoardService} from "../../board/services/board.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-project-board',
  standalone: true,
  imports: [CommonModule, DragDropModule, CdkDropList, CdkDrag],
  templateUrl: './project-board.component.html'
})
export class ProjectBoardComponent implements  OnInit, OnDestroy{

  @Input() projectId!: string;
  private reportService = inject(ReportService);
  private boardHubService = inject(BoardHubService);
  private subs = new Subscription();

  isDownloadingPdf = false;
  isDownloadingExcel = false;

  columns: BoardColumn[] = [];
  isLoading:boolean = false;

  constructor(private boardService:BoardService,
              private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('id')!;

    if (!this.projectId) {
      console.error('El ID del proyecto no está presente en la ruta.');
      return;
    }

    this.loadBoardData();

    // Conectarse a la sala de SignalR exclusiva para este proyecto
    this.boardHubService.joinBoard(this.projectId).then(() => {
      console.log(`Conectado al hub en tiempo real para el proyecto: ${this.projectId}`);
    }).catch(err => console.error('Error conectando a SignalR', err));

    // Escuchar y actualizar dinámicamente: Tarea Creada
    this.subs.add(
      this.boardHubService.taskCreated$.subscribe((newTask: BoardTask) => {
        if (!newTask) return;
        const column = this.columns.find(c => c.id === newTask.columnId);
        if (column) {
          if (!column.tasks.some(t => t.id === newTask.id)) {
            column.tasks.push(newTask);
            this.sortColumnTasks(column);
          }
        }
      })
    );

    //  Escuchar y actualizar dinámicamente: Tarea Movida
    this.subs.add(
      this.boardHubService.taskMoved$.subscribe((payload: { taskId: string, sourceColumnId: string, targetColumnId: string, newOrder: number }) => {
        if (!payload) return;

        let movedTask: BoardTask | undefined;

        for (const col of this.columns) {
          const taskIndex = col.tasks.findIndex(t => t.id === payload.taskId);
          if (taskIndex !== -1) {
            movedTask = col.tasks.splice(taskIndex, 1)[0];
            break;
          }
        }

        if (movedTask) {
          movedTask.columnId = payload.targetColumnId;
          movedTask.order = payload.newOrder;
          const targetCol = this.columns.find(c => c.id === payload.targetColumnId);
          if (targetCol) {
            targetCol.tasks.push(movedTask);
            this.sortColumnTasks(targetCol);
          }
        }
      })
    );

   ///  updated task
    this.subs.add(
      this.boardHubService.taskUpdated$.subscribe((updatedTask: BoardTask) => {
        if (!updatedTask) return;
        for (const col of this.columns) {
          const taskIndex = col.tasks.findIndex(t => t.id === updatedTask.id);
          if (taskIndex !== -1) {
            // Si la tarea cambió de columna inesperadamente por actualización directa
            if (col.id !== updatedTask.columnId) {
              col.tasks.splice(taskIndex, 1);
              const targetCol = this.columns.find(c => c.id === updatedTask.columnId);
              if (targetCol) {
                targetCol.tasks.push(updatedTask);
                this.sortColumnTasks(targetCol);
              }
            } else {
              col.tasks[taskIndex] = updatedTask;
              this.sortColumnTasks(col);
            }
            break;
          }
        }
      })
    );

    //delete task
    this.subs.add(
      this.boardHubService.taskDeleted$.subscribe((taskId: string) => {
        if (!taskId) return;
        for (const col of this.columns) {
          const taskIndex = col.tasks.findIndex(t => t.id === taskId);
          if (taskIndex !== -1) {
            col.tasks.splice(taskIndex, 1);
            break;
          }
        }
      })
    );

    //  Columna Creada
    this.subs.add(
      this.boardHubService.columnCreated$.subscribe((newColumn: BoardColumn) => {
        if (!newColumn) return;
        if (!this.columns.some(c => c.id === newColumn.id)) {
          if (!newColumn.tasks) {
            newColumn.tasks = [];
          }
          this.columns.push(newColumn);
          this.sortColumns();
        }
      })
    );

    //  Columna Actualizada
    this.subs.add(
      this.boardHubService.columnUpdated$.subscribe((updatedColumn: BoardColumn) => {
        if (!updatedColumn) return;
        const index = this.columns.findIndex(c => c.id === updatedColumn.id);
        if (index !== -1) {
          // Mantener las tareas existentes si el evento de columna no las trae pobladas
          const existingTasks = this.columns[index].tasks;
          this.columns[index] = { ...updatedColumn, tasks: updatedColumn.tasks || existingTasks };
          this.sortColumns();
        }
      })
    );

    //  Columna Eliminada
    this.subs.add(
      this.boardHubService.columnDeleted$.subscribe((columnId: string) => {
        if (!columnId) return;
        this.columns = this.columns.filter(c => c.id !== columnId);
      })
    );

    //  Columna Movida / Reordenada
    this.subs.add(
      this.boardHubService.columnMoved$.subscribe((reorderedColumns: BoardColumn[]) => {
        if (!reorderedColumns || !Array.isArray(reorderedColumns)) return;
        // Si el backend envía el array completo o un nuevo orden, sincronizamos
        this.columns = reorderedColumns.map(rc => {
          const existing = this.columns.find(c => c.id === rc.id);
          return {
            ...rc,
            tasks: existing ? existing.tasks : (rc.tasks || [])
          };
        });
        this.sortColumns();
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    this.boardHubService.leaveBoard();
  }
  private loadBoardData(): void {
    this.isLoading = true;
    this.boardService.getBoard(this.projectId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => {
          this.columns = data.columns || [];
        },
        error: (err) => {
          console.error('Error al cargar la estructura del tablero:', err);
        }
      });
  }

  private sortColumnTasks(column: BoardColumn): void {
    if (column && column.tasks) {
      column.tasks.sort((a, b) => a.order - b.order);
    }
  }

  private sortColumns(): void {
    // Ordena las columnas si tu modelo cuenta con una propiedad 'order' o 'position'
     this.columns.sort((a, b) => (a.order || 0) - (b.order || 0));
  }

  connectedLists(): string[] {
    return this.columns.map(c => c.id);
  }

  // --- Drag & drop: actualización optimista + reversión en error ---
  onTaskDrop(event: CdkDragDrop<BoardTask[]>): void {
    const targetColumn = this.columns.find(c => c.tasks === event.container.data)!;
    const task = event.previousContainer.data[event.previousIndex];

    // Snapshot para poder revertir si el servidor responde con error
    const snapshot = this.columns.map(c => ({ id: c.id, tasks: [...c.tasks] }));

    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
    }

    this.boardService.moveTask(task.id, targetColumn.id, event.currentIndex).subscribe({
      next: (updatedTask) => {
        // Sincroniza order/columnId reales que calculó el backend
        task.order = updatedTask.order;
        task.columnId = updatedTask.columnId;
      },
      error: (err) => {
        // Reversión visible: restauramos el estado previo al drop
        this.columns.forEach(c => {
          const snap = snapshot.find(s => s.id === c.id);
          if (snap) c.tasks = snap.tasks;
        });
        console.error('No se pudo mover la tarea. Se revirtió el cambio.', err);
      }
    });
  }

  downloadReport(format: 'PDF' | 'EXCEL') {
    if (format === 'PDF') this.isDownloadingPdf = true;
    if (format === 'EXCEL') this.isDownloadingExcel = true;

    this.reportService.downloadProjectReport(this.projectId, format)
      .pipe(
        finalize(() => {
          this.isDownloadingPdf = false;
          this.isDownloadingExcel = false;
        })
      )
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const anchor = document.createElement('a');
          anchor.href = url;
          const extension = format === 'PDF' ? 'pdf' : 'xlsx';
          anchor.download = `Reporte_Proyecto_${this.projectId}.${extension}`;
          document.body.appendChild(anchor);
          anchor.click();
          document.body.removeChild(anchor);
          window.URL.revokeObjectURL(url);
        },
        error: (err) => {
          console.error(`Error al descargar el reporte en ${format}`, err);
        }
      });
  }
}
