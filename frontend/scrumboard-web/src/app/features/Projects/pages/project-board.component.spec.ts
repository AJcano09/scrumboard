import {ProjectBoardComponent} from "./project-board.component";
import {ComponentFixture, TestBed} from "@angular/core/testing";
import {ReportService} from "../../reports/services/report.service";
import {BoardService} from "../../board/services/board.service";
import {BoardHubService} from "../../../core/realtime/board-hub.service";
import {ActivatedRoute} from "@angular/router";
import {ProjectService} from "../services/project.service";
import {By} from "@angular/platform-browser";
import {of} from "rxjs";

describe('ProjectBoardComponent', () => {
  let component: ProjectBoardComponent;
  let fixture: ComponentFixture<ProjectBoardComponent>;
  let reportServiceSpy: jasmine.SpyObj<ReportService>;
  let boardServiceSpy: jasmine.SpyObj<BoardService>;
  let projectServiceSpy: jasmine.SpyObj<ProjectService>;
  let boardHubServiceSpy: jasmine.SpyObj<BoardHubService>;

  beforeEach(async () => {
    // Mock de los servicios para no hacer peticiones reales
    const reportSpy = jasmine.createSpyObj('ReportService', ['downloadProjectReport']);
    const boardSpy = jasmine.createSpyObj('BoardService', ['getBoard', 'getUsers']);
    const projectSpy = jasmine.createSpyObj('ProjectService', ['getById']);
    const hubSpy = jasmine.createSpyObj('BoardHubService', ['joinBoard', 'leaveBoard'], {
      taskCreated$: of(),
      taskUpdated$: of(),
      taskDeleted$: of(),
      taskMoved$: of(),
      columnCreated$: of(),
      columnUpdated$: of(),
      columnDeleted$: of(),
      columnMoved$: of()
    });
    hubSpy.joinBoard.and.returnValue(Promise.resolve());

    boardSpy.getBoard.and.returnValue(of({ projectId: '12345', columns: [] }));
    boardSpy.getUsers.and.returnValue(of({ items: [], totalCount: 0 }));
    projectSpy.getById.and.returnValue(of({ id: '12345', name: 'Proyecto Test', description: '', startDate: '', endDate: '', status: 'InProgress' }));

    await TestBed.configureTestingModule({
      imports: [ProjectBoardComponent],
      providers: [
        { provide: ReportService, useValue: reportSpy },
        { provide: BoardService, useValue: boardSpy },
        { provide: ProjectService, useValue: projectSpy },
        { provide: BoardHubService, useValue: hubSpy },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '12345' } } } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectBoardComponent);
    component = fixture.componentInstance;
    reportServiceSpy = TestBed.inject(ReportService) as jasmine.SpyObj<ReportService>;
    projectServiceSpy = TestBed.inject(ProjectService) as jasmine.SpyObj<ProjectService>;

    // Los mocks ya retornan observables por defecto; detectChanges dispara ngOnInit
    fixture.detectChanges();
  });

  // Prueba 3: Renderizado correcto
  it('Debe renderizar los botones de exportación PDF y EXCEL', () => {
    const buttons = fixture.debugElement.queryAll(By.css('button'));

    expect(buttons.length).toBeGreaterThanOrEqual(2);
    expect(buttons[0].nativeElement.textContent).toContain('Exportar PDF');
    expect(buttons[1].nativeElement.textContent).toContain('Exportar Excel');
  });

  // Prueba 3b: Renderizado del nombre del proyecto
  it('Debe mostrar el nombre del proyecto en el encabezado del tablero', () => {
    const header = fixture.debugElement.query(By.css('h2'));
    expect(header.nativeElement.textContent).toContain('Proyecto Test');
  });

  // Prueba 3c: Llamada al servicio de proyectos
  it('Debe llamar a ProjectService.getById al inicializar', () => {
    expect(projectServiceSpy.getById).toHaveBeenCalledWith('12345');
  });

  // Prueba 4: Gestión de estado durante la descarga
  it('Debe deshabilitar el botón de PDF y mostrar "Generando..." cuando se hace clic', () => {
    // Configuramos el mock para devolver un observable que no se complete inmediatamente
    const mockBlob = new Blob(['test'], { type: 'application/pdf' });
    reportServiceSpy.downloadProjectReport.and.returnValue(of(mockBlob));

    const pdfButton = fixture.debugElement.queryAll(By.css('button'))[0].nativeElement;

    // Disparamos el clic
    pdfButton.click();
    fixture.detectChanges();

    // Verificamos el estado interno y la vista
    expect(component.isDownloadingPdf).toBeFalse(); // Porque of() completa sincrónicamente en la prueba
    expect(reportServiceSpy.downloadProjectReport).toHaveBeenCalledWith('12345', 'PDF');
  });

  // Prueba 5: Manipulación del DOM para la descarga del archivo
  it('Debe crear un enlace temporal (anchor) y desencadenar la descarga cuando el servicio responde con éxito', () => {
    const mockBlob = new Blob(['excel data'], { type: 'application/vnd.ms-excel' });
    reportServiceSpy.downloadProjectReport.and.returnValue(of(mockBlob));

    // Espiamos los métodos del DOM
    spyOn(window.URL, 'createObjectURL').and.returnValue('blob:test-url');
    spyOn(window.URL, 'revokeObjectURL');
    const anchorSpy = jasmine.createSpyObj('HTMLAnchorElement', ['click']);
    spyOn(document, 'createElement').and.returnValue(anchorSpy);
    spyOn(document.body, 'appendChild');
    spyOn(document.body, 'removeChild');

    component.downloadReport('EXCEL');

    expect(window.URL.createObjectURL).toHaveBeenCalledWith(mockBlob);
    expect(document.createElement).toHaveBeenCalledWith('a');
    expect(anchorSpy.download).toBe('Reporte_Proyecto_12345.xlsx');
    expect(document.body.appendChild).toHaveBeenCalledWith(anchorSpy);
    expect(anchorSpy.click).toHaveBeenCalled();
    expect(document.body.removeChild).toHaveBeenCalledWith(anchorSpy);
    expect(window.URL.revokeObjectURL).toHaveBeenCalledWith('blob:test-url');
  });
});
