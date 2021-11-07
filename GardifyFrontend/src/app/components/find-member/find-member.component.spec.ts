import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindMemberComponent } from './find-member.component';

describe('FindMemberComponent', () => {
  let component: FindMemberComponent;
  let fixture: ComponentFixture<FindMemberComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FindMemberComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FindMemberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
