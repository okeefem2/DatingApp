import { Component, OnInit } from '@angular/core';
import { tap } from 'rxjs/operators';
import { UserService } from '../services/user.service';
import { AlertService } from '../services/alert.service';
import { Pagination, PaginatedResult } from '../models/pagination.model';
import { UserModel } from '../models/user.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {


  public users: Observable<PaginatedResult<UserModel[]>>;
  public user = JSON.parse(localStorage.getItem('user')); // Yuck!! wouldn't do it this way

  likesParam = 'likers';

  public pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 5
  };

  public constructor(
    private userService: UserService,
    private alertService: AlertService
  ) { }

  public ngOnInit(): void {
    this.loadUsers();
  }

  public pageChanged(event: any): void {
    if (event.page !== this.pagination.currentPage) {
      this.pagination.currentPage = event.page;
      this.loadUsers();
    }
  }

  public loadUsers(): void {
    this.users = this.userService.getUsers(
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      null,
      this.likesParam).pipe(
      tap(result => {
        this.pagination = result.pagination;
      })
    );
  }

  public onUserLiked(userId: number): void {
    this.userService.likeUser(this.user.id, userId).subscribe((result) => {
      this.alertService.success('User Liked!');
    }, error => {
      this.alertService.error(error);
    });
  }

}
