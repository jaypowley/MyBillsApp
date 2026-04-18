# .NET 6 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 6.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 6.0 upgrade.
3. Upgrade MyBills.Core\MyBills.Core.csproj
4. Upgrade MyBills.Domain\MyBills.Domain.csproj
5. Upgrade MyBills.Data\MyBills.Data.csproj
6. Upgrade MyBills.Services\MyBills.Services.csproj
7. Upgrade MyBills.Mvc\MyBills.Mvc.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                      | Current Version | New Version | Description                                   |
|:--------------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.EntityFrameworkCore                     | 3.1.1           | 6.0.36      | Recommended for .NET 6.0                      |
| Microsoft.EntityFrameworkCore.Relational          | 3.1.1           | 6.0.36      | Recommended for .NET 6.0                      |
| Microsoft.EntityFrameworkCore.SqlServer           | 3.1.1           | 6.0.36      | Recommended for .NET 6.0                      |
| Microsoft.EntityFrameworkCore.Tools               | 3.1.1           | 6.0.36      | Recommended for .NET 6.0                      |
| Microsoft.Extensions.Caching.Abstractions         | 3.1.1           | 6.0.1       | Recommended for .NET 6.0                      |
| Microsoft.Extensions.Caching.Memory               | 3.1.1           | 6.0.3       | Recommended for .NET 6.0                      |
| Microsoft.Extensions.Configuration                | 3.1.1           | 6.0.2       | Recommended for .NET 6.0                      |
| Microsoft.Extensions.Logging.Debug                | 3.1.1           | 6.0.1       | Recommended for .NET 6.0                      |
| Microsoft.VisualStudio.Web.CodeGeneration.Design  | 3.1.1           | 6.0.18      | Recommended for .NET 6.0                      |
| Newtonsoft.Json                                   | 13.0.1          | 13.0.4      | Recommended for .NET 6.0                      |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### MyBills.Core\MyBills.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `netstandard2.1` to `net6.0`

NuGet packages changes:
  - Microsoft.Extensions.Caching.Abstractions should be updated from `3.1.1` to `6.0.1` (*recommended for .NET 6.0*)
  - Microsoft.Extensions.Caching.Memory should be updated from `3.1.1` to `6.0.3` (*recommended for .NET 6.0*)

#### MyBills.Domain\MyBills.Domain.csproj modifications

Project properties changes:
  - Target framework should be changed from `netstandard2.1` to `net6.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.1` to `13.0.4` (*recommended for .NET 6.0*)

#### MyBills.Data\MyBills.Data.csproj modifications

Project properties changes:
  - Target framework should be changed from `netstandard2.1` to `net6.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `3.1.1` to `6.0.36` (*recommended for .NET 6.0*)
  - Microsoft.EntityFrameworkCore.Relational should be updated from `3.1.1` to `6.0.36` (*recommended for .NET 6.0*)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `3.1.1` to `6.0.36` (*recommended for .NET 6.0*)
  - Microsoft.Extensions.Configuration should be updated from `3.1.1` to `6.0.2` (*recommended for .NET 6.0*)

#### MyBills.Services\MyBills.Services.csproj modifications

Project properties changes:
  - Target framework should be changed from `netstandard2.1` to `net6.0`

#### MyBills.Mvc\MyBills.Mvc.csproj modifications

Project properties changes:
  - Target framework should be changed from `netcoreapp3.1` to `net6.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Tools should be updated from `3.1.1` to `6.0.36` (*recommended for .NET 6.0*)
  - Microsoft.Extensions.Logging.Debug should be updated from `3.1.1` to `6.0.1` (*recommended for .NET 6.0*)
  - Microsoft.VisualStudio.Web.CodeGeneration.Design should be updated from `3.1.1` to `6.0.18` (*recommended for .NET 6.0*)
