import { Component, OnInit } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { UserService } from '../../services/user.service';
import { Observable } from 'rxjs';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  public users: Observable<UserModel[]>;

  public constructor(
    private userService: UserService,
    private alertService: AlertService
  ) { }

  public ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.users = this.userService.getUsers();
  }

}
