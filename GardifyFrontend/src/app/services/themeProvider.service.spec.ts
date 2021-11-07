import { TestBed } from '@angular/core/testing';

import { ThemeProviderService } from './themeProvider.service';

describe('ThemeProviderService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ThemeProviderService = TestBed.get(ThemeProviderService);
    expect(service).toBeTruthy();
  });
});
