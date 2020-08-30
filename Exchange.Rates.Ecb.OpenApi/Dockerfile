#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Exchange.Rates.Ecb.OpenApi/Exchange.Rates.Ecb.OpenApi.csproj", "Exchange.Rates.Ecb.OpenApi/"]
COPY ["Exchange.Rates.Contracts/Exchange.Rates.Contracts.csproj", "Exchange.Rates.Contracts/"]
COPY ["Exchange.Rates.Core/Exchange.Rates.Core.csproj", "Exchange.Rates.Core/"]
RUN dotnet restore "Exchange.Rates.Ecb.OpenApi/Exchange.Rates.Ecb.OpenApi.csproj"
COPY . .
WORKDIR "/src/Exchange.Rates.Ecb.OpenApi"
RUN dotnet build "Exchange.Rates.Ecb.OpenApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Exchange.Rates.Ecb.OpenApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exchange.Rates.Ecb.OpenApi.dll"]