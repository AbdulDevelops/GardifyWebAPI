import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OekoscanResultComponent } from './oekoscan-result.component';

describe('OekoscanResultComponent', () => {
  let component: OekoscanResultComponent;
  let fixture: ComponentFixture<OekoscanResultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OekoscanResultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OekoscanResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
