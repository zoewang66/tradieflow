# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/TradieFlow.Domain/*.csproj         src/TradieFlow.Domain/
COPY src/TradieFlow.Application/*.csproj     src/TradieFlow.Application/
COPY src/TradieFlow.Infrastructure/*.csproj  src/TradieFlow.Infrastructure/
COPY src/TradieFlow.Api/*.csproj             src/TradieFlow.Api/
RUN dotnet restore src/TradieFlow.Api/TradieFlow.Api.csproj

COPY . .
RUN dotnet publish src/TradieFlow.Api/TradieFlow.Api.csproj -c Release -o /app

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TradieFlow.Api.dll"]
