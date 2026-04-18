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
  - Replace Newtonsoft.Json serialization with System.Text.Json in all entity classes
  - Add nullable reference type annotations throughout the project
  - Update JsonProperty attributes to System.Text.Json.Serialization attributes

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
  - Add nullable reference type annotations for DbContext and repository classes
  - Review EF Core 8.0 breaking changes and update affected queries

#### MyBills.Services\MyBills.Services.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

Feature upgrades:
  - Add nullable reference type annotations for service classes

#### MyBills.Mvc\MyBills.Mvc.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net8.0`
  - Enable nullable reference types by adding `<Nullable>enable</Nullable>`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Tools should be updated from `6.0.36` to `8.0.26` (*recommended for .NET 8.0*)
  - Microsoft.Extensions.Logging.Debug should be updated from `6.0.1` to `8.0.1` (*recommended for .NET 8.0*)
  - Microsoft.VisualStudio.Web.CodeGeneration.Design should be updated from `6.0.18` to `8.0.23` (*recommended for .NET 8.0*)

Feature upgrades:
  - Add nullable reference type annotations for controllers and views
  - Update Program.cs if needed for .NET 8 best practices
