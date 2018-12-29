import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user.model';
import { PaginatedResult } from '../models/pagination.model';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl: string;
  public constructor(private http: HttpClient) {
    this.baseUrl = `${environment.apiUrl}/users`;
  }

  public getUsers(pageNumber?: number, itemsPerPage?: number, userParams?: any): Observable<PaginatedResult<UserModel[]>> {
    let params = new HttpParams();
    if (!!pageNumber && !!itemsPerPage) {
      params = params.append('pageNumber', pageNumber.toString());
      params = params.append('itemsPerPage', itemsPerPage.toString());
    }

    if (!!userParams) {
      params = params.append('minAge', userParams.minAge.toString());
      params = params.append('maxAge', userParams.maxAge.toString());
      params = params.append('gender', userParams.gender.toString());
      params = params.append('orderBy', userParams.orderBy.toString());
    }
    return this.http.get<UserModel[]>(`${this.baseUrl}`, { observe: 'response', params }).pipe(
      map((response: HttpResponse<UserModel[]>) => {
        const paginatedResult = new PaginatedResult<UserModel[]>();
        paginatedResult.result = response.body;
        if (!!response.headers.get('Pagination')) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  public getUser(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.baseUrl}/${id}`);
  }

  public updateUser(id: number, user: UserModel): Observable<UserModel> {
    return this.http.put<UserModel>(`${this.baseUrl}/${id}`, user);
  }

  public setMainPhoto(id: number, photoId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/${id}/photos/${photoId}/setMain`, {});
  }

  public deletePhoto(id: number, photoId: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/${id}/photos/${photoId}`);
  }
}
