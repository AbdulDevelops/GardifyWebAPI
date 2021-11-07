import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherPresentationComponent } from './other-presentation.component';

describe('OtherPresentationComponent', () => {
  let component: OtherPresentationComponent;
  let fixture: ComponentFixture<OtherPresentationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OtherPresentationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherPresentationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
