import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseFailComponent } from './purchase-fail.component';

describe('PurchaseFailComponent', () => {
  let component: PurchaseFailComponent;
  let fixture: ComponentFixture<PurchaseFailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseFailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseFailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
