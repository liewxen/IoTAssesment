# Mosquitto MQTT Broker Setup Guide (Windows)

## What is Mosquitto?

**Eclipse Mosquitto** is an open-source MQTT broker that runs on your local machine. It handles all MQTT message routing between your IoT devices and your application.

---

## Installation Guide (Windows)

### Step 1: Download Mosquitto

1. Visit: https://mosquitto.org/download/
2. Click **"Windows"** under Binary Installation
3. Download the **Windows 64-bit installer** (e.g., `mosquitto-2.0.18-install-windows-x64.exe`)

### Step 2: Install Mosquitto

1. Run the installer as **Administrator**
2. Follow the installation wizard:
   ```
   Installation Directory: C:\Program Files\mosquitto
   Components: ✓ Service ✓ Clients ✓ Documentation
   ```
3. Click **Install**
4. Click **Finish**

### Step 3: Add Mosquitto to System Path

1. Press `Windows + R`, type `sysdm.cpl`, press Enter
2. Click **"Advanced"** tab → **"Environment Variables"**
3. Under **"System variables"**, find **"Path"**, click **"Edit"**
4. Click **"New"** and add:
   ```
   C:\Program Files\mosquitto
   ```
5. Click **OK** on all dialogs
6. **Restart your terminal/command prompt**

### Step 4: Verify Installation

Open a new command prompt and run:
```bash
mosquitto -h
```

You should see:
```
mosquitto version 2.0.18

mosquitto is an MQTT v5.0/v3.1.1/v3.1 broker.
...
```

---

## Configuration

### Step 1: Create Configuration File

1. Navigate to Mosquitto directory:
   ```bash
   cd "C:\Program Files\mosquitto"
   ```

2. Create a configuration file named `mosquitto.conf`:
   ```bash
   notepad mosquitto.conf
   ```

3. Add the following configuration:
   ```conf
   # Mosquitto Configuration File
   # Basic MQTT Broker Setup for Local Development

   # Listener Configuration
   listener 1883
   protocol mqtt

   # Allow anonymous connections (for development only)
   allow_anonymous true

   # Logging
   log_dest file C:\Program Files\mosquitto\mosquitto.log
   log_type all
   log_timestamp true

   # Persistence
   persistence true
   persistence_location C:\Program Files\mosquitto\data\

   # Autosave interval (seconds)
   autosave_interval 60

   # Maximum QoS
   max_queued_messages 1000
   ```

4. Save and close the file

### Step 2: Create Data Directory

```bash
mkdir data
```

---

## Running Mosquitto

### Option 1: Run as Windows Service (Recommended)

#### Install Service:
```bash
# Open Command Prompt as Administrator
cd "C:\Program Files\mosquitto"

# Install the service
mosquitto install

# Start the service
net start mosquitto
```

#### Service Management:
```bash
# Stop the service
net stop mosquitto

# Start the service
net start mosquitto

# Restart the service
net stop mosquitto && net start mosquitto

# Check service status
sc query mosquitto
```

### Option 2: Run Manually (For Testing)

```bash
# Open Command Prompt
cd "C:\Program Files\mosquitto"

# Run with verbose output
mosquitto -v -c mosquitto.conf
```

**You should see:**
```
1699999999: mosquitto version 2.0.18 starting
1699999999: Config loaded from mosquitto.conf.
1699999999: Opening ipv4 listen socket on port 1883.
1699999999: mosquitto version 2.0.18 running
```

**Keep this terminal window open** while testing. Press `Ctrl+C` to stop.

---

## Testing Your Broker

### Test 1: Using Mosquitto Clients

Mosquitto includes test clients: `mosquitto_pub` and `mosquitto_sub`

#### Subscribe (Terminal 1):
```bash
mosquitto_sub -h localhost -t test/topic -v
```

#### Publish (Terminal 2):
```bash
mosquitto_pub -h localhost -t test/topic -m "Hello MQTT!"
```

**Expected Result**: Terminal 1 should show:
```
test/topic Hello MQTT!
```

### Test 2: Test with Your Application Topics

#### Subscribe to All Application Topics:
```bash
mosquitto_sub -h localhost -t "v1/#" -v
```

#### Send Test Telemetry (replace with your device's Client ID):
```bash
mosquitto_pub -h localhost -t "v1/telemetry" -m "{\"clientid\":\"SENSOR_001\",\"temperature\":25.0,\"humidity\":60.0,\"battery_level\":90.0}"
```

---

## Firewall Configuration

If you have issues, you may need to allow Mosquitto through Windows Firewall:

1. Open **Windows Defender Firewall** → **Advanced settings**
2. Click **Inbound Rules** → **New Rule...**
3. Select **Port** → **Next**
4. Select **TCP**, enter port **1883** → **Next**
5. Select **Allow the connection** → **Next**
6. Check all profiles (Domain, Private, Public) → **Next**
7. Name: `Mosquitto MQTT Broker` → **Finish**

---

## Troubleshooting

### Issue 1: "Service failed to start"

**Solution 1**: Check if port 1883 is already in use
```bash
netstat -an | findstr 1883
```

If something is using port 1883, either:
- Stop that service, or
- Change Mosquitto port in `mosquitto.conf`:
  ```conf
  listener 1884
  ```

**Solution 2**: Run as Administrator
```bash
# Right-click Command Prompt → "Run as administrator"
net start mosquitto
```

**Solution 3**: Check permissions
```bash
# Ensure the data directory exists and has write permissions
cd "C:\Program Files\mosquitto"
mkdir data
icacls data /grant Everyone:(OI)(CI)F
```

### Issue 2: "mosquitto: command not found"

**Solution**: Add to PATH (see Step 3 above), then restart your terminal

### Issue 3: "Error: Unable to open config file"

**Solution**: Specify the full path
```bash
cd "C:\Program Files\mosquitto"
mosquitto -c "C:\Program Files\mosquitto\mosquitto.conf" -v
```

### Issue 4: Application can't connect to broker

**Check**:
1. ✅ Mosquitto is running: `sc query mosquitto` shows "RUNNING"
2. ✅ Port 1883 is listening: `netstat -an | findstr 1883`
3. ✅ Your application's `appsettings.json` has correct settings:
   ```json
   "MQTT": {
     "BrokerHost": "localhost",
     "BrokerPort": 1883
   }
   ```
4. ✅ Firewall allows connections on port 1883

---

## Advanced Configuration

### Enable Authentication (Production)

For production environments, enable authentication:

1. Create password file:
   ```bash
   cd "C:\Program Files\mosquitto"
   mosquitto_passwd -c passwords.txt admin
   ```
   Enter password when prompted.

2. Update `mosquitto.conf`:
   ```conf
   allow_anonymous false
   password_file C:\Program Files\mosquitto\passwords.txt
   ```

3. Restart Mosquitto:
   ```bash
   net stop mosquitto
   net start mosquitto
   ```

4. Update your application's `appsettings.json`:
   ```json
   "MQTT": {
     "BrokerHost": "localhost",
     "BrokerPort": 1883,
     "Username": "admin",
     "Password": "your_password"
   }
   ```

### Enable SSL/TLS (Production)

For secure connections:

1. Generate certificates (or use Let's Encrypt)
2. Update `mosquitto.conf`:
   ```conf
   listener 8883
   cafile C:\Program Files\mosquitto\certs\ca.crt
   certfile C:\Program Files\mosquitto\certs\server.crt
   keyfile C:\Program Files\mosquitto\certs\server.key
   ```

### Access Control Lists (ACL)

To restrict topic access per user:

1. Create `acl.txt`:
   ```
   # Admin can do everything
   user admin
   topic readwrite #

   # Device can only publish to its own topics
   user device_001
   topic write v1/telemetry
   topic write v1/status
   topic write v1/heartbeat
   topic read v1/command
   ```

2. Update `mosquitto.conf`:
   ```conf
   acl_file C:\Program Files\mosquitto\acl.txt
   ```

---

## Monitoring

### View Logs
```bash
# Real-time log viewing
Get-Content "C:\Program Files\mosquitto\mosquitto.log" -Wait -Tail 50

# Or use notepad
notepad "C:\Program Files\mosquitto\mosquitto.log"
```

### Monitor Active Connections
```bash
# Show all connections on port 1883
netstat -an | findstr 1883
```

### Statistics
Add to `mosquitto.conf`:
```conf
# Publish broker statistics every 10 seconds
sys_interval 10
```

Then subscribe to see stats:
```bash
mosquitto_sub -h localhost -t '$SYS/#' -v
```

---

## Useful Commands

```bash
# Start Mosquitto service
net start mosquitto

# Stop Mosquitto service
net stop mosquitto

# Restart Mosquitto service
net stop mosquitto && net start mosquitto

# Check if service is running
sc query mosquitto

# View service configuration
sc qc mosquitto

# Run manually with verbose output
mosquitto -v -c mosquitto.conf

# Test connection
mosquitto_pub -h localhost -t test -m "test"

# Monitor all messages
mosquitto_sub -h localhost -t '#' -v

# Monitor your application's messages
mosquitto_sub -h localhost -t 'v1/#' -v
```

---

## Integration with Your Application

Your application should automatically connect to Mosquitto when it starts. Look for this in your application logs:

```
info: IoTAssesment.Services.MqttBackgroundService[0]
      MQTT Background Service is starting

info: IoTAssesment.Services.MqttService[0]
      MQTT service started successfully. Connected to localhost:1883
```

If you see connection errors, check:
1. ✅ Mosquitto is running
2. ✅ Configuration in `appsettings.json` is correct
3. ✅ No firewall blocking port 1883

---

## Quick Start Checklist

- [ ] Download and install Mosquitto
- [ ] Add to System PATH
- [ ] Create `mosquitto.conf` configuration file
- [ ] Create `data` directory
- [ ] Start Mosquitto service: `net start mosquitto`
- [ ] Test with `mosquitto_pub` and `mosquitto_sub`
- [ ] Configure firewall if needed
- [ ] Start your application
- [ ] Verify connection in application logs
- [ ] Test with MQTTX or send test message
- [ ] Check device details page for updated data

---

## Additional Resources

- **Official Documentation**: https://mosquitto.org/documentation/
- **Configuration Reference**: https://mosquitto.org/man/mosquitto-conf-5.html
- **MQTT Protocol**: https://mqtt.org/
- **MQTT Explorer**: http://mqtt-explorer.com/ (Alternative GUI tool)
- **MQTTX**: https://mqttx.app/ (Recommended GUI tool)

---

## Summary

✅ **Mosquitto** is the MQTT broker that runs on your local machine  
✅ **Install** from mosquitto.org and add to PATH  
✅ **Configure** `mosquitto.conf` for basic settings  
✅ **Run** as Windows service: `net start mosquitto`  
✅ **Test** with `mosquitto_pub` and `mosquitto_sub`  
✅ **Monitor** with `mosquitto_sub -t '#' -v`  
✅ **Integrate** with your IoT application automatically  

**You're now ready to test MQTT communication! 🚀**

