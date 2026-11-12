param([ValidateSet('all','items','controller')][string]$Scenario = 'all')
$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/check.ps1"
if ($LASTEXITCODE) { exit $LASTEXITCODE }
if (!$env:FORGEAX_BROWSER_EXECUTABLE) {
  $edge = Join-Path ${env:ProgramFiles(x86)} 'Microsoft/Edge/Application/msedge.exe'
  if (Test-Path -LiteralPath $edge) { $env:FORGEAX_BROWSER_EXECUTABLE = $edge }
}
$scripts = switch ($Scenario) {
  'items' { @('tests/items-playthrough.mjs') }
  'controller' { @('tests/playthrough.mjs') }
  'all' { @('tests/items-playthrough.mjs','tests/playthrough.mjs') }
}
foreach ($script in $scripts) {
  pnpm exec forgeax project play $script --require-ui --viewports '1280x720,1280x600' --timeout-ms 240000 --json
  if ($LASTEXITCODE) { exit $LASTEXITCODE }
}
exit 0
