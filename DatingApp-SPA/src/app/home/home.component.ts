import { Component, OnInit, OnDestroy } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { AuthService } from '../services/auth.service';
import { Subscription } from 'rxjs';
import { AlertService } from '../services/alert.service';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  public register = false;
  private registerSubscription = new Subscription();

  constructor(private authService: AuthService, private alertService: AlertService) { }

  public ngOnInit(): void {
  }

  public onToggleRegister(): void {
    this.register = !this.register;
  }

  public onRegister(registerModel: LoginModel): void {
    this.registerSubscription.add(this.authService.register(registerModel).pipe(
      switchMap(() => {
        console.log('Registration success');
        this.alertService.success('Registered successfully');
        return this.authService.login(registerModel);
      })
    ).subscribe(() => {
      console.log('logged in');
    }, (error: any) => this.alertService.error(`Registration failed: ${error}`)));
  }

  public ngOnDestroy(): void {
    if (this.registerSubscription) { this.registerSubscription.unsubscribe(); }
  }

}
