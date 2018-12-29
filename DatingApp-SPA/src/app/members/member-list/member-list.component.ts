import { Component, OnInit } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { UserService } from '../../services/user.service';
import { Observable } from 'rxjs';
import { UserModel } from '../../models/user.model';
import { PaginatedResult, Pagination } from 'src/app/models/pagination.model';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  public users: Observable<PaginatedResult<UserModel[]>>;
  public user = JSON.parse(localStorage.getItem('user')); // Yuck!! wouldn't do it this way
  public genders = [
    {value: 'female', displayName: 'Females'},
    {value: 'male', displayName: 'Males'},
  ];

  userParams = {
    gender: '',
    minAge: 18,
    maxAge: 99,
    orderBy: 'lastActive'
  };

  public pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 5
  };

  public constructor(
    private userService: UserService,
    private alertService: AlertService
  ) { }

  public ngOnInit(): void {
    this.resetFilter();
  }

  public resetFilter(): void {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }

  public pageChanged(event: any): void {
    if (event.page !== this.pagination.currentPage) {
      this.pagination.currentPage = event.page;
      this.loadUsers();
    }
  }

  public loadUsers(): void {
    this.users = this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams).pipe(
      tap(result => {
        this.pagination = result.pagination;
      })
    );
  }

  public setOrderBy(orderBy: string): void {
    this.userParams.orderBy = orderBy;
    this.loadUsers();
  }

  public onUserLiked(userId: number): void {
    this.userService.likeUser(this.user.id, userId).subscribe((result) => {
      this.alertService.success('User Liked!');
    }, error => {
      this.alertService.error(error);
    });
  }

}
