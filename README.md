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

The application follows a clean, layered architecture designed for scalability and maintainability. 
Key architectural decisions were made to ensure efficient data flow, reliable scheduling, and clear separation of concerns.

For detailed discussions on specific architectural choices (technology selection, data flow patterns, 
or scaling considerations), I'm available to walk through the reasoning and alternatives considered.

## Path architecture

ExchangeRates/
├── 📂 ExchangeRatesAPI (Backend)/  
│ ├── 📂 Controllers/ # REST API endpoints  
│ ├── 📂 Services/ # Business logic  
│ ├── 📂 Repositories/ # Service whow work with database  
│ ├── 📂 Models/ # Data contracts  
│ ├── 📂 Jobs/ # Quartz scheduled tasks  
│ └── 📂 Data/ # MongoDB context or other databases  
├── 📂 ExchangeRatesUI (Frontend)/  
│ ├── 📂 src/app/  
│ │ ├── 📂 components/ # UI components  
│ │ ├── 📂 services/ # API clients  
│ │ ├── 📂 models/ # TypeScript interfaces  
├── 📄 docker-compose.yml # Infrastructure  
└── 📄 README.md # This file  

## Architecture diagramm




## 4. Technology Stack
| Layer | Technology | Version |
|-------|------------|---------|
| Backend | .NET | 8.0 |
| Frontend | Angular | 20+ |
| Database | MongoDB | 6.0 |
| Scheduler | Quartz.NET | 3.8 |
| Containerization | Docker | 24+ |
| Documentation | Swagger | -- |

**Backend Architecture**: Clean Architecture with interface-based design allows easy database switching (MongoDB chosen for simplicity with document-based rate data).

**Development Experience**: Docker provides standardized environment setup without IDE dependencies.

**API Documentation**: Swagger enables instant API exploration and testing.


## 5. Data Volume Calculation




## 6. API Endpoints

## 7. Contacts


