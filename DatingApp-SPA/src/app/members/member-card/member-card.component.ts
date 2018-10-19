import { Component, Input } from '@angular/core';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  @Input() public  user: UserModel;

}
