$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/check.ps1"
if ($LASTEXITCODE) { exit $LASTEXITCODE }
if (!$env:FORGEAX_BROWSER_EXECUTABLE) {
  $edge = Join-Path ${env:ProgramFiles(x86)} 'Microsoft/Edge/Application/msedge.exe'
  if (Test-Path -LiteralPath $edge) { $env:FORGEAX_BROWSER_EXECUTABLE = $edge }
}
pnpm exec forgeax project play tests/playthrough.mjs --timeout-ms 20000 --json
exit $LASTEXITCODE

