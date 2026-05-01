# AirQualityAIAPI

REST API for managing air quality readings, built with **ASP.NET Core 8 Web API** following **Clean Architecture** principles.

---

## Architecture

```
AirQualityAIAPI/
├── src/
│   ├── AirQualityAIAPI.Domain/          # Core domain (no external dependencies)
│   │   ├── Entities/                    # Domain entities (AirQualityReading)
│   │   └── Interfaces/                  # Repository contracts (IAirQualityRepository)
│   │
│   ├── AirQualityAIAPI.Application/     # Use cases / application logic
│   │   ├── DTOs/                        # Request & response DTOs
│   │   ├── Interfaces/                  # Service contracts (IAirQualityService)
│   │   └── Services/                    # Service implementations (AirQualityService)
│   │
│   ├── AirQualityAIAPI.Infrastructure/  # External concerns
│   │   ├── Repositories/               # In-memory repository (InMemoryAirQualityRepository)
│   │   └── Extensions/                 # DI registration helpers
│   │
│   └── AirQualityAIAPI.Api/             # Presentation layer (entry point)
│       ├── Controllers/                 # HTTP controllers (AirQualityController)
│       ├── Properties/                  # Launch settings
│       ├── Program.cs                   # App bootstrap & DI composition root
│       └── appsettings*.json
│
└── AirQualityAIAPI.sln
```

### Dependency rules

```
Domain  <──  Application  <──  Infrastructure
                               ↑
                              Api
```

- **Domain** has zero external dependencies.
- **Application** depends only on **Domain**.
- **Infrastructure** depends on **Application** + **Domain** (implements their interfaces).
- **Api** depends on **Application** (service interfaces) and **Infrastructure** (for DI wiring only).

---

## Persistence

The current implementation uses an **in-memory repository** (`InMemoryAirQualityRepository`).  
Data is lost on restart — suitable for development and demos.  
To switch to a real database (e.g. SQL Server / PostgreSQL via EF Core), implement `IAirQualityRepository` in Infrastructure and swap the DI registration in `InfrastructureServiceExtensions`.

---

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/airquality` | List all readings |
| `GET` | `/api/airquality/{id}` | Get a reading by ID |
| `GET` | `/api/airquality/location/{location}` | Get readings by location |
| `POST` | `/api/airquality` | Create a new reading |
| `PUT` | `/api/airquality/{id}` | Update an existing reading |
| `DELETE` | `/api/airquality/{id}` | Delete a reading |

### Sample `POST` body

```json
{
  "location": "Madrid",
  "airQualityIndex": 45,
  "pm25": 12.5,
  "pm10": 20.3,
  "co2": 400,
  "no2": 15.2
}
```

The `category` field is automatically computed from `airQualityIndex`:

| AQI range | Category |
|-----------|----------|
| 0–50 | Good |
| 51–100 | Moderate |
| 101–150 | Unhealthy for Sensitive Groups |
| 151–200 | Unhealthy |
| 201–300 | Very Unhealthy |
| 301+ | Hazardous |

---

## How to run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build

```bash
dotnet build AirQualityAIAPI.sln
```

### Run

```bash
cd src/AirQualityAIAPI.Api
dotnet run
```

The API will be available at `http://localhost:5000`.  
Swagger UI (Development only): `http://localhost:5000/swagger`

---

## Where each responsibility lives

| Concern | Location |
|---------|----------|
| Domain entities & repository contracts | `AirQualityAIAPI.Domain` |
| Business logic / use-case orchestration | `AirQualityAIAPI.Application` |
| In-memory data store | `AirQualityAIAPI.Infrastructure` |
| HTTP controllers & DI setup | `AirQualityAIAPI.Api` |
