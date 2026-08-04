import {Injectable, signal} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {Observable, tap} from "rxjs";
import {environment} from "../../../environments/environment";
import {ApiRoutes} from "../constants/api.routes.constant";
import {StorageKeys} from "../constants/storage-keys.constant";

export interface LoginResponse{
  token: string;
  name: string;
  email: string;

}

@Injectable({providedIn: 'root'})
export class AuthService {
  private readonly tokenSignal = signal<string|null>(this.getInitialToken());
  private readonly userNameSignal = signal<string | null>(this.getInitialUserName());
  constructor(private http: HttpClient,
              private router: Router,) {}

  login(email:string,password:string) :Observable<LoginResponse>{
    console.log(`${environment.apiUrl}${ApiRoutes.Auth.Login}`);
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}${ApiRoutes.Auth.Login}`, {email, password})
      .pipe(tap(res => this.setToken(res.token, res.name)));

  }

  logout():void{
    localStorage.removeItem(StorageKeys.AuthToken);
    localStorage.removeItem(StorageKeys.UserName);
    this.tokenSignal.set(null);
    this.userNameSignal.set(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean{
    return !!this.tokenSignal();
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  getUserName(): string | null {
    return this.userNameSignal();
  }

  private setToken(token:string, userName?: string){
    localStorage.setItem(StorageKeys.AuthToken, token);
    this.tokenSignal.set(token);
    if (userName) {
      localStorage.setItem(StorageKeys.UserName, userName);
      this.userNameSignal.set(userName);
    }
  }

  private getInitialToken(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem(StorageKeys.AuthToken);
    }
    return null;
  }

  private getInitialUserName(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem(StorageKeys.UserName);
    }
    return null;
  }
}
