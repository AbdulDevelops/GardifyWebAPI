import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GardenarchivComponent } from './gardenarchiv.component';

describe('GardenarchivComponent', () => {
  let component: GardenarchivComponent;
  let fixture: ComponentFixture<GardenarchivComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GardenarchivComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GardenarchivComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
