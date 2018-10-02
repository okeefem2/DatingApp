import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Subscription, Observable } from 'rxjs';
import { LoginModel } from '../models/login.model';
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, OnDestroy {
  public loginForm: FormGroup;
  public loggedIn: Observable<boolean>;
  private loginSubscription = new Subscription();
  constructor(private formBuilder: FormBuilder,
              private authService: AuthService) { }

  public ngOnInit(): void {
    this.loggedIn = this.authService.checkLoggedIn();
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  public onSubmit(): void {
    if (this.loginForm.valid) {
      // Submit the form
      this.loginSubscription.add(
        this.authService.login(this.loginForm.value as LoginModel)
                        .subscribe((response: any) => {
                          console.log('Login succesful');
                        },
                        (error: any) => {
                          console.log('Login failed');
                        })
      );
    }
  }

  public ngOnDestroy(): void {
    if (this.loginSubscription) { this.loginSubscription.unsubscribe(); }
  }

  public onLogout(): void {
    this.authService.logout();
  }
}
