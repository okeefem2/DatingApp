import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserModel } from '../models/user.model';
import { PaginatedResult } from '../models/pagination.model';
import { map, take } from 'rxjs/operators';
import { MessageModel } from '../models/message.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl: string;
  public constructor(private http: HttpClient) {
    this.baseUrl = `${environment.apiUrl}/users`;
  }

  public getUsers(pageNumber?: number, itemsPerPage?: number, userParams?: any,
    likesParam?: string): Observable<PaginatedResult<UserModel[]>> {
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

    if (!!likesParam) {
      params = likesParam === 'likers' ? params.append('likers', 'true') : params.append('likees', 'true');
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

  public getUserMessages(userId: number,
    pageNumber?: number,
    itemsPerPage?: number,
    messageContainer?: string): Observable<PaginatedResult<MessageModel[]>> {
    let params = new HttpParams();
    if (!!pageNumber && !!itemsPerPage) {
      params = params.append('pageNumber', pageNumber.toString());
      params = params.append('itemsPerPage', itemsPerPage.toString());
    }

    if (!!messageContainer) {
      params = params.append('messageContainer', messageContainer);
    }
    return this.http.get<MessageModel[]>(`${this.baseUrl}/${userId}/messages`, { observe: 'response', params }).pipe(
      // Need the observe config so that we can get the whole response
      // so that we can access the headers of the response for the pagination headers
      // I'm not entirely sure why we do not just return everything together in an object /shrug
      // that's probably how I would do it rather than  storing the pagination in the headers
      map((response: HttpResponse<MessageModel[]>) => {
        const paginatedResult = new PaginatedResult<MessageModel[]>();
        paginatedResult.result = response.body;
        if (!!response.headers.get('Pagination')) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  public getMessageThread(userId: number, recipientId: number): Observable<MessageModel[]> {
    return this.http.get<MessageModel[]>(`${this.baseUrl}/${userId}/messages/thread/${recipientId}`);
  }

  public sendMessage(userId: number, message: Partial<MessageModel>): Observable<MessageModel> {
    return this.http.post<MessageModel>(`${this.baseUrl}/${userId}/messages/`, message);
  }

  public deleteMessage(userId: number, messageId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/${userId}/messages/${messageId}`, {});
  }

  public readMessage(userId: number, messageId: number): void {
    this.http.post<any>(`${this.baseUrl}/${userId}/messages/read/${messageId}`, {}).pipe(take(1)).subscribe();
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

  public likeUser(likerId: number, likeeId: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/${likerId}/like/${likeeId}`, {});
  }
}
