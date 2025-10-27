import { TestBed } from '@angular/core/testing';

import { ExchangeRatesApi } from './exchange-rates-api';

describe('ExchangeRatesApi', () => {
  let service: ExchangeRatesApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExchangeRatesApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
