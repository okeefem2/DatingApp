import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl: string;
  public constructor(private http: HttpClient) {
    this.baseUrl = `${environment.apiUrl}/users`;
  }

  public getUsers(): Observable<UserModel[]> {
    return this.http.get<UserModel[]>(`${this.baseUrl}`);
  }

  public getUser(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.baseUrl}/${id}`);
  }
}
