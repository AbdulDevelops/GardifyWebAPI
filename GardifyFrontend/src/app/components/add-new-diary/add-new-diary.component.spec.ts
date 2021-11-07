import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewDiaryComponent } from './add-new-diary.component';

describe('AddNewDiaryComponent', () => {
  let component: AddNewDiaryComponent;
  let fixture: ComponentFixture<AddNewDiaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddNewDiaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddNewDiaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
