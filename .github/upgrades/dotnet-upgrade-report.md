# .NET 6 Upgrade Report

## Project target framework modifications

| Project name                     | Old Target Framework | New Target Framework | Status     |
|:---------------------------------|:--------------------:|:--------------------:|:-----------|
| MyBills.Core\MyBills.Core.csproj | netstandard2.1       | net6.0               | ✅ Complete |
| MyBills.Domain\MyBills.Domain.csproj | netstandard2.1   | net6.0               | ✅ Complete |
| MyBills.Data\MyBills.Data.csproj | netstandard2.1       | net6.0               | ✅ Complete |
| MyBills.Services\MyBills.Services.csproj | netstandard2.1 | net6.0             | ✅ Complete |
| MyBills.Mvc\MyBills.Mvc.csproj   | netcoreapp3.1        | net6.0               | ✅ Complete |

## NuGet Packages

| Package Name                                    | Old Version | New Version | Projects                          |
|:------------------------------------------------|:-----------:|:-----------:|:----------------------------------|
| Microsoft.EntityFrameworkCore                   | 3.1.1       | 6.0.36      | MyBills.Data                      |
| Microsoft.EntityFrameworkCore.Relational        | 3.1.1       | 6.0.36      | MyBills.Data                      |
| Microsoft.EntityFrameworkCore.SqlServer         | 3.1.1       | 6.0.36      | MyBills.Data                      |
| Microsoft.EntityFrameworkCore.Tools             | 3.1.1       | 6.0.36      | MyBills.Mvc                       |
| Microsoft.Extensions.Caching.Abstractions       | 3.1.1       | 6.0.1       | MyBills.Core                      |
| Microsoft.Extensions.Caching.Memory             | 3.1.1       | 6.0.3       | MyBills.Core                      |
| Microsoft.Extensions.Configuration              | 3.1.1       | 6.0.2       | MyBills.Data                      |
| Microsoft.Extensions.Logging.Debug              | 3.1.1       | 6.0.1       | MyBills.Mvc                       |
| Microsoft.VisualStudio.Web.CodeGeneration.Design| 3.1.1       | 6.0.18      | MyBills.Mvc                       |
| Newtonsoft.Json                                 | 13.0.1      | 13.0.4      | MyBills.Domain                    |

## Summary

Successfully upgraded your MyBills solution from `.NET Core 3.1` / `.NET Standard 2.1` to `.NET 6.0`:

- **5 projects** upgraded with target framework modifications
- **10 NuGet packages** updated, including critical Entity Framework Core packages upgraded from `3.1.1` to `6.0.36`
- All projects validated successfully with no build errors

## Next steps

### Critical: Startup.cs Migration

Your **MyBills.Mvc** project still contains `Startup.cs`, which needs to be migrated to the new `.NET 6` minimal hosting model in `Program.cs`:

1. **Review Startup.cs** - Document all service registrations and middleware configuration
2. **Merge into Program.cs** - Move:
   - `ConfigureServices` → `builder.Services...` 
   - `Configure` → `app...` pipeline
3. **Preserve middleware order** - Ensure routing, authentication, endpoints remain in correct sequence
4. **Test thoroughly** - Verify DI, EF contexts, configuration binding all work
5. **Delete Startup.cs** - Only after confirming Program.cs parity

### Entity Framework Validation

Although EF packages are upgraded to `6.0.36`, you should:

1. **Test LINQ queries** - EF Core 6 has stricter client evaluation rules
2. **Review DbContext registrations** - Confirm lifetimes and connection strings in new hosting model
3. **Validate migrations** - Run `dotnet ef migrations list` to ensure migration history is intact
4. **Check for breaking changes** - Review [EF Core 6 breaking changes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/breaking-changes)

### Future Modernization Path

- **.NET 6 → .NET 8** (recommended LTS before targeting .NET 10)
- Consider replacing `Newtonsoft.Json` with `System.Text.Json` for better performance
- Evaluate nullable reference types across all projects

Would you like help with the **Startup.cs → Program.cs migration** next?
