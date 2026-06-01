$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$sourceRoot = Join-Path $repoRoot "Source"
$demosRoot = Join-Path $repoRoot "Demos"
$outRoot = Join-Path $repoRoot ".github/ai-context"
$packagesOut = Join-Path $outRoot "packages"

New-Item -ItemType Directory -Force -Path $packagesOut | Out-Null

$sourceProjects = Get-ChildItem -Path $sourceRoot -Recurse -Filter *.csproj | Sort-Object FullName
$demoProjects = Get-ChildItem -Path $demosRoot -Recurse -Filter *.csproj | Sort-Object FullName

$demoByPackage = @{}
foreach ($demo in $demoProjects) {
    $rel = [System.IO.Path]::GetRelativePath($repoRoot, $demo.FullName).Replace('\', '/')
    if ($rel -notmatch '^Demos/([^/]+)/') {
        continue
    }

    $pkg = $Matches[1]

    if (-not $demoByPackage.ContainsKey($pkg)) {
        $demoByPackage[$pkg] = New-Object System.Collections.Generic.List[string]
    }

    $demoByPackage[$pkg].Add($rel)
}

function Get-CsprojValue {
    param(
        [Parameter(Mandatory = $true)]
        [xml] $Xml,

        [Parameter(Mandatory = $true)]
        [string] $Name
    )

    foreach ($pg in $Xml.Project.PropertyGroup) {
        $node = $pg.$Name

        if ($null -ne $node -and [string]::IsNullOrWhiteSpace([string] $node) -eq $false) {
            return [string] $node
        }
    }

    return ""
}

function Get-PublicApiSignals {
    param(
        [Parameter(Mandatory = $true)]
        [string] $PackageDir
    )

    $codeFiles = Get-ChildItem -Path $PackageDir -Recurse -Filter *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

    $typeHits = @{}
    $extMethodHits = @{}
    $nsHits = @{}

    foreach ($file in $codeFiles) {
        $text = Get-Content -Path $file.FullName -Raw

        [regex]::Matches($text, '(?m)^\s*namespace\s+([A-Za-z0-9_\.]+)') | ForEach-Object {
            $name = $_.Groups[1].Value

            if (-not $nsHits.ContainsKey($name)) {
                $nsHits[$name] = 0
            }

            $nsHits[$name]++
        }

        [regex]::Matches($text, '(?m)^\s*public\s+(?:sealed\s+|static\s+|abstract\s+|partial\s+)*(class|record|interface|enum)\s+([A-Za-z0-9_]+)') | ForEach-Object {
            $kind = $_.Groups[1].Value
            $name = $_.Groups[2].Value
            $key = "$kind $name"

            if (-not $typeHits.ContainsKey($key)) {
                $typeHits[$key] = 0
            }

            $typeHits[$key]++
        }

        if ($text -match '(?m)^\s*public\s+static\s+class\s+[A-Za-z0-9_]*Extensions\b') {
            [regex]::Matches($text, '(?m)^\s*public\s+static\s+[A-Za-z0-9_<>,\[\]\?\.\s]+\s+([A-Za-z0-9_]+)\s*\(\s*this\s+') | ForEach-Object {
                $name = $_.Groups[1].Value

                if (-not $extMethodHits.ContainsKey($name)) {
                    $extMethodHits[$name] = 0
                }

                $extMethodHits[$name]++
            }
        }
    }

    return [PSCustomObject] @{
        TopTypes            = @($typeHits.GetEnumerator() | Sort-Object -Property @{ Expression = 'Value'; Descending = $true }, Name | Select-Object -First 12 | ForEach-Object { $_.Name })
        TopExtensionMethods = @($extMethodHits.GetEnumerator() | Sort-Object -Property @{ Expression = 'Value'; Descending = $true }, Name | Select-Object -First 12 | ForEach-Object { $_.Name })
        TopNamespaces       = @($nsHits.GetEnumerator() | Sort-Object -Property @{ Expression = 'Value'; Descending = $true }, Name | Select-Object -First 10 | ForEach-Object { $_.Name })
    }
}

$manifest = New-Object System.Collections.Generic.List[object]

foreach ($proj in $sourceProjects) {
    [xml] $xml = Get-Content -Path $proj.FullName -Raw
    $packageName = [System.IO.Path]::GetFileNameWithoutExtension($proj.Name)
    $packageDir = Split-Path -Path $proj.FullName -Parent
    $packageRelDir = [System.IO.Path]::GetRelativePath($repoRoot, $packageDir).Replace('\', '/')

    $description = Get-CsprojValue -Xml $xml -Name "Description"
    $tags = (Get-CsprojValue -Xml $xml -Name "PackageTags") -split ';' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() }
    $frameworks = Get-CsprojValue -Xml $xml -Name "TargetFrameworks"

    if ([string]::IsNullOrWhiteSpace($frameworks)) {
        $frameworks = Get-CsprojValue -Xml $xml -Name "TargetFramework"
    }

    $refs = @()

    foreach ($itemGroup in $xml.Project.ItemGroup) {
        foreach ($pr in $itemGroup.ProjectReference) {
            if ($null -ne $pr.Include) {
                $refName = [System.IO.Path]::GetFileNameWithoutExtension([string] $pr.Include)
                $refs += $refName
            }
        }
    }

    $refs = $refs | Sort-Object -Unique

    $api = Get-PublicApiSignals -PackageDir $packageDir

    $packageDemos = @()

    if ($demoByPackage.ContainsKey($packageName)) {
        $packageDemos = $demoByPackage[$packageName] | Sort-Object
    }

    $slug = $packageName.ToLowerInvariant().Replace('.', '-')
    $docRel = ".github/ai-context/packages/$slug.md"
    $docPath = Join-Path $repoRoot $docRel

    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("# $packageName")
    $lines.Add("")
    $lines.Add("## Purpose")

    if ([string]::IsNullOrWhiteSpace($description)) {
        $lines.Add("Description not declared in the project file. Review source APIs listed below.")
    }
    else {
        $lines.Add($description.Trim())
    }

    $lines.Add("")
    $lines.Add("## Package Metadata")
    $lines.Add("- Project: $packageRelDir/$($proj.Name)")

    if (-not [string]::IsNullOrWhiteSpace($frameworks)) {
        $lines.Add("- Target frameworks: $frameworks")
    }

    if ($tags.Count -gt 0) {
        $lines.Add("- Tags: " + ($tags -join ', '))
    }

    if ($refs.Count -gt 0) {
        $lines.Add("- Depends on: " + ($refs -join ', '))
    }

    $lines.Add("")
    $lines.Add("## Public API Signals (from source)")

    if ($api.TopTypes.Count -gt 0) {
        $lines.Add("### Representative public types")

        foreach ($t in $api.TopTypes) {
            $lines.Add("- $t")
        }
    }

    if ($api.TopExtensionMethods.Count -gt 0) {
        $lines.Add("")
        $lines.Add("### Representative extension methods")

        foreach ($m in $api.TopExtensionMethods) {
            $lines.Add("- $m")
        }
    }

    if ($api.TopNamespaces.Count -gt 0) {
        $lines.Add("")
        $lines.Add("### Active namespaces")

        foreach ($n in $api.TopNamespaces) {
            $lines.Add("- $n")
        }
    }

    $lines.Add("")
    $lines.Add("## Demo Projects")

    if ($packageDemos.Count -eq 0) {
        $lines.Add("- No package-specific demo folder found under Demos/$packageName")
    }
    else {
        foreach ($d in $packageDemos) {
            $lines.Add("- $d")
        }
    }

    $lines.Add("")
    $lines.Add("## Notes For Agents")
    $lines.Add("- Prefer APIs listed under this package over ad-hoc implementations when requirements match.")
    $lines.Add("- Verify runtime and target compatibility before suggesting package usage.")

    Set-Content -Path $docPath -Value ($lines -join "`r`n") -Encoding UTF8

    $manifest.Add([PSCustomObject] @{
            Package          = $packageName
            Project          = "$packageRelDir/$($proj.Name)"
            TargetFrameworks = $frameworks
            Description      = $description
            Tags             = @($tags)
            DependsOn        = @($refs)
            DemoProjects     = @($packageDemos)
            Doc              = $docRel
        }) | Out-Null
}

$manifestPath = Join-Path $outRoot "package-manifest.json"
$manifest | Sort-Object Package | ConvertTo-Json -Depth 8 | Set-Content -Path $manifestPath -Encoding UTF8

$indexLines = New-Object System.Collections.Generic.List[string]
$indexLines.Add("# AllOverIt Agent Knowledge Base")
$indexLines.Add("")
$indexLines.Add("This folder is generated from source projects and demo projects.")
$indexLines.Add("Use this as the canonical package capability map for coding agents.")
$indexLines.Add("")
$indexLines.Add("## Regenerate")
$indexLines.Add("- Run: ./_build/generate-agent-docs.ps1")
$indexLines.Add("- Input sources: Source/**/*.csproj, Source/**/*.cs, and Demos/**/*.csproj")
$indexLines.Add("")
$indexLines.Add("## How To Use")
$indexLines.Add("- Start with package-manifest.json for quick package discovery.")
$indexLines.Add("- Open the relevant file under packages/ for source-derived API signals and demo links.")
$indexLines.Add("- Prefer package APIs over bespoke implementations when a package already covers the scenario.")
$indexLines.Add("")
$indexLines.Add("## Packages")

foreach ($pkg in ($manifest | Sort-Object Package)) {
    $demoCount = $pkg.DemoProjects.Count
    $refCount = $pkg.DependsOn.Count
    $indexLines.Add("- [$($pkg.Package)]($($pkg.Doc.Replace('.github/ai-context/', ''))) - demos: $demoCount, internal dependencies: $refCount")
}

Set-Content -Path (Join-Path $outRoot "README.md") -Value ($indexLines -join "`r`n") -Encoding UTF8

Write-Output "Generated $($manifest.Count) package docs at .github/ai-context/packages"