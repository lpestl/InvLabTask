# Use the official .NET SDK image for assembly
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy .sln file and needed projects files
COPY *.sln ./
COPY DataProcessorService/DataProcessorService.csproj DataProcessorService/
COPY ModelLayer/ModelLayer.csproj ModelLayer/
# Restore dependencies
RUN dotnet restore DataProcessorService/DataProcessorService.csproj
# Copy all code
COPY . .
# build and publish
RUN dotnet publish DataProcessorService/DataProcessorService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
# Copy builded app
COPY --from=build /app/publish .
# Run app
ENTRYPOINT ["dotnet", "DataProcessorService.dll"]
