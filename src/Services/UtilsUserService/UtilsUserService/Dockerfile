FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/Services/UtilsUserService/UtilsUserService/UtilsUserService.csproj ./src/Services/UtilsUserService/UtilsUserService/
COPY src/Packages/UtilityHub.Contracts/UtilityHub.Contracts.csproj        ./src/Packages/UtilityHub.Contracts/
COPY src/Packages/UtilityHub.Bootstrap/UtilityHub.Bootstrap.csproj        ./src/Packages/UtilityHub.Bootstrap/
RUN dotnet restore ./src/Services/UtilsUserService/UtilsUserService/UtilsUserService.csproj

COPY src/Services/UtilsUserService/UtilsUserService/                      ./src/Services/UtilsUserService/UtilsUserService/
COPY src/Packages/                                                        ./src/Packages/

RUN dotnet publish ./src/Services/UtilsUserService/UtilsUserService/UtilsUserService.csproj -c $BUILD_CONFIGURATION -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UtilsUserService.dll"]
