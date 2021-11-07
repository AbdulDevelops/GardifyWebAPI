import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAbumComponent } from './edit-abum.component';

describe('EditAbumComponent', () => {
  let component: EditAbumComponent;
  let fixture: ComponentFixture<EditAbumComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditAbumComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditAbumComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
