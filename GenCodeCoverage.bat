rmdir /s /q TestResults
rmdir /s /q TestCoverage

REM Add '--no-build' to run the tests without building the project
REM Not using the --framework option as this causes the pipes tests to be excluded, and it's not possible to provide a 'windows' related moniker
dotnet test AllOverIt.sln --configuration Release /p:CoverletOutputFormat=cobertura --collect "XPlat Code Coverage" --results-directory TestResults
REM dotnet test AllOverIt.sln --framework net8.0 /p:CoverletOutputFormat=cobertura --collect "XPlat Code Coverage" --results-directory TestResults

REM Usage: https://reportgenerator.io/usage
ReportGenerator.exe "-reports:.\TestResults\/**/*.cobertura.xml" "-targetdir:.\TestCoverage" "-assemblyfilters:-TestUtils" "-reporttypes:Cobertura;Badges;Html;HtmlSummary;MarkdownSummary"              

rmdir /s /q TestResults

copy ".\TestCoverage\summary.html" ".\Docs\Code Coverage\summary.html"
copy ".\TestCoverage\Summary.md" ".\Docs\Code Coverage\summary.md"
copy ".\TestCoverage\badge_linecoverage.svg" ".\Docs\Code Coverage\badge_linecoverage.svg"
copy ".\TestCoverage\badge_branchcoverage.svg" ".\Docs\Code Coverage\badge_branchcoverage.svg"
copy ".\TestCoverage\badge_methodcoverage.svg" ".\Docs\Code Coverage\badge_methodcoverage.svg"

cd TestCoverage

rem pause

explorer index.html
