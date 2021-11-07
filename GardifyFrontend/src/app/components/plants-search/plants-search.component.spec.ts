import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantsSearchComponent } from './plants-search.component';

describe('PlantsSearchComponent', () => {
  let component: PlantsSearchComponent;
  let fixture: ComponentFixture<PlantsSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlantsSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlantsSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
