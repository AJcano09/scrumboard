import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {ButtonModule} from "primeng/button";
import {InputTextModule} from "primeng/inputtext";
import {DragDropModule} from "primeng/dragdrop";
import {CardModule} from "primeng/card";
import {DialogModule} from "primeng/dialog";
import {InputTextareaModule} from "primeng/inputtextarea";
import {DropdownModule} from "primeng/dropdown";
import {TagModule} from "primeng/tag";
import {ToastModule} from "primeng/toast";
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {BoardService} from "../services/board.service";
import {ActivatedRoute} from "@angular/router";
import {ConfirmationService, MessageService} from "primeng/api";
import {Board, BoardTask, TaskPriority, User} from "../models/board.model";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem} from "@angular/cdk/drag-drop";

@Component({
  selector: "app-board",
  standalone: true,
  imports:
  [
    CommonModule, ReactiveFormsModule, DragDropModule,
    ButtonModule, CardModule, DialogModule, InputTextModule,
    InputTextareaModule, DropdownModule, TagModule, ToastModule, ConfirmDialogModule
,CdkDropList,CdkDrag
  ],
  templateUrl: "./board.component.html",
})
export class BoardComponent implements OnInit {

  constructor(private boardService : BoardService,
              private route:ActivatedRoute,
              private fb:FormBuilder,
              private confirmationService:ConfirmationService,
              private messageService:MessageService) {
  }

  projectId!: string;
  board: Board = { projectId: '', columns: [] };
  users: User[] = [];
  loading = false;

  dialogVisible = false;
  editingTask: BoardTask | null = null;
  targetColumnIdForCreate = '';

  priorityOptions: { label: string; value: TaskPriority }[] = [
    { label: 'Baja', value: 'Baja' }, { label: 'Media', value: 'Media' }, { label: 'Alta', value: 'Alta' }
  ];

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.maxLength(1000)],
    priority: ['Media' as TaskPriority, Validators.required],
    responsibleId: ['', Validators.required]
  })
    ngOnInit(): void {
      this.projectId = this.route.snapshot.paramMap.get('id') ?? '';
      this.loadUsers();
      this.loadBoard();
    }


  loadBoard(): void {
    this.loading = true;
    this.boardService.getBoard(this.projectId).subscribe({
      next: (board) => { this.board = board; this.loading = false; },
      error: () => { this.loading = false; this.showError(null, 'No se pudo cargar el tablero.'); }
    });
  }

  loadUsers(): void {
    this.boardService.getUsers(1, 100).subscribe({
      next: (response) => (this.users = response.items),
      error: () => this.showError(null, 'No se pudieron cargar los usuarios.')
    });
  }

  connectedLists(): string[] {
    return this.board.columns.map(c => c.id);
  }

  openCreateDialog(columnId: string): void {
    this.editingTask = null;
    this.targetColumnIdForCreate = columnId;
    this.form.reset({ title: '', description: '', priority: 'Media', responsibleId: this.users[0]?.id ?? '' });
    this.dialogVisible = true;
  }

  openEditDialog(task: BoardTask): void {
    this.editingTask = task;
    this.form.reset({
      title: task.title, description: task.description, priority: task.priority as TaskPriority, responsibleId: task.responsibleId
    });
    this.dialogVisible = true;
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();

    const request$ = this.editingTask
      ? this.boardService.updateTask(this.editingTask.id, {
        title: value.title!, description: value.description ?? '', priority: value.priority!, responsibleId: value.responsibleId!
      })
      : this.boardService.createTask({
        title: value.title!, description: value.description ?? '', priority: value.priority!,
        responsibleId: value.responsibleId!, columnId: this.targetColumnIdForCreate
      });

    request$.subscribe({
      next: () => { this.dialogVisible = false; this.loadBoard(); },
      error: (err) => this.showError(err, 'No se pudo guardar la tarea.')
    });
  }

  confirmDeleteTask(task: BoardTask): void {
    this.confirmationService.confirm({
      message: `¿Eliminar la tarea "${task.title}"?`,
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.boardService.deleteTask(task.id).subscribe({
          next: () => this.loadBoard(),
          error: (err) => this.showError(err, 'No se pudo eliminar la tarea.')
        });
      }
    });
  }

  // --- Drag & drop: actualización optimista + reversión en error ---
  onTaskDrop(event: CdkDragDrop<BoardTask[]>): void {
    const targetColumn = this.board.columns.find(c => c.tasks === event.container.data)!;
    const task = event.previousContainer.data[event.previousIndex];

    // snapshot para poder revertir si el servidor responde con error
    const snapshot = this.board.columns.map(c => ({ id: c.id, tasks: [...c.tasks] }));

    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
    }

    this.boardService.moveTask(task.id, targetColumn.id, event.currentIndex).subscribe({
      next: (updatedTask) => {
        // sincroniza order/columnId reales que calculó el backend
        task.order = updatedTask.order;
        task.columnId = updatedTask.columnId;
      },
      error: (err) => {
        // reversión visible: restauramos el estado previo al drop
        this.board.columns.forEach(c => {
          const snap = snapshot.find(s => s.id === c.id);
          if (snap) c.tasks = snap.tasks;
        });
        this.showError(err, 'No se pudo mover la tarea. Se revirtió el cambio.');
      }
    });
  }

  priorityColor(priority: string): 'success' | 'warning' | 'danger' | 'info' {
    switch (priority) {
      case 'Alta': return 'danger';
      case 'Media': return 'warning';
      default: return 'info';
    }
  }

  private showError(err: any, fallback: string): void {
    const detail = err?.error?.message ?? fallback;
    this.messageService.add({ severity: 'error', summary: 'Error', detail });
  }

}
