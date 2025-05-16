# Use the official .NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY foto_manager/ foto_manager/
COPY foto_manager/foto_manager.csproj foto_manager/
RUN dotnet restore foto_manager/foto_manager.csproj
WORKDIR /src/foto_manager
RUN dotnet publish -c Release -o /app/publish --no-restore

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "foto_manager.dll"]
