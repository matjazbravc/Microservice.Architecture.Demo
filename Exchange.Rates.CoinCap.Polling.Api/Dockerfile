#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Exchange.Rates.CoinCap.Polling.Api/Exchange.Rates.CoinCap.Polling.Api.csproj", "Exchange.Rates.CoinCap.Polling.Api/"]
COPY ["Exchange.Rates.Contracts/Exchange.Rates.Contracts.csproj", "Exchange.Rates.Contracts/"]
COPY ["Exchange.Rates.Core/Exchange.Rates.Core.csproj", "Exchange.Rates.Core/"]
RUN dotnet restore "Exchange.Rates.CoinCap.Polling.Api/Exchange.Rates.CoinCap.Polling.Api.csproj"
COPY . .
WORKDIR "/src/Exchange.Rates.CoinCap.Polling.Api"
RUN dotnet build "Exchange.Rates.CoinCap.Polling.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Exchange.Rates.CoinCap.Polling.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exchange.Rates.CoinCap.Polling.Api.dll"]