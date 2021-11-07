import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarnungenComponent } from './warnungen.component';

describe('WarnungenComponent', () => {
  let component: WarnungenComponent;
  let fixture: ComponentFixture<WarnungenComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WarnungenComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WarnungenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
