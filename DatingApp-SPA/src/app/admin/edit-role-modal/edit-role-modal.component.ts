import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { UserModel } from 'src/app/models/user.model';

@Component({
  selector: 'app-edit-role-modal',
  templateUrl: './edit-role-modal.component.html',
  styleUrls: ['./edit-role-modal.component.css']
})
export class EditRoleModalComponent implements OnInit {

  public roles: any[] = [];
  public user: UserModel;

  @Output() public rolesUpdated = new EventEmitter();

  public constructor(public bsModalRef: BsModalRef) {}

  public ngOnInit(): void {
  }

  public updateRoles() {
    this.rolesUpdated.emit(this.roles);
    this.bsModalRef.hide();
  }

}
