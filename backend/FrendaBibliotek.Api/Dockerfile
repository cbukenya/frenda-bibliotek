# ─── Build stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Restore dependencies first (layer-cached)
COPY FrendaBibliotek.sln .
COPY FrendaBibliotek.Api/FrendaBibliotek.Api.csproj FrendaBibliotek.Api/
RUN dotnet restore FrendaBibliotek.Api/FrendaBibliotek.Api.csproj

# Copy source and publish
COPY FrendaBibliotek.Api/ FrendaBibliotek.Api/
RUN dotnet publish FrendaBibliotek.Api/FrendaBibliotek.Api.csproj \
    -c Release -o /app/publish --no-restore

# ─── Runtime stage ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "FrendaBibliotek.Api.dll"]
