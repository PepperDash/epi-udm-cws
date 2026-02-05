# UDM-CWS Testing Guide

## Quick Concept: How Rooms and Routes Work

Each **UDM-CWS instance** in your config = **one room endpoint** with its own:
- **Route** (URL path via `routePrefix`)
- **Device list** (which devices to monitor via `deviceMappings`)
- **Room device** (which room to control via `roomDeviceKey`)
- **Occupancy sensor** (which sensor to read via `occupancyDeviceKey`)

### Example: Combinable Rooms with Shared Devices

```json
{
  "key": "udmCws-roomA",
  "type": "udmcws",
  "properties": {
    "routePrefix": "roomA",
    "deviceMappings": [
      { "deviceKey": "codec-1", "deviceIndex": 1 },
      { "deviceKey": "display-1", "deviceIndex": 2 }
    ],
    "standardProperties": {
      "roomDeviceKey": "room-a",
      "occupancyDeviceKey": "occupancy-a"
    }
  }
},
{
  "key": "udmCws-roomB",
  "type": "udmcws",
  "properties": {
    "routePrefix": "roomB",
    "deviceMappings": [
      { "deviceKey": "codec-1", "deviceIndex": 1 },    // SHARED with Room A
      { "deviceKey": "display-2", "deviceIndex": 2 }   // Different display
    ],
    "standardProperties": {
      "roomDeviceKey": "room-b",
      "occupancyDeviceKey": "occupancy-b"
    }
  }
},
{
  "key": "udmCws-roomAB",
  "type": "udmcws",
  "properties": {
    "routePrefix": "roomAB",
    "deviceMappings": [
      { "deviceKey": "codec-1", "deviceIndex": 1 },    // SHARED
      { "deviceKey": "display-1", "deviceIndex": 2 },  // From Room A
      { "deviceKey": "display-2", "deviceIndex": 3 }   // From Room B
    ],
    "standardProperties": {
      "roomDeviceKey": "room-ab-combined",
      "occupancyDeviceKey": "occupancy-a"
    }
  }
}
```

**Result**:
- `/app01/udmcws/roomA/roomstatus` → codec-1, display-1
- `/app01/udmcws/roomB/roomstatus` → codec-1, display-2
- `/app01/udmcws/roomAB/roomstatus` → codec-1, display-1, display-2

---

## Testing Steps

### 1. Build and Deploy

```bash
# Build all packages
dotnet build UdmCws.sln --configuration Release
dotnet build UdmCws-Test-Room-Epi/UdmCws-Test-Room-Epi.csproj --configuration Release
dotnet build UdmCws-Test-Occ-Epi/UdmCws-Test-Occ-Epi.csproj --configuration Release
dotnet build UdmCws-Test-Device-Epi/UdmCws-Test-Device-Epi.csproj --configuration Release
```

**Deploy to processor** (`User/PepperDashEssentials/plugins/`):
- `output/UdmCws-Epi.1.0.0-local.cplz`
- `output/UdmCws-Test-Room-Epi.1.0.0-local.cplz`
- `output/UdmCws-Test-Occ-Epi.1.0.0-local.cplz`
- `output/UdmCws-Test-Device-Epi.1.0.0-local.cplz`

### 2. Choose Your Test Scenario

#### Scenario A: Simple Single Room (Start Here)

Use this minimal config to verify basics work:

```json
{
  "template": {
    "devices": [
      {
        "key": "processor",
        "name": "Control Processor",
        "type": "processor",
        "properties": {}
      },
      {
        "key": "mock-device-1",
        "name": "Mock Display",
        "type": "mockdevice",
        "properties": {}
      },
      {
        "key": "mock-occupancy-1",
        "name": "Mock Occupancy",
        "type": "mockoccupancy",
        "properties": {}
      },
      {
        "key": "udmCws",
        "name": "UDM-CWS Monitoring",
        "type": "udmcws",
        "properties": {
          "apiVersion": "1.0.0",
          "feedbackMode": "deferred",
          "deviceMappings": [
            {
              "deviceKey": "mock-device-1",
              "deviceIndex": 1,
              "customLabel": "Main Display"
            }
          ],
          "standardProperties": {
            "version": "1.0.0",
            "roomDeviceKey": "mock-room-1",
            "occupancyDeviceKey": "mock-occupancy-1"
          }
        }
      }
    ],
    "rooms": [
      {
        "key": "mock-room-1",
        "name": "Test Room",
        "type": "mockroom",
        "properties": {
          "helpRequestMessage": "Test help message",
          "defaultActivity": "idle"
        }
      }
    ]
  }
}
```

**Test**:
```bash
# GET status
curl http://<processor-ip>/app01/udmcws/roomstatus | jq

# Power off room
curl -X PATCH http://<processor-ip>/app01/udmcws/roomstatus \
  -H "Content-Type: application/json" \
  -d '{"standard": {"state": "off"}}'

# Power on room
curl -X PATCH http://<processor-ip>/app01/udmcws/roomstatus \
  -H "Content-Type: application/json" \
  -d '{"standard": {"state": "on"}}'
```

**Expected**:
- `standard.state`: "On" or "Off"
- `standard.occupancy`: toggles every 30 seconds
- `status.devices.device1.status`: "On", "Off", "Warming", "Cooling"
- `status.devices.device1.usage`: session minutes when powered on

#### Scenario B: Multi-Room with Route Prefixes

Add multiple UDM-CWS instances for multiple endpoints:

```json
{
  "devices": [
    // ... mock devices ...
    {
      "key": "udmCws-room1",
      "type": "udmcws",
      "properties": {
        "routePrefix": "room1",
        "deviceMappings": [
          { "deviceKey": "mock-device-1", "deviceIndex": 1 }
        ],
        "standardProperties": {
          "roomDeviceKey": "mock-room-1",
          "occupancyDeviceKey": "mock-occupancy-1"
        }
      }
    },
    {
      "key": "udmCws-room2",
      "type": "udmcws",
      "properties": {
        "routePrefix": "room2",
        "deviceMappings": [
          { "deviceKey": "mock-device-2", "deviceIndex": 1 }
        ],
        "standardProperties": {
          "roomDeviceKey": "mock-room-2",
          "occupancyDeviceKey": "mock-occupancy-2"
        }
      }
    }
  ]
}
```

**Test**:
```bash
# Room 1
curl http://<processor-ip>/app01/udmcws/room1/roomstatus | jq

# Room 2
curl http://<processor-ip>/app01/udmcws/room2/roomstatus | jq
```

#### Scenario C: Real Devices (Production Test)

Replace mock devices with real ones:

```json
{
  "devices": [
    {
      "key": "codec-1",
      "type": "ciscoCodec",  // Your actual codec type
      "properties": { /* ... connection details ... */ }
    },
    {
      "key": "occupancy-1",
      "type": "cenodtcpoe",  // Crestron CEN-ODT-C-POE
      "properties": { /* ... IPID/address ... */ }
    },
    {
      "key": "udmCws",
      "type": "udmcws",
      "properties": {
        "deviceMappings": [
          { "deviceKey": "codec-1", "deviceIndex": 1 }
        ],
        "standardProperties": {
          "roomDeviceKey": "room-1",  // Your kpmgRoom or other room
          "occupancyDeviceKey": "occupancy-1"
        }
      }
    }
  ],
  "rooms": [
    {
      "key": "room-1",
      "type": "kpmgRoom",  // Your actual room plugin
      "properties": { /* ... room config ... */ }
    }
  ]
}
```

### 3. Verify Logs

Check Essentials console output:

```
UdmCwsStateManager: UDM-CWS endpoint available at /app01/udmcws/roomstatus
UdmCwsActionPathsHandler: Route added to server at path: roomstatus

// Or with route prefix:
UdmCwsActionPathsHandler: Route added to server at path: room1/roomstatus
```

### 4. Test Device Features

**Mock Device Features**:
- Power on → 2 second warming → status becomes "On"
- Power off → 2 second cooling → status becomes "Off"
- Usage tracking starts when powered on, stops when powered off
- Communication monitor shows online/offline status

**Mock Occupancy**:
- Auto-toggles every 30 seconds
- Watch `standard.occupancy` flip between true/false

**Mock Room**:
- Responds to PATCH state changes
- Has 20 custom properties (if configured in room properties)
- Has help request message (if configured)

---

## Quick Reference

### Configuration Structure

```
UDM-CWS Instance
├─ routePrefix: "room1" (optional)
│   └─ Creates: /app01/udmcws/room1/roomstatus
├─ deviceMappings: [ ... ]
│   └─ Defines which devices appear in this endpoint
└─ standardProperties
    ├─ roomDeviceKey: "room-1"
    │   └─ Which IEssentialsRoom to control via PATCH
    └─ occupancyDeviceKey: "occupancy-1"
        └─ Which IOccupancyStatusProvider to read
```

### Common Issues

**"Cannot load unknown device type 'X'"**
- Device type doesn't match any loaded plugin
- Check device type name matches plugin factory TypeNames
- Deploy missing plugin .cplz file

**"Room device not found"**
- Check `roomDeviceKey` matches a room key in "rooms" array
- Verify room plugin is loaded

**"Route not found" / 404**
- Check routePrefix in config matches URL path
- Verify UdmCwsStateManager activated successfully
- Check logs for "Route added to server"

**Device shows status "Off" but should be "On"**
- Real devices: Check device actually supports IHasPowerControlWithFeedback
- Mock devices: Verify device activated successfully
- Check logs for device key not found errors

---

## Next Steps

1. **Start Simple**: Use Scenario A (single mock room) to verify basics
2. **Add Devices**: Add more mock devices to test device mapping
3. **Multi-Room**: Use Scenario B to test route prefixes
4. **Real Devices**: Use Scenario C when ready for production testing
5. **Custom Properties**: Configure property1-20 in mock room to test custom properties
6. **SIMPL+**: Test SIMPL+ modules with SetRoutePrefix() before Initialize()

---

## Files Reference

- **This guide**: `UdmCws-Epi/Testing/TESTING_GUIDE.md`
- **Example config (mock)**: `UdmCws-Epi/Testing/mockDeviceTestConfig.json`
- **Example config (full)**: `UdmCws-Epi/Testing/configurationFile.json`
- **Deployment guide**: `UdmCws-Epi/Testing/DEPLOYMENT.md`
