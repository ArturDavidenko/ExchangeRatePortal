import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrencyHistory } from './currency-history';

describe('CurrencyHistory', () => {
  let component: CurrencyHistory;
  let fixture: ComponentFixture<CurrencyHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CurrencyHistory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurrencyHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
