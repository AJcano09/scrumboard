import {Injectable} from "@angular/core";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import { Project, ProjectFormValue} from "../models/project.model";
import {environment} from "../../../../environments/environment";
import {ApiRoutes} from "../../../core/constants/api.routes.constant";
import {PagedResult} from "../../../core/Common/models/page.result.model";

@Injectable({providedIn: 'root'})
export class ProjectService{
  constructor(private http: HttpClient) {}

  getPaged(search: string, pageNumber: number, pageSize: number): Observable<PagedResult<Project>> {
    const params = new HttpParams()
      .set('search', search ?? '')
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<PagedResult<Project>>(`${environment.apiUrl}${ApiRoutes.Projects.GetPaged}`, { params });
  }

  create(request: ProjectFormValue): Observable<Project> {
    return this.http.post<Project>(`${environment.apiUrl}${ApiRoutes.Projects.Create}`, request);
  }

  update(id: string, request: ProjectFormValue): Observable<Project> {
    return this.http.put<Project>(`${environment.apiUrl}${ApiRoutes.Projects.Update(id)}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}${ApiRoutes.Projects.Delete(id)}`);
  }
}
