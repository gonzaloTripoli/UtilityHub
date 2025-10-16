FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1) Copiar csproj respetando la ESTRUCTURA (./src/...)
COPY src/Services/UtilsAuthService/UtilsAuthService/UtilsAuthService.csproj ./src/Services/UtilsAuthService/UtilsAuthService/
COPY src/Packages/UtilityHub.Contracts/UtilityHub.Contracts.csproj      ./src/Packages/UtilityHub.Contracts/
COPY src/Packages/UtilityHub.Bootstrap/UtilityHub.Bootstrap.csproj      ./src/Packages/UtilityHub.Bootstrap/

# Restore usando la misma ruta que en el repo
RUN dotnet restore ./src/Services/UtilsAuthService/UtilsAuthService/UtilsAuthService.csproj

# 2) Copiar código fuente manteniendo la estructura ./src/...
COPY src/Services/UtilsAuthService/UtilsAuthService/                    ./src/Services/UtilsAuthService/UtilsAuthService/
COPY src/Packages/                                                      ./src/Packages/

# 3) Publish usando la ruta real
RUN dotnet publish ./src/Services/UtilsAuthService/UtilsAuthService/UtilsAuthService.csproj -c $BUILD_CONFIGURATION -o /app

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UtilsAuthService.dll"]
