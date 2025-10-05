# Use the official .NET SDK image for assembly
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy csproj and restore dependencies
COPY FileParserService/*.csproj ./
RUN dotnet restore
# Copy all code
COPY FileParserService/. ./
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
# Copy builded app
COPY --from=build /app ./
# Run app
ENTRYPOINT ["dotnet", "FileParserService.dll"]