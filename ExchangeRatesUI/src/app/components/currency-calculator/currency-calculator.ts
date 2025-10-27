import { Component, computed, inject, signal } from '@angular/core';
import { ExchangeRates } from '../../services/exchange-rates';
import { CURRENCY_NAMES, getCurrencyName, RegionType } from '../../models/exchange-rates.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-currency-calculator',
  imports: [CommonModule, FormsModule],
  templateUrl: './currency-calculator.html'
})
export class CurrencyCalculator {
  private ratesService = inject(ExchangeRates);

  amount = signal<number>(100);
  fromCurrency = signal<string>('EUR');
  toCurrency = signal<string>('USD');
  selectedRegion = signal<RegionType>(RegionType.EU);

  calculationResult = signal<any>(null);

  availableCurrencies = Object.keys(CURRENCY_NAMES);

  loading = this.ratesService.loadingState;
  error = this.ratesService.errorState;
  currentRates = this.ratesService.currentRatesState;

  availableTargetCurrencies = computed(() => 
    this.availableCurrencies.filter(currency => currency !== this.fromCurrency())
  );

  getCurrencyName = getCurrencyName;

  ngOnInit() {
    this.ratesService.loadCurrentRates(this.selectedRegion());
  }

  calculateExchange() {
    const request = {
      amount: this.amount(),
      fromCurrency: this.fromCurrency(),
      toCurrency: this.toCurrency(),
      region: this.selectedRegion()
    };

    this.ratesService.calculateExchange(request, (result) => {
      if (result) {
        this.calculationResult.set(result);
      }
    });
  }

  swapCurrencies() {
    const currentFrom = this.fromCurrency();
    const currentTo = this.toCurrency();
    
    this.fromCurrency.set(currentTo);
    this.toCurrency.set(currentFrom);

    if (this.calculationResult()) {
      this.calculateExchange();
    }
  }

  onRegionChange(region: RegionType) {
    this.selectedRegion.set(region);
    this.ratesService.loadCurrentRates(region);

    if (this.calculationResult()) {
      setTimeout(() => this.calculateExchange(), 500);
    }
  }

  formatRate(rate: number): string {
    return rate.toFixed(4);
  }

  formatAmount(amount: number): string {
    return amount.toFixed(2);
  }
}
