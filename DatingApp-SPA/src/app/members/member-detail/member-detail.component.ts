import { Component, OnInit } from '@angular/core';
import { UserModel } from '../../models/user.model';
import { ActivatedRoute, Params } from '@angular/router';
import { UserService } from '../../services/user.service';
import { map, switchMap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  public userObs: Observable<UserModel>;
  public constructor(private route: ActivatedRoute, private userService: UserService) { }

  public ngOnInit(): void {
    this.userObs = this.route.params.pipe(
      map((params: Params) => +params.id),
      switchMap((id: number) => this.userService.getUser(id))
    );
  }
}
