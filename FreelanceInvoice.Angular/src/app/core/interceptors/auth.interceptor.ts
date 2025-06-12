import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export const authInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<any> => {
  // Get token from localStorage directly instead of using AuthService
  const getToken = () => {
    const tokens = localStorage.getItem('auth_tokens');
    return tokens ? JSON.parse(tokens) : null;
  };

  const tokens = getToken();
  
  if (tokens?.accessToken) {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${tokens.accessToken}`
      }
    });
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !request.url.includes('refresh-token')) {
        // Clear token and redirect to login
        localStorage.removeItem('auth_tokens');
        window.location.href = '/auth/login';
      }
      return throwError(() => error);
    })
  );
}; 