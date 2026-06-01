# AllOverIt.Csv

## Purpose
Using CsvHelper to perform the actual writing of CSV content, this package provides a configuration based serializer that can export data using a model with fixed columns as well as dynamic columns derived from complex or array type properties.

## Package Metadata
- Project: Source/AllOverIt.Csv/AllOverIt.Csv.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, CSV, Export
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class BufferedCsvExporterBase
- class BufferedCsvExporterConfiguration
- class CsvExporterException
- class CsvSerializer
- class CsvSerializerException
- class CsvSerializerExtensions
- class FieldIdentifier
- class FileCsvExporterBase
- class MemoryCsvExporterBase
- interface IBufferedCsvExporter
- interface ICsvSerializer
- interface IFieldResolver

### Active namespaces
- AllOverIt.Csv.Exporter
- AllOverIt.Csv
- AllOverIt.Csv.Exceptions
- AllOverIt.Csv.Extensions

## Demo Projects
- Demos/AllOverIt.Csv/CsvExporterDemo/CsvExporterDemo.csproj
- Demos/AllOverIt.Csv/CsvSerializerDemo/CsvSerializerDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
