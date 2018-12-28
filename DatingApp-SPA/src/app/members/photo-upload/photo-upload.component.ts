import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PhotoModel } from 'src/app/models/photo.model';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/services/auth.service';
import { take, switchMap } from 'rxjs/operators';
import { UserService } from 'src/app/services/user.service';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-photo-upload',
  templateUrl: './photo-upload.component.html',
  styleUrls: ['./photo-upload.component.css']
})
export class PhotoUploadComponent implements OnInit {
  @Input() photos: PhotoModel[];
  @Output() mainPhotoChanged = new EventEmitter<string>();
  public uploader: FileUploader;
  public hasBaseDropZoneOver = false;

  constructor(private authService: AuthService,
    private alertService: AlertService,
    private userService: UserService) { }

  ngOnInit() {
    this.initUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  public initUploader(): void {
    this.authService.checkAuthState().pipe(take(1)).subscribe((user) => {
      this.uploader = new FileUploader({
        url: `${environment.apiUrl}/users/${user.id}/photos`,
        authToken: `Bearer ${localStorage.getItem('token')}`,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 10 * 1024 * 1024 * 1024,
      });

      this.uploader.onAfterAddingFile = file => file.withCredentials = false;
      this.uploader.onSuccessItem = (item, response, status, headers) => {
        if (response) {
          const photo: PhotoModel = JSON.parse(response);
          this.photos.push(photo);
          if (photo.isMain) {
            this.mainPhotoChanged.emit(photo.url);
          }
        }
      };
    });
  }

  public setMainPhoto(photoId: number): void {
    this.authService.checkAuthState().pipe(
      take(1),
      switchMap((user) => {
        return this.userService.setMainPhoto(user.id, photoId);
      })
    ).subscribe((result) => {
      console.log(result);
      this.photos = this.photos.map((photo: PhotoModel) => {
        if (photo.isMain && photo.id !== photoId) {
          photo.isMain = false;
        } else if (photo.id === photoId) {
          photo.isMain = true;
          this.mainPhotoChanged.emit(photo.url);
        }
        return photo;
      });
      this.alertService.success('Main photo set succesfully!');
    }, error => {
      this.alertService.error(error);
    });
  }

  public deletePhoto(photoId: number): void {
    this.alertService.confirm('Are you sure you want to delete this photo? This action cannot be undone', (choice: any) => {
      this.authService.checkAuthState().pipe(
        take(1),
        switchMap((user) => {
          return this.userService.deletePhoto(user.id, photoId);
        })
      ).subscribe((result) => {
        this.photos = this.photos.filter((photo: PhotoModel) => {
          return photo.id !== photoId;
        });
        this.alertService.success('Photo deleted!');
      }, error => {
        this.alertService.error(error);
      });
    });
  }

}
