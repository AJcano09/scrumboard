import {HttpInterceptorFn} from "@angular/common/http";
import {AuthService} from "./auth.service";
import {inject} from "@angular/core";
import {catchError, throwError} from "rxjs";
import {StorageKeys} from "../constants/storage-keys.constant";

export const authInterceptor:HttpInterceptorFn=(req,next)=>{
  const auth = inject(AuthService);
  if (req.url.includes('/api/auth/login')) {
    return next(req);
  }
  const token = typeof window !== 'undefined' ? window.localStorage.getItem(StorageKeys.AuthToken) : null;

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
