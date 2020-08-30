# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Exchange.Rates.Ecb.OpenApi/Exchange.Rates.Ecb.OpenApi.csproj", "Exchange.Rates.Ecb.OpenApi/"]
COPY ["Exchange.Rates.Contracts/Exchange.Rates.Contracts.csproj", "Exchange.Rates.Contracts/"]
COPY ["Exchange.Rates.Core/Exchange.Rates.Core.csproj", "Exchange.Rates.Core/"]
RUN dotnet restore "./Exchange.Rates.Ecb.OpenApi/Exchange.Rates.Ecb.OpenApi.csproj"
COPY . .
WORKDIR "/src/Exchange.Rates.Ecb.OpenApi"
RUN dotnet build "./Exchange.Rates.Ecb.OpenApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Exchange.Rates.Ecb.OpenApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exchange.Rates.Ecb.OpenApi.dll"]