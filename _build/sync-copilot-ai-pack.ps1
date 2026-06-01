[CmdletBinding(SupportsShouldProcess = $true, PositionalBinding = $false)]
# SupportsShouldProcess enables built-in -WhatIf and -Confirm behavior for this script.
param()

$ErrorActionPreference = "Stop"
$targetRepoName = "copilot-ai-pack"
$packName = "alloverit"

$sourceRepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$sourceAiContext = Join-Path $sourceRepoRoot ".github/ai-context"
$sourceCopilotInstructions = Join-Path $sourceRepoRoot ".github/copilot-instructions.md"
$generateScript = Join-Path $sourceRepoRoot "_build/generate-agent-docs.ps1"
$sourceRepoUrl = "https://github.com/mjfreelancing/AllOverIt"

if (-not (Test-Path $sourceAiContext)) {
    throw "Missing '$sourceAiContext'. Run '$generateScript' first."
}

if (-not (Test-Path $sourceCopilotInstructions)) {
    throw "Missing '$sourceCopilotInstructions'."
}

$parentPath = Split-Path -Path $sourceRepoRoot -Parent
$targetRepoRoot = Join-Path $parentPath $targetRepoName

if (-not (Test-Path $targetRepoRoot)) {
    throw "Target repository not found: '$targetRepoRoot'"
}

if (-not (Test-Path (Join-Path $targetRepoRoot ".git"))) {
    throw "Target path is not a git repository: '$targetRepoRoot'"
}

$targetPackRoot = Join-Path $targetRepoRoot "packs/$packName"
$targetAiContext = Join-Path $targetPackRoot "ai-context"
$targetTemplates = Join-Path $targetPackRoot "templates"
$targetTemplateInstructions = Join-Path $targetTemplates "copilot-instructions.md"
$targetPackReadme = Join-Path $targetPackRoot "README.md"
$targetReusableInstruction = Join-Path $targetRepoRoot "instructions/alloverit-suite.instructions.md"

function Reset-Directory {
    param(
        [Parameter(Mandatory = $true)]
        [string] $Path
    )

    if (Test-Path $Path) {
        Remove-Item -Path $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}

function Write-Utf8File {
    param(
        [Parameter(Mandatory = $true)]
        [string] $Path,

        [Parameter(Mandatory = $true)]
        [string] $Content
    )

    $directory = Split-Path -Path $Path -Parent

    if (-not (Test-Path $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    Set-Content -Path $Path -Value $Content -Encoding UTF8
}

$packReadme = @"
# AllOverIt Pack

This pack is synchronized from the sibling `AllOverIt` repository.

## Contents

- `ai-context/` - package index, manifest, and one package capability file per package
- `templates/copilot-instructions.md` - ready-to-copy repository-level Copilot routing file

## Consumer Setup

Copy these files into your repository:

- `packs/alloverit/ai-context/**` -> `.github/ai-context/alloverit/**`
- `packs/alloverit/templates/copilot-instructions.md` -> `.github/copilot-instructions.md`
- `instructions/alloverit-suite.instructions.md` -> `.github/instructions/alloverit-suite.instructions.md`

## Sync Source

Source repository: sibling `AllOverIt`

Sync script (run from `AllOverIt`):

- `./_build/sync-copilot-ai-pack.ps1`
"@

$instructionContent = @"
---
applyTo: "**/*"
---

# AllOverIt Suite Guidance

Use this guidance when the repository consumes one or more `AllOverIt.*` NuGet packages.

## Package Discovery Flow

1. Start at `packs/$packName/ai-context/README.md`.
2. Use `packs/$packName/ai-context/package-manifest.json` to identify candidate packages.
3. Read only the needed package files under `packs/$packName/ai-context/packages/`.
4. Prefer existing AllOverIt package APIs over ad-hoc implementations when requirements match.

## Response Requirements

- Name the exact package(s) used.
- Cite at least one relevant public type or extension method from package docs and source.
- Include one or more demo project references when available.
- If no package fits, say so explicitly before proposing custom code.

## Validation Rules

- Treat `packs/$packName/ai-context/*` as generated capability references.
- If uncertain, verify against current source and demos in this repository.

## When Information Is Missing

- Do not infer undocumented behavior.
- State clearly when capability details are missing from the local pack.
- Prefer asking for clarification over guessing.

If additional evidence is needed and internet access is available, use the public source as a secondary reference:

1. Source packages: $sourceRepoUrl/tree/main/Source
2. Demo projects: $sourceRepoUrl/tree/main/Demos
3. Repository root: $sourceRepoUrl

When fallback references are used, call this out explicitly in the response.
"@

# Guard all write operations so -WhatIf previews the sync without modifying files.
if ($PSCmdlet.ShouldProcess($targetPackRoot, "Sync AllOverIt pack content to copilot-ai-pack")) {
    New-Item -ItemType Directory -Path $targetPackRoot -Force | Out-Null
    New-Item -ItemType Directory -Path $targetTemplates -Force | Out-Null

    Reset-Directory -Path $targetAiContext
    Copy-Item -Path (Join-Path $sourceAiContext "*") -Destination $targetAiContext -Recurse -Force

    Copy-Item -Path $sourceCopilotInstructions -Destination $targetTemplateInstructions -Force
    Write-Utf8File -Path $targetPackReadme -Content $packReadme.Trim()
    Write-Utf8File -Path $targetReusableInstruction -Content $instructionContent.Trim()
}

Write-Host "Synchronized AllOverIt capability pack to: $targetPackRoot"
Write-Host "Reusable instruction file: $targetReusableInstruction"
