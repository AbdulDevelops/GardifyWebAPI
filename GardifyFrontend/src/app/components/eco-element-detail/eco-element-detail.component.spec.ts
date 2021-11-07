import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcoElementDetailComponent } from './eco-element-detail.component';

describe('EcoElementDetailComponent', () => {
  let component: EcoElementDetailComponent;
  let fixture: ComponentFixture<EcoElementDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcoElementDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcoElementDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
