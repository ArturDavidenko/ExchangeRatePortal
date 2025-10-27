import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrencyCalculator } from './currency-calculator';

describe('CurrencyCalculator', () => {
  let component: CurrencyCalculator;
  let fixture: ComponentFixture<CurrencyCalculator>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CurrencyCalculator]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurrencyCalculator);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
