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
  constructor(private http: HttpClient,
              private router: Router,) {}

  login(email:string,password:string) :Observable<LoginResponse>{
    console.log(`${environment.apiUrl}${ApiRoutes.Auth.Login}`);
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}${ApiRoutes.Auth.Login}`, {email, password})
      .pipe(tap(res => this.setToken(res.token)));

  }

  logout():void{
    localStorage.removeItem(StorageKeys.AuthToken);
    this.tokenSignal.set(null)
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean{
    return !!this.tokenSignal();
  }

  private setToken(token:string){
    localStorage.setItem(StorageKeys.AuthToken, token);
    this.tokenSignal.set(token);
  }

  private getInitialToken(): string | null{
   if(typeof window !== 'undefined') {
     return localStorage.getItem(StorageKeys.AuthToken);
   }
     return null;
  }
}
