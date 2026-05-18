# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SmartInventorySystem.sln ./
COPY SmartInventorySystem.API/SmartInventorySystem.API.csproj SmartInventorySystem.API/
COPY SmartInventorySystem.Application/SmartInventorySystem.Application.csproj SmartInventorySystem.Application/
COPY SmartInventorySystem.Domain/SmartInventorySystem.Domain.csproj SmartInventorySystem.Domain/
COPY SmartInventorySystem.Infrastructure/SmartInventorySystem.Infrastructure.csproj SmartInventorySystem.Infrastructure/

RUN dotnet restore SmartInventorySystem.sln

COPY . .
RUN dotnet publish SmartInventorySystem.API/SmartInventorySystem.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SmartInventorySystem.API.dll"]
