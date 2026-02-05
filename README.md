# UDM-CWS (Unified Device Management - Crestron Web Services)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)

A standardized REST API for monitoring and controlling room devices in PepperDash Essentials. UDM-CWS provides a vendor-neutral API that enables unified device management across different room types and configurations.

**api reference: <https://portal.cloud.pepperdash.com/apidocs/udm/driver>**

## Table of Contents

- [Features](#features)
- [Quick Start](#quick-start)
- [Installation](#installation)
  - [Essentials Plugin (EPI)](#essentials-plugin-epi)
  - [SIMPL Plus Module](#simpl-plus-module)
  - [Core Library](#core-library)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [Auto-Detection](#auto-detection)
- [Advanced Features](#advanced-features)
- [Testing](#testing)
- [Building from Source](#building-from-source)

---

## Features

- **Auto-Detection** - Automatically discovers device capabilities via Essentials interfaces
- **Zero Per-Device Config** - No manual property mapping required for standard features
- **Multi-Platform** - Available as Essentials plugin (EPI), SIMPL+ modules, and standalone library
- **Multi-Room Support** - Multiple independent rooms in a single app instance
- **Flexible Property Mapping** - Optional reflection-based property access for custom scenarios
- **Convention-Based** - Intelligent interface detection for device state
- **REST API** - Standard HTTP endpoints for GET (status) and PATCH (control)

---

## Quick Start

### 1. Install the Plugin

Download `UdmCws-Epi.1.0.0-local.cplz` from releases and deploy to your processor:

```bash
# Copy to processor plugins directory
/user/program1/loadedPlugins/
```

### 2. Add Configuration

Add to your Essentials `configurationFile.json`:

```json
{
  "key": "udmCws",
  "type": "udmcws",
  "properties": {
    "deviceMappings": [
      {
        "deviceKey": "codec-1",
        "deviceIndex": 1,
        "customLabel": "Video Codec"
      },
      {
        "deviceKey": "display-1",
        "deviceIndex": 2,
        "customLabel": "Main Display"
      }
    ],
    "standardProperties": {
      "roomDeviceKey": "room-1",
      "occupancyDeviceKey": "occupancy-1"
    }
  }
}
```

### 3. Access the API

```bash
# Get room status
curl http://<processor-ip>/app01/udmcws/roomstatus

# Power on the room
curl -X PATCH http://<processor-ip>/app01/udmcws/roomstatus \
  -H "Content-Type: application/json" \
  -d '{"apiVersion":"1.0.0","standard":{"state":"on"}}'
```

---

## Installation

### Essentials Plugin (EPI)

The Essentials Plugin integrates with PepperDash Essentials and provides full UDM-CWS functionality.

**Requirements:**

- PepperDash Essentials 2.24.0+
- .NET Framework 4.7.2

**Installation:**

1. Download `UdmCws-Epi.1.0.0-local.cplz`
2. Upload to `/user/program1/loadedPlugins/` on your processor
3. Add configuration to your Essentials config (see [Configuration](#configuration))
4. Restart Essentials application

**Endpoints:**

- GET `/app01/udmcws/roomstatus` - Get current room and device status
- PATCH `/app01/udmcws/roomstatus` - Update room state (on/off)

### SIMPL Plus Module

A single SIMPL+ module provides signal-based integration for SIMPL Windows programs.

**Module:**

- **UdmCws.usp** - All-in-one module handling status, control, and property mapping

**Requirements:**

- SIMPL Windows (for compilation)
- UdmCws-Epi.cplz (deployed to processor)

**Key Signals:**

```plus
// Parameters
STRING_PARAMETER RoutePrefix[50];    // Optional for multi-room
INTEGER_PARAMETER iFeedbackMode;     // 0=Deferred, 1=Immediate

// Control Inputs
DIGITAL_INPUT Enable;                // Must be high for operation
DIGITAL_INPUT Init;                  // Pulse to sync all signals

// Feedback Outputs
DIGITAL_OUTPUT Initialized_fb;       // High when ready
DIGITAL_OUTPUT ReportState_Request;  // Pulses on GET request
STRING_OUTPUT Desired_RoomState;     // "on"/"off" from PATCH
STRING_OUTPUT Desired_RoomActivity;  // Activity from PATCH

// Standard Properties (20 inputs)
ANALOG_INPUT Device_Usage[20];
STRING_INPUT Standard_Version, Standard_State, Standard_Error, etc.
STRING_INPUT Device_Label[20], Device_Status[20], etc.
STRING_INPUT Custom_Label[20], Custom_Value[20];
```

See [UdmCws-Plus/UdmCws.usp](UdmCws-Plus/UdmCws.usp) for complete signal reference.

### Core Library

The core library (`UdmCws-Lib`) provides framework-agnostic models and interfaces.

**Use Cases:**

- Building custom integrations
- Creating alternative transport layers
- Unit testing

**NuGet Package:**

```bash
dotnet add package UdmCws-Lib
```

**Key Components:**

- **Models** - State, DeviceStatus, StandardProperties, CustomProperties
- **Interfaces** - Event system for state changes
- **Configuration** - PropertyMapping, DeviceMapping, UdmCwsConfiguration

---

## Configuration

### Basic Configuration

Minimal configuration with auto-detected properties:

```json
{
  "key": "udmCws",
  "type": "udmcws",
  "properties": {
    "deviceMappings": [
      {
        "deviceKey": "codec-1",
        "deviceIndex": 1
      }
    ],
    "standardProperties": {
      "roomDeviceKey": "room-1"
    }
  }
}
```

### Full Configuration

Complete configuration with all optional features:

```json
{
  "key": "udmCws",
  "type": "udmcws",
  "properties": {
    "apiVersion": "1.0.0",
    "feedbackMode": "deferred",
    "routePrefix": "room1",

    "deviceMappings": [
      {
        "deviceKey": "codec-1",
        "deviceIndex": 1,
        "customLabel": "Video Codec",
        "description": "Main room codec for conferencing"
      },
      {
        "deviceKey": "display-1",
        "deviceIndex": 2,
        "customLabel": "Main Display",
        "description": "Primary display panel"
      }
    ],

    "standardProperties": {
      "version": "1.0.0",
      "roomDeviceKey": "room-1",
      "occupancyDeviceKey": "occupancy-1",

      "activityMapping": {
        "getProperty": {
          "deviceKey": "room-1",
          "propertyPath": "Activity"
        }
      },

      "helpRequestMapping": {
        "deviceKey": "room-1",
        "propertyPath": "HelpRequest"
      },

      "customPropertyMappings": [
        {
          "propertyKey": "property1",
          "label": "Codec In Call",
          "mapping": {
            "deviceKey": "codec-1",
            "propertyPath": "IsInCall"
          }
        },
        {
          "propertyKey": "property2",
          "label": "Room Occupied",
          "mapping": {
            "deviceKey": "occupancy-1",
            "propertyPath": "RoomIsOccupiedFeedback.BoolValue"
          }
        }
      ]
    }
  }
}
```

### Configuration Properties

| Property | Type | Required | Description |
| ---------- | ------ | ---------- | ------------- |
| `apiVersion` | string | No | API version (default: "1.0.0") |
| `feedbackMode` | string | No | "deferred" (202) or "immediate" (200) |
| `routePrefix` | string | No | Route prefix for multi-room (e.g., "room1") |
| `deviceMappings` | array | Yes | Device mappings (see below) |
| `standardProperties` | object | Yes | Standard properties config (see below) |

#### Device Mapping

| Property | Type | Required | Description |
| ---------- | ------ | ---------- | ------------- |
| `deviceKey` | string | Yes | Essentials device key |
| `deviceIndex` | number | Yes | Device number (1-20) |
| `customLabel` | string | No | Display name (defaults to device key) |
| `description` | string | No | Device description |

#### Standard Properties

| Property | Type | Required | Description |
| ---------- | ------ | ---------- | ------------- |
| `roomDeviceKey` | string | Yes* | Room device key (IEssentialsRoom) |
| `occupancyDeviceKey` | string | No | Occupancy sensor key |
| `activityMapping` | object | No | Activity property mapping |
| `helpRequestMapping` | object | No | Help request property mapping |
| `customPropertyMappings` | array | No | Custom properties (1-20) |

**Note:** `roomDeviceKey` is required for PATCH operations to work.

---

## API Reference

<https://portal.cloud.pepperdash.com/apidocs/udm/driver>

**Behavior:**

- `"state": "on"` → Calls `IEssentialsRoom.PowerOnToDefaultOrLastSource()`
- `"state": "off"` → Calls `IEssentialsRoom.Shutdown()`

The room itself manages which devices to power on/off.

### Multi-Room Endpoints

When using route prefixes:

```REST
GET  /app01/udmcws/room1/roomstatus
PATCH /app01/udmcws/room1/roomstatus

GET  /app01/udmcws/room2/roomstatus
PATCH /app01/udmcws/room2/roomstatus
```

---

## Auto-Detection

UDM-CWS automatically populates device properties by detecting Essentials interfaces. No per-device configuration required.

### Device Properties

| Property | Auto-Detection | Description |
| ---------- | --------------- | ------------- |
| **label** | Config or `device.Key` | Device display name |
| **status** | `IHasPowerControlWithFeedback`  `IWarmingCooling`  `VideoCodecBase.IsInCall` | On/Off/Warming/Cooling/In Call |
| **description** | Config only | User-provided description |
| **usage** | `IUsageTracking`  `IDisplayUsage` | Session minutes or lamp hours |
| **error** | `ICommunicationMonitor`  `IOnline` | Communication status |
| **videoSource** **audioSource** | `IRoutingSink.CurrentSourceInfo` | Current routing source |

### Room Properties

| Property | Auto-Detection | Description |
| ---------- | --------------- | ------------- |
| **state** | `IEssentialsRoom.OnFeedback` | Room power state (On/Off) |
| **occupancy** | `IOccupancyStatusProvider` | Room occupancy status |
| **activity** | Optional property mapping | Current activity (call/presentation/idle) |
| **helpRequest** | Optional property mapping | Help request status |

### Usage Tracking Logic

1. **IUsageTracking** (preferred) - Returns session minutes when device is in use, null when idle
2. **IDisplayUsage** (fallback) - Returns total lamp hours for displays
3. **null** (default) - Device doesn't support usage tracking

### Codec Standby Handling

Video/audio codecs in standby mode (online but PowerIsOnFeedback=false) are reported as "On" for monitoring purposes.

---

## Advanced Features

### Multi-Room Support

Run multiple independent rooms in a single Essentials app instance:

```json
{
  "key": "udmCws-room1",
  "type": "udmcws",
  "properties": {
    "routePrefix": "room1",
    "deviceMappings": [/*...*/],
    "standardProperties": {/*...*/}
  }
},
{
  "key": "udmCws-room2",
  "type": "udmcws",
  "properties": {
    "routePrefix": "room2",
    "deviceMappings": [/*...*/],
    "standardProperties": {/*...*/}
  }
}
```

### Custom Property Mapping

Map any device property to custom properties using reflection:

```json
"customPropertyMappings": [
  {
    "propertyKey": "property1",
    "label": "Room Temperature",
    "mapping": {
      "deviceKey": "thermostat-1",
      "propertyPath": "TemperatureFeedback.IntValue",
      "format": "{0}°F",
      "defaultValue": "72"
    }
  },
  {
    "propertyKey": "property2",
    "label": "Participant Count",
    "mapping": {
      "deviceKey": "codec-1",
      "propertyPath": "ParticipantCountFeedback.IntValue",
      "valueMap": {
        "0": "No participants",
        "1": "1 participant"
      }
    }
  }
]
```

**Property Path Format:**

- Simple: `"IsInCall"` (boolean property)
- Nested: `"RoomIsOccupiedFeedback.BoolValue"` (feedback object property)
- Deep: `"CurrentSourceInfo.Name"` (multiple levels)

**Value Transformation:**

- `valueMap` - Map raw values to display values
- `format` - String format pattern (e.g., `"{0}°F"`)
- `defaultValue` - Value if property not found or null

**Performance:**

- Property accessors are compiled once at startup using reflection
- Lazy compilation on first access (handles device loading order)
- Cached delegates for fast access (~0.1ms per property)

### Activity Mapping (Read/Write)

Map room activity with both get (read) and set (write) operations:

```json
"activityMapping": {
  "getProperty": {
    "deviceKey": "room-1",
    "propertyPath": "Activity",
    "valueMap": {
      "codec": "call",
      "laptop": "presentation"
    }
  },
  "setFunctions": {
    "call": "StartCall",
    "presentation": "StartPresentation",
    "idle": "EndAllActivity"
  },
  "setDeviceKey": "room-1"
}
```

---

## Testing

### Mock Plugins

The repository includes comprehensive mock plugins for testing without hardware:

**UdmCws-Test-Room-Epi** - Mock room with:

- 20 configurable custom properties
- Activity and help request support
- Device power control

**UdmCws-Test-Occ-Epi** - Mock occupancy sensor with:

- Auto-toggle every 30 seconds
- Manual control

**UdmCws-Test-Device-Epi** - Mock device with:

- Power control (with 2s warming/cooling simulation)
- Usage tracking
- Communication monitoring
- Routing support

### Test Configuration

See [UdmCws-Epi/Testing/configurationFile.json](UdmCws-Epi/Testing/configurationFile.json) for a complete test configuration using mock plugins and real devices.

### Deployment Guide

See [UdmCws-Epi/Testing/TESTING_GUIDE.md](UdmCws-Epi/Testing/TESTING_GUIDE.md) for detailed testing instructions.

---

## Building from Source

### Requirements

- .NET Framework 4.7.2 SDK
- PepperDash Essentials 2.24.0+
- SIMPL Windows (for SIMPL+ modules)

### Build Commands

```bash
# Build entire solution
dotnet build UdmCws.sln --configuration Release

# Build specific projects
dotnet build UdmCws-Lib/UdmCws-Lib.csproj --configuration Release
dotnet build UdmCws-Epi/UdmCws-Epi.csproj --configuration Release

# Build mock plugins
dotnet build UdmCws-Test-Room-Epi/UdmCws-Test-Room-Epi.csproj --configuration Release
dotnet build UdmCws-Test-Occ-Epi/UdmCws-Test-Occ-Epi.csproj --configuration Release
dotnet build UdmCws-Test-Device-Epi/UdmCws-Test-Device-Epi.csproj --configuration Release
```

### Output Files

Build artifacts are generated in `output/`:

- `UdmCws-Epi.1.0.0-local.cplz` - Essentials plugin
- `UdmCws-Test-Room-Epi.1.0.0-local.cplz` - Mock room plugin
- `UdmCws-Test-Occ-Epi.1.0.0-local.cplz` - Mock occupancy plugin
- `UdmCws-Test-Device-Epi.1.0.0-local.cplz` - Mock device plugin
- `UdmCws-Lib.1.0.0-local.nupkg` - Core library NuGet package

### Download Dependencies

```bash
# macOS/Linux
./GetPackages.sh

# Windows
GetPackages.BAT
```

---

## Documentation

- **[TESTING_GUIDE.md](UdmCws-Epi/Testing/TESTING_GUIDE.md)** - Complete testing guide
- **[Device Monitoring API.openapi.yaml](Device%20Monitoring%20API.openapi.yaml)** - OpenAPI 3.1 specification
- **[LICENSE.md](LICENSE.md)** - MIT License

---

## License

This project is licensed under the MIT License - see [LICENSE.md](LICENSE.md) for details.

---

## Supported Types

**Production:**

- `udmcws` - UDM-CWS Essentials Plugin

**Testing:**

- `mockroom` - Mock Room Plugin
- `mockoccupancy` - Mock Occupancy Sensor
- `mockdevice` - Mock Device

---

## Minimum Essentials Framework Version

2.24.0
