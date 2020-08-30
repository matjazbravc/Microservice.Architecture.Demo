#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Exchange.Rates.CoinCap.OpenApi/Exchange.Rates.CoinCap.OpenApi.csproj", "Exchange.Rates.CoinCap.OpenApi/"]
COPY ["Exchange.Rates.Contracts/Exchange.Rates.Contracts.csproj", "Exchange.Rates.Contracts/"]
COPY ["Exchange.Rates.Core/Exchange.Rates.Core.csproj", "Exchange.Rates.Core/"]
RUN dotnet restore "Exchange.Rates.CoinCap.OpenApi/Exchange.Rates.CoinCap.OpenApi.csproj"
COPY . .
WORKDIR "/src/Exchange.Rates.CoinCap.OpenApi"
RUN dotnet build "Exchange.Rates.CoinCap.OpenApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Exchange.Rates.CoinCap.OpenApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exchange.Rates.CoinCap.OpenApi.dll"]