REM Add '--no-build' to run the tests without building the project

dotnet test ".\Tests\AllOverIt.Tests\AllOverIt.Tests.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=coverage\
"%USERPROFILE%\.nuget\packages\reportgenerator\4.6.6\tools\netcoreapp3.0\ReportGenerator.exe" "-reports:.\Tests\AllOverIt.Tests\coverage\coverage.cobertura.xml" "-targetdir:.\Tests\AllOverIt.Tests\coverage\output"                  

dotnet test ".\Tests\AllOverIt.Fixture.Tests\AllOverIt.Fixture.Tests.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=coverage\
"%USERPROFILE%\.nuget\packages\reportgenerator\4.6.6\tools\netcoreapp3.0\ReportGenerator.exe" "-reports:.\Tests\AllOverIt.Fixture.Tests\coverage\coverage.cobertura.xml" "-targetdir:.\Tests\AllOverIt.Fixture.Tests\coverage\output"                  

dotnet test ".\Tests\AllOverIt.Evaluator.Tests\AllOverIt.Evaluator.Tests.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=coverage\
"%USERPROFILE%\.nuget\packages\reportgenerator\4.6.6\tools\netcoreapp3.0\ReportGenerator.exe" "-reports:.\Tests\AllOverIt.Evaluator.Tests\coverage\coverage.cobertura.xml" "-targetdir:.\Tests\AllOverIt.Evaluator.Tests\coverage\output"                  

pause
