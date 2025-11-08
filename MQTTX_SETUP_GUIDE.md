# MQTTX Setup Guide for IoT Device Management System

## What is MQTTX?
MQTTX is a cross-platform MQTT 5.0 desktop client with a beautiful GUI that makes it easy to test and debug MQTT connections.

## Download MQTTX
- **Website**: https://mqttx.app/
- **Direct Download**: https://github.com/emqx/MQTTX/releases
- Available for Windows, macOS, and Linux

---

## Step-by-Step Setup

### 1. Install and Launch MQTTX

1. Download MQTTX for your operating system
2. Install and open the application
3. You'll see the main dashboard with "New Connection" button

### 2. Create a New Connection

Click the **"+ New Connection"** button and configure:

#### Basic Settings:
```
Name:               IoT Device Manager
Client ID:          mqttx_test_client
Host:               localhost  (or 127.0.0.1)
Port:               1883
```

#### Connection Settings:
```
Protocol:           mqtt://
Username:           (leave empty unless you configured authentication)
Password:           (leave empty unless you configured authentication)
Clean Session:      ✓ Enabled
Connect Timeout:    30 seconds
Keep Alive:         60 seconds
```

#### SSL/TLS:
```
SSL/TLS:            ✗ Disabled (unless you configured SSL)
```

### 3. Connect to the Broker

1. Click **"Connect"** button (top right)
2. If successful, you'll see:
   - Status changes to **"Connected"** (green dot)
   - Connection info appears at the top

#### Troubleshooting Connection Issues:
If connection fails, check:
- ✅ Mosquitto broker is running: `mosquitto -v`
- ✅ Port 1883 is not blocked by firewall
- ✅ No other service is using port 1883

---

## Step 3: Subscribe to Topics

To monitor all device messages:

### Subscribe to All Topics:

1. In the connection window, find the **"New Subscription"** section
2. Click **"+ New Subscription"**
3. Configure:

```
Topic:              v1/#
QoS:                0
Color:              (choose any color you like)
```

4. Click **"Subscribe"**

This subscribes to ALL topics under `v1/` (telemetry, status, heartbeat, etc.)

### Alternative: Subscribe to Specific Topics

You can create separate subscriptions for better organization:

```
Telemetry:          v1/telemetry          (QoS: 0, Color: Blue)
Status:             v1/status             (QoS: 0, Color: Green)
Heartbeat:          v1/heartbeat          (QoS: 0, Color: Yellow)
Error:              v1/error              (QoS: 0, Color: Red)
Commands:           v1/command            (QoS: 1, Color: Purple)
Response:           v1/response           (QoS: 0, Color: Orange)
```

---

## Step 4: Test Publishing Messages

### Test 1: Send Telemetry Data

1. Make sure you have a device created in your application with Client ID (e.g., `SENSOR_001`)
2. In MQTTX, scroll down to the **message input area** at the bottom
3. Configure:

```
Topic:              v1/telemetry
QoS:                0
Payload (JSON):
```

```json
{
  "clientid": "SENSOR_001",
  "temperature": 25.5,
  "humidity": 60.0,
  "battery_level": 90.0
}
```

4. Click **"Send"** (or press Enter)
5. You should see the message appear in the message list above

### Test 2: Send Status Update

```
Topic:              v1/status
Payload:
```

```json
{
  "clientid": "SENSOR_001",
  "status": "online",
  "timestamp": "2024-11-08T10:30:00Z"
}
```

### Test 3: Send Heartbeat

```
Topic:              v1/heartbeat
Payload:
```

```json
{
  "clientid": "SENSOR_001",
  "timestamp": "2024-11-08T10:30:00Z"
}
```

---

## Step 5: Verify Messages are Received

### In MQTTX:
✅ You should see your published messages in the message list
✅ Each message should show:
   - Topic name
   - Timestamp
   - Payload content
   - QoS level

### In Your Application:
1. Check the **device details page** - sensor data should update
2. Check the **logs page** - you should see log entries for received messages
3. Check the **database**:
   ```sql
   SELECT * FROM "Telemetries" ORDER BY "Timestamp" DESC LIMIT 10;
   ```

### In Application Logs:
Check your application console for these log messages:
```
info: IoTAssesment.Services.MqttService[0]
      Received MQTT message on topic: v1/telemetry, Payload: {"clientid":"SENSOR_001",...}

info: IoTAssesment.Services.MqttService[0]
      Processing message for device 1 (ClientId: SENSOR_001) on topic: v1/telemetry

info: IoTAssesment.Services.MqttService[0]
      Stored 3 telemetry values from client SENSOR_001 for device 1: temperature, humidity, battery_level
```

---

## Advanced Features

### 1. Save and Reuse Messages

MQTTX allows you to save frequently used messages:
1. Click the **bookmark icon** next to the send button
2. Give it a name (e.g., "Temperature Test")
3. Access saved messages from the sidebar

### 2. Multiple Connections

You can create multiple connections to:
- Test different devices simultaneously
- Connect to different brokers
- Simulate multiple clients

### 3. Message Scripting (Pro Feature)

MQTTX supports scripting for automated testing:
```javascript
// Example: Send telemetry every 5 seconds
setInterval(() => {
  publish('v1/telemetry', JSON.stringify({
    clientid: 'SENSOR_001',
    temperature: 20 + Math.random() * 10,
    humidity: 50 + Math.random() * 20,
    battery_level: 70 + Math.random() * 30
  }));
}, 5000);
```

---

## Troubleshooting

### Issue 1: Messages Not Received by Application

**Symptoms**: Messages show in MQTTX but don't appear in the application

**Check**:
1. ✅ Your application is running
2. ✅ MQTT background service started successfully
3. ✅ Client ID in the message matches a device in your database
4. ✅ Check application logs for errors

**Fix**:
```bash
# Check if your application's MQTT service is running
# Look for this log on startup:
MQTT service started successfully. Connected to localhost:1883
```

### Issue 2: "Connection Refused" Error

**Symptoms**: MQTTX shows "Connection Refused" or "Connection Failed"

**Check**:
1. ✅ Mosquitto broker is running
   ```bash
   # Windows
   net start mosquitto
   
   # Linux/Mac
   sudo systemctl status mosquitto
   ```

2. ✅ Port 1883 is open
   ```bash
   netstat -an | findstr 1883     # Windows
   netstat -an | grep 1883        # Linux/Mac
   ```

### Issue 3: Messages Received but Not Stored

**Symptoms**: Messages appear in logs but not in database

**Check**:
1. ✅ Client ID matches exactly (case-sensitive)
2. ✅ Check device details page to confirm Client ID
3. ✅ Database connection is working
4. ✅ Check application logs for database errors

---

## Quick Test Workflow

### 1. Start Everything
```bash
# Terminal 1: Start Mosquitto
mosquitto -v

# Terminal 2: Start Your Application
cd IoTAssesment
dotnet run
```

### 2. In MQTTX
1. Connect to `localhost:1883`
2. Subscribe to `v1/#`
3. Send a telemetry message

### 3. Verify
- ✅ Message appears in MQTTX message list
- ✅ Application logs show "Stored X telemetry values..."
- ✅ Device details page shows updated sensor data
- ✅ Database contains new telemetry records

---

## MQTTX Tips & Tricks

### Color Coding
Use different colors for different message types to easily identify them:
- 🔵 Blue: Telemetry data
- 🟢 Green: Status updates
- 🟡 Yellow: Heartbeats
- 🔴 Red: Errors
- 🟣 Purple: Commands
- 🟠 Orange: Responses

### Keyboard Shortcuts
- `Ctrl/Cmd + Enter`: Send message
- `Ctrl/Cmd + K`: Clear message history
- `Ctrl/Cmd + N`: New connection
- `Ctrl/Cmd + W`: Close current connection

### Export Messages
Right-click on the message list → **"Export"** to save message history as JSON or CSV

---

## Alternative: MQTT Explorer

If you prefer a different tool, **MQTT Explorer** is another excellent option:
- Download: http://mqtt-explorer.com/
- More visual tree view of topics
- Better for exploring topic hierarchies
- Free and open-source

---

## Summary

✅ **MQTTX Setup**: Connect to `localhost:1883`  
✅ **Subscribe**: To `v1/#` to monitor all messages  
✅ **Publish**: Send test messages with your device's `clientid`  
✅ **Verify**: Check MQTTX, application logs, and database  
✅ **Debug**: Use MQTTX message history to troubleshoot  

**Happy Testing! 🚀**

For more help, check:
- MQTTX Documentation: https://mqttx.app/docs
- Your application logs in the console
- Device details page for Client ID reference

