import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OekoscanComponent } from './oekoscan.component';

describe('OekoscanComponent', () => {
  let component: OekoscanComponent;
  let fixture: ComponentFixture<OekoscanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OekoscanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OekoscanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
