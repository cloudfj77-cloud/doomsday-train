$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$env:FORGEAX_HISTORY_DIR = Join-Path $PWD 'qa/operation-history'
$skill = Join-Path $env:USERPROFILE '.codex/plugins/cache/game-builder-zaohua/zaohua-game-builder/1.1.74/skills/zaohua-builder-usage'
$launcher = Join-Path $skill 'scripts/game-builder-cli.mjs'
if (!(Test-Path -LiteralPath $launcher)) { throw '造化插件未安装或版本已变化，请更新 tools/preview.ps1 中的已验证插件路径。' }
node $launcher preview start
exit $LASTEXITCODE
