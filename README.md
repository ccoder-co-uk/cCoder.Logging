# cCoder.Logging

`cCoder.Logging` contains the Logging domain for the cCoder platform.

## Contents

- `src/cCoder.Logging`
  The main library package published to NuGet.
- `src/Logging.Web`
  The standalone web host for the Logging domain.
- `src/cCoder.Logging.Tests`
  Unit tests for the domain.
- `src/Logging.AcceptanceTests`
  Acceptance tests for the standalone host.

## Build

```powershell
dotnet build src/cCoder.Logging.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Logging.sln -v minimal --no-build
```

## Package

The NuGet package produced by this repository is:

- `cCoder.Logging`

## Publishing

GitHub Actions is configured to publish the main package using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.Logging`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.
