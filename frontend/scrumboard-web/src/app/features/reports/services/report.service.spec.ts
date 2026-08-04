import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ReportService } from './report.service';
import {environment} from "../../../../environments/environment";

describe('ReportService', () => {
  let service: ReportService;
  let httpMock: HttpTestingController;
  const mockProjectId = '12345-abcde';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ReportService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ReportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Verifica que no haya peticiones pendientes
  });

  // Prueba 1: Verifica la petición HTTP y el tipo de respuesta (Blob) para PDF
  it('Debe realizar una petición GET solicitando un Blob al exportar a PDF', () => {
    service.downloadProjectReport(mockProjectId, 'PDF').subscribe((response) => {
      expect(response).toBeTruthy();
      expect(response instanceof Blob).toBeTrue();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/projects/${mockProjectId}/reports/PDF`);
    expect(req.request.method).toBe('GET');
    expect(req.request.responseType).toBe('blob');

    // Simulamos la respuesta del backend
    req.flush(new Blob(['pdf content'], { type: 'application/pdf' }));
  });

  // Prueba 2: Verifica la petición HTTP para Excel
  it('Debe construir correctamente la URL al solicitar el formato EXCEL', () => {
    service.downloadProjectReport(mockProjectId, 'EXCEL').subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/projects/${mockProjectId}/reports/EXCEL`);
    expect(req.request.method).toBe('GET');

    req.flush(new Blob(['excel content'], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' }));
  });
});
