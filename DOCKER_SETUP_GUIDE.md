# Docker Setup Guide - IoT Device Management System

## 🐳 What's Included

This Docker Compose setup includes:
- ✅ **ASP.NET Core 8.0 Application** - Your IoT management app
- ✅ **PostgreSQL 16** - Database for devices, telemetry, and logs
- ✅ **Eclipse Mosquitto 2.0** - MQTT broker for device communication
- ✅ **Health Checks** - Automatic service monitoring
- ✅ **Persistent Storage** - Data survives container restarts
- ✅ **Network Isolation** - Secure container networking

---

## 📋 Prerequisites

### Install Docker Desktop

**Windows / Mac**:
1. Download from: https://www.docker.com/products/docker-desktop
2. Install and start Docker Desktop
3. Verify installation:
   ```bash
   docker --version
   docker-compose --version
   ```

**Linux**:
```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify
docker --version
docker-compose --version
```

---

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd IoTAssesment
```

### 2. Create Mosquitto Directories
```bash
# Windows (PowerShell)
New-Item -ItemType Directory -Force -Path mosquitto\data
New-Item -ItemType Directory -Force -Path mosquitto\log

# Linux/Mac
mkdir -p mosquitto/data mosquitto/log
```

### 3. Start All Services
```bash
docker-compose up -d
```

**Expected Output:**
```
Creating network "iot-network" with driver bridge
Creating volume "iot-postgres-data" with local driver
Creating iot-postgres ... done
Creating iot-mosquitto ... done
Creating iot-app       ... done
```

### 4. Wait for Services to Start
```bash
# Check service health
docker-compose ps
```

All services should show "healthy" or "running":
```
NAME             STATUS                    PORTS
iot-app         Up (healthy)              0.0.0.0:5212->8080/tcp
iot-mosquitto   Up (healthy)              0.0.0.0:1883->1883/tcp
iot-postgres    Up (healthy)              0.0.0.0:5432->5432/tcp
```

### 5. Access the Application

Open your browser: **http://localhost:5212**

🎉 **Done!** Your IoT Device Management System is running!

---

## 📊 Service URLs

| Service | URL | Purpose |
|---------|-----|---------|
| **Web Application** | http://localhost:5212 | Main IoT dashboard |
| **Health Check** | http://localhost:5212/health | Service health status |
| **PostgreSQL** | localhost:5432 | Database (user: postgres, pass: postgres) |
| **MQTT Broker** | localhost:1883 | MQTT communication |
| **MQTT WebSocket** | ws://localhost:9001 | MQTT over WebSocket |

---

## 🔧 Docker Commands

### Start Services
```bash
# Start all services in background
docker-compose up -d

# Start and view logs
docker-compose up

# Start specific service
docker-compose up -d iot-app
```

### Stop Services
```bash
# Stop all services
docker-compose stop

# Stop and remove containers
docker-compose down

# Stop and remove containers + volumes (⚠️ DELETES DATA)
docker-compose down -v
```

### View Logs
```bash
# View all logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# View specific service logs
docker-compose logs iot-app
docker-compose logs postgres
docker-compose logs mosquitto

# View last 50 lines
docker-compose logs --tail=50 iot-app
```

### Check Status
```bash
# List running containers
docker-compose ps

# Check service health
docker-compose ps iot-app

# View resource usage
docker stats
```

### Restart Services
```bash
# Restart all
docker-compose restart

# Restart specific service
docker-compose restart iot-app
```

### Execute Commands in Container
```bash
# Open bash in app container
docker-compose exec iot-app /bin/bash

# Open psql in postgres
docker-compose exec postgres psql -U postgres -d IoTDeviceManagement

# Check mosquitto status
docker-compose exec mosquitto mosquitto_sub -t '$SYS/#' -v
```

---

## 🗄️ Database Management

### Connect to PostgreSQL
```bash
# Using docker-compose
docker-compose exec postgres psql -U postgres -d IoTDeviceManagement

# Using psql from host (if installed)
psql -h localhost -U postgres -d IoTDeviceManagement
```

### Common Database Commands
```sql
-- List tables
\dt

-- View devices
SELECT * FROM "IoTDevices";

-- View telemetry
SELECT * FROM "Telemetries" ORDER BY "Timestamp" DESC LIMIT 10;

-- View logs
SELECT * FROM "DeviceLogs" ORDER BY "Timestamp" DESC LIMIT 10;

-- Exit
\q
```

### Backup Database
```bash
# Create backup
docker-compose exec postgres pg_dump -U postgres IoTDeviceManagement > backup.sql

# Restore backup
docker-compose exec -T postgres psql -U postgres IoTDeviceManagement < backup.sql
```

---

## 📡 MQTT Testing

### Test MQTT from Host
```bash
# Subscribe to all topics
docker-compose exec mosquitto mosquitto_sub -t 'v1/#' -v

# Publish test message
docker-compose exec mosquitto mosquitto_pub -t 'v1/telemetry' -m '{"clientid":"SENSOR_001","temperature":25.0,"humidity":60.0,"battery_level":90.0}'
```

### Test MQTT from Outside Docker
```bash
# If you have mosquitto clients installed locally
mosquitto_sub -h localhost -t 'v1/#' -v
mosquitto_pub -h localhost -t 'v1/telemetry' -m '{"clientid":"SENSOR_001","temperature":25.0}'
```

### View MQTT Logs
```bash
docker-compose logs -f mosquitto
```

---

## 🔍 Troubleshooting

### Issue 1: Port Already in Use

**Error:**
```
Error starting userland proxy: listen tcp4 0.0.0.0:5432: bind: address already in use
```

**Solution:**
```bash
# Check what's using the port
# Windows
netstat -ano | findstr :5432

# Linux/Mac
lsof -i :5432

# Option 1: Stop the conflicting service

# Option 2: Change port in docker-compose.yml
# For example, change PostgreSQL port:
ports:
  - "5433:5432"  # Use 5433 instead of 5432
```

### Issue 2: Services Not Healthy

**Check health status:**
```bash
docker-compose ps
```

**View detailed logs:**
```bash
docker-compose logs iot-app
```

**Common fixes:**
```bash
# Restart unhealthy service
docker-compose restart iot-app

# Rebuild if code changed
docker-compose up -d --build

# Reset everything (⚠️ deletes data)
docker-compose down -v
docker-compose up -d
```

### Issue 3: Database Connection Failed

**Check PostgreSQL is running:**
```bash
docker-compose logs postgres
```

**Test connection:**
```bash
docker-compose exec postgres pg_isready -U postgres
```

**Reset database:**
```bash
docker-compose down -v  # Removes volume
docker-compose up -d    # Recreates with fresh database
```

### Issue 4: MQTT Not Connecting

**Check Mosquitto logs:**
```bash
docker-compose logs mosquitto
```

**Test MQTT broker:**
```bash
docker-compose exec mosquitto mosquitto_sub -t test -v
```

**In another terminal:**
```bash
docker-compose exec mosquitto mosquitto_pub -t test -m "hello"
```

### Issue 5: Application Not Starting

**View detailed logs:**
```bash
docker-compose logs -f iot-app
```

**Check if migrations ran:**
```bash
docker-compose logs iot-app | grep -i migration
```

**Rebuild application:**
```bash
docker-compose build --no-cache iot-app
docker-compose up -d iot-app
```

---

## 🔐 Production Configuration

For production deployment, update these settings:

### 1. Update docker-compose.yml

```yaml
# Change PostgreSQL credentials
postgres:
  environment:
    POSTGRES_PASSWORD: your_secure_password_here

# Update app connection string
iot-app:
  environment:
    ConnectionStrings__DefaultConnection: "Host=postgres;Database=IoTDeviceManagement;Username=postgres;Password=your_secure_password_here"
```

### 2. Enable MQTT Authentication

Update `mosquitto/config/mosquitto.conf`:
```conf
allow_anonymous false
password_file /mosquitto/config/passwords.txt
```

Create password file:
```bash
docker-compose exec mosquitto mosquitto_passwd -c /mosquitto/config/passwords.txt admin
```

### 3. Use Environment Variables

Create `.env` file:
```env
POSTGRES_PASSWORD=your_secure_password
MQTT_USERNAME=your_mqtt_user
MQTT_PASSWORD=your_mqtt_password
```

Update docker-compose.yml:
```yaml
environment:
  POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
```

### 4. Enable SSL/TLS

Add SSL certificates and update configurations accordingly.

---

## 📦 Data Persistence

### Where Data is Stored

**PostgreSQL Data:**
- Volume: `iot-postgres-data`
- Location: Docker managed volume

**Mosquitto Data:**
- Directory: `./mosquitto/data/`
- Mosquitto logs: `./mosquitto/log/`

### Backup All Data
```bash
# Create backup directory
mkdir backup

# Backup PostgreSQL
docker-compose exec postgres pg_dump -U postgres IoTDeviceManagement > backup/database.sql

# Backup Mosquitto data
cp -r mosquitto/data backup/mosquitto-data
```

### Restore Data
```bash
# Restore PostgreSQL
docker-compose exec -T postgres psql -U postgres IoTDeviceManagement < backup/database.sql

# Restore Mosquitto
cp -r backup/mosquitto-data/* mosquitto/data/
docker-compose restart mosquitto
```

---

## 🚀 Deployment Tips

### Development
```bash
# Start with logs visible
docker-compose up

# Hot reload (rebuild on code changes)
docker-compose up -d --build
```

### Staging/Production
```bash
# Start in background
docker-compose up -d

# Monitor logs
docker-compose logs -f --tail=100

# Set resource limits in docker-compose.yml:
services:
  iot-app:
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 2G
        reservations:
          cpus: '1'
          memory: 1G
```

### CI/CD Integration
```bash
# Build images
docker-compose build

# Run tests
docker-compose run --rm iot-app dotnet test

# Deploy
docker-compose up -d
```

---

## 📊 Monitoring

### View Resource Usage
```bash
# Real-time stats
docker stats

# Check disk usage
docker system df
```

### Health Checks
```bash
# Application health
curl http://localhost:5212/health

# PostgreSQL health
docker-compose exec postgres pg_isready

# Mosquitto health
docker-compose exec mosquitto mosquitto_sub -t '$SYS/broker/uptime' -C 1
```

---

## 🧹 Cleanup

### Remove Everything
```bash
# Stop and remove containers
docker-compose down

# Remove containers and volumes (⚠️ DELETES DATA)
docker-compose down -v

# Remove images
docker-compose down --rmi all

# Clean up Docker system
docker system prune -a --volumes
```

### Selective Cleanup
```bash
# Remove only stopped containers
docker container prune

# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune
```

---

## 📚 Additional Resources

- **Docker Documentation**: https://docs.docker.com/
- **Docker Compose**: https://docs.docker.com/compose/
- **PostgreSQL Docker**: https://hub.docker.com/_/postgres
- **Mosquitto Docker**: https://hub.docker.com/_/eclipse-mosquitto

---

## 🎯 Quick Command Reference

```bash
# Start
docker-compose up -d

# Stop
docker-compose stop

# Restart
docker-compose restart

# Logs
docker-compose logs -f

# Status
docker-compose ps

# Rebuild
docker-compose up -d --build

# Clean up
docker-compose down

# Full reset (⚠️ deletes data)
docker-compose down -v && docker-compose up -d
```

---

## ✨ Summary

✅ **One Command Setup** - `docker-compose up -d`  
✅ **All Services Included** - App, Database, MQTT Broker  
✅ **Automatic Health Checks** - Ensures services are ready  
✅ **Persistent Data** - Survives container restarts  
✅ **Easy Management** - Simple docker-compose commands  
✅ **Production Ready** - With proper configuration  

**Your IoT system in Docker - Simple, Reliable, Scalable! 🚀**

