import {Subject} from "rxjs";
import {Injectable} from "@angular/core";
import * as signalR from '@microsoft/signalr';
import {AuthService} from "../auth/auth.service";
import {environment} from "../../../environments/environment";
import {BoardHubEvents} from "../constants/board-hub-events.constant";

@Injectable({ providedIn: 'root' })
export class BoardHubService {
  private connection?: signalR.HubConnection;
  private currentProjectId?: string;

  taskCreated$ = new Subject<any>();
  taskUpdated$ = new Subject<any>();
  taskDeleted$ = new Subject<any>();
  taskMoved$ = new Subject<any>();

  columnCreated$ = new Subject<any>();
  columnUpdated$ = new Subject<any>();
  columnDeleted$ = new Subject<any>();
  columnMoved$ = new Subject<any>();

  constructor(private authService: AuthService) {}

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/board`, {
        accessTokenFactory: () => this.authService.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on(BoardHubEvents.TaskCreated, (payload) => this.taskCreated$.next(payload));
    this.connection.on(BoardHubEvents.TaskUpdated, (payload) => this.taskUpdated$.next(payload));
    this.connection.on(BoardHubEvents.TaskDeleted, (payload) => this.taskDeleted$.next(payload));
    this.connection.on(BoardHubEvents.TaskMoved, (payload) => this.taskMoved$.next(payload));

    this.connection.on(BoardHubEvents.ColumnCreated, (payload) => this.columnCreated$.next(payload));
    this.connection.on(BoardHubEvents.ColumnUpdated, (payload) => this.columnUpdated$.next(payload));
    this.connection.on(BoardHubEvents.ColumnDeleted, (payload) => this.columnDeleted$.next(payload));
    this.connection.on(BoardHubEvents.ColumnMoved, (payload) => this.columnMoved$.next(payload));

    await this.connection.start();
  }

  async joinBoard(projectId: string): Promise<void> {
    await this.connect();
    if (this.currentProjectId && this.currentProjectId !== projectId) {
      await this.connection!.invoke('UnsubscribeFromBoard', this.currentProjectId);
    }
    this.currentProjectId = projectId;
    await this.connection!.invoke('SubscribeToBoard', projectId);
  }

  async leaveBoard(): Promise<void> {
    if (this.connection && this.currentProjectId) {
      await this.connection.invoke('UnsubscribeFromBoard', this.currentProjectId);
      this.currentProjectId = undefined;
    }
  }

  async disconnect(): Promise<void> {
    await this.leaveBoard();
    await this.connection?.stop();
    this.connection = undefined;
  }
}
