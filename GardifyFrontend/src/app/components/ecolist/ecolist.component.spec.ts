import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcolistComponent } from './ecolist.component';

describe('EcolistComponent', () => {
  let component: EcolistComponent;
  let fixture: ComponentFixture<EcolistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcolistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcolistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
