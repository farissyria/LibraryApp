# ==========================================
# STAGE 1: Build Stage
# ==========================================
# Use the full .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
# This layer is cached unless .csproj files change
COPY ["Library.Api/Library.Api.csproj", "Library.Api/"]
COPY ["Library.Application/Library.Application.csproj", "Library.Application/"]
COPY ["Library.Core/Library.Core.csproj", "Library.Core/"]
COPY ["Library.Infrastructure/Library.Infrastructure.csproj", "Library.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "Library.Api/Library.Api.csproj"

# Copy all source code
COPY . .

# Build the application in Release mode
WORKDIR "/src/Library.Api"
RUN dotnet build "Library.Api.csproj" -c Release -o /app/build

# ==========================================
# STAGE 2: Publish Stage
# ==========================================
FROM build AS publish
RUN dotnet publish "Library.Api.csproj" -c Release -o /app/publish

# ==========================================
# STAGE 3: Runtime Stage
# ==========================================
# Use the smaller ASP.NET Core runtime image (no SDK)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=publish /app/publish .

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443

# Set environment variable for container
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point - runs when container starts
ENTRYPOINT ["dotnet", "Library.Api.dll"]