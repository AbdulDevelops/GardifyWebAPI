import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantsDocDetailsComponent } from './plants-doc-details.component';

describe('PlantsDocDetailsComponent', () => {
  let component: PlantsDocDetailsComponent;
  let fixture: ComponentFixture<PlantsDocDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlantsDocDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlantsDocDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
