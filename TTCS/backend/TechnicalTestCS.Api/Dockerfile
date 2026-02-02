# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first for better layer caching
COPY ./TechnicalTestCS.Api/TechnicalTestCS.Api.csproj ./TechnicalTestCS.Api/
COPY ./TechnicalTestCS.Domain/TechnicalTestCS.Domain.csproj ./TechnicalTestCS.Domain/
COPY ./TechnicalTestCS.Infrastructure/TechnicalTestCS.Infrastructure.csproj ./TechnicalTestCS.Infrastructure/

RUN dotnet restore ./TechnicalTestCS.Api/TechnicalTestCS.Api.csproj

# Copy the rest
COPY . .

RUN dotnet publish ./TechnicalTestCS.Api/TechnicalTestCS.Api.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TechnicalTestCS.Api.dll"]
