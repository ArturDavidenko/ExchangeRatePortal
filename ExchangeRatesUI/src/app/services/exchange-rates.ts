import { computed, Injectable, signal } from '@angular/core';
import { CalculationRequest, CalculationResult, CurrencyHistoryPoint, FxRate, RegionType } from '../models/exchange-rates.model';
import { ExchangeRatesApi } from './exchange-rates-api';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRates {
    private currentRates = signal<FxRate | null>(null);
    private history = signal<CurrencyHistoryPoint[]>([]);
    private loading = signal<boolean>(false);
    private error = signal<string | null>(null);

    currentRatesState = computed(() => this.currentRates());
    historyState = computed(() => this.history());
    loadingState = computed(() => this.loading());
    errorState = computed(() => this.error());

    availableCurrencies = computed(() => 
    this.currentRates()?.rates.map(rate => rate.currency) || []
  );

  currentRegion = computed(() => 
    this.currentRates()?.regionType || null
  );

  constructor(private apiService: ExchangeRatesApi) {}

  loadCurrentRates(region: RegionType): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.apiService.getCurrentRates(region).subscribe({
      next: (rates) => {
        this.currentRates.set(rates);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load current rates');
        this.loading.set(false);
      }
    });
  }

  loadCurrencyHistory(region: RegionType, currency: string, days: number = 90): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.apiService.getCurrencyHistory(region, currency, days).subscribe({
      next: (history) => {
        this.history.set(history);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load currency history');
        this.loading.set(false);
      }
    });
  }

  calculateExchange(request: CalculationRequest, callback: (result: CalculationResult | null) => void): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.apiService.calculateExchange(request).subscribe({
      next: (result) => {
        this.loading.set(false);
        callback(result);
      },
      error: (err) => {
        this.error.set('Calculation failed');
        this.loading.set(false);
        callback(null);
      }
    });
  }

  clearError(): void {
    this.error.set(null);
  }
}
