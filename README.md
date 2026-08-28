# cCoder.Logging

`cCoder.Logging` contains the Logging domain for the cCoder platform.

[View the latest code coverage report](https://ccoder-co-uk.github.io/cCoder.Logging/)

## Local Configuration

Each executable binds the complete configuration root to its own
`AppConfiguration`. The Web composition root registers `CoreData`, `Logging`,
`SecurityData`, `Security`, and `Eventing` side by side. The HostedServices
composition root registers `CoreData`, `Logging`, and `Eventing`.

Persistence belongs to the Data domains. `LoggingConfiguration` contains only
Logging behavior; `CoreData` owns the database connection, Data registration,
and migrations. Likewise, `SecurityData` owns the Security database and
`Security` contains authentication behavior.

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
dotnet build src/cCoder.Logging.slnx -v minimal
```

## Test

```powershell
dotnet test src/cCoder.Logging.slnx -v minimal --no-build
```

Leave secrets blank in `appsettings.json`, define these user-level or
machine-level environment variables, restart Visual Studio, and press F5:

- `CoreData__ConnectionString`
- `SecurityData__ConnectionString` (Web only)
- `Security__DecryptionKey` (Web only)
- `Eventing__ServiceBus__ConnectionString` when `Eventing__ProviderType` is
  `ServiceBus`

`CoreData__AdminConnectionString` and
`SecurityData__AdminConnectionString` are optional migration-only overrides. If
an admin connection is configured, startup migrations use it and normal runtime
operations continue to use the regular connection. If it is omitted, migrations
use the regular connection.

No `.env` file or configuration conversion step is required.

Library consumers register persistence and behavior explicitly at their own
composition root: call `AddData` before `AddLoggingWeb` or
`AddLoggingHostedServices`; Web hosts also call `AddSecurityData` before
`AddSecurityWeb`. An application that consumes `cCoder.Core` should use Core's
composite API instead; Core deliberately composes its configured child domains
recursively.

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
- `RequestLoggingEnabled`
  Enables automatic HTTP request capture from `StartLoggingWeb`. Defaults to `true`.
- `RequestLoggingQueueCapacity`
  Maximum number of request and application log snapshots awaiting background storage. Defaults to `1024`.
- `RequestLoggingQueueFullBehavior`
  Selects whether queue pressure drops the newest or oldest snapshot. Defaults to `DropNewest`; request processing never waits for queue capacity.
- `DatabaseMinimumLogLevel`
  Minimum `ILogger` level persisted to SQL. Defaults to `Warning`. Useful entries below the threshold remain available to LogHub streaming. HTTP request summaries are unaffected by this threshold.

Acceptance tests read the same structured environment variables. A single shared
test configuration source derives isolated database names by appending
`-acceptance-{guid}`; tests never use the configured development databases
directly.

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