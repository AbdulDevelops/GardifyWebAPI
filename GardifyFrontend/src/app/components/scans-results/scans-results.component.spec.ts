import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScansResultsComponent } from './scans-results.component';

describe('ScansResultsComponent', () => {
  let component: ScansResultsComponent;
  let fixture: ComponentFixture<ScansResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScansResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScansResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
