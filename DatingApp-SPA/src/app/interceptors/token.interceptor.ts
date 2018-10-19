import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    request = request.clone({
      headers: request.headers
        .set('Cache-Control', 'no-cache')
        .set('Authorization', `Bearer ${localStorage.getItem('token')}`)
    });
    return next.handle(request);
    // .pipe(
    //   catchError(response => {
    //     if (response instanceof HttpErrorResponse) {
    //       this.handleError(response);
    //     }
    //     return observableThrowError(response);
    //   })
    // );
  }

  /**
   * Implementing handleError from the ErrorHandler interface.
   * Intercepts HttpErrors and handles generic errors without showing too much information
   * @param {Error} error
   */
  // public handleError(httpErrorResponse: HttpErrorResponse) {
  //   // Deal with bad http responses
  //   if (httpErrorResponse instanceof HttpErrorResponse) {
  //     switch (httpErrorResponse.status) {
  //       case 401:
  //         this.authService.clearAuthenticatedUser();
  //         this.router.navigate(['/login']);
  //         // Do not log in here. If a user logs out, it will flood browser console with errors
  //         break;
  //       default:
  //         console.log(`There was an error with your request ${httpErrorResponse.message}`);
  //         break;
  //     }
  //   }
  // }
}

export const TokenInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: TokenInterceptor,
  multi: true
};
