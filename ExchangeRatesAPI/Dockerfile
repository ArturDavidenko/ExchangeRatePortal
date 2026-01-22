FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ExchangeRatesAPI.csproj .
RUN dotnet restore ExchangeRatesAPI.csproj

COPY . .
RUN dotnet publish ExchangeRatesAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExchangeRatesAPI.dll"]
