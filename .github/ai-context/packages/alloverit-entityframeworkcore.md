# AllOverIt.EntityFrameworkCore

## Purpose
This package provides EntityFramework Core utilities such as a database migration helper and a value converter for AllOverIt based enriched enums.

## Package Metadata
- Project: Source/AllOverIt.EntityFrameworkCore/AllOverIt.EntityFrameworkCore.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Query, EntityFramework
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class DatabaseMigratorBase
- class EnrichedEnumEntityOptions
- class EnrichedEnumModelBuilderOptions
- class EnrichedEnumNameConverter
- class EnrichedEnumPropertyOptions
- class EnrichedEnumValueConverter
- class MigrationCheckerBase
- class MigrationEventArgs
- class MigrationOptions
- class ModelBuilderExtensions
- enum MigrationStatus
- interface IDatabaseMigrator

### Representative extension methods
- UseEnrichedEnum

### Active namespaces
- AllOverIt.EntityFrameworkCore.EnrichedEnum
- AllOverIt.EntityFrameworkCore.Migrator
- AllOverIt.EntityFrameworkCore.ValueConverters
- AllOverIt.EntityFrameworkCore.Extensions
- AllOverIt.EntityFrameworkCore.Migrator.Events

## Demo Projects
- Demos/AllOverIt.EntityFrameworkCore/EFEnumerationDemo/EFEnumerationDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
