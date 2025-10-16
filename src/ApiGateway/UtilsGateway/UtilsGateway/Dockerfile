FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/ApiGateway/UtilsGateway/UtilsGateway/UtilsGateway.csproj ./src/ApiGateway/UtilsGateway/UtilsGateway/
RUN dotnet restore ./src/ApiGateway/UtilsGateway/UtilsGateway/UtilsGateway.csproj

COPY src/ApiGateway/UtilsGateway/UtilsGateway/ ./src/ApiGateway/UtilsGateway/UtilsGateway/
RUN dotnet publish ./src/ApiGateway/UtilsGateway/UtilsGateway/UtilsGateway.csproj -c $BUILD_CONFIGURATION -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UtilsGateway.dll"]
