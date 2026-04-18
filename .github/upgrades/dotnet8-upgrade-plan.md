# .NET 8 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade MyBills.Core\MyBills.Core.csproj
4. Upgrade MyBills.Domain\MyBills.Domain.csproj
5. Upgrade MyBills.Data\MyBills.Data.csproj
6. Upgrade MyBills.Services\MyBills.Services.csproj
7. Upgrade MyBills.Mvc\MyBills.Mvc.csproj
8. Replace Newtonsoft.Json with System.Text.Json in MyBills.Domain
9. Enable nullable reference types across all projects
10. Validate and test the upgraded solution

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                      | Current Version | New Version | Description                                   |
|:--------------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.EntityFrameworkCore                     | 6.0.36          | 8.0.26      | Recommended for .NET 8.0                      |
| Microsoft.EntityFrameworkCore.Relational          | 6.0.36          | 8.0.26      | Recommended for .NET 8.0                      |
| Microsoft.EntityFrameworkCore.SqlServer           | 6.0.36          | 8.0.26      | Recommended for .NET 8.0                      |
| Microsoft.EntityFrameworkCore.Tools               | 6.0.36          | 8.0.26      | Recommended for .NET 8.0                      |
| Microsoft.Extensions.Caching.Abstractions         | 6.0.1           | 8.0.0       | Recommended for .NET 8.0                      |
| Microsoft.Extensions.Caching.Memory               | 6.0.3           | 8.0.1       | Recommended for .NET 8.0                      |
| Microsoft.Extensions.Configuration                | 6.0.2           | 8.0.0       | Recommended for .NET 8.0                      |
| Microsoft.Extensions.Logging.Debug                | 6.0.1           | 8.0.1       | Recommended for .NET 8.0                      |
| Microsoft.VisualStudio.Web.CodeGeneration.Design  | 6.0.18          | 8.0.23      | Recommended for .NET 8.0                      |
| Newtonsoft.Json                                   | 13.0.4          |             | Remove - replace with System.Text.Json        |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### MyBills.Core\MyBills.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

NuGet packages changes:
  - Microsoft.Extensions.Caching.Abstractions should be updated from `6.0.1` to `8.0.0` (*recommended for .NET 8.0*)
  - Microsoft.Extensions.Caching.Memory should be updated from `6.0.3` to `8.0.1` (*recommended for .NET 8.0*)

#### MyBills.Domain\MyBills.Domain.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

NuGet packages changes:
  - Newtonsoft.Json should be removed - replace with System.Text.Json (built into .NET 8)

Feature upgrades:
  - Replace Newtonsoft.Json serialization with System.Text.Json in all entity classes (10 files identified):
    - BiMonthlyRecurrence.cs
    - BiWeeklyEvenRecurrence.cs
    - BiWeeklyOddRecurrence.cs
    - BiYearlyRecurrence.cs
    - DailyRecurrence.cs
    - MonthlyRecurrence.cs
    - OnetimeRecurrence.cs
    - QuarterlyRecurrence.cs
    - WeeklyRecurrence.cs
    - YearlyRecurrence.cs
  - Replace `[JsonProperty]` attributes with `[JsonPropertyName]` from System.Text.Json.Serialization
  - Remove `using Newtonsoft.Json;` and add `using System.Text.Json.Serialization;`
  - Update any JsonConvert.SerializeObject/DeserializeObject calls to use System.Text.Json.JsonSerializer

#### MyBills.Data\MyBills.Data.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `6.0.36` to `8.0.26` (*recommended for .NET 8.0*)
  - Microsoft.EntityFrameworkCore.Relational should be updated from `6.0.36` to `8.0.26` (*recommended for .NET 8.0*)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `6.0.36` to `8.0.26` (*recommended for .NET 8.0*)
  - Microsoft.Extensions.Configuration should be updated from `6.0.2` to `8.0.0` (*recommended for .NET 8.0*)

Feature upgrades:
  - Add nullable reference type annotations for DbContext classes (MyBillsContext, EfLogContext)
  - Add nullable annotations for repository classes and configurations
  - Review EF Core 8.0 breaking changes:
    - Check for DateOnly/TimeOnly usage (new types with better support in EF Core 8)
    - Review any raw SQL queries for compatibility
    - Check for changes in query translation behavior

#### MyBills.Services\MyBills.Services.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

Feature upgrades:
  - Add nullable reference type annotations for all service classes
  - Update method signatures to use nullable reference types where appropriate

#### MyBills.Mvc\MyBills.Mvc.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Tools should be updated from `6.0.36` to `8.0.26` (*recommended for .NET 8.0*)
  - Microsoft.Extensions.Logging.Debug should be updated from `6.0.1` to `8.0.1` (*recommended for .NET 8.0*)
  - Microsoft.VisualStudio.Web.CodeGeneration.Design should be updated from `6.0.18` to `8.0.23` (*recommended for .NET 8.0*)

Feature upgrades:
  - Add nullable reference type annotations for all controllers
  - Update view models to use nullable reference types
  - Review and update any JSON serialization in controllers to use System.Text.Json
  - Verify Program.cs uses .NET 8 best practices

## Newtonsoft.Json to System.Text.Json Migration Strategy

### Why System.Text.Json?

- **Performance**: 2-3x faster serialization/deserialization
- **Memory**: Lower memory allocations and garbage collection pressure
- **Native**: Built into .NET 8, no external dependencies
- **Modern**: Better support for modern C# features like records and nullable reference types

### Migration Approach

1. **Attribute replacement**: `[JsonProperty("name")]` ? `[JsonPropertyName("name")]`
2. **Namespace replacement**: `using Newtonsoft.Json;` ? `using System.Text.Json.Serialization;`
3. **API replacement**: 
   - `JsonConvert.SerializeObject(obj)` ? `JsonSerializer.Serialize(obj)`
   - `JsonConvert.DeserializeObject<T>(json)` ? `JsonSerializer.Deserialize<T>(json)`
4. **Configuration**: If custom settings are needed, configure JsonSerializerOptions

### Known Differences to Address

- **Case sensitivity**: System.Text.Json is case-sensitive by default
- **Property naming**: Configure PropertyNamingPolicy if needed (camelCase vs PascalCase)
- **Null handling**: Different default null handling behavior
- **Polymorphism**: Different approach for type discrimination

## Nullable Reference Types Strategy

### Benefits

- **Compile-time safety**: Catch null reference errors at compile time
- **Better documentation**: Clear intent about nullability in APIs
- **IDE support**: Better IntelliSense and code analysis
- **Modern C#**: Aligns with .NET 8 best practices

### Phased Approach

**Phase 1: Enable with warnings**
- Add `<Nullable>enable</Nullable>` to all .csproj files
- Initially suppress warnings to prevent overwhelming errors
- Gradually address warnings project by project

**Phase 2: Fix entities and DTOs**
- Start with domain entities (MyBills.Domain)
- Add `?` to nullable properties
- Use `= null!;` for properties initialized in constructors
- Add `required` keyword where appropriate (.NET 8 feature)

**Phase 3: Fix data layer**
- Update DbContext and repositories
- Annotate navigation properties correctly
- Handle nullable foreign keys

**Phase 4: Fix service layer**
- Update service interfaces and implementations
- Fix method return types and parameters
- Handle null checks properly

**Phase 5: Fix presentation layer**
- Update controllers and view models
- Fix view model binding
- Handle nullable user input

### Common Patterns

```csharp
// Property that can't be null (initialized by EF or constructor)
public string Name { get; set; } = null!;

// Property that can be null
public string? Description { get; set; }

// Required property (.NET 8)
public required string Email { get; set; }

// Nullable reference with default
public List<Item> Items { get; set; } = new();
```

## Testing Strategy

1. **Build validation**: Ensure solution compiles with no errors
2. **Unit tests**: Run existing tests (if any) to verify behavior
3. **Integration tests**: Test database operations with EF Core 8
4. **JSON serialization tests**: Verify System.Text.Json produces expected output
5. **Manual testing**: Test critical user flows
6. **Performance testing**: Validate performance improvements from System.Text.Json

## Risk Mitigation

- **Incremental commits**: Commit after each major step
- **Branch strategy**: Work on feature/upgrade-to-dotnet8 branch
- **Backup**: Ensure .NET 6 version is committed before starting
- **Rollback plan**: Can revert to .NET 6 if critical issues arise
- **Testing**: Thorough testing at each phase before proceeding
