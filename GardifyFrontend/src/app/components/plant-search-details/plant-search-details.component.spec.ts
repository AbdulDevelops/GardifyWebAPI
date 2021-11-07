import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantSearchDetailsComponent } from './plant-search-details.component';

describe('PlantSearchDetailsComponent', () => {
  let component: PlantSearchDetailsComponent;
  let fixture: ComponentFixture<PlantSearchDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlantSearchDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlantSearchDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
