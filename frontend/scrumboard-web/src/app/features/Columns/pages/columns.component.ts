import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {ActivatedRoute, RouterModule} from "@angular/router";
import {FormsModule} from "@angular/forms";
import {DragDropModule} from "primeng/dragdrop";
import {ButtonModule} from "primeng/button";
import {InputTextModule} from "primeng/inputtext";
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {ToastModule} from "primeng/toast";
import {ConfirmationService, MessageService} from "primeng/api";
import {Column} from "../models/column.model";
import {ColumnService} from "../services/column.service";
import {
  CdkDrag,
  CdkDragDrop,
  CdkDragHandle,
  CdkDropList,
  CdkDropListGroup,
  moveItemInArray
} from "@angular/cdk/drag-drop";

@Component({
  selector: 'app-columns',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, DragDropModule, ButtonModule,
    InputTextModule, ConfirmDialogModule, ToastModule,
  CdkDropList,CdkDrag,CdkDropListGroup,CdkDragHandle],
  providers: [ConfirmationService, MessageService],
  templateUrl: './columns.component.html'
})
export class ColumnsComponent implements OnInit {
  projectId!: string;
  columns: Column[] = [];
  loading = false;
  newColumnName = '';
  editingId: string | null = null;
  editingName = '';

  constructor(
    private route: ActivatedRoute,
    private columnService: ColumnService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('id')!;
    this.load();
  }

  load(): void {
    this.loading = true;
    this.columnService.getByProject(this.projectId).subscribe({
      next: (columns) => {
        this.columns = columns.sort((a, b) => a.order - b.order);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.showError(null, 'No se pudieron cargar las columnas.');
      }
    });
  }

  add(): void {
    const name = this.newColumnName.trim();
    if (!name) return;
    this.columnService.create(this.projectId, name).subscribe({
      next: () => {
        this.newColumnName = '';
        this.load();
      },
      error: (err) => this.showError(err, 'No se pudo crear la columna.')
    });
  }

  startEdit(column: Column): void {
    this.editingId = column.id;
    this.editingName = column.name;
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editingName = '';
  }

  saveEdit(column: Column): void {
    const name = this.editingName.trim();
    if (!name) return;
    this.columnService.update(this.projectId, column.id, name).subscribe({
      next: () => {
        this.cancelEdit();
        this.load();
      },
      error: (err) => this.showError(err, 'No se pudo actualizar la columna.')
    });
  }

  confirmDelete(column: Column): void {
    this.confirmationService.confirm({
      message: `¿Eliminar la columna "${column.name}"?`,
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.columnService.delete(this.projectId, column.id).subscribe({
          next: () => {
            this.messageService.add({severity: 'success', summary: 'Eliminada', detail: 'Columna eliminada.'});
            this.load();
          },
          error: (err) => this.showError(err, 'No se pudo eliminar la columna.')
        });
      }
    });
  }

  isReordering: boolean = false;
  onDrop(event: CdkDragDrop<Column[]>): void {
    // 1. Evitar procesamiento si se suelta en la misma posición o ya hay una petición en curso
    if (event.previousIndex === event.currentIndex || this.isReordering) {
      return;
    }

    // Guardar copia de respaldo para revertir de forma limpia en caso de error
    const previousColumns = [...this.columns];

    // Actualización optimista de la interfaz
    moveItemInArray(this.columns, event.previousIndex, event.currentIndex);

    const orderedColumnIds = this.columns.map(c => c.id);
    this.isReordering = true;

    this.columnService.reorder(this.projectId, orderedColumnIds).subscribe({
      next: (columns) => {
        this.columns = columns.sort((a, b) => a.order - b.order);
        this.isReordering = false;
      },
      error: (err) => {
        this.showError(err, 'No se pudo reordenar. Se revierte el cambio.');
        this.columns = previousColumns; // Revierte al estado previo inmediato
        this.isReordering = false;
      }
    });
  }

  private showError(err: any, fallback: string): void {
    const detail = err?.error?.message ?? fallback;
    this.messageService.add({severity: 'error', summary: 'Error', detail});
  }
}
