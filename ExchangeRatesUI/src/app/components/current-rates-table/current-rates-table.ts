import { Component, inject, signal } from '@angular/core';
import { getCurrencyName, getRegionName, RegionType } from '../../models/exchange-rates.model';
import { CommonModule } from '@angular/common';
import { ExchangeRates } from '../../services/exchange-rates';

@Component({
  selector: 'app-current-rates-table',
  imports: [CommonModule],
  templateUrl: './current-rates-table.html',
})
export class CurrentRatesTable {
  private ratesService = inject(ExchangeRates);
  
  currentRates = this.ratesService.currentRatesState;
  loading = this.ratesService.loadingState;
  error = this.ratesService.errorState;
  
  selectedRegion = signal<RegionType>(RegionType.EU);

  ngOnInit() {
    this.loadRates(this.selectedRegion());
  }

  onRegionChange(region: RegionType) {
    this.selectedRegion.set(region);
    this.loadRates(region);
  }

  private loadRates(region: RegionType) {
    this.ratesService.loadCurrentRates(region);
  }

  getCurrencyName = getCurrencyName;
  getRegionName = getRegionName;

  formatRate(rate: number): string {
    return rate.toFixed(4);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
