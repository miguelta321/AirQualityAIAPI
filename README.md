# AirQualityAIAPI

ASP.NET Core 8 Web API project structured following **Clean Architecture**.

---

## Project structure

```
AirQualityAIAPI/
├── src/
│   ├── AirQualityAIAPI.Domain/          # Core – no external dependencies
│   │   ├── Entities/                    # Domain entities go here
│   │   └── Interfaces/                  # Repository / domain-service contracts
│   │
│   ├── AirQualityAIAPI.Application/     # Use-cases / application logic
│   │   ├── DTOs/                        # Request & response objects
│   │   ├── Interfaces/                  # Application-service contracts
│   │   └── Services/                    # Service implementations
│   │
│   ├── AirQualityAIAPI.Infrastructure/  # External concerns (DB, HTTP clients, …)
│   │   ├── Repositories/               # Repository implementations
│   │   └── Extensions/                 # DI registration helpers
│   │
│   └── AirQualityAIAPI.Api/             # Presentation layer (entry point)
│       ├── Controllers/                 # HTTP controllers
│       ├── Properties/
│       └── Program.cs
│
└── AirQualityAIAPI.sln
```

## Dependency rules

```
Domain  <──  Application  <──  Infrastructure
                               ↑
                              Api
```

- **Domain** has zero dependencies on other projects.
- **Application** references only **Domain**.
- **Infrastructure** references **Application** + **Domain**.
- **Api** references **Application** + **Infrastructure** (for DI wiring).

## How to run

```bash
# Build
dotnet build AirQualityAIAPI.sln

# Run the API
cd src/AirQualityAIAPI.Api
dotnet run
```
