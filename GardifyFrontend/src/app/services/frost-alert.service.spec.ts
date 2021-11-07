import { TestBed } from '@angular/core/testing';

import { FrostAlertService } from './frost-alert.service';

describe('FrostAlertService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FrostAlertService = TestBed.get(FrostAlertService);
    expect(service).toBeTruthy();
  });
});
