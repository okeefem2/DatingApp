import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user.model';
import { PhotoModel } from '../models/photo.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private baseUrl: string;
  public constructor(private http: HttpClient) {
    this.baseUrl = `${environment.apiUrl}/admin`;
  }

  public getUsersWithRoles(): Observable<UserModel[]> {
    return this.http.get<UserModel[]>(`${this.baseUrl}/usersWithRoles`);
  }

  public getPhotosForModeration(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/photosForModeration`);
  }

  public updateUserRoles(username: string, roles: string[]): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/editRoles/${username}`, { roleNames: roles });
  }

  public approvePhoto(photoId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/approvePhoto/${photoId}`, {});
  }

  public denyPhoto(photoId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/denyPhoto/${photoId}`, {});
  }
}
