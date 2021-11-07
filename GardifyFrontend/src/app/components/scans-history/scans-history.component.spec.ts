import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScansHistoryComponent } from './scans-history.component';

describe('ScansHistoryComponent', () => {
  let component: ScansHistoryComponent;
  let fixture: ComponentFixture<ScansHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScansHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScansHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
