# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/MeraStore.Services.Logging.Api/MeraStore.Services.Logging.Api.csproj", "src/MeraStore.Services.Logging.Api/"]
COPY ["src/MeraStore.Services.Logging.Application/MeraStore.Services.Logging.Application.csproj", "src/MeraStore.Services.Logging.Application/"]
COPY ["src/MeraStore.Services.Logging.Domain/MeraStore.Services.Logging.Domain.csproj", "src/MeraStore.Services.Logging.Domain/"]
COPY ["src/MeraStore.Services.Logging.Infrastructure/MeraStore.Services.Logging.Infrastructure.csproj", "src/MeraStore.Services.Logging.Infrastructure/"]

ARG NUGET_PASSWORD
RUN dotnet nuget add source "https://pkgs.dev.azure.com/merastore/MeraStore/_packaging/mera-store-feed/nuget/v3/index.json" --name "mera-store" --username "sanjyotagureddy" --password $NUGET_PASSWORD --store-password-in-clear-text

RUN dotnet restore "./src/MeraStore.Services.Logging.Api/MeraStore.Services.Logging.Api.csproj"
COPY . .
WORKDIR "/src/src/MeraStore.Services.Logging.Api"
RUN dotnet build "./MeraStore.Services.Logging.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MeraStore.Services.Logging.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeraStore.Services.Logging.Api.dll"]