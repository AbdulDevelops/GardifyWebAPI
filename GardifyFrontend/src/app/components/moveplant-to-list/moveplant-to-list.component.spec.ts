import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoveplantToListComponent } from './moveplant-to-list.component';

describe('MoveplantToListComponent', () => {
  let component: MoveplantToListComponent;
  let fixture: ComponentFixture<MoveplantToListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MoveplantToListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MoveplantToListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
