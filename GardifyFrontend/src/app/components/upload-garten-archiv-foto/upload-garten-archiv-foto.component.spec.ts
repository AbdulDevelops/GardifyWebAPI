import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadGartenArchivFotoComponent } from './upload-garten-archiv-foto.component';

describe('UploadGartenArchivFotoComponent', () => {
  let component: UploadGartenArchivFotoComponent;
  let fixture: ComponentFixture<UploadGartenArchivFotoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadGartenArchivFotoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadGartenArchivFotoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
