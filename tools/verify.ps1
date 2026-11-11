$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/check.ps1"
if ($LASTEXITCODE) { exit $LASTEXITCODE }
if (!$env:FORGEAX_BROWSER_EXECUTABLE) {
  $edge = Join-Path ${env:ProgramFiles(x86)} 'Microsoft/Edge/Application/msedge.exe'
  if (Test-Path -LiteralPath $edge) { $env:FORGEAX_BROWSER_EXECUTABLE = $edge }
}
pnpm exec forgeax project play tests/playthrough.mjs --viewports '1280x720,1280x600' --timeout-ms 120000 --json
exit $LASTEXITCODE
