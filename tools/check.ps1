$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$env:FORGEAX_HISTORY_DIR = Join-Path $PWD 'qa/operation-history'
pnpm exec forgeax project check --json
if ($LASTEXITCODE) { exit $LASTEXITCODE }
pnpm typecheck
if ($LASTEXITCODE) { exit $LASTEXITCODE }
pnpm test
exit $LASTEXITCODE
