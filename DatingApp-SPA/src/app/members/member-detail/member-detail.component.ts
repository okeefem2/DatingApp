import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { UserModel } from '../../models/user.model';
import { ActivatedRoute, Params } from '@angular/router';
import { UserService } from '../../services/user.service';
import { map, switchMap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, AfterViewInit {
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
  public userObs: Observable<UserModel>;
  public user: UserModel;
  public constructor(private route: ActivatedRoute, private userService: UserService) { }

  public ngOnInit(): void {

  }

  public ngAfterViewInit(): void {
    this.route.params.pipe(
      map((params: Params) => +params.id),
      switchMap((id: number) => {
        return this.userService.getUser(id);
      }),
      switchMap((user) => {
        this.user = user;
        return this.route.queryParams;
      }),
    ).subscribe((params: Params) => {
      console.log(params);
      this.selectTab(+params.tab);
    });
  }

  selectTab(tabId: number): void {
    if (!!this.memberTabs && tabId > 0) {
      this.memberTabs.tabs[tabId].active = true;
    }
  }
}
