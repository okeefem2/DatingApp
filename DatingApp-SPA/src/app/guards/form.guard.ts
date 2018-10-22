import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanDeactivate } from '@angular/router';
import { Observable, of as observableOf } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { AlertService } from '../services/alert.service';

@Injectable({
  providedIn: 'root'
})
export class FormGuard implements CanDeactivate<MemberEditComponent> {

  public constructor(private alertService: AlertService) {}
  public canDeactivate(
    component: MemberEditComponent,
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      if (component.editForm.dirty) {
        // return this.alertService.confirm('Are you sure you want to continue? Any unsaved changes will be lost.', (choice: any) => {
        //   return !choice.cancel;
        // });
        return confirm('Are you sure you want to continue? Any unsaved changes will be lost.');
      }
    return true;
  }
}
