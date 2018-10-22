import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { UserModel } from '../models/user.model';
import { Observable, of as observableOf } from 'rxjs';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { switchMap, catchError, map, first } from 'rxjs/operators';
import { AlertService } from '../services/alert.service';

@Injectable({
  providedIn: 'root'
})
export class MemberEditResolver implements Resolve<UserModel> {

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertService: AlertService,
    private router: Router
  ) { }

  public resolve(route: ActivatedRouteSnapshot): Observable<UserModel> {
    return this.authService.checkAuthState().pipe(
      switchMap((authState: any) => {
        return this.userService.getUser(authState.id);
      }),
      first(),
      catchError(error => {
        this.alertService.error('There was a problem retrieving your data');
        this.router.navigate(['/members']);
        return observableOf(null);
      })
    );
  }
}
