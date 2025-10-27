import { Component, inject, signal } from '@angular/core';
import { ExchangeRates } from '../../services/exchange-rates';
import { CURRENCY_NAMES, getCurrencyName, RegionType } from '../../models/exchange-rates.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-currency-history',
  imports: [CommonModule, FormsModule],
  templateUrl: './currency-history.html',
})
export class CurrencyHistory {
  private ratesService = inject(ExchangeRates);

  selectedRegion = signal<RegionType>(RegionType.EU);
  selectedCurrency = signal<string>('USD');
  selectedDays = signal<number>(90);

  availableCurrencies = Object.keys(CURRENCY_NAMES);

  history = this.ratesService.historyState;
  loading = this.ratesService.loadingState;
  error = this.ratesService.errorState;

  getCurrencyName = getCurrencyName;

  loadHistory() {
    this.ratesService.loadCurrencyHistory(
      this.selectedRegion(),
      this.selectedCurrency(), 
      this.selectedDays()
    );
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  formatRate(rate: number): string {
    return rate.toFixed(4);
  }

  ngOnInit() {
    this.loadHistory();
  }
}
