# IoT Device Management System

A comprehensive ASP.NET Core application for managing IoT devices with real-time MQTT communication, built following clean architecture principles with Entity Framework Core and PostgreSQL.

## 🏗️ Architecture Overview

This application follows a layered architecture pattern:

- **Models**: Entity Framework models for database interaction
- **ViewModels**: Data transfer objects for frontend-backend communication
- **Services**: Business logic layer with dependency injection
- **Controllers**: MVC and API endpoints for device management
- **Extensions**: Dependency injection configuration
- **MQTT Service**: Real-time device communication

## 🚀 Features

### ✅ Core Features
- **Device Management**: Create, read, update, delete IoT devices
- **Real-time Status**: Monitor device online/offline status
- **Device Logs**: Track all device actions and events
- **MQTT Communication**: Send commands and receive data from devices
- **Dashboard**: Overview with statistics and recent activity
- **Filtering & Search**: Advanced filtering by device type, status, and location
- **Pagination**: Efficient data handling for large device lists

### 📊 Device Properties
- Device name, type, and description
- Location tracking
- Battery level monitoring
- Temperature and humidity sensors
- Serial number and firmware version
- Connection status and last seen timestamp

### 📱 User Interface
- Modern Tailwind CSS design
- Real-time status updates
- Interactive device management
- Dashboard with statistics
- Activity logs with filtering
- Toast notifications
- Responsive layout

## 📸 Screenshots & Features

Want to see what the application looks like? Check out our **[Visual Feature Showcase](FEATURE_SHOWCASE.md)** with screenshots of:

- 📊 **Dashboard** - System overview with statistics
- 🔧 **Device Management** - Browse, create, edit devices
- 📱 **Device Details** - Complete device information and sensor data
- 📜 **Activity Logs** - Filter and monitor all system events
- 🛰️ **MQTT Simulator** - Test MQTT without a physical broker

[**→ View Feature Showcase with Screenshots**](FEATURE_SHOWCASE.md)

## 🛠️ Technology Stack

### Backend
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Database
- **MQTTnet** - MQTT communication
- **Dependency Injection** - Service management

### Database
- **PostgreSQL** with optimized schema
- **Entity Framework Code-First** approach
- **Automatic migrations** handling
- **Seed data** for development

### MQTT Integration
- **Real-time device communication**
- **Command publishing** to devices
- **Status updates** from devices
- **Sensor data collection**

## 📋 Prerequisites

### Option 1: Docker (Recommended - Easiest Setup)
- **Docker Desktop** - All services included (app, database, MQTT)
- **No manual configuration needed**
- See [DOCKER_SETUP_GUIDE.md](DOCKER_SETUP_GUIDE.md) for details

### Option 2: Manual Setup
- **.NET 8.0 SDK** or later
- **PostgreSQL** database server
- **MQTT Broker** (optional - Mosquitto recommended for testing)
- **Visual Studio 2022** or **Visual Studio Code** (recommended)

## ⚙️ Installation & Setup

### 🐳 Quick Start with Docker (Recommended)

The easiest way to get started:

```bash
# Windows PowerShell
.\setup-docker.ps1

# Linux/Mac
chmod +x setup-docker.sh
./setup-docker.sh
```

Or manually:

```bash
# Create mosquitto directories
mkdir -p mosquitto/data mosquitto/log

# Start all services
docker-compose up -d

# Access application at http://localhost:5212
```

**That's it!** The application, database, and MQTT broker are all running.

For detailed Docker instructions, see [DOCKER_SETUP_GUIDE.md](DOCKER_SETUP_GUIDE.md)

---

### 📦 Manual Setup (Alternative)

If you prefer to run services manually:

#### 1. Clone the Repository
```bash
git clone <repository-url>
cd IoTAssessment
```

#### 2. Database Configuration

#### Install PostgreSQL
Download and install PostgreSQL from [postgresql.org](https://www.postgresql.org/download/)

#### Configure Connection String
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=IoTDeviceManagement;Username=postgres;Password=your_password_here"
  }
}
```

#### Create Database
The application will automatically create the database and apply migrations on startup.

### 3. MQTT Configuration (Optional)

#### Install Mosquitto Broker (Recommended for testing)
```bash
# Windows (using Chocolatey)
choco install mosquitto

# macOS (using Homebrew)
brew install mosquitto

# Ubuntu/Debian
sudo apt-get install mosquitto mosquitto-clients

# Start Mosquitto
mosquitto -v
```

#### Configure MQTT Settings
Update MQTT settings in `appsettings.json`:

```json
{
  "MQTT": {
    "BrokerHost": "localhost",
    "BrokerPort": 1883,
    "ClientId": "IoTDeviceManager",
    "Username": "",
    "Password": ""
  }
}
```

### 4. Restore Dependencies
```bash
dotnet restore
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

## 🔧 Configuration

### Database Settings

The application uses **PostgreSQL** with Entity Framework Core. Key configuration points:

```csharp
// Connection string format
"Host=localhost;Database=IoTDeviceManagement;Username=postgres;Password=your_password"

// Features enabled:
- Automatic migrations
- Retry on failure
- Connection pooling
- Detailed errors (development)
```

### MQTT Settings

Configure MQTT broker connection:

```json
{
  "MQTT": {
    "BrokerHost": "localhost",     // MQTT broker hostname
    "BrokerPort": 1883,            // MQTT broker port
    "ClientId": "IoTDeviceManager", // Unique client identifier
    "Username": "",                // Authentication (optional)
    "Password": ""                 // Authentication (optional)
  }
}
```

### Logging Configuration

Customize logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "IoTAssesment.Services": "Debug"
    }
  }
}
```

## 📡 API Endpoints

### Device Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/devices` | Get all devices with filtering and pagination |
| GET | `/api/devices/{id}` | Get device by ID |
| POST | `/api/devices` | Create new device |
| PUT | `/api/devices/{id}` | Update existing device |
| DELETE | `/api/devices/{id}` | Delete device |
| POST | `/api/devices/{id}/toggle-status` | Toggle device online/offline status |
| POST | `/api/devices/{id}/request-sensor-data` | Request sensor data from device |

### Logs and Monitoring

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/logs` | Get device logs with filtering |
| GET | `/api/devices/{id}/logs` | Get logs for specific device |
| GET | `/api/logs/recent` | Get recent activity logs |
| GET | `/api/dashboard` | Get dashboard statistics |
| GET | `/api/dashboard/statistics` | Get device statistics |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Application health status |

## 📊 MQTT Topics & Commands

### Device Communication Topics

```
devices/{deviceId}/status     - Device status updates
devices/{deviceId}/sensor     - Sensor data from devices
devices/{deviceId}/command    - Commands to devices
devices/{deviceId}/response   - Device command responses
system/devices/+              - System-wide device events
```

### MQTT Message Examples

#### Send Command to Device
```json
{
  "DeviceId": 1,
  "Command": "REQUEST_SENSOR_DATA",
  "Timestamp": "2023-11-08T10:30:00Z"
}
```

#### Device Status Update
```json
{
  "DeviceId": 1,
  "IsOnline": true,
  "Timestamp": "2023-11-08T10:30:00Z"
}
```

#### Sensor Data from Device
```json
{
  "DeviceId": 1,
  "Temperature": 22.5,
  "Humidity": 45.2,
  "BatteryLevel": 85.5,
  "Timestamp": "2023-11-08T10:30:00Z"
}
```

## 🧪 Testing the Application

### 1. Access the Dashboard
Navigate to `http://localhost:5000` to view the main dashboard.

### 2. Manage Devices
- Click "Devices" in the navigation to view all devices
- Use "Add New Device" to create devices
- Toggle device status using the power button
- View device details and logs

### 3. Test MQTT Communication

#### Using Mosquitto Clients
```bash
# Subscribe to all device topics
# Monitor all MQTT messages
mosquitto_sub -h localhost -t "v1/#" -v

# Publish telemetry data (use your device's Client ID)
mosquitto_pub -h localhost -t "v1/telemetry" \
  -m '{"clientid":"device_001","temperature":25.0,"humidity":60.0,"battery_level":90.0}'

# Publish status update
mosquitto_pub -h localhost -t "v1/status" \
  -m '{"clientid":"device_001","status":"online","timestamp":"2024-11-08T10:30:00Z"}'
```

### 4. Test API Endpoints

#### Get All Devices
```bash
curl -X GET "http://localhost:5000/api/devices"
```

#### Create a Device
```bash
curl -X POST "http://localhost:5000/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Sensor",
    "deviceType": "Temperature Sensor",
    "description": "Test device for API",
    "location": "Test Lab",
    "serialNumber": "TEST-001"
  }'
```

## 🔍 Development & Debugging

### Entity Framework Commands

```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Logging

The application includes comprehensive logging:

- **Device operations** (create, update, delete)
- **MQTT communication** (connect, disconnect, messages)
- **Database operations** (Entity Framework logs)
- **API requests** and responses

### Development Mode Features

When running in Development mode:
- **Detailed error pages** with stack traces
- **Entity Framework sensitive data logging**
- **Developer exception page**
- **Hot reload** support

## 🚀 Deployment

### Production Configuration

1. **Update connection strings** for production database
2. **Configure MQTT broker** for production environment
3. **Set environment variables**:
   ```bash
   ASPNETCORE_ENVIRONMENT=Production
   ```
4. **Update `appsettings.Production.json`**:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Warning"
       }
     }
   }
   ```

### Docker Deployment (Future Enhancement)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IoTAssesment.csproj", "."]
RUN dotnet restore "./IoTAssesment.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "IoTAssesment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IoTAssesment.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IoTAssesment.dll"]
```

## 🗂️ Project Structure

```
IoTAssessment/
├── Controllers/           # MVC and API controllers
│   ├── DashboardController.cs
│   ├── DevicesController.cs
│   ├── HomeController.cs
│   └── LogsController.cs
├── Extensions/           # Dependency injection configuration
│   └── ServiceCollectionExtensions.cs
├── Models/              # Entity Framework models
│   ├── DeviceLog.cs
│   ├── IoTDevice.cs
│   ├── IoTDeviceContext.cs
│   └── ErrorViewModel.cs
├── Services/           # Business logic services
│   ├── DeviceService.cs
│   ├── DeviceLogService.cs
│   ├── IDeviceService.cs
│   ├── IDeviceLogService.cs
│   ├── IMqttService.cs
│   ├── MqttService.cs
│   └── MqttBackgroundService.cs
├── ViewModels/         # Data transfer objects
│   ├── DeviceViewModel.cs
│   └── DeviceLogViewModel.cs
├── Views/             # MVC views (basic implementation)
│   ├── Dashboard/
│   ├── Devices/
│   ├── Logs/
│   └── Shared/
├── Migrations/        # Entity Framework migrations
├── appsettings.json   # Configuration
└── Program.cs        # Application entry point
```

## 📈 Performance Considerations

### Database Optimization
- **Indexed fields** for better query performance
- **Pagination** for large datasets
- **Connection pooling** enabled
- **Retry policies** for transient failures

### MQTT Optimization
- **Asynchronous operations** throughout
- **Connection management** with auto-reconnect
- **Message queuing** for reliability

### Caching Strategy (Future Enhancement)
- **In-memory caching** for frequently accessed data
- **Redis integration** for distributed caching
- **Response caching** for API endpoints

## 🐛 Troubleshooting

### Common Issues

#### Database Connection Issues
```bash
# Check PostgreSQL status
sudo systemctl status postgresql

# Test connection
psql -h localhost -U postgres -d IoTDeviceManagement
```

#### MQTT Connection Issues
```bash
# Test MQTT broker
mosquitto_pub -h localhost -t test -m "Hello World"
mosquitto_sub -h localhost -t test
```

#### Migration Issues
```bash
# Reset database (development only)
dotnet ef database drop
dotnet ef database update
```

## 🤝 Contributing

### Code Standards
- Follow **C# coding conventions**
- Use **async/await** for asynchronous operations
- Implement **comprehensive logging**
- Add **XML documentation** for public APIs
- Follow **SOLID principles**

### Development Workflow
1. Fork the repository
2. Create a feature branch
3. Make changes with tests
4. Submit a pull request

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- **Microsoft** for ASP.NET Core and Entity Framework
- **MQTTnet** for excellent MQTT client library
- **PostgreSQL** team for the robust database
- **Bootstrap** for UI components

---

## 📞 Support

For issues and questions:
1. Check the **troubleshooting section**
2. Review the **application logs**
3. Submit an **issue** on the repository

---

**Built with ❤️ using ASP.NET Core and modern web technologies**