import { Component, OnInit, Input } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { MessageModel } from 'src/app/models/message.model';
import { Observable, of } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { take, switchMap, catchError, tap } from 'rxjs/operators';
import { UserModel } from 'src/app/models/user.model';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() public recipientId: number;

  public messages: MessageModel[];
  public newMessage: Partial<MessageModel> = {
    content: null,
    recipientId: null,
  };

  private currentUserId: number;

  constructor(private userService: UserService, private authService: AuthService, private alertService: AlertService) { }

  public ngOnInit() {
    this.loadMessageThread(); // should really be on input change but oh well
  }

  public loadMessageThread() {
    this.authService.checkAuthState().pipe(
      take(1),
      switchMap((currentUser: UserModel) => {
        this.currentUserId = currentUser.id;
        return this.userService.getMessageThread(this.currentUserId, this.recipientId);
      }),
      catchError(error => {
        this.alertService.error(error);
        return of([]);
      }),
      tap((messages: MessageModel[]) => {
        messages.forEach(message => {
          if (!message.isRead && message.recipientId === this.currentUserId) {
            this.userService.readMessage(this.currentUserId, message.id);
          }
        });
      })
    ).subscribe((messages: MessageModel[]) => {
      this.messages = messages;
    });
  }

  public sendMessage(): void {
    this.newMessage.recipientId = this.recipientId;
    this.authService.checkAuthState()
      .pipe(
        take(1),
        switchMap((currentUser: UserModel) => this.userService.sendMessage(currentUser.id, this.newMessage)),
        catchError(error => {
          this.alertService.error(error);
          return of(null);
        })
      )
      .subscribe((result: MessageModel) => {
        this.alertService.success('Message Sent!');
        this.messages.unshift(result);
        this.newMessage = {
          content: null,
          recipientId: null,
        };
      }, error => this.alertService.error('Message Failed to send.'));
  }

}
