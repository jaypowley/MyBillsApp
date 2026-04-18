# MyBills Application - .NET 8 Modernization Roadmap

## Executive Summary

This document outlines the strategic plan to modernize the MyBills application from `.NET 6` to `.NET 8 LTS`, including two critical improvements:
1. **Replace Newtonsoft.Json with System.Text.Json** for better performance
2. **Enable nullable reference types** across all projects for improved type safety

## Current State

- **Framework**: .NET 6.0
- **Projects**: 5 (Core, Domain, Data, Services, Mvc)
- **Key Dependencies**:
  - Entity Framework Core 6.0.36
  - Newtonsoft.Json 13.0.4 (MyBills.Domain)
  - ASP.NET Core MVC with minimal hosting model
  - Cookie-based authentication
  - SQL Server database

## Target State

- **Framework**: .NET 8.0 (LTS - supported until November 2026)
- **JSON Serialization**: System.Text.Json (built-in)
- **Type Safety**: Nullable reference types enabled
- **Entity Framework**: EF Core 8.0.26
- **Performance**: Improved JSON serialization (2-3x faster)
- **Code Quality**: Compile-time null safety

## Timeline & Phases

### Phase 1: Framework & Package Upgrade (Est. 2-4 hours)

**Objective**: Upgrade all projects to .NET 8 and update NuGet packages

**Tasks**:
1. ? Validate .NET 8 SDK installation
2. ? Check global.json compatibility
3. ?? Upgrade MyBills.Core to .NET 8
   - Update TargetFramework to `net8.0`
   - Update Microsoft.Extensions.Caching.* packages to 8.0.x
4. ?? Upgrade MyBills.Domain to .NET 8
   - Update TargetFramework to `net8.0`
   - Keep Newtonsoft.Json for now (will remove in Phase 2)
5. ?? Upgrade MyBills.Data to .NET 8
   - Update TargetFramework to `net8.0`
   - Update all EF Core packages to 8.0.26
6. ?? Upgrade MyBills.Services to .NET 8
   - Update TargetFramework to `net8.0`
7. ?? Upgrade MyBills.Mvc to .NET 8
   - Update TargetFramework to `net8.0`
   - Update EF Tools and logging packages to 8.0.x
8. ? Build and validate all projects

**Success Criteria**:
- All projects compile successfully on .NET 8
- No breaking changes from EF Core upgrade
- Application runs and connects to database

### Phase 2: Newtonsoft.Json ? System.Text.Json Migration (Est. 3-6 hours)

**Objective**: Replace Newtonsoft.Json with System.Text.Json

**Affected Files** (10 entity files in MyBills.Domain\Entities):
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

**Migration Steps**:

1. **Analyze current usage**
   - Review each entity file for Newtonsoft.Json attributes
   - Document any custom serialization settings
   - Identify JsonConvert usage patterns

2. **Update attributes in entity files**
   ```csharp
   // Before
   using Newtonsoft.Json;
   [JsonProperty("propertyName")]

   // After
   using System.Text.Json.Serialization;
   [JsonPropertyName("propertyName")]
   ```

3. **Replace serialization calls**
   ```csharp
   // Before
   var json = JsonConvert.SerializeObject(obj);
   var obj = JsonConvert.DeserializeObject<T>(json);

   // After
   var json = JsonSerializer.Serialize(obj);
   var obj = JsonSerializer.Deserialize<T>(json);
   ```

4. **Configure global JSON options** (in Program.cs if needed)
   ```csharp
   builder.Services.ConfigureHttpJsonOptions(options =>
   {
       options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
       options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
   });
   ```

5. **Remove Newtonsoft.Json package**
   - Remove from MyBills.Domain.csproj
   - Verify no transitive dependencies remain

6. **Test JSON serialization**
   - Test each entity type
   - Verify JSON output format matches expected schema
   - Check database serialization/deserialization

**Success Criteria**:
- No Newtonsoft.Json references in code
- Package removed from all projects
- All JSON operations work correctly
- No data corruption in database
- API responses match expected format

### Phase 3: Enable Nullable Reference Types (Est. 4-8 hours)

**Objective**: Enable nullable reference types for compile-time null safety

**Project-by-Project Approach**:

#### 3.1 MyBills.Core (Est. 1 hour)
- Add `<Nullable>enable</Nullable>` to .csproj
- Annotate AppSettings class
- Fix caching-related classes
- Address compiler warnings

#### 3.2 MyBills.Domain (Est. 2 hours)
- Add `<Nullable>enable</Nullable>` to .csproj
- Annotate all entity classes
  - Use `= null!;` for EF-initialized properties
  - Use `?` for truly nullable properties
  - Use `required` for required properties (.NET 8)
- Update interface definitions
- Fix all compiler warnings

**Example entity annotation**:
```csharp
public class Bill
{
    public int Id { get; set; }

    // Required property, initialized by EF
    public string Name { get; set; } = null!;

    // Nullable property
    public string? Description { get; set; }

    // Required property (.NET 8)
    public required string Category { get; set; }

    // Navigation property (required)
    public User User { get; set; } = null!;

    // Collection navigation (never null)
    public List<Payment> Payments { get; set; } = new();
}
```

#### 3.3 MyBills.Data (Est. 1-2 hours)
- Add `<Nullable>enable</Nullable>` to .csproj
- Annotate DbContext classes
- Update repository interfaces and implementations
- Annotate entity configurations
- Handle nullable foreign keys correctly
- Fix all compiler warnings

#### 3.4 MyBills.Services (Est. 1-2 hours)
- Add `<Nullable>enable</Nullable>` to .csproj
- Annotate service interfaces
- Update service implementations
- Add null checks where appropriate
- Use null-coalescing operators where helpful
- Fix all compiler warnings

#### 3.5 MyBills.Mvc (Est. 1-2 hours)
- Add `<Nullable>enable</Nullable>` to .csproj
- Annotate controller classes
- Update view models
- Handle nullable model binding
- Update Program.cs if needed
- Fix all compiler warnings

**Success Criteria**:
- All projects build with nullable enabled
- Zero nullable reference warnings
- Proper null handling throughout codebase
- No runtime null reference exceptions in testing

### Phase 4: Validation & Testing (Est. 2-4 hours)

**Objective**: Comprehensive testing of the upgraded application

**Test Areas**:

1. **Build Validation**
   - Clean build of entire solution
   - No warnings (nullable or otherwise)
   - All dependencies resolved

2. **Unit Tests** (if they exist)
   - Run all existing unit tests
   - Fix any broken tests
   - Add tests for JSON serialization changes

3. **Database Operations**
   - Test all CRUD operations
   - Verify EF Core 8 queries work correctly
   - Check migration history integrity
   - Test any raw SQL queries

4. **JSON Serialization**
   - Test API endpoints (if any)
   - Verify database JSON column serialization
   - Check configuration serialization
   - Validate error handling

5. **Application Functionality**
   - User registration/login
   - Bill CRUD operations
   - Recurrence scheduling
   - Reports and queries
   - All major user flows

6. **Performance Testing**
   - Baseline JSON serialization performance
   - Compare with previous Newtonsoft.Json performance
   - Check memory usage
   - Monitor database query performance

**Success Criteria**:
- All tests pass
- Application functions correctly
- No runtime errors
- Performance meets or exceeds .NET 6 baseline
- Database operations work correctly

### Phase 5: Documentation & Deployment (Est. 1-2 hours)

**Objective**: Document changes and prepare for deployment

**Tasks**:

1. **Update Documentation**
   - Update README with .NET 8 requirements
   - Document any breaking changes
   - Update setup instructions
   - Note JSON serialization changes

2. **Generate Upgrade Report**
   - Comprehensive changes summary
   - Performance improvements
   - Breaking changes addressed
   - Next steps recommendations

3. **Code Review**
   - Review all nullable annotations
   - Check JSON serialization changes
   - Verify EF Core 8 usage
   - Ensure best practices

4. **Prepare Deployment**
   - Update deployment scripts
   - Update CI/CD pipeline (if any)
   - Verify .NET 8 runtime on servers
   - Plan rollback strategy

**Success Criteria**:
- Complete documentation
- Upgrade report generated
- Deployment plan ready
- Team briefed on changes

## Rollback Plan

If critical issues arise during the upgrade:

1. **Immediate Rollback**
   - Switch back to `feature/upgrade-to-dotnet6` branch
   - Redeploy .NET 6 version
   - Investigate issues offline

2. **Partial Rollback Options**
   - Keep .NET 8, revert to Newtonsoft.Json if System.Text.Json causes issues
   - Keep .NET 8, disable nullable if too many warnings to address quickly

3. **Data Safety**
   - No database schema changes expected
   - Existing data should remain compatible
   - Backup database before testing

## Expected Benefits

### Performance Improvements
- **JSON Serialization**: 2-3x faster with System.Text.Json
- **Memory Usage**: Lower GC pressure from System.Text.Json
- **EF Core 8**: Improved query performance and translation
- **Runtime**: General .NET 8 performance improvements

### Code Quality Improvements
- **Type Safety**: Compile-time null checking prevents runtime errors
- **Maintainability**: Clearer intent with nullable annotations
- **Modern C#**: Leverage latest language features (.NET 8)
- **Dependency Management**: One less external dependency

### Long-term Support
- **.NET 8 LTS**: Supported until November 2026
- **Security Updates**: Latest security patches
- **Future-Ready**: Easier path to .NET 10 and beyond

## Risks & Mitigation

| Risk | Impact | Probability | Mitigation |
|:-----|:------:|:-----------:|:-----------|
| EF Core 8 breaking changes | High | Medium | Thorough testing of all queries; review breaking changes doc |
| JSON serialization differences | High | Medium | Comprehensive testing; configure options to match Newtonsoft behavior |
| Nullable warnings overwhelming | Medium | High | Phased approach; suppress initially if needed |
| Performance regression | Medium | Low | Performance testing; rollback plan ready |
| Database compatibility issues | High | Low | No schema changes; test thoroughly; backup data |

## Success Metrics

- ? All projects on .NET 8.0
- ? Zero Newtonsoft.Json dependencies
- ? Nullable enabled with zero warnings
- ? All tests passing
- ? Application fully functional
- ? Performance improved or maintained
- ? No runtime errors in validation testing

## Next Steps After .NET 8

Once .NET 8 upgrade is stable:

1. **Consider .NET 10** (when available, likely November 2025)
2. **Modernize DbContext**: Move from OnConfiguring to DI registration
3. **Add API endpoints**: Consider minimal APIs for better performance
4. **Improve logging**: Leverage .NET 8 logging improvements
5. **Performance optimization**: Use .NET 8 performance features
6. **Code cleanup**: Remove obsolete patterns and improve architecture

## Conclusion

This phased approach balances:
- **Safety**: Incremental changes with testing at each phase
- **Efficiency**: Logical grouping of related changes
- **Quality**: Comprehensive testing and validation
- **Maintainability**: Clear documentation and rollback plans

Total estimated time: **12-24 hours** depending on:
- Complexity of JSON serialization usage
- Number of nullable warnings to address
- Extent of testing required
- Team familiarity with .NET 8 features

The upgrade provides significant value through improved performance, better type safety, and long-term support for the MyBills application.
