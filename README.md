# Logging Service

## Overview
The **Logging Service** is a centralized logging solution for MeraStore applications. It collects, processes, and stores logs in Elasticsearch for observability and troubleshooting.

## Features
- **Traceability**: Automatic correlation ID, trace ID, and request ID injection.
- **Structured Logging**: Uses Serilog for consistent, structured logs.
- **Elasticsearch Integration**: Sends logs to Elasticsearch for analysis and visualization in Kibana.
- **Log Filtering**: Enforces predefined log schema to avoid unnecessary fields.
- **Docker Support**: Can be containerized and deployed using Docker.

## Getting Started

### Prerequisites
- .NET 9 SDK
- Docker (optional, for containerized deployment)
- Elasticsearch & Kibana (running locally or on a server)

### Installation
Clone the repository:
```sh
git clone https://github.com/MeraStore/logging-service.git

cd logging-service
```

### Configuration
Update the `appsettings.json`:
```json
{
    "ElasticsearchUrl": "http://localhost:9200"
}
```

Or use environment variables:
```sh
export ElasticSearchUrl=http://elastic.search:9200
```

### Running the Service
#### Using .NET CLI
```sh
dotnet run --project src/MeraStore.Services.Logging.Api
```

#### Using Docker
Build and run the container:
```sh
docker build -t logging-service .
docker run -d -p 5000:5000 -e ElasticSearchUrl=http://host.docker.internal:9200 logging-service
```

## API Endpoints
- **Health Check**: `GET /health`
- **Log Ingestion**: `POST /logs`

## Development
### Running Tests
```sh
dotnet test
```

### Linting & Formatting
```sh
dotnet format
```

## Contributing
1. Fork the repo
2. Create a new feature branch (`git checkout -b feature-xyz`)
3. Commit changes (`git commit -m "Add new feature"`)
4. Push to the branch (`git push origin feature-xyz`)
5. Open a pull request

## License
MIT License © 2025 MeraStore
