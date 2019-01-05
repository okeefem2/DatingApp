import { Component, OnInit } from '@angular/core';
import { PhotoModel } from 'src/app/models/photo.model';
import { AdminService } from 'src/app/services/admin.service';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  public photos: any[];
  constructor(private adminService: AdminService, private alertService: AlertService) { }

  ngOnInit() {
    this.adminService.getPhotosForModeration().subscribe((photos: any[]) => {
      this.photos = photos;
    });
  }

  public approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
      this.alertService.success('Photo approved');
      this.removePhoto(photoId);
    },
    error => this.alertService.error(error));
  }

  public denyPhoto(photoId: number) {
    this.adminService.denyPhoto(photoId).subscribe(() => {
      this.alertService.success('Photo denied');
      this.removePhoto(photoId);
    },
    error => this.alertService.error(error));
  }

  private removePhoto(photoId: number) {
    this.photos = this.photos.filter(photo => photo.id !== photoId);
  }

}
