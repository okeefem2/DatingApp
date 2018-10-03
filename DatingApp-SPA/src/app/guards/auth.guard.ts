import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs/operators';
import { AlertService } from '../services/alert.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  public constructor(private authService: AuthService, private alertService: AlertService) {}
  public canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      this.authService.checkToken();
      return this.authService.checkAuthState().pipe(
        map((authState: any) => {
          const isAuth = authState != null;
          if (!isAuth) {
            this.alertService.error('Humans Are Not Allowed In The Dog Park');
          }
          return isAuth;
        })
      );
  }
}
