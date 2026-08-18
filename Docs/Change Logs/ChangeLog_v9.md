#  Version 9.4.0
## 17 Aug 2026

# AllOverIt
* ProcessExecutor will now only attempt to kill a process if it has not exited following a OperationCanceledException

# AllOverIt.GenericHost
* [Breaking] `IConsoleApp` now exposes `UserCancelled` and `CancelledExitCode`. Direct implementers
  must add them; derive from `ConsoleAppBase` to get the defaults.
* Added `UserCancelled` (set on Ctrl+C / Ctrl+Break / SIGINT; not SIGTERM; backed by a volatile field)
  to `ConsoleAppBase` and `IConsoleApp` so an application can detect a user-requested shutdown.
* Added `CancelledExitCode` (default -2) to `ConsoleAppBase` and `IConsoleApp` - the exit code reported
  when the user cancels and the application did not set an `ExitCode`.
* Fixed an exit-code race in `HostedConsoleService`: `StopAsync` now waits (bounded) for the console
  application to finish unwinding before reading its exit code, so a Ctrl+C mid-run no longer exits 0
  and discards the application's own exit code. The wait period is configurable via
  `IOptions<ConsoleHostOptions>` (`ShutdownWaitTimeout`, default 5 seconds).
* When the user cancels and the application does not set an exit code, the host now reports
  `CancelledExitCode` (-2) instead of the success default (0).
* `HostedConsoleService` now hands the console application a token linked against `ApplicationStopping`
  (held for the whole command), so the `StartAsync` cancellation token actually cancels on
  Ctrl+C/SIGTERM instead of the frozen host startup token. Applications no longer need their own
  `ApplicationStopping`-linked token workaround.
* Added `ConsoleHostOptions` (`ShutdownWaitTimeout`, default 5 seconds) to configure the shutdown wait.

---


#  Version 9.3.2
## 12 Jul 2026

# Bug Fix
* Changed ProcessExecutor so OperationCsnceledException is thrown as-is and not converted to a TimeoutException

---


#  Version 9.3.1
## 06 Jul 2026

# Maintenance update
* Package updates to allow minor version updates to be applied to consuming applications.

---


#  Version 9.2.1
## 25 May 2026

# Maintenance update
* Package updates and convert projects to use Central Package Management (https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
  Used the utility at https://github.com/Vannevelj/directory-packages-props-converter for a quick head start (I had to manually deal with version conditionals).
  Edit: Have since started using https://github.com/Webreaper/CentralisedPackageConverter as it dealt with conditionals.
* Published nugets will not set explicit upper version limit as less than next major version number.

---


#  Version 9.2.0
## 25 Apr 2026

# Maintenance update
* Package updates and switch all demos / benchmarks to NET 10.0.0 instead of a mixture


---


#  Version 9.1.0
## 31 Jan 2026

### AllOverIt
* Added stream extension that reads a full block of data into a byte array, handling partial reads.

### AllOverIt.Cryptography
* RsaEncrypter previously only supported encryption/decryption of data up to the maximum block size allowed by the key.
  Added support for encrypting/decrypting larger data by splitting into multiple blocks.

---


#  Version 9.0.0
## 26 Nov 2025

# Updated to support NET 10

### AllOverIt
* EnumerableExtensions / AsyncEnumerableExtensions updates - Change Task to ValueTask and remove ToListAsync()
  and ToArrayAsync() methods for NET 10 and above as these are not available in the runtime.
* Added support for ReadOnlyMemory<T> as an input type in relevant methods.
* Included extension methods for IAsyncEnumerable<T>.
* Added a mockable ZipPackage to support the creation of zip files.

### AllOverIt.ReactiveUI
* Updated CommandFactory to support providing a scheduler when creating cancellable commands.

### AllOverIt.EntityFrameworkCore.Diagrams
* Added support for join tables added via UsingEntity() in many-to-many relationships.
* Added support for defining groups based on the table name rather than a generic - for when shadow tables are used.

---
