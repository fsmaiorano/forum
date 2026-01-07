FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/Forum/Forum.csproj", "src/Forum/"]
RUN dotnet restore "src/Forum/Forum.csproj"
COPY . .
WORKDIR "/src/src/Forum"
RUN dotnet build "./Forum.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Forum.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh && mkdir -p /https
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["/bin/bash", "/docker-entrypoint.sh", "Forum.dll"]
