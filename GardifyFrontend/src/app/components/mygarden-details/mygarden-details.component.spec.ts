import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MygardenDetailsComponent } from './mygarden-details.component';

describe('MygardenDetailsComponent', () => {
  let component: MygardenDetailsComponent;
  let fixture: ComponentFixture<MygardenDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MygardenDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MygardenDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
