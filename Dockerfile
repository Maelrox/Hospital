# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Hospital.API/Hospital.API.csproj", "Hospital.API/"]
COPY ["Hospital.Domain/Hospital.Domain.csproj", "Hospital.Domain/"]
COPY ["Hospital.Data/Hospital.Data.csproj", "Hospital.Data/"]
RUN dotnet restore "Hospital.API/Hospital.API.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/Hospital.API"
RUN dotnet build "Hospital.API.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "Hospital.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Install required runtime dependencies
RUN apk add --no-cache icu-libs

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:80 \
    DOTNET_RUNNING_IN_CONTAINER=true

# Copy the published application
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "Hospital.API.dll"] t