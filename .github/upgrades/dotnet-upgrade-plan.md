# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade MyBills.Core\MyBills.Core.csproj
4. Upgrade MyBills.Domain\MyBills.Domain.csproj
5. Upgrade MyBills.Data\MyBills.Data.csproj
6. Upgrade MyBills.Services\MyBills.Services.csproj
7. Upgrade MyBills.Mvc\MyBills.Mvc.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                      | Current Version | New Version | Description                         |
|:--------------------------------------------------|:---------------:|:-----------:|:------------------------------------|
| Microsoft.EntityFrameworkCore                     | 8.0.26          | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.EntityFrameworkCore.Relational          | 8.0.26          | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.EntityFrameworkCore.SqlServer           | 8.0.26          | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.EntityFrameworkCore.Tools               | 8.0.26          | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.Extensions.Caching.Abstractions         | 8.0.0           | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.Extensions.Caching.Memory               | 8.0.1           | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.Extensions.Configuration                | 8.0.0           | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.Extensions.Logging.Debug                | 8.0.1           | 10.0.6      | Recommended for .NET 10.0           |
| Microsoft.VisualStudio.Web.CodeGeneration.Design  | 8.0.23          | 10.0.2      | Recommended for .NET 10.0           |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### MyBills.Core\MyBills.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Caching.Abstractions should be updated from `8.0.0` to `10.0.6` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Caching.Memory should be updated from `8.0.1` to `10.0.6` (*recommended for .NET 10.0*)

#### MyBills.Domain\MyBills.Domain.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### MyBills.Data\MyBills.Data.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.26` to `10.0.6` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Relational should be updated from `8.0.26` to `10.0.6` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `8.0.26` to `10.0.6` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration should be updated from `8.0.0` to `10.0.6` (*recommended for .NET 10.0*)

#### MyBills.Services\MyBills.Services.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### MyBills.Mvc\MyBills.Mvc.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Tools should be updated from `8.0.26` to `10.0.6` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging.Debug should be updated from `8.0.1` to `10.0.6` (*recommended for .NET 10.0*)
  - Microsoft.VisualStudio.Web.CodeGeneration.Design should be updated from `8.0.23` to `10.0.2` (*recommended for .NET 10.0*)
