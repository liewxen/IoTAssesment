# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["IoTAssesment/IoTAssesment.csproj", "IoTAssesment/"]
RUN dotnet restore "IoTAssesment/IoTAssesment.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/IoTAssesment"
RUN dotnet build "IoTAssesment.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "IoTAssesment.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the ASP.NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "IoTAssesment.dll"]

