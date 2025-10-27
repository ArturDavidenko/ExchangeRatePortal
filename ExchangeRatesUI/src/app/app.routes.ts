import { Routes } from '@angular/router';
import { CurrentRatesTable } from './components/current-rates-table/current-rates-table';
import { CurrencyHistory } from './components/currency-history/currency-history';
import { CurrencyCalculator } from './components/currency-calculator/currency-calculator';

export const routes: Routes = [
    { path: '', component: CurrentRatesTable },
    { path: 'history', component: CurrencyHistory },
    { path: 'calculator', component: CurrencyCalculator },
    { path: '**', redirectTo: '' }
];
