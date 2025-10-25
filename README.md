# CourseHub

Setup and run guide for the CourseHub solution.

## Overview

CourseHub is a .NET 8 multi-project solution that powers an online course platform.

- **CourseHub.API** – ASP.NET Core Web API that hosts the back-end services.
- **CourseHub.UI** – Razor Pages front-end that consumes the API.
- **CourseHub.Core** – Shared domain models, interfaces, and business services.
- **CourseHub.Infrastructure** – Data access layer, repositories, and EF Core integrations.
- **CourseHub.Tests** – xUnit test suite with Moq and FluentAssertions.

## Prerequisites

- .NET SDK 8.0.100 or newer (`dotnet --version`).
- SQL Server instance (LocalDB, SQL Express, or an external server).
- PowerShell 5.1+ or Windows Terminal for running CLI commands.

## Configuration

1. Copy `CourseHub.API/appsettings.json` to `appsettings.Development.json` if you want environment-specific overrides.
2. Update the key settings:
   - `ConnectionStrings` – point to your SQL Server.
   - `TargetConnectionStrings:Context` – choose the connection string key (e.g., `Local`).
   - `JwtOptions:Secret` – must be at least 32 characters (256 bits) to satisfy HS256 signing requirements.
   - `External` – configure Google OAuth, VNPay, Gmail, and other integrations if needed.
3. In `CourseHub.UI/appsettings.json`, adjust `ServicePaths:ApiServerPath` if the API is not running on `https://localhost:7277`.

> Tokens are stored in cookies. Ensure you run over HTTPS and review `CookieOptions` before deploying to production.

## Running the solution

All commands assume the working directory is the repository root (`CourseHub`).

### 1. Restore dependencies

```powershell
dotnet restore
```

### 2. Start the API

```powershell
dotnet run --project CourseHub.API/CourseHub.API.csproj
```

The API listens on `https://localhost:7277` (defined in `launchSettings.json`).

### 3. Start the UI

Open a second shell and run:

```powershell
dotnet run --project CourseHub.UI/CourseHub.UI.csproj
```

The UI listens on `https://localhost:7117`. Confirm CORS origins in the API match the UI host.

### 4. Static assets

All necessary front-end libraries (Bootstrap, jQuery, SignalR) are included in `wwwroot`; no extra npm build step is required.

## Running tests

```powershell
dotnet test CourseHub.sln --configuration Release
```

GitHub Actions (`.github/workflows/dotnet-ci.yml`) performs restore, build, and test with the .NET 8 SDK.

## Báo cáo Coverage (Tiếng Việt)

- Chạy kiểm thử kèm báo cáo coverage:
  ```powershell
  dotnet test CourseHub.sln --configuration Release --collect:"XPlat Code Coverage"
  ```
- Tệp Cobertura được tạo tại `CourseHub.Tests/TestResults/<test-run-id>/coverage.cobertura.xml`.
- Có thể dùng `dotnet tool install -g dotnet-reportgenerator-globaltool` và
  ```powershell
  reportgenerator -reports:CourseHub.Tests/TestResults/**/coverage.cobertura.xml -targetdir:coverage-report
  ```
  để tạo báo cáo HTML để dễ quan sát hơn.

## Common issues

- **IDX10720: key size** – increase `JwtOptions:Secret` to at least 32 characters.
- **jQuery `$().fn` undefined** – ensure only one jQuery/Bootstrap bundle is loaded; the layout already uses the supported set.
- **ImageSharp advisory** – monitor `SixLabors.ImageSharp` for patched versions addressing GHSA-rxmq-m78w-7wmc.
- **Nullable warnings** – many models lack nullability annotations; plan to mark members as nullable or use the `required` keyword.

## Solution structure

```
CourseHub.sln
├── CourseHub.API/
├── CourseHub.Core/
├── CourseHub.Infrastructure/
├── CourseHub.UI/
└── CourseHub.Tests/
```

## Contributing

1. Branch from `upgrade-to-NET8`.
2. Implement changes and add/update tests where necessary.
3. Run `dotnet test` before submitting.
4. Open a pull request with a clear summary of your changes.

## Licensing notes

The solution depends on third-party libraries (ASP.NET Core, EF Core, ImageSharp, etc.). Review their licenses before redistributing or deploying.
