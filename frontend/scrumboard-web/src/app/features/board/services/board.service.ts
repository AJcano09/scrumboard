import {Injectable} from "@angular/core";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Board, BoardTask, User} from "../models/board.model";
import {environment} from "../../../../environments/environment";
import {Observable} from "rxjs";
import {ApiRoutes} from "../../../core/constants/api.routes.constant";
import {PagedResult} from "../../../core/Common/models/page.result.model";

@Injectable({providedIn:'root'})
export class BoardService {

  constructor(private http: HttpClient) {
  }

  getBoard(projectId: string): Observable<Board> {
    return this.http.get<Board>(`${environment.apiUrl}${ApiRoutes.Board.Get(projectId)}`);
  }

  createTask(payload: { title: string; description: string; priority: string; responsibleId: string; columnId: string }): Observable<BoardTask> {
    return this.http.post<BoardTask>(`${environment.apiUrl}${ApiRoutes.Tasks.Create}`, payload);
  }

  updateTask(id: string, payload: { title: string; description: string; priority: string; responsibleId: string }): Observable<BoardTask> {
    return this.http.put<BoardTask>(`${environment.apiUrl}${ApiRoutes.Tasks.Update(id)}`, payload);
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}${ApiRoutes.Tasks.Delete(id)}`);
  }

  moveTask(id: string, targetColumnId: string, newIndex: number): Observable<BoardTask> {
    return this.http.put<BoardTask>(`${environment.apiUrl}${ApiRoutes.Tasks.Move(id)}`, { targetColumnId, newIndex });
  }

  getUsers(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResult<User>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<User>>(`${environment.apiUrl}${ApiRoutes.Users.GetAll}`, { params });
  }
}
