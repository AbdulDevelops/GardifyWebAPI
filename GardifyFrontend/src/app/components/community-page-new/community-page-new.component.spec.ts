import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommunityPageNewComponent } from './community-page-new.component';

describe('CommunityPageNewComponent', () => {
  let component: CommunityPageNewComponent;
  let fixture: ComponentFixture<CommunityPageNewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CommunityPageNewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CommunityPageNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
