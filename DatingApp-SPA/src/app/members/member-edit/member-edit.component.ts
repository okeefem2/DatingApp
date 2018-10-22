import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserModel } from '../../models/user.model';
import { AlertService } from '../../services/alert.service';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  public user: UserModel;
  @ViewChild('editForm') editForm: NgForm;

  public constructor(private route: ActivatedRoute,
    private alertService: AlertService,
    private userService: UserService,
    private authService: AuthService) { }

  public ngOnInit(): void {
    this.route.data.subscribe((data: any) => this.user = data.user);
  }

  public updateUser(): void {
    this.authService.checkAuthState().pipe(
      switchMap((authState: any) => this.userService.updateUser(authState.id, this.user))
    ).subscribe(next => {
      this.alertService.success('Profile updated successfully');
      this.editForm.resetForm(this.user);
    }, error => this.alertService.error(`There was an error updating the user ${error}`));
  }

  @HostListener('window:beforeunload', ['$event'])
  public unloadNotification($event: any): void {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
}
