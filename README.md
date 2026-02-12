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
    "psk": "your-secure-pre-shared-key-here",
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
| `apiVersion` | string | No | API version (default: "1.0.0") - Logged against `PDT-API-VERSION` header (warning only, not rejected) |
| `psk` | string | No | Pre-shared key for request authentication - Validated against `PDT-PSK` header. Leave empty to disable PSK validation |
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

## Security

### Pre-Shared Key (PSK) Authentication

UDM-CWS supports optional PSK authentication for securing API requests:

**Configuration:**

```json
{
  "key": "udmCws",
  "type": "udmcws",
  "properties": {
    "apiVersion": "1.0.0",
    "psk": "your-secure-key-here"
  }
}
```

**Request Headers:**

UDM will send required headers. PSK must be set on device.

All requests must include these headers when PSK is configured:

```bash
curl -X GET http://<processor-ip>/app01/udmcws/roomstatus \
  -H "PDT-PSK: your-secure-key-here" \
  -H "PDT-API-VERSION: 1.0.0"
```

**Validation Behavior:**

- **API Version (`PDT-API-VERSION`)**: Logged as warning if missing or mismatched. Request is **not rejected** - allows backward compatibility while alerting developers to version mismatches in console and error logs.
- **PSK (`PDT-PSK`)**: Only validated if configured (non-empty). Returns `401 Unauthorized` if PSK is configured but missing or incorrect.
- **No PSK Configured**: Requests are accepted with or without the `PDT-PSK` header (no security).

**Important Notes:**

- Empty string PSK (`psk: ""`) is treated as "no security" - PSK validation is disabled
- Monitoring platforms should send empty/omit `PDT-PSK` header when PSK is not configured
- API version mismatches only log warnings - requests proceed normally for backward compatibility

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
<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 2.0.0
- 2.24.0
- 2.24.0
- 2.24.0
<!-- END Minimum Essentials Framework Versions -->
<!-- START Config Example -->
### Config Example

```json
{
    "key": "GeneratedKey",
    "uid": 1,
    "name": "GeneratedName",
    "type": "UdmCwsStateManager",
    "group": "Group",
    "properties": {
        "Property1": "SampleString",
        "Property2": "SampleString",
        "Property3": "SampleString",
        "Property4": "SampleString",
        "Property5": "SampleString",
        "Property6": "SampleString",
        "Property7": "SampleString",
        "Property8": "SampleString",
        "Property9": "SampleString",
        "Property10": "SampleString",
        "Property11": "SampleString",
        "Property12": "SampleString",
        "Property13": "SampleString",
        "Property14": "SampleString",
        "Property15": "SampleString",
        "Property16": "SampleString",
        "Property17": "SampleString",
        "Property18": "SampleString",
        "Property19": "SampleString",
        "Property20": "SampleString",
        "HelpRequestMessage": "SampleString",
        "DefaultActivity": "SampleString",
        "DeviceKeys": "SampleValue"
    }
}
```
<!-- END Config Example -->
<!-- START Supported Types -->
### Supported Types

- UdmCwsStateManager
- udmcws
<!-- END Supported Types -->
<!-- START Join Maps -->

<!-- END Join Maps -->
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- IHttpCwsHandler
- IOccupancyStatusProvider
- IHasPowerControlWithFeedback
- IWarmingCooling
- ICommunicationMonitor
- IUsageTracking
- IRoutingSink
- IEssentialsRoom
<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- EventArgs
- WebRequestHandlerBase
- EssentialsDevice
- EssentialsRoomPropertiesConfig
<!-- END Base Classes -->
<!-- START Public Methods -->
### Public Methods

- public State BuildState()
- public string ExtractDesiredRoomState(State desiredState)
- public string ExtractDesiredRoomActivity(State desiredState)
- public void SetDeviceLabel(int index, string value)
- public void SetDeviceStatus(int index, string value)
- public void SetDeviceDescription(int index, string value)
- public void SetDeviceVideoSource(int index, string value)
- public void SetDeviceAudioSource(int index, string value)
- public void SetDeviceUsage(int index, ushort value)
- public void SetDeviceError(int index, string value)
- public void SetCustomLabel(int index, string value)
- public void SetCustomValue(int index, string value)
- public void SetStandardVersion(string value)
- public void SetStandardState(string value)
- public void SetStandardError(string value)
- public void SetStandardOccupancy(bool value)
- public void SetStandardHelpRequest(string value)
- public void SetStandardActivity(string value)
- public void AddRoute(HttpCwsServer server, string routePrefix = "")
- public void ProcessRequest(HttpCwsContext context)
- public void ExecuteStateChange(string desiredState, string desiredActivity)
- public void PopulateState(State state)
- public void SetActivity(string desiredActivity)
- public void PopulateStandardProperties(StandardProperties standard)
- public void PopulateActivity(StandardProperties standard)
- public void PopulateHelpRequest(StandardProperties standard)
- public void PopulateCustomProperties(Dictionary<string, CustomProperties> customProps)
- public void SetActivity(string desiredActivity)
- public void PopulateRoutingSources(object device, DeviceStatus status)
- public void PopulateDeviceProperties(object device, DeviceStatus status, DeviceMapping mapping)
- public void CompilePropertyGetter(string cacheKey, object device, string propertyPath)
- public void CacheMethod(string cacheKey, object device, string methodName)
- public object GetPropertyValue(string cacheKey, object device)
- public void InvokeMethod(string cacheKey, object device, params object[] parameters)
- public void Initialize(UdmCwsConfiguration config)
- public string GetMappedValue(string cacheKey, PropertyMapping mapping)
- public void SetActivity(ActivityMapping mapping, string desiredActivity)
- public void SetOccupied(bool occupied)
- public void SetAutoToggle(bool enabled)
- public void PowerOn()
- public void PowerOff()
- public void PowerToggle()
- public bool ShouldBeOnForActivity(string activity)
- public void SetSource(string videoSource, string audioSource = null)
- public void StartShutdown(eShutdownType type)
- public void Shutdown()
- public void PowerOnToDefaultOrLastSource()
- public void SetActivity(string activity)
- public string GetCustomProperty(string propertyKey)
- public void SetConfig(DeviceConfig config)
- public bool RunDefaultPresentRoute()
- public void RunRouteAction(string routeKey, string sourceListKey)
- public void RunDefaultCallRoute()
<!-- END Public Methods -->
<!-- START Bool Feedbacks -->
### Bool Feedbacks

- RoomIsOccupiedFeedback
- PowerIsOnFeedback
- IsWarmingUpFeedback
- IsCoolingDownFeedback
- OnFeedback
- IsWarmingUpFeedback
- IsCoolingDownFeedback
<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->

<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->
### String Feedbacks

- ErrorFeedback
- VideoSourceFeedback
- AudioSourceFeedback
- ActivityFeedback
<!-- END String Feedbacks -->
