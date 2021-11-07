import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantsDocComponent } from './plants-doc.component';

describe('PlantsDocComponent', () => {
  let component: PlantsDocComponent;
  let fixture: ComponentFixture<PlantsDocComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlantsDocComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlantsDocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
