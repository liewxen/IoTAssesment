# IoT Device Management System - User Guide

Welcome to the IoT Device Management System! This guide will help you understand how to use the system effectively for managing your IoT devices and working with MQTT communication.

## 📋 Table of Contents

1. [How to Run the Project](#how-to-run-the-project)
2. [Getting Started](#getting-started)
3. [Device Management (CRUD Operations)](#device-management-crud-operations)
4. [MQTT Communication](#mqtt-communication)
5. [Telemetry Data Management](#telemetry-data-management)
6. [Dashboard & Monitoring](#dashboard--monitoring)
7. [Troubleshooting](#troubleshooting)

---

## 🐳 How to Run the Project

### Prerequisites

- **Docker Desktop** installed on your machine
  - Download from: https://www.docker.com/products/docker-desktop
  - Make sure Docker Desktop is **running** before proceeding

### Quick Start (3 Steps)

#### Step 1: Download/Clone the Project

```bash
# If using Git
git clone <repository-url>
cd IoTAssessment

# Or extract the downloaded ZIP file
# Navigate to the extracted folder
```

#### Step 2: Configure Settings (Optional)

Open `docker-compose.yml` and customize if needed:

```yaml
# Default configuration (works out of the box):
postgres:
  environment:
    POSTGRES_PASSWORD: postgres  # Change if desired

iot-app:
  environment:
    # Database connection
    ConnectionStrings__DefaultConnection: "Host=postgres;Database=IoTDeviceManagement;Username=postgres;Password=postgres"
    
    # MQTT settings
    MQTT__BrokerHost: mosquitto
    MQTT__BrokerPort: 1883
```

**Note:** The default settings work perfectly for local development. You only need to change them if you want custom passwords or configurations.

#### Step 3: Run Docker Compose

Open **PowerShell** or **Terminal** in the project directory:

```powershell
# Windows PowerShell - Easy setup script
.\setup-docker.ps1

# Or manually start all services
docker-compose up -d
```

```bash
# Linux/Mac - Easy setup script
chmod +x setup-docker.sh
./setup-docker.sh

# Or manually start all services
docker-compose up -d
```

**That's it!** Wait 30-60 seconds for all services to start.

### Verify Everything is Running

```powershell
# Check container status
docker-compose ps

# You should see:
# - iot-app       (healthy)
# - iot-postgres  (healthy)
# - iot-mosquitto (healthy)
```

### Access the Application

Open your browser and go to: **http://localhost:5212**

🎉 **Your IoT Device Management System is now running!**

### What's Running?

| Service | Description | URL/Port |
|---------|-------------|----------|
| **Web Application** | ASP.NET Core app | http://localhost:5212 |
| **PostgreSQL Database** | Stores devices, telemetry, logs | localhost:5432 |
| **Mosquitto MQTT Broker** | Handles device communication | localhost:1883 |

### Useful Commands

```powershell
# View logs
docker-compose logs -f

# Stop all services
docker-compose stop

# Start services again
docker-compose start

# Stop and remove everything
docker-compose down

# Stop and remove everything INCLUDING data
docker-compose down -v
```

### Troubleshooting Docker Setup

**Problem: "Cannot connect to Docker daemon"**
- Solution: Make sure Docker Desktop is running

**Problem: "Port already in use"**
- Solution: Change the port in `docker-compose.yml`:
  ```yaml
  ports:
    - "5213:8080"  # Changed from 5212 to 5213
  ```

**Problem: Services not starting**
- Solution: Check logs: `docker-compose logs iot-app`
- Solution: Restart: `docker-compose restart`

For more detailed Docker instructions, see [DOCKER_SETUP_GUIDE.md](DOCKER_SETUP_GUIDE.md)

---

## 🚀 Getting Started

### Accessing the System

1. **Open your web browser** and navigate to: http://localhost:5212 (or your configured port)
2. **Dashboard**: The main page shows an overview of all your devices
3. **Navigation**: Use the top menu to access different sections:
   - **Dashboard**: Overview and statistics
   - **Devices**: Manage your IoT devices
   - **Logs**: View system activity and device logs

---

## 🔧 Device Management (CRUD Operations)

### 📱 Viewing Devices

#### Dashboard Overview
- **Total Devices**: See the count of all registered devices
- **Online/Offline Status**: Monitor device connectivity in real-time
- **Recent Activity**: View latest device actions and events

#### Device List
1. Click **"Devices"** in the navigation menu
2. **Filter Options**:
   - **Search**: Type device name or serial number
   - **Device Type**: Filter by sensor type, access control, etc.
   - **Status**: Show only online or offline devices
   - **Location**: Filter by installation location

### ➕ Adding New Devices

#### Step-by-Step Process:

1. **Navigate to Add Device**:
   - Go to Devices → Click **"Add New Device"** button

2. **Fill Basic Information**:
   ```
   Device Name*: Temperature Sensor - Lab A
   Device Type*: Temperature Sensor
   Location*: Laboratory A - Room 101
   Description: Monitors temperature for sensitive equipment
   ```

3. **Technical Details**:
   ```
   Serial Number: TEMP-LAB-001
   Firmware Version: 1.0.0
   Manufacturer: YourCompany
   Model Number: TS-100
   ```

4. **Initial Configuration**:
   - ☑️ **Device is Online** (if currently connected)
   - Set initial sensor values if known

5. **Click "Create Device"**

#### Device Types Available:
- Temperature Sensor
- Humidity Sensor
- Environmental Sensor
- Access Control
- Security Sensor
- Motion Detector
- Smart Lock
- Camera
- Air Quality Monitor
- Pressure Sensor
- Light Sensor
- Sound Sensor

### ✏️ Editing Devices

1. **From Device List**:
   - Click the **Edit** button (pencil icon) next to any device

2. **From Device Details**:
   - Click **"Edit Device"** button

3. **Update Information**:
   - Modify any field except Device ID
   - **Real-time Actions**:
     - Toggle device status (Online/Offline)
     - Request fresh sensor data
     - Update configuration

4. **Save Changes**: Click **"Update Device"**

### 🔍 Device Details

#### Viewing Detailed Information:
1. Click the **"View"** button (eye icon) or device name
2. **Information Displayed**:
   - **Status Card**: Current online/offline status with connection quality
   - **Basic Info**: Name, type, location, description
   - **Technical Specs**: Serial number, firmware, manufacturer
   - **Sensor Data**: Real-time readings (temperature, humidity, battery)
   - **Timeline**: Creation, last update, last seen dates
   - **Recent Activity**: Latest device logs and events

#### Quick Actions from Details:
- **Toggle Status**: Change online/offline status
- **Request Data**: Ask device for fresh sensor readings
- **View Logs**: See all device activity history
- **Edit**: Modify device settings

### 🗑️ Deleting Devices

#### Safety Features:
- **Double Confirmation**: System requires explicit confirmation
- **Type Device Name**: Must type exact device name to confirm
- **Warning Display**: Shows what will be permanently deleted

#### Deletion Process:
1. **Navigate**: Device Details → Click **"Delete"** button
2. **Review Warning**: See what data will be permanently removed
3. **Confirmation Steps**:
   - ☑️ Check: "I understand this action cannot be undone"
   - Type exact device name in confirmation field
4. **Final Confirmation**: Click **"Yes, Delete Device Permanently"**

#### What Gets Deleted:
- ✗ Device configuration and settings
- ✗ All sensor data and telemetry readings
- ✗ Device status and connectivity history
- ✗ All activity logs and events
- ✗ MQTT communication history
- ✗ All related database entries

> **⚠️ Important**: Physical hardware is NOT affected, only data in the management system.

---

## 📡 MQTT Communication

### Understanding MQTT Topics

The system uses **standardized topic structure** where all devices use the same topics, identified by their **Client ID**:

```
v1/telemetry    → Send sensor data and telemetry
v1/status       → Send device online/offline status
v1/heartbeat    → Send periodic keepalive signals
v1/error        → Send error messages and alerts
v1/command      → Receive commands from the system
v1/response     → Send command acknowledgments
```

**Key Changes from Traditional MQTT**:
- ✅ All devices use identical topic names
- ✅ Device identification via MQTT Client ID (not topic path)
- ✅ Simplified topic structure for easier management
- ✅ Standardized payload formats across all devices

### 📤 Sending Commands to Devices

#### Via Web Interface:
1. **Device Details Page**:
   - Click **"Request Fresh Data"** to get latest sensor readings
   - Click **"Toggle Status"** to change online/offline state

2. **API Endpoints**:
   ```bash
   # Toggle device status
   POST /api/devices/{id}/toggle-status

   # Request sensor data
   POST /api/devices/{id}/request-sensor-data
   ```

#### Via MQTT Client:

**Example Commands**:

1. **Request Sensor Data**:
   ```bash
   # Topic: devices/1/command
   # Message:
   {
     "DeviceId": 1,
     "Command": "REQUEST_SENSOR_DATA",
     "Timestamp": "2023-11-08T10:30:00Z"
   }
   ```

2. **Firmware Update Command**:
   ```bash
   # Topic: devices/1/command
   # Message:
   {
     "DeviceId": 1,
     "Command": "FIRMWARE_UPDATE",
     "Payload": {
       "FirmwareVersion": "1.2.4",
       "DownloadUrl": "https://your-server.com/firmware/1.2.4.bin"
     },
     "Timestamp": "2023-11-08T10:30:00Z"
   }
   ```

### 📥 Receiving Data from Devices

#### Device Status Updates:
```bash
# Topic: devices/1/status
# Message from device:
{
  "DeviceId": 1,
  "IsOnline": true,
  "Timestamp": "2023-11-08T10:30:00Z"
}
```

#### Sensor Data:
```bash
# Topic: devices/1/sensor
# Message from device:
{
  "DeviceId": 1,
  "Temperature": 25.0,
  "Humidity": 60.0,
  "BatteryLevel": 90.0,
  "Timestamp": "2023-11-08T10:30:00Z"
}
```

### 🔧 Testing MQTT with Mosquitto

#### Install Mosquitto:
```bash
# Windows (Chocolatey)
choco install mosquitto

# macOS (Homebrew)
brew install mosquitto

# Ubuntu/Debian
sudo apt-get install mosquitto mosquitto-clients
```

#### Start MQTT Broker:
```bash
mosquitto -v
```

#### Subscribe to All Device Topics:
```bash
mosquitto_sub -h localhost -t "devices/+/+"
```

#### Publish Test Messages:

1. **Simulate Device Telemetry** (Use the device's Client ID from device details):
   ```bash
   mosquitto_pub -h localhost -t "v1/telemetry" -m '{"clientid":"device_001","temperature":25.0,"humidity":60.0,"battery_level":90.0}'
   ```

2. **Simulate Device Status Update**:
   ```bash
   mosquitto_pub -h localhost -t "v1/status" -m '{"clientid":"device_001","status":"online","timestamp":"2024-11-08T10:30:00Z"}'
   ```

3. **Simulate Device Heartbeat**:
   ```bash
   mosquitto_pub -h localhost -t "v1/heartbeat" -m '{"clientid":"device_001","timestamp":"2024-11-08T10:30:00Z"}'
   ```

**Note**: Replace `device_001` with your actual device's Client ID (found in device details page).

---

## 📊 Telemetry Data Management

### Understanding the Generic Data Model

The system uses a flexible, generic approach to store device data:

#### Key Components:
1. **KeyDictionary**: Defines available attributes (temperature, humidity, etc.)
2. **Telemetry**: Stores actual values with generic data types
3. **Partitioning**: Time-based data partitioning for performance

#### Data Types Supported:
- **Double**: Decimal numbers (temperature: 22.5°C)
- **Long**: Integers (uptime: 3600 seconds)
- **String**: Text values (status: "operational")
- **JSON**: Complex objects (GPS coordinates, configurations)

### 📈 Viewing Telemetry Data

#### Real-time Data:
- **Device Details**: Shows latest sensor readings
- **Dashboard**: Displays current values for all devices
- **Auto-refresh**: Pages update every 30 seconds

#### Historical Data:
- **Device Logs**: View chronological activity
- **API Access**: Query historical telemetry via REST API

### 🔍 Querying Telemetry via API

#### Get Latest Values:
```bash
GET /api/devices/1
# Returns device info with latest telemetry values
```

#### Get Device Logs:
```bash
GET /api/devices/1/logs?count=20
# Returns recent device activity logs
```

#### Filter Logs:
```bash
GET /api/logs?deviceId=1&action=SensorUpdate&fromDate=2023-11-01
# Returns filtered activity logs
```

---

## 📊 Dashboard & Monitoring

### Main Dashboard Features

#### Statistics Cards:
- **Total Devices**: Count of all registered devices
- **Online Devices**: Currently connected devices with percentage
- **Offline Devices**: Disconnected devices with percentage
- **Low Battery**: Devices with battery below 25%

#### Device Overview:
- **Recent Devices**: Newly added devices
- **Recent Activity**: Latest system events and device actions
- **Device Types**: Breakdown by sensor/device type
- **Locations**: Distribution across installation sites

#### Low Battery Alerts:
- **Warning Cards**: Highlighted devices needing attention
- **Battery Status**: Visual indicators for battery levels
- **Action Buttons**: Quick access to device management

### 📱 Real-time Monitoring

#### Connection Status:
- **Green Badge**: Device online and responsive
- **Red Badge**: Device offline or unreachable
- **Connection Quality**: Excellent, Good, Fair, Poor based on last contact

#### Auto-refresh:
- **30-second intervals**: Automatic page updates
- **Real-time MQTT**: Live data from connected devices
- **Status changes**: Immediate notification of device state changes

---

## 🔧 Troubleshooting

### Common Issues & Solutions

#### 1. Device Not Appearing Online

**Possible Causes**:
- Device not connected to network
- MQTT broker not running
- Incorrect MQTT configuration

**Solutions**:
```bash
# Check MQTT broker status
mosquitto_pub -h localhost -t test -m "hello"
mosquitto_sub -h localhost -t test

# Monitor all device messages
mosquitto_sub -h localhost -t "v1/#" -v

# Test device telemetry manually (use your device's Client ID)
mosquitto_pub -h localhost -t "v1/telemetry" \
  -m '{"clientid":"device_001","temperature":22.5,"humidity":55.0}'
```

#### 2. Sensor Data Not Updating

**Check These**:
- Device is publishing to `devices/{id}/sensor` topic
- JSON format is correct
- Device ID matches registered device
- MQTT service is running in application

**Test Sensor Update** (use your device's Client ID):
```bash
mosquitto_pub -h localhost -t "v1/telemetry" \
  -m '{"clientid":"device_001","temperature":23.5,"humidity":55.0,"battery_level":85.0}'
```

#### 3. Cannot Delete Device

**Requirements**:
- Check confirmation checkbox
- Type exact device name in confirmation field
- Device name is case-sensitive
- No leading/trailing spaces

#### 4. MQTT Connection Issues

**Configuration Check** (`appsettings.json`):
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

**Test MQTT Connectivity**:
```bash
# Test if broker is running
telnet localhost 1883

# Check application logs for MQTT errors
# Look for "MQTT service started successfully" message
```

#### 5. Database Connection Issues

**PostgreSQL Check**:
```bash
# Test database connection
psql -h localhost -U postgres -d IoTDeviceManagement

# Verify connection string in appsettings.json
"DefaultConnection": "Host=localhost;Database=IoTDeviceManagement;Username=postgres;Password=your_password"
```

#### 6. Performance Issues with Large Data

**Partitioning Configuration**:
```json
{
  "Telemetry": {
    "Partitioning": {
      "Enabled": true,
      "Type": "Monthly",
      "RetentionDays": 365,
      "AutoCreatePartitions": true
    }
  }
}
```

---

## 🎯 Best Practices

### Device Naming
- ✅ Use descriptive names: "Temperature Sensor - Server Room"
- ✅ Include location: "Parking Lot Motion Detector"
- ✅ Keep names unique across the system
- ❌ Avoid generic names: "Sensor 1", "Device A"

### MQTT Communication
- ✅ Use structured JSON messages
- ✅ Include timestamps in all messages
- ✅ Handle connection failures gracefully
- ✅ Implement retry logic in devices
- ❌ Don't send excessive data (respect bandwidth)

### Data Management
- ✅ Regular monitoring of device status
- ✅ Set up alerts for offline devices
- ✅ Monitor battery levels proactively
- ✅ Regular firmware updates
- ❌ Don't ignore low battery warnings

### Security
- ✅ Use authentication for MQTT if required
- ✅ Encrypt sensitive data
- ✅ Regular backup of device configurations
- ❌ Don't expose MQTT broker to internet without security

---

## 📚 API Quick Reference

### Device Management
```bash
GET    /api/devices                    # List all devices
GET    /api/devices/{id}               # Get device by ID
POST   /api/devices                    # Create new device
PUT    /api/devices/{id}               # Update device
DELETE /api/devices/{id}               # Delete device
POST   /api/devices/{id}/toggle-status # Toggle online/offline
```

### Telemetry & Logs
```bash
GET /api/devices/{id}/logs             # Get device logs
GET /api/logs                          # Get all logs (with filters)
GET /api/logs/recent                   # Get recent activity
GET /api/dashboard                     # Get dashboard data
GET /api/dashboard/statistics          # Get device statistics
```

### Health Check
```bash
GET /health                            # Application health status
```

---

## 💡 Tips for Success

1. **Start Small**: Begin with 1-2 test devices to understand the system
2. **Monitor Regularly**: Check the dashboard daily for device health
3. **Test MQTT**: Use mosquitto clients to verify device communication
4. **Document Devices**: Keep good descriptions and location information
5. **Plan for Scale**: Use the partitioning features for large deployments
6. **Backup Data**: Regular database backups for critical installations

---

## 🆘 Getting Help

If you encounter issues not covered in this guide:

1. **Check Application Logs**: Look for error messages in the console
2. **Verify Configuration**: Ensure database and MQTT settings are correct
3. **Test Components**: Use mosquitto tools to test MQTT connectivity
4. **Database Health**: Verify PostgreSQL is running and accessible
5. **GitHub Issues**: Report bugs or request features at the project repository

---

**🎉 Congratulations!** You're now ready to effectively manage your IoT devices using this system. Start with a few test devices and gradually expand your deployment as you become more familiar with the features.

---

*Last Updated: November 2023*
*System Version: 1.0.0*