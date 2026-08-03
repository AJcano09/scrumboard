import {Component, OnInit, ViewChild, viewChild} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {Table, TableLazyLoadEvent, TableModule} from "primeng/table";
import {ButtonModule} from "primeng/button";
import {InputTextModule} from "primeng/inputtext";
import {InputTextareaModule} from "primeng/inputtextarea";
import {DialogModule} from "primeng/dialog";
import {CalendarModule} from "primeng/calendar";
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {ToastModule} from "primeng/toast";
import {DropdownModule} from "primeng/dropdown";
import {ConfirmationService, MessageService} from "primeng/api";
import {Project, ProjectStatus} from "../models/project.model";
import {debounceTime, Subject} from "rxjs";
import {ProjectService} from "../services/project.service";


@Component(
  {
    selector: 'app-projects-list',
    standalone: true,
    imports:[
      CommonModule,ReactiveFormsModule,TableModule,
      ButtonModule,DialogModule,InputTextModule,
      InputTextareaModule,CalendarModule,DropdownModule,
      ConfirmDialogModule,ToastModule
    ],
    providers:[
      ConfirmationService,MessageService
    ],
    templateUrl:'projects.list.component.html'
  }
)
export class ProjectsListComponent implements OnInit {
  constructor(private fb:FormBuilder,
              private projectService:ProjectService,
              private confirmationService:ConfirmationService,
              private messageService:MessageService) {
  }
  @ViewChild('dt') table!: Table;

  projects: Project[]=[];
  totalRecords =0;
  loading =false;
  searchTerm ='';

  dialogVisible = false;
  editingProject:Project | null =null;

  statusOptions = [
    {
      label:'Pendiente',value: ProjectStatus.Pending
    },
    {
      label:'En Progreso',value: ProjectStatus.InProgress
    },
    {
      label:'Completada',value: ProjectStatus.Completed
    },
    {
      label:'Cancelada',value: ProjectStatus.Cancelled
    },
  ];

  form = this.fb.group({
    name:['',[Validators.required,Validators.maxLength(150)]],
    description: ['',[Validators.required,Validators.maxLength(500)]],
    startDate: [new Date(),Validators.required],
    endDate: [new Date(),Validators.required],
    status:[ProjectStatus.Pending,Validators.required]
  })
  private searchSubject = new Subject<string>()


    ngOnInit(): void {
      this.searchSubject.pipe(debounceTime(400)).subscribe(
        term=>{
          this.searchTerm = term;
          this.table.first =0;
          this.reload();

        }
      );
    }

    onSearchInput(value:string) :void{
    this.searchSubject.next(value);
    }

    loadProjects(event:TableLazyLoadEvent):void
    {

      this.loading =true;
      const pageSize = event.rows ?? 10 ;
      const pageNumber = Math.floor((event.first ?? 0) / pageSize) +1 ;
      this.projectService.getPaged(this.searchTerm,pageNumber,pageSize).subscribe({
        next:(res)=>{
          this.projects = res.items;
          this.totalRecords = res.totalCount;
          this.loading =false ;
        },
        error:(res)=>{
          this.loading =false;
          this.messageService.add({
            severity:'error',
            summary: 'Error',
            detail:'No fue posible cargar los proyectos .'
          })
        }
      });
    }

    reload():void{
         this.loadProjects({first: this.table.first ?? 0,rows: this.table.rows});
    }

    openCreateDialog():void{
    this.editingProject =null;
      this.form.reset({
        name: '',
        description: '',
        startDate: new Date(),
        endDate: new Date(),
        status: ProjectStatus.Pending
      });
      this.dialogVisible=true;
    }

    openEditDialog(project:Project):void{
    this.editingProject = project;
      this.form.reset({
        name: project.name,
        description: project.description,
        startDate: new Date(project.startDate),
        endDate: new Date(project.endDate),
        status: project.status
      });
      this.dialogVisible =true ;
    }

    save():void{
    if( this.form.invalid){
        this.form.markAllAsTouched();
        return;
      }

      const value = this.form.getRawValue();
      const request = {
        name: value.name!,
        description: value.description ?? '',
        startDate: (value.startDate as Date).toISOString(),
        endDate: (value.endDate as Date).toISOString(),
        status: value.status!
      };

      const request$ = this.editingProject
        ? this.projectService.update(this.editingProject.id, request)
        : this.projectService.create(request);

      request$.subscribe({
        next: () => {
          this.dialogVisible = false;
          this.messageService.add({ severity: 'success', summary: 'Guardado', detail: 'Proyecto guardado correctamente.' });
          this.reload();
        },
        error: (err) => {
          const detail = err?.error?.message ?? 'No se pudo guardar el proyecto.';
          this.messageService.add({ severity: 'error', summary: 'Error', detail });
        }
      });
    }

  confirmDelete(project: Project): void {
    this.confirmationService.confirm({
      message: `¿Eliminar el proyecto "${project.name}"? Esta acción no se puede deshacer.`,
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.projectService.delete(project.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Eliminado', detail: 'Proyecto eliminado.' });
            this.reload();
          },
          error: () => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar el proyecto.' });
          }
        });
      }
    });
  }
}
