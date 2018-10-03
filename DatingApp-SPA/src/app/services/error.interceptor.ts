import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  public constructor(private authService: AuthService) {}
  public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: any) => {
        if (error instanceof HttpErrorResponse) { // API error
          const applicationError = error.headers.get('Application-Error');
          if (error.status === 401) {
            this.authService.checkToken();
            return throwError('Unauthorized');
          }
          if (applicationError) {
            console.error(applicationError);
            return throwError(applicationError);
          }
          // Model state error most likely
          if (error.status === 400) {
            const apiError = error.error;
            return throwError(apiError || 'Unknown Server Error');
          }
          return throwError('Unknown Server Error');
        }
      })
    );
  }
}

// Provider for the app module
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
