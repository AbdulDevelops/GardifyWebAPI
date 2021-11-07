import { TestBed } from '@angular/core/testing';

import { PlantSearchService } from './plant-search.service';

describe('PlantSearchService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PlantSearchService = TestBed.get(PlantSearchService);
    expect(service).toBeTruthy();
  });
});
