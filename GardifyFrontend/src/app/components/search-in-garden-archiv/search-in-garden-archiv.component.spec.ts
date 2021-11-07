import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchInGardenArchivComponent } from './search-in-garden-archiv.component';

describe('SearchInGardenArchivComponent', () => {
  let component: SearchInGardenArchivComponent;
  let fixture: ComponentFixture<SearchInGardenArchivComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SearchInGardenArchivComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchInGardenArchivComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
