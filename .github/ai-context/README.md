# AllOverIt Agent Knowledge Base

This folder is generated from source projects and demo projects.
Use this as the canonical package capability map for coding agents.

## Regenerate
- Run: ./_build/generate-agent-docs.ps1
- Input sources: Source/**/*.csproj, Source/**/*.cs, and Demos/**/*.csproj

## How To Use
- Start with package-manifest.json for quick package discovery.
- Open the relevant file under packages/ for source-derived API signals and demo links.
- Prefer package APIs over bespoke implementations when a package already covers the scenario.

## Packages
- [AllOverIt](packages/alloverit.md) - demos: 26, internal dependencies: 1
- [AllOverIt.AspNetCore](packages/alloverit-aspnetcore.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Assertion](packages/alloverit-assertion.md) - demos: 0, internal dependencies: 0
- [AllOverIt.Aws.AppSync.Client](packages/alloverit-aws-appsync-client.md) - demos: 1, internal dependencies: 2
- [AllOverIt.Aws.Cdk.AppSync](packages/alloverit-aws-cdk-appsync.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Cryptography](packages/alloverit-cryptography.md) - demos: 3, internal dependencies: 1
- [AllOverIt.Csv](packages/alloverit-csv.md) - demos: 2, internal dependencies: 1
- [AllOverIt.DependencyInjection](packages/alloverit-dependencyinjection.md) - demos: 3, internal dependencies: 1
- [AllOverIt.EntityFrameworkCore](packages/alloverit-entityframeworkcore.md) - demos: 1, internal dependencies: 1
- [AllOverIt.EntityFrameworkCore.Diagrams](packages/alloverit-entityframeworkcore-diagrams.md) - demos: 1, internal dependencies: 1
- [AllOverIt.EntityFrameworkCore.Pagination](packages/alloverit-entityframeworkcore-pagination.md) - demos: 1, internal dependencies: 2
- [AllOverIt.Evaluator](packages/alloverit-evaluator.md) - demos: 7, internal dependencies: 1
- [AllOverIt.Filtering](packages/alloverit-filtering.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Fixture](packages/alloverit-fixture.md) - demos: 0, internal dependencies: 1
- [AllOverIt.Fixture.FakeItEasy](packages/alloverit-fixture-fakeiteasy.md) - demos: 0, internal dependencies: 1
- [AllOverIt.GenericHost](packages/alloverit-generichost.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Logging](packages/alloverit-logging.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Logging.Testing](packages/alloverit-logging-testing.md) - demos: 0, internal dependencies: 1
- [AllOverIt.Mapping](packages/alloverit-mapping.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Pagination](packages/alloverit-pagination.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Pipes](packages/alloverit-pipes.md) - demos: 5, internal dependencies: 2
- [AllOverIt.Reactive](packages/alloverit-reactive.md) - demos: 2, internal dependencies: 1
- [AllOverIt.ReactiveUI](packages/alloverit-reactiveui.md) - demos: 2, internal dependencies: 1
- [AllOverIt.ReactiveUI.Wpf](packages/alloverit-reactiveui-wpf.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Serialization.Binary](packages/alloverit-serialization-binary.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Serialization.Json.Abstractions](packages/alloverit-serialization-json-abstractions.md) - demos: 0, internal dependencies: 1
- [AllOverIt.Serialization.Json.Newtonsoft](packages/alloverit-serialization-json-newtonsoft.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Serialization.Json.SystemText](packages/alloverit-serialization-json-systemtext.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Serilog](packages/alloverit-serilog.md) - demos: 2, internal dependencies: 1
- [AllOverIt.Validation](packages/alloverit-validation.md) - demos: 3, internal dependencies: 1
- [AllOverIt.Validation.Options](packages/alloverit-validation-options.md) - demos: 1, internal dependencies: 1
- [AllOverIt.Wpf](packages/alloverit-wpf.md) - demos: 2, internal dependencies: 1
- [AllOverIt.Wpf.Controls](packages/alloverit-wpf-controls.md) - demos: 2, internal dependencies: 1
