# 🚀 Quick Start - IoT Device Management System

## Run in 3 Steps

### Step 1️⃣: Navigate to Project Folder

```bash
cd IoTAssessment
```

### Step 2️⃣: (Optional) Edit Configuration

Open `docker-compose.yml` if you want to change:
- Database password
- MQTT settings
- Port numbers

**Default settings work perfectly!** Skip this step if you're okay with defaults.

### Step 3️⃣: Start Everything

```powershell
# Windows
.\setup-docker.ps1
```

```bash
# Linux/Mac
./setup-docker.sh
```

**Or manually:**

```bash
docker-compose up -d
```

---

## ✅ Done!

**Open:** http://localhost:5212

---

## 📊 What You Get

✅ **Web Application** - Full IoT management interface  
✅ **PostgreSQL Database** - All data stored automatically  
✅ **MQTT Broker** - Device communication ready  

---

## 🎯 Quick Test

1. Open http://localhost:5212
2. Go to **"Devices"** → Create a device
3. Go to **"MQTT Simulator"** → Send telemetry
4. Check device details → See the data!

---

## 🛑 Stop/Start Commands

```bash
# Stop
docker-compose stop

# Start
docker-compose start

# Remove everything
docker-compose down

# Remove everything + data
docker-compose down -v
```

---

## 📚 Need More Help?

- **USER_GUIDE.md** - Complete usage guide
- **DOCKER_SETUP_GUIDE.md** - Detailed Docker instructions
- **README.md** - Project overview

---

## ⚡ That's It!

**No manual database setup**  
**No MQTT broker installation**  
**No configuration needed**  

Just run `docker-compose up -d` and you're ready! 🎉

