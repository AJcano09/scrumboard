import {HttpInterceptorFn} from "@angular/common/http";
import {AuthService} from "./auth.service";
import {inject} from "@angular/core";
import {catchError, throwError} from "rxjs";

export const authInterceptor:HttpInterceptorFn=(req,next)=>{
  const auth = inject(AuthService);
  const token = typeof window !== 'undefined' ? window.localStorage.getItem('authToken') : null;

  const authReq =token
  ? req.clone(
    {
      setHeaders:
        {
          Authorization: `Bearer ${token}`
        }
    })
    : req;
  return next(authReq).pipe(
    catchError(err=>{
      if(err.status ===401){
        auth.logout();
      }
      return throwError(()=>err);
    })
  );
}
