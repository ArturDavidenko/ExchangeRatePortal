import { TestBed } from '@angular/core/testing';

import { ExchangeRates } from './exchange-rates';

describe('ExchangeRates', () => {
  let service: ExchangeRates;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExchangeRates);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
