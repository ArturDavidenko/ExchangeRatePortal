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

### Backend Architecture  
- **Layered Clean Architecture** with clear separation of concerns  
- **Repository Pattern** for database abstraction (MongoDB implementation)  
- **Service Layer** containing business logic and coordinating repositories  
- **Controller Layer** handling HTTP requests and responses  
- **Scheduled Jobs** (Quartz.NET) for automated rate updates  
- **Helper/Utility classes** following Single Responsibility Principle (SOLID)  

### Frontend Architecture    
- **Component-based Structure** with reusable UI components  
- **Service Layer** for API communication and business logic  
- **TypeScript Interfaces** for type safety and contracts  
- **Modular Design** enabling easy feature additions  

### Key Benefits  
- **Scalable Foundation** - Easy to add new features and endpoints  
- **Testable Code** - Clear boundaries enable unit testing  
- **Maintainable** - Changes localized to specific layers  
- **Future-proof** - Ready for database switching or additional data sources  

## Path architecture

ExchangeRates/  
├── 📂 ExchangeRatesAPI (Backend)/  
│ ├── 📂 Controllers/   # REST API endpoints  
│ ├── 📂 Services/   # Business logic  
│ ├── 📂 Repositories/   # Service who work with database  
│ ├── 📂 Models/   # Data contracts  
│ ├── 📂 Jobs/   # Quartz scheduled tasks  
│ ├── 📂 Helper/  # Additionals functions to use  
│ └── 📂 Data/   # MongoDB context or other databases  
├── 📂 ExchangeRatesUI (Frontend)/  
│ ├── 📂 src/app/  
│ │ ├── 📂 components/   # UI components    
│ │ ├── 📂 services/   # Business logic  
│ │ ├── 📂 models/   # TypeScript interfaces    
├── 📄 docker-compose.yml   # Infrastructure  
└── 📄 README.md   # This file  

## 4. Technology Stack
| Layer | Technology | Version |
|-------|------------|---------|
| Backend | .NET | 8.0 |
| Frontend | Angular | 20+ |
| Database | MongoDB | 6.0 |
| Scheduler | Quartz.NET | 3.8 |
| Containerization | Docker | 24+ |
| Documentation | Swagger | -- |

## 5.  API Endpoints

## 6. Data Volume Calculation

## 7. Contacts


