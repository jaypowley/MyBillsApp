# Newtonsoft.Json ? System.Text.Json Migration Guide

## Quick Reference for MyBills Application

### 1. Namespace Changes

```csharp
// ? Remove this
using Newtonsoft.Json;

// ? Add this
using System.Text.Json;
using System.Text.Json.Serialization;
```

### 2. Attribute Changes

```csharp
// ? Newtonsoft.Json
[JsonProperty("property_name")]
public string PropertyName { get; set; }

// ? System.Text.Json
[JsonPropertyName("property_name")]
public string PropertyName { get; set; }
```

### 3. Serialization API Changes

```csharp
// ? Newtonsoft.Json
var json = JsonConvert.SerializeObject(myObject);
var json = JsonConvert.SerializeObject(myObject, Formatting.Indented);

// ? System.Text.Json
var json = JsonSerializer.Serialize(myObject);
var json = JsonSerializer.Serialize(myObject, new JsonSerializerOptions { WriteIndented = true });
```

### 4. Deserialization API Changes

```csharp
// ? Newtonsoft.Json
var obj = JsonConvert.DeserializeObject<MyType>(json);

// ? System.Text.Json
var obj = JsonSerializer.Deserialize<MyType>(json);
```

### 5. Common Attribute Mappings

| Newtonsoft.Json | System.Text.Json |
|:----------------|:-----------------|
| `[JsonProperty("name")]` | `[JsonPropertyName("name")]` |
| `[JsonIgnore]` | `[JsonIgnore]` (same) |
| `[JsonConverter(typeof(T))]` | `[JsonConverter(typeof(T))]` (same) |
| `[JsonExtensionData]` | `[JsonExtensionData]` (same) |
| `[JsonConstructor]` | `[JsonConstructor]` (same) |

### 6. Configuration Options

If you need custom behavior, configure in `Program.cs`:

```csharp
// Global configuration for MVC/API
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Use camelCase for property names
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    // Ignore null values when writing
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    // Allow trailing commas in JSON
    options.SerializerOptions.AllowTrailingCommas = true;

    // Case-insensitive property matching
    options.SerializerOptions.PropertyNameCaseInsensitive = true;

    // Include fields (not just properties)
    options.SerializerOptions.IncludeFields = false;

    // Write indented (pretty) JSON
    options.SerializerOptions.WriteIndented = false; // true for debugging
});
```

### 7. For Direct JsonSerializer Usage

```csharp
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
};

var json = JsonSerializer.Serialize(myObject, options);
var obj = JsonSerializer.Deserialize<MyType>(json, options);
```

### 8. Key Differences to Watch

#### Case Sensitivity
```csharp
// Newtonsoft.Json: case-insensitive by default
// System.Text.Json: case-sensitive by default

// Make it case-insensitive:
var options = new JsonSerializerOptions 
{ 
    PropertyNameCaseInsensitive = true 
};
```

#### Null Handling
```csharp
// Newtonsoft.Json: ignores nulls by default (often)
// System.Text.Json: includes nulls by default

// Ignore nulls:
var options = new JsonSerializerOptions 
{ 
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
};
```

#### Property Naming
```csharp
// Newtonsoft.Json: Uses PascalCase by default
// System.Text.Json: Uses the property name as-is by default

// Use camelCase:
var options = new JsonSerializerOptions 
{ 
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
};
```

### 9. Entity File Migration Checklist

For each of the 10 entity files in `MyBills.Domain\Entities`:

- [ ] Replace `using Newtonsoft.Json;` with `using System.Text.Json.Serialization;`
- [ ] Replace all `[JsonProperty("...")]` with `[JsonPropertyName("...")]`
- [ ] Remove any `[JsonConverter]` if using Newtonsoft-specific converters
- [ ] Test serialization/deserialization
- [ ] Verify JSON output matches expected format

**Files to update:**
1. [ ] BiMonthlyRecurrence.cs
2. [ ] BiWeeklyEvenRecurrence.cs
3. [ ] BiWeeklyOddRecurrence.cs
4. [ ] BiYearlyRecurrence.cs
5. [ ] DailyRecurrence.cs
6. [ ] MonthlyRecurrence.cs
7. [ ] OnetimeRecurrence.cs
8. [ ] QuarterlyRecurrence.cs
9. [ ] WeeklyRecurrence.cs
10. [ ] YearlyRecurrence.cs

### 10. Testing Checklist

After migration:

- [ ] Build solution successfully
- [ ] No compiler errors
- [ ] Run unit tests (if any)
- [ ] Test JSON serialization manually for each entity type
- [ ] Verify database operations that use JSON columns
- [ ] Test any API endpoints that return JSON
- [ ] Verify configuration files that might use JSON
- [ ] Check error handling and exceptions

### 11. Performance Verification

```csharp
// Simple benchmark
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();
for (int i = 0; i < 10000; i++)
{
    var json = JsonSerializer.Serialize(myObject);
}
stopwatch.Stop();
Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}ms");
```

Expected improvement: **2-3x faster** than Newtonsoft.Json

### 12. Troubleshooting Common Issues

#### Issue: JSON output format changed

**Solution**: Configure naming policy
```csharp
options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
```

#### Issue: Null values appearing in JSON

**Solution**: Ignore nulls
```csharp
options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
```

#### Issue: Deserialization fails due to case mismatch

**Solution**: Enable case-insensitive matching
```csharp
options.PropertyNameCaseInsensitive = true;
```

#### Issue: Custom converter not working

**Solution**: Re-implement converter for System.Text.Json
```csharp
public class MyConverter : JsonConverter<MyType>
{
    public override MyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Implementation
    }

    public override void Write(Utf8JsonWriter writer, MyType value, JsonSerializerOptions options)
    {
        // Implementation
    }
}
```

### 13. Complete Example

**Before (Newtonsoft.Json):**
```csharp
using Newtonsoft.Json;

public class Bill
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("bill_name")]
    public string Name { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonIgnore]
    public string InternalNotes { get; set; }
}

// Usage
var json = JsonConvert.SerializeObject(bill);
var bill = JsonConvert.DeserializeObject<Bill>(json);
```

**After (System.Text.Json):**
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

public class Bill
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("bill_name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonIgnore]
    public string InternalNotes { get; set; } = null!;
}

// Usage
var json = JsonSerializer.Serialize(bill);
var bill = JsonSerializer.Deserialize<Bill>(json);
```

### 14. Package Removal

After migration is complete and tested:

```xml
<!-- Remove from MyBills.Domain.csproj -->
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
```

Run `dotnet build` to verify no broken references.

### 15. Summary

**Benefits of System.Text.Json:**
- ? 2-3x faster serialization
- ? Lower memory allocation
- ? Built into .NET 8 (no external dependency)
- ? Better integration with modern C# features
- ? Active development and improvements

**Migration Effort:**
- 10 entity files to update
- Simple find-replace for most changes
- Minimal code changes required
- Estimated time: 2-4 hours including testing

**Risk Level:** ?? Medium
- Breaking change if JSON format expectations differ
- Thoroughly test serialization/deserialization
- Keep rollback plan ready
