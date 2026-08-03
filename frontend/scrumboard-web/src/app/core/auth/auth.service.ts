import {Injectable, signal} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {Observable, tap} from "rxjs";
import {environment} from "../../../environments/environment";

export interface LoginResponse{
  token: string;
  name: string;
  email: string;

}

const TOKEN_KEY = 'authToken';
@Injectable({providedIn: 'root'})
export class AuthService {
  private readonly tokenSignal = signal<string|null>(localStorage.getItem(TOKEN_KEY));
  constructor(private http: HttpClient,
              private router: Router,) {}

  login(email:string,password:string) :Observable<LoginResponse>{
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, {email, password})
      .pipe(tap(res => this.setToken(res.token)));

  }

  logout():void{
    localStorage.removeItem(TOKEN_KEY);
    this.tokenSignal.set(null)
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean{
    return !!this.tokenSignal();
  }

  private setToken(token:string){
    localStorage.setItem(TOKEN_KEY, token);
    this.tokenSignal.set(token);
  }
}
