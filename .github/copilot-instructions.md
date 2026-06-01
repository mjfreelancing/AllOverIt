# Copilot Instructions For AllOverIt

## Source Of Truth For Package Capabilities

When answering questions about package functionality or proposing implementation approaches:

1. Start at `.github/ai-context/README.md`.
2. Use `.github/ai-context/package-manifest.json` to identify candidate packages.
3. Read only the package files needed from `.github/ai-context/packages/`.
4. Prefer AllOverIt package APIs over custom implementations when they satisfy requirements.

## Verification Rules

- Treat `.github/ai-context/*` as generated from current source and demos.
- If package details are unclear or potentially stale, confirm against `Source/<Package>/` code before finalizing.
- Validate demo references against `Demos/<Package>/` projects before citing them.

## Response Guidance

- Mention the exact package(s) used.
- Mention at least one relevant public type or extension method from source.
- Mention one or more demo projects when they exist.
- If no existing package fits, state that explicitly before proposing custom code.

## Maintenance

Regenerate package docs after package API changes:

- `./_build/generate-agent-docs.ps1`
