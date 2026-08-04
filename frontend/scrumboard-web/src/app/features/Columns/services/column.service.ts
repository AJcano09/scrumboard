import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Column} from "../models/column.model";
import {Observable} from "rxjs";
import {environment} from "../../../../environments/environment";
import {ApiRoutes} from "../../../core/constants/api.routes.constant";

@Injectable({ providedIn: 'root' })
export class ColumnService {
  constructor(private http: HttpClient) {}

  getByProject(projectId: string): Observable<Column[]> {
    return this.http.get<Column[]>(`${environment.apiUrl}${ApiRoutes.Columns.GetByProject(projectId)}`);
  }
  create(projectId: string, name: string): Observable<Column> {
    return this.http.post<Column>(`${environment.apiUrl}${ApiRoutes.Columns.Create(projectId)}`, { name });
  }
  update(projectId: string, id: string, name: string): Observable<Column> {
    return this.http.put<Column>(`${environment.apiUrl}${ApiRoutes.Columns.Update(projectId, id)}`, { name });
  }
  delete(projectId: string, id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}${ApiRoutes.Columns.Delete(projectId, id)}`);
  }
  reorder(projectId: string, orderedColumnIds: string[]): Observable<Column[]> {
    return this.http.put<Column[]>(`${environment.apiUrl}${ApiRoutes.Columns.Reorder(projectId)}`, { orderedColumnIds });
  }
}
