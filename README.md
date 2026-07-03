# cCoder.Logging

`cCoder.Logging` contains the Logging domain for the cCoder platform.

## Functionality

The Logging domain provides:

- OData endpoints for reading and posting log entries and attached log data.
- A DB-backed `ILogger` provider that can capture application logging calls into the logging tables.
- App-aware log storage: persisted log entries include `AppId` so logs remain scoped to the owning aggregate root without adding a hard foreign key.
- SignalR log streaming through the Web host at `/Api/Hubs/Logs`.
- A hosted retention cleaner that removes persisted log entries older than the configured retention period.
- A lightweight authenticated tester UI at `/tools/index.html`.

Public log table APIs intentionally expose only `GET` and `POST`. Log records are append-only from the API surface; update, merge, patch, and delete operations are not exposed.

## Contents

- `src/cCoder.Logging`
  The main library package published to NuGet.
- `src/Apps/Logging.Web`
  The standalone web host for the Logging domain. It hosts API, Swagger, SignalR log streaming, health, and the tester UI.
- `src/Apps/Logging.HostedServices`
  The hosted services process for scheduled logging work. It currently hosts `LogRetentionCleaner`.
- `src/cCoder.Logging.Tests`
  Unit tests for the domain.
- `src/Apps/Logging.Web.AcceptanceTests`
  Acceptance tests for the Web host.
- `src/Apps/Logging.HostedServices.AcceptanceTests`
  Acceptance tests for the Hosted Services host.

There is no integration test project currently because Logging does not initiate a cross-process application call chain.

## Build

```powershell
dotnet build src/cCoder.Logging.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Logging.sln -v minimal --no-build
```

## Local Configuration

The standalone web host reads local secrets from environment variables rather than committed config.

Before running `src/Apps/Logging.Web` or `src/Apps/Logging.HostedServices`, set:

- `ConnectionStrings__Core`

Before running `src/Apps/Logging.Web`, also set:

- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

Logging behavior is configured through `LoggingConfiguration`:

- `StoreLogEntries`
  Enables DB persistence for captured `ILogger` calls.
- `StreamLogEntries`
  Enables SignalR streaming from captured `ILogger` calls in the Web host.
- `RetentionDays`
  Number of days of logs to retain. Defaults to `30`.
- `RetentionIntervalMinutes`
  How often the hosted retention cleaner wakes up. Defaults to `60`.
- `DefaultAppId`
  Fallback App aggregate root id for captured logs when the app cannot be resolved from the request domain.
- `DefaultAppDomain`
  Fallback app/domain thread name for captured logs and stream messages.

Acceptance tests require these environment variables or equivalent values in `src/Apps/Logging.Web.AcceptanceTests/appsettings.testing.json`:

- `CCODER_ACCEPTANCE_CORE_CONNECTION_STRING`
- `CCODER_ACCEPTANCE_SSO_CONNECTION_STRING`

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
