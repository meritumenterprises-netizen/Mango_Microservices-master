// auth.interceptor.ts
import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './services/AuthenticationService';

export const AuthInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService); // inject service in a function
  const token = authService.getToken();

  if (token) {
    if (token !== null) {
      const authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });

      return next(authReq);
    }

    return next(req);
  }

  return next(req);
};
