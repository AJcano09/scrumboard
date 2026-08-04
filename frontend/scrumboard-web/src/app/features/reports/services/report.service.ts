import {inject, Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/projects`;

  downloadProjectReport(projectId: string, format: 'PDF' | 'EXCEL'): Observable<Blob> {
    const url = `${this.apiUrl}/${projectId}/reports/${format}`;
    // Se requiere 'blob' para manejar correctamente el archivo binario descargado
    return this.http.get(url, { responseType: 'blob' });
  }
}
