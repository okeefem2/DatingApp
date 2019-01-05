import { Component, OnInit } from '@angular/core';
import { UserModel } from 'src/app/models/user.model';
import { AdminService } from 'src/app/services/admin.service';
import { AlertService } from 'src/app/services/alert.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { EditRoleModalComponent } from '../edit-role-modal/edit-role-modal.component';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  public bsModalRef: BsModalRef;

  public users: UserModel[];
  private roles = [
    'Admin',
    'Moderator',
    'Member',
    'VIP'
  ];
  public constructor(private adminService: AdminService, private alertService: AlertService, private modalService: BsModalService) { }

  public ngOnInit() {
    this.getUsersWithRoles();
  }

  public getUsersWithRoles(): void {
    this.adminService.getUsersWithRoles().subscribe((users: UserModel[]) => {
      this.users = users;
    }, error => this.alertService.error(error));
  }

  public openEditRoleModal(user: UserModel): void {
    const initialState = {
      roles: this.getAvailableRoles(user.roles),
      user: user
    };
    this.bsModalRef = this.modalService.show(EditRoleModalComponent, {initialState});

    this.bsModalRef.content.rolesUpdated.pipe(
      switchMap((roles: any[]) => {
        const newUserRoles = roles.reduce((newRoles: string[], role: any) => {
          if (role.checked) {
            newRoles.push(role.role);
          }
          return newRoles;
        }, []);
        console.log(newUserRoles);
        user.roles = newUserRoles;
        return this.adminService.updateUserRoles(user.username, newUserRoles);
      })
    ).subscribe(() => {
      this.alertService.success(`Roles for user ${user.username} updated succesfully`);
    }, error => this.alertService.error(`Failed to update roles for user ${user.username}`));
  }

  private getAvailableRoles(userRoles: string[]): any[] {
    return this.roles.map((role: string) => {
      return { role: role, checked: userRoles.indexOf(role) >= 0 };
    });
  }

}
