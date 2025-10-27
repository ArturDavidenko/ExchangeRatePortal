import { Injectable } from '@angular/core';
import { environment } from '../envirovment';
import { HttpClient } from '@angular/common/http';
import { CalculationRequest, CalculationResult, CurrencyHistoryPoint, FxRate, RegionType } from '../models/exchange-rates.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRatesApi {
  private baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  getCurrentRates(region: RegionType): Observable<FxRate> {
    return this.http.get<FxRate>(`${this.baseUrl}/current/${region}`);
  }

  getCurrencyHistory(
    region: RegionType, 
    currency: string, 
    days: number = 90
  ): Observable<CurrencyHistoryPoint[]> {
    return this.http.get<CurrencyHistoryPoint[]>(
      `${this.baseUrl}/history/${region}/${currency}?days=${days}`
    );
  }

  calculateExchange(request: CalculationRequest): Observable<CalculationResult> {
    return this.http.post<CalculationResult>(
      `${this.baseUrl}/calculate`,
      request
    );
  }
}
