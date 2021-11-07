import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GardeningAZComponent } from './gardening-az.component';

describe('GardeningAZComponent', () => {
  let component: GardeningAZComponent;
  let fixture: ComponentFixture<GardeningAZComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GardeningAZComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GardeningAZComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
