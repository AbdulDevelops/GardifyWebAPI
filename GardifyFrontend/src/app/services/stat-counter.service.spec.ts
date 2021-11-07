import { TestBed } from '@angular/core/testing';

import { StatCounterService } from './stat-counter.service';

describe('StatCounterService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: StatCounterService = TestBed.get(StatCounterService);
    expect(service).toBeTruthy();
  });
});
