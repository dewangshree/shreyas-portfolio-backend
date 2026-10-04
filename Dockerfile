FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so NuGet restore can be cached
COPY src/Shreyas.Profile.Api/Shreyas.Profile.Api.csproj src/Shreyas.Profile.Api/
COPY src/Shreyas.Profile.Application/Shreyas.Profile.Application.csproj src/Shreyas.Profile.Application/
COPY src/Shreyas.Profile.Domain/Shreyas.Profile.Domain.csproj src/Shreyas.Profile.Domain/
COPY src/Shreyas.Profile.Infrastructure/Shreyas.Profile.Infrastructure.csproj src/Shreyas.Profile.Infrastructure/

# Restore dependencies
RUN dotnet restore src/Shreyas.Profile.Api/Shreyas.Profile.Api.csproj

# Copy the rest of the source code
COPY . .

# Publish without restoring again
RUN dotnet publish src/Shreyas.Profile.Api/Shreyas.Profile.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet Shreyas.Profile.Api.dll --urls http://0.0.0.0:${PORT:-10000}"]