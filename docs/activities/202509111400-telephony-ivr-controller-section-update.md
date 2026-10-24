> Migrated from sufficit-project-management/docs/202509111400-telephony-ivr-controller-section-update.md on 2026-10-24.

# TelephonyIVRControllerSection - Updated to Match IVRController API

**Date**: 2025-09-11  
**Version**: 202509111400  
**Component**: Sufficit.Client.Controllers.Telephony.TelephonyIVRControllerSection  

## Problem Description

The `TelephonyIVRControllerSection` in the `sufficit-client` project was outdated and missing several methods that correspond to the current `IVRController` API in the `sufficit-endpoints` project.

## Solution Implemented

Updated the client controller section to match all endpoints available in the server-side `IVRController`, ensuring full API coverage for client applications.

### Added Methods

#### 1. Single Method
- **Purpose**: Get single IVR by search parameters (replaces Find for consistency)
- **Endpoint**: `GET /telephony/ivr?{query}`
- **Returns**: `Task<IVR?>`

#### 2. Remove Method
- **Purpose**: Delete a single IVR
- **Endpoint**: `DELETE /telephony/ivr?ivrid={ivrId}`
- **Returns**: `Task`

#### 3. AddOrUpdate Method
- **Purpose**: Add or update an IVR
- **Endpoint**: `POST /telephony/ivr`
- **Returns**: `Task<EndPointResponse>`

#### 4. Search Method
- **Purpose**: Search IVRs with parameters
- **Endpoint**: `POST /telephony/ivr` (with search parameters in body)
- **Returns**: `Task<IEnumerable<IVR>>`

#### 5. PostOptions Method
- **Purpose**: Update IVR options (renamed from Update for clarity)
- **Endpoint**: `POST /telephony/ivr/options?ivrid={ivrId}`
- **Returns**: `Task`

### Updated Methods

#### 1. GetOptions Method
- **Fixed return type**: Changed from `Task<IEnumerable<IVROption>>` to `Task<IEnumerable<AsteriskMenuOption>>`
- **Reason**: Matches the actual API response type

### Legacy Support

Added backward compatibility methods marked with `[Obsolete]` attribute:
- `Find` methods → Use `Single` instead
- `Update` methods → Use `AddOrUpdate` or `PostOptions` instead

## API Correspondence

| Client Method | Server Endpoint | HTTP Method | Purpose |
|--------------|----------------|-------------|---------|
| `Single` | `/telephony/ivr` | GET | Get single IVR by parameters |
| `Remove` | `/telephony/ivr` | DELETE | Delete IVR |
| `AddOrUpdate` | `/telephony/ivr` | POST | Create/Update IVR |
| `Search` | `/telephony/ivr` | POST | Search IVRs |
| `ByContext` | `/telephony/ivr/bycontext` | GET | Get IVRs by context |
| `GetOptions` | `/telephony/ivr/options` | GET | Get IVR options |
| `PostOptions` | `/telephony/ivr/options` | POST | Update IVR options |

## Technical Details

- **File Modified**: `Z:\Desenvolvimento\sufficit-client\src\Controllers\Telephony\TelephonyIVRControllerSection.cs`
- **Added Using**: `Sufficit.EndPoints` for `EndPointResponse`
- **Return Types**: All methods properly typed to match server responses
- **HTTP Methods**: Correctly mapped to server endpoints
- **Serialization**: Uses existing JSON serialization options

## Benefits

1. **Complete API Coverage**: All server endpoints now have corresponding client methods
2. **Type Safety**: Proper return types for all methods
3. **Consistency**: Method names and signatures match server conventions
4. **Backward Compatibility**: Existing code continues to work with obsolete warnings
5. **Future Proof**: Easy to maintain as server API evolves

## Usage Examples

### Creating or Updating an IVR
```csharp
var ivr = new IVR 
{
    Id = Guid.NewGuid(),
    Title = "Main Menu",
    ContextId = contextId
};

var response = await telephonyClient.IVR.AddOrUpdate(ivr, cancellationToken);
```

### Searching IVRs
```csharp
var parameters = new IVRSearchParameters 
{
    ContextId = contextId,
    Title = "Menu"
};

var ivrs = await telephonyClient.IVR.Search(parameters, cancellationToken);
```

### Managing IVR Options
```csharp
var options = new List<AsteriskMenuOption>
{
    new AsteriskMenuOption { Key = "1", Action = "Dial", Data = "100" },
    new AsteriskMenuOption { Key = "2", Action = "Dial", Data = "200" }
};

await telephonyClient.IVR.PostOptions(ivrId, options, cancellationToken);
```

## Migration Guide

For existing code using deprecated methods:

```csharp
// Old way (deprecated)
var ivr = await client.IVR.Find(parameters);

// New way
var ivr = await client.IVR.Single(parameters);

// Old way (deprecated) 
await client.IVR.Update(ivr);

// New way
await client.IVR.AddOrUpdate(ivr);

// Old way (deprecated)
await client.IVR.Update(ivrId, options);

// New way
await client.IVR.PostOptions(ivrId, options);
```

## Related Components

- `Sufficit.EndPoints.Controllers.Telephony.IVRController` - Server-side API controller
- `Sufficit.Telephony.IVR` - IVR entity model
- `Sufficit.Telephony.IVRSearchParameters` - Search parameters model
- `Sufficit.Telephony.Asterisk.AsteriskMenuOption` - Menu option model
- `Sufficit.EndPoints.EndPointResponse` - Standard API response wrapper
