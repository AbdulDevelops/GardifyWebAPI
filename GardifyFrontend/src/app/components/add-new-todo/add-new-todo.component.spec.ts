import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewTodoComponent } from './add-new-todo.component';

describe('AddNewTodoComponent', () => {
  let component: AddNewTodoComponent;
  let fixture: ComponentFixture<AddNewTodoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddNewTodoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddNewTodoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
