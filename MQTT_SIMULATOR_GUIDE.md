# MQTT Simulator Guide

## 🎯 Purpose

The MQTT Simulator allows you to **test MQTT functionality without needing an actual MQTT broker**. Perfect for:
- ✅ **Demonstrations and assessments**
- ✅ **Quick testing during development**
- ✅ **Showcasing features without infrastructure setup**
- ✅ **Learning how MQTT messages work**

## 🚀 How to Access

Click **"MQTT Simulator"** in the navigation menu, or visit: `http://localhost:5212/MqttSimulator`

---

## 📋 Features

### 1. **Device Selection**
- Dropdown shows all your registered devices
- Auto-fills the Client ID when you select a device
- Shows device name and Client ID for easy identification

### 2. **Topic Selection**
Choose from standard MQTT topics:
- **v1/telemetry** - Send sensor data (temperature, humidity, battery, etc.)
- **v1/status** - Update device online/offline status
- **v1/heartbeat** - Send device heartbeat signal
- **v1/error** - Report device errors

### 3. **Quick Templates**
One-click templates for common message types:
- 🌡️ **Telemetry** - Pre-filled with temperature, humidity, battery data
- ⚡ **Status** - Device online/offline message
- 💓 **Heartbeat** - Simple heartbeat signal
- ⚠️ **Error** - Error report with message and code

### 4. **JSON Payload Editor**
- Write or paste custom JSON payloads
- **Format JSON** button for pretty printing
- Real-time validation
- Auto-fills Client ID from selected device

### 5. **Response Display**
After sending, you'll see:
- Success/failure status
- Processed message details
- Direct links to view updated device details
- Direct links to view device logs

---

## 🎬 Step-by-Step Usage

### Example 1: Send Telemetry Data

1. **Go to MQTT Simulator page**
   - Click "MQTT Simulator" in the navigation

2. **Select a device**
   - Choose from the dropdown (e.g., "Temperature Sensor")
   - Client ID is auto-filled

3. **Choose template** (optional)
   - Click the "Telemetry" quick template button
   - JSON payload is generated automatically

4. **Customize data** (optional)
   - Edit the JSON to change values:
     ```json
     {
       "clientid": "SENSOR_001",
       "temperature": 28.5,
       "humidity": 65.0,
       "battery_level": 85.0,
       "pressure": 1013.25
     }
     ```

5. **Send message**
   - Click "Send Simulated Message"
   - See success message and response

6. **Verify results**
   - Click "View Device Details" to see updated sensor data
   - Check the telemetry database
   - View activity logs

### Example 2: Update Device Status

1. Select your device
2. Choose topic: **v1/status**
3. Click "Status" template
4. Send the message
5. Device status is now updated!

### Example 3: Send Multiple Sensor Readings

```json
{
  "clientid": "SENSOR_001",
  "temperature": 22.5,
  "humidity": 55.0,
  "battery_level": 90.0,
  "light_level": 450,
  "pressure": 1013.25,
  "co2_level": 400,
  "noise_level": 45.5
}
```

The simulator will:
- ✅ Find or create entries in `KeyDictionaries` table
- ✅ Store all values in `Telemetries` table
- ✅ Update device's sensor data
- ✅ Create activity log entries

---

## 🔍 How It Works

The simulator uses the **same backend logic** as the real MQTT service:

1. **Message Validation**
   - Validates JSON format
   - Checks for required `clientid` field
   - Verifies device exists in database

2. **Device Lookup**
   - Finds device by Client ID
   - Same as real MQTT processing

3. **Data Processing**
   - Calls `TelemetryService.StoreBatchValuesAsync`
   - Auto-creates key dictionary entries
   - Stores values in telemetry table
   - Updates legacy sensor fields
   - Creates activity logs

4. **Response**
   - Returns success/failure
   - Shows processed data
   - Provides navigation links

---

## 💡 Use Cases

### For Assessment/Demo
```
"Let me demonstrate the MQTT telemetry system..."
1. Open MQTT Simulator
2. Select a device
3. Click Telemetry template
4. Send message
5. Show device details page - data updated!
6. Show telemetry database - records created!
7. Show logs page - activity recorded!
```

### For Testing
- Quickly test different sensor values
- Test error handling
- Verify data storage logic
- Test UI updates

### For Development
- No need to set up Mosquitto broker
- No need to configure MQTT clients
- Instant feedback
- Easy to replicate scenarios

---

## 📊 Comparison: Simulator vs Real MQTT

| Feature | MQTT Simulator | Real MQTT |
|---------|----------------|-----------|
| **Setup Required** | None | Install Mosquitto broker |
| **Processing Logic** | Same | Same |
| **Data Storage** | Same | Same |
| **Activity Logs** | Same | Same |
| **Use Case** | Testing/Demo | Production |
| **Speed** | Instant | Instant |
| **Multiple Devices** | Sequential | Concurrent |

---

## 🎯 Tips & Tricks

### Tip 1: Custom Sensor Types
You can send ANY sensor data - it will be auto-stored:
```json
{
  "clientid": "SENSOR_001",
  "custom_sensor_1": 123.45,
  "custom_sensor_2": 678.90,
  "status_message": "All systems operational"
}
```

### Tip 2: Batch Testing
Send multiple messages quickly:
1. Send telemetry
2. Send status
3. Send heartbeat
4. Check device details - all data updated!

### Tip 3: Test Error Cases
Try invalid JSON to see error handling:
```json
{
  "clientid": "NONEXISTENT_DEVICE",
  "temperature": 25.0
}
```
Result: "No device found with Client ID: NONEXISTENT_DEVICE"

### Tip 4: Integration with Real MQTT
The simulator and real MQTT can coexist:
- Use simulator for demos
- Use real MQTT for production
- Both use the same database and logic

---

## 🔧 Advanced Usage

### Sending Complex Data

```json
{
  "clientid": "SENSOR_001",
  "temperature": 22.5,
  "humidity": 55.0,
  "location": {
    "lat": 40.7128,
    "lng": -74.0060
  },
  "sensors": ["temp", "humidity", "pressure"],
  "firmware_version": "1.2.3",
  "uptime_seconds": 86400
}
```

### Error Reporting

```json
{
  "clientid": "SENSOR_001",
  "message": "Temperature sensor malfunction - reading out of range",
  "code": "ERR_TEMP_001",
  "severity": "HIGH",
  "timestamp": "2024-11-08T10:30:00Z"
}
```

---

## ✅ Benefits for Assessment

### 1. **No Infrastructure Required**
- Assessors don't need to install Mosquitto
- No configuration needed
- Works immediately

### 2. **Easy Demonstration**
- Click, fill, send - done!
- Instant visual feedback
- Clear success/error messages

### 3. **Full Functionality**
- Shows complete MQTT processing
- Demonstrates database integration
- Shows activity logging
- Updates UI in real-time

### 4. **Professional Presentation**
- Beautiful modern UI
- Intuitive workflow
- Clear documentation
- Production-quality code

---

## 🚀 Quick Demo Script

**"Let me show you the IoT telemetry system..."**

1. **Navigate to MQTT Simulator**
   - "Here's our MQTT message simulator"

2. **Select Device**
   - "I'll select this Temperature Sensor device"
   - "Note it auto-fills the Client ID"

3. **Load Template**
   - "Click Telemetry template"
   - "Pre-filled with realistic sensor data"

4. **Customize (optional)**
   - "I can adjust values - let's set temperature to 28.5°C"

5. **Send Message**
   - "Click Send"
   - "Success! Stored 5 telemetry values"

6. **Show Results**
   - "Click View Device Details"
   - "Here's the updated sensor data - temperature shows 28.5°C"
   - "Check the database - new records in Telemetries table"
   - "View logs - telemetry received event logged"

7. **Explain**
   - "This uses the same backend logic as real MQTT"
   - "Perfect for testing without broker setup"
   - "Production-ready when broker is configured"

---

## 📚 Related Documentation

- **MOSQUITTO_SETUP_GUIDE.md** - How to set up real MQTT broker
- **MQTTX_SETUP_GUIDE.md** - How to use MQTTX client
- **USER_GUIDE.md** - Complete application usage guide
- **README.md** - Project overview

---

## 🎉 Summary

✅ **No Setup Required** - Works immediately  
✅ **Same Logic as Real MQTT** - Production-quality processing  
✅ **Perfect for Demos** - Professional presentation  
✅ **Easy to Use** - Intuitive interface  
✅ **Full Functionality** - Complete feature showcase  
✅ **Beautiful UI** - Modern Tailwind CSS design  

**The MQTT Simulator is your secret weapon for impressive demonstrations! 🚀**

