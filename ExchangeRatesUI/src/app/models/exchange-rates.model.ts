export interface FxRate {
  id: string;
  regionType: RegionType;
  date: Date;
  baseCurrency: string;
  rates: CurrencyRate[];
}

export interface CurrencyRate {
  currency: string;
  rate: number;
}

export interface CurrencyHistoryPoint {
  date: Date;
  rate: number;
  currency: string;
  regionType: RegionType;
}

export interface CalculationRequest {
  amount: number;
  fromCurrency: string;
  toCurrency: string;
  region: RegionType;
}

export interface CalculationResult {
  amount: number;
  fromCurrency: string;
  toCurrency: string;
  exchangeRate: number;
  calculatedAmount: number;
  rateDate: Date;
  region: RegionType;
}

export enum RegionType {
  EU = 0,
  LT = 1
}

export const CURRENCY_NAMES: { [key: string]: string } = {
  'USD': 'US Dollar',
  'EUR': 'Euro', 
  'GBP': 'British Pound',
  'JPY': 'Japanese Yen',
  'CAD': 'Canadian Dollar',
  'CHF': 'Swiss Franc',
  'AUD': 'Australian Dollar',
  'CNY': 'Chinese Yuan',
  'SEK': 'Swedish Krona',
  'NOK': 'Norwegian Krone',
  'DKK': 'Danish Krone',
  'PLN': 'Polish Zloty',
  'HUF': 'Hungarian Forint',
  'CZK': 'Czech Koruna',
  'RON': 'Romanian Leu',
  'BGN': 'Bulgarian Lev',
  'TRY': 'Turkish Lira',
  'RUB': 'Russian Ruble',
  'INR': 'Indian Rupee',
  'BRL': 'Brazilian Real',
  'ZAR': 'South African Rand',
  'MXN': 'Mexican Peso',
  'SGD': 'Singapore Dollar',
  'HKD': 'Hong Kong Dollar',
  'NZD': 'New Zealand Dollar',
  'THB': 'Thai Baht',
  'KRW': 'South Korean Won',
  'MYR': 'Malaysian Ringgit',
  'IDR': 'Indonesian Rupiah',
  'PHP': 'Philippine Peso',
  'ILS': 'Israeli Shekel',
  'AED': 'UAE Dirham',
  'SAR': 'Saudi Riyal',
  'QAR': 'Qatari Riyal',
  'KWD': 'Kuwaiti Dinar',
  'BHD': 'Bahraini Dinar',
  'OMR': 'Omani Rial'
};

export function getCurrencyName(currencyCode: string): string {
  return CURRENCY_NAMES[currencyCode] || currencyCode;
}

export function getRegionName(region: RegionType): string {
  return region === RegionType.EU ? 'EU' : 'Lithuania';
}