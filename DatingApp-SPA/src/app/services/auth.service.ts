import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../models/login.model';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authState = new BehaviorSubject<any>(null);
  private baseUrl: string;

  constructor(private http: HttpClient,
              private jwtHelper: JwtHelperService,
              private router: Router) {
    this.checkToken();
    this.baseUrl = environment.apiUrl + '/auth/';
  }

  public checkAuthState(): Observable<any> {
    return this.authState.asObservable();
  }

  public changeAuthState(authState: any): void {
    this.authState.next(authState);
  }

  public login(loginModel: LoginModel): Observable<any> {
    return this.http.post(this.baseUrl + 'login', loginModel).pipe(
      map((response: any) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.checkToken();
          this.router.navigate(['/members']);
        }
      })
    );
  }

  public checkToken(): void {
    const token = localStorage.getItem('token');
    if (!this.jwtHelper.isTokenExpired(token)) {
      const decodedToken = this.jwtHelper.decodeToken(token);
      const user = {
        id: decodedToken.name_id,
        username: decodedToken.unique_name
      };
      this.changeAuthState(user);
    } else {
      this.changeAuthState(null);
    }
  }

  public logout(): void {
    localStorage.removeItem('token');
    this.checkToken();
    this.router.navigate(['/home']);
  }

  public register(registerModel: LoginModel): Observable<any> {
    return this.http.post(this.baseUrl + 'register', registerModel).pipe(
      map((response: any) => {
        // if (response && response.token) {
        //   localStorage.setItem('token', response.token);
        //   this.changeauthState(true);
        // }
      })
    );
  }
}
