import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from '../models/pagination.model';
import { UserService } from '../services/user.service';
import { AlertService } from '../services/alert.service';
import { tap, catchError, take } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { MessageModel } from '../models/message.model';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  public messages: MessageModel[];
  public user = JSON.parse(localStorage.getItem('user')); // Yuck!! wouldn't do it this way
  public boxes = [
    {value: 'inbox', displayName: 'Inbox'},
    {value: 'outbox', displayName: 'Outbox'},
  ];

  public messageContainer = 'inbox';

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
    this.messageContainer = 'inbox';
    this.loadMessages();
  }

  public pageChanged(event: any): void {
    if (event.page !== this.pagination.currentPage) {
      this.pagination.currentPage = event.page;
      this.loadMessages();
    }
  }

  public loadMessages(): void {
    console.log(this.messageContainer);
    this.userService.getUserMessages(this.user.id,
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.messageContainer)
    .pipe(
      take(1),
      catchError((error) => this.alertService.error(error))
    ).subscribe((paginatedMessages: PaginatedResult<MessageModel[]>) => {
      this.messages = paginatedMessages.result;
      this.pagination = paginatedMessages.pagination;
    });
  }

  public deleteMessage(messageId: number): void {
    this.alertService.confirm('Are you sure you want to delete this message?', (choice) => {
      if (choice) {
        this.userService.deleteMessage(this.user.id, messageId).pipe(take(1)).subscribe(
          result => {
            this.alertService.success('Message deleted');
            this.messages = this.messages.filter(m => m.id !== messageId);
          },
          error => this.alertService.error(error)
        );
      }
    });
  }

}
