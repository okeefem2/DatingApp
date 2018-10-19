import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberGalleryComponent } from './member-gallery.component';

describe('MemberGalleryComponent', () => {
  let component: MemberGalleryComponent;
  let fixture: ComponentFixture<MemberGalleryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MemberGalleryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberGalleryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
