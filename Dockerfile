# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Marketplace.API/Marketplace.API.csproj", "src/Marketplace.API/"]
COPY ["src/Marketplace.Application/Marketplace.Application.csproj", "src/Marketplace.Application/"]
COPY ["src/Marketplace.Domain/Marketplace.Domain.csproj", "src/Marketplace.Domain/"]
COPY ["src/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj", "src/Marketplace.Infrastructure/"]
RUN dotnet restore "src/Marketplace.API/Marketplace.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Marketplace.API"
RUN dotnet build "Marketplace.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Marketplace.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.API.dll"]
