import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HeaderGardenArchivComponent } from './header-garden-archiv.component';

describe('HeaderGardenArchivComponent', () => {
  let component: HeaderGardenArchivComponent;
  let fixture: ComponentFixture<HeaderGardenArchivComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HeaderGardenArchivComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderGardenArchivComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
