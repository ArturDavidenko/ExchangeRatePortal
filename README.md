# Exchange Rate Portal

## 1. Project Overview
A web application for tracking and analyzing currency exchange rates from the Bank of Lithuania. 
The portal provides real-time exchange rates, historical data visualization, and currency conversion capabilities. 
Rates are automatically synchronized daily from the LB.lt web service, with initial data populated for the last 90 days.

Key functionality:
- Display current exchange rates from Bank of Lithuania
- Show historical exchange rate data with charts/tables
- Currency calculator for amount conversion
- Automated daily rate updates via Quartz scheduler

## 2. Quick Start

### Prerequisites
- Docker Desktop App
- Node.js 18+ 
- Angular CLI

### Installation & Launch

1. **Clone and navigate to the project:**
   ```bash
   git clone https://github.com/ArturDavidenko/ExchangeRatePortal
   cd ExchangeRates
   ```
   
2. **Start backend services with Docker:**
  ```bash
  docker compose up -d
  ```

3. **Launch the frontend application:**
  ```bash
  cd ExchangeRatesUI
  npm install
  ng serve
  ```

## Access ponts

- Web Application: http://localhost:4200
- API Documentation: http://localhost:5000/swagger
  
## 3. System Architecture  



## 4. Technology Stack
| Layer | Technology | Version |
|-------|------------|---------|
| Backend | .NET | 8.0 |
| Frontend | Angular | 20+ |
| Database | MongoDB | 6.0 |
| Scheduler | Quartz.NET | 3.8 |
| Containerization | Docker | 24+ |
| Documentation | Swagger | -- |


## 5. Data Volume Calculation




## 6. API Endpoints

## 7. Contacts


