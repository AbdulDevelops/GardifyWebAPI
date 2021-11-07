import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PageNotReadyComponent } from './page-not-ready.component';

describe('PageNotReadyComponent', () => {
  let component: PageNotReadyComponent;
  let fixture: ComponentFixture<PageNotReadyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PageNotReadyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PageNotReadyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
