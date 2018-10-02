import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../models/login.model';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(this.checkToken());
  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.apiUrl + '/auth/';
  }

  public checkLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  public changeLoggedIn(loggedIn: boolean): void {
    this.loggedIn.next(loggedIn);
  }

  public login(loginModel: LoginModel): Observable<any> {
    return this.http.post(this.baseUrl + 'login', loginModel).pipe(
      map((response: any) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.changeLoggedIn(true);
        }
      })
    );
  }

  private checkToken(): boolean {
    return localStorage.getItem('token') != null;
  }

  public logout(): void {
    localStorage.removeItem('token');
    this.changeLoggedIn(false);
  }

  public register(registerModel: LoginModel): Observable<any> {
    return this.http.post(this.baseUrl + 'register', registerModel).pipe(
      map((response: any) => {
        // if (response && response.token) {
        //   localStorage.setItem('token', response.token);
        //   this.changeLoggedIn(true);
        // }
      })
    );
  }
}
