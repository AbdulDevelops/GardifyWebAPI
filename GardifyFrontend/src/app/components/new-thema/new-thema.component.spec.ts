import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewThemaComponent } from './new-thema.component';

describe('NewThemaComponent', () => {
  let component: NewThemaComponent;
  let fixture: ComponentFixture<NewThemaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewThemaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewThemaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
