import { Component, OnInit, Input } from '@angular/core';
import { NgxGalleryImage, NgxGalleryOptions, NgxGalleryAnimation } from 'ngx-gallery';
import { PhotoModel } from '../../models/photo.model';

@Component({
  selector: 'app-member-gallery',
  templateUrl: './member-gallery.component.html',
  styleUrls: ['./member-gallery.component.css']
})
export class MemberGalleryComponent implements OnInit {
  public galleryOptions: NgxGalleryOptions[];
  public galleryImages: NgxGalleryImage[];

  @Input()
  public set photos(photos: PhotoModel[]) {
    this.galleryImages = photos.map((photo: PhotoModel) => (
      {
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      }
    ));
  }

  public ngOnInit(): void {
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false // if true clicking shows a full screen view of image
      }
    ];
  }
}
