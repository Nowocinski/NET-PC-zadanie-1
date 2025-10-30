FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend-zadanie-1/backend-zadanie-1.csproj", "backend-zadanie-1/"]
RUN dotnet restore "backend-zadanie-1/backend-zadanie-1.csproj"
COPY . .
WORKDIR "/src/backend-zadanie-1"
RUN dotnet build "./backend-zadanie-1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./backend-zadanie-1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend-zadanie-1.dll"]
