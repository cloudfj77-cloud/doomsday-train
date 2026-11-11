[CmdletBinding()]
param(
    [ValidateSet('start', 'stop', 'status', 'capture', 'verify')]
    [string]$Action = 'start'
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$runtimeRoot = Join-Path $projectRoot 'node_modules/.cache/forgeax-preview'
$savedEnvironment = @{}
$exitCode = 0

function Find-Program([string]$Name, [string[]]$Candidates) {
    $command = Get-Command $Name -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($command) { return $command.Source }
    foreach ($candidate in $Candidates) {
        if ($candidate -and (Test-Path -LiteralPath $candidate -PathType Leaf)) { return $candidate }
    }
    return $null
}

function Set-PreviewEnvironment([string]$Name, [string]$Value) {
    $savedEnvironment[$Name] = [Environment]::GetEnvironmentVariable($Name, 'Process')
    [Environment]::SetEnvironmentVariable($Name, $Value, 'Process')
}

Push-Location $projectRoot
try {
    $node = Find-Program 'node.exe' @(
        "$env:NODE_HOME/node.exe", "$env:ProgramFiles/nodejs/node.exe",
        "$env:LOCALAPPDATA/Programs/nodejs/node.exe"
    )
    if (!$node) { throw 'Node.js was not found. Install Node.js and add it to PATH, then retry.' }
    $nodeVersion = & $node --version
    if ($LASTEXITCODE -ne 0 -or [int](($nodeVersion -replace '^v', '') -split '\.')[0] -lt 22) { throw 'Node.js 22 or newer is required.' }
    $pnpm = Find-Program 'pnpm.cmd' @(
        "$projectRoot/.forgeax-local/tool/node_modules/.bin/pnpm.cmd",
        "$env:PNPM_HOME/pnpm.cmd", "$env:APPDATA/npm/pnpm.cmd",
        (Join-Path (Split-Path $node) 'pnpm.cmd')
    )
    if (!$pnpm) { $pnpm = Find-Program 'pnpm.exe' @() }
    if (!$pnpm) { throw 'pnpm was not found. Install pnpm or set PNPM_HOME, then retry.' }
    # All environment changes apply only to this script and its child processes.
    $directories = @{
        FORGEAX_HISTORY_DIR = "$projectRoot/.forgeax/history"
        FORGEAX_USER_STATE_DIR = "$runtimeRoot/user-state"
        npm_config_cache = "$runtimeRoot/npm-cache"
        XDG_CACHE_HOME = "$runtimeRoot/xdg-cache"
        XDG_DATA_HOME = "$runtimeRoot/xdg-data"
        XDG_STATE_HOME = "$runtimeRoot/xdg-state"
        COREPACK_HOME = "$runtimeRoot/corepack"
        PLAYWRIGHT_BROWSERS_PATH = "$runtimeRoot/playwright-browsers"
        TEMP = "$runtimeRoot/tmp"
        TMP = "$runtimeRoot/tmp"
    }
    foreach ($entry in $directories.GetEnumerator()) {
        New-Item -ItemType Directory -Force -Path $entry.Value | Out-Null
        Set-PreviewEnvironment $entry.Key $entry.Value
    }
    Set-PreviewEnvironment 'PATH' ((Split-Path $node) + [IO.Path]::PathSeparator + (Split-Path $pnpm) + [IO.Path]::PathSeparator + $env:PATH)
    $engineManifest = Join-Path $projectRoot 'node_modules/@forgeax/engine/package.json'
    $engineCommand = Join-Path $projectRoot 'node_modules/.bin/forgeax.cmd'
    $expected = (Get-Content package.json -Raw | ConvertFrom-Json).dependencies.'@forgeax/engine'
    if (!(Test-Path -LiteralPath $engineManifest) -or !(Test-Path -LiteralPath $engineCommand)) {
        if ($Action -ne 'start') { throw 'Project dependencies are missing. Run this script with start first.' }
        Write-Host 'Installing locked project dependencies (cache stays inside the project)...'
        & $pnpm install --frozen-lockfile --store-dir "$runtimeRoot/pnpm-store"
        if ($LASTEXITCODE -ne 0) { throw "Dependency installation failed ($LASTEXITCODE)." }
    }
    $installed = (Get-Content $engineManifest -Raw | ConvertFrom-Json).version
    if ($expected -ne $installed) { throw "Engine version mismatch: project=$expected, installed=$installed. Run pnpm install before retrying." }
    if ($Action -in @('start', 'capture', 'verify')) {
        $browserCandidates = @($env:FORGEAX_BROWSER_EXECUTABLE)
        foreach ($base in @(${env:ProgramFiles}, ${env:ProgramFiles(x86)}, $env:LOCALAPPDATA)) {
            if ($base) {
                $browserCandidates += "$base/Microsoft/Edge/Application/msedge.exe"
                $browserCandidates += "$base/Google/Chrome/Application/chrome.exe"
            }
        }
        $browser = $browserCandidates | Where-Object { $_ -and (Test-Path -LiteralPath $_ -PathType Leaf) } | Select-Object -First 1
        if (!$browser) { throw 'No Edge/Chrome found. Set FORGEAX_BROWSER_EXECUTABLE to a Chromium browser executable, then retry.' }
        Set-PreviewEnvironment 'FORGEAX_BROWSER_EXECUTABLE' $browser
        Write-Host "Browser: $browser"
    }
    Write-Host "Project: $projectRoot"
    Write-Host "Node: $node; pnpm: $pnpm; Engine: $installed"
    if ($Action -eq 'start') {
        Write-Host 'Checking the existing development session...'
        $statusOutput = & $pnpm --config.verify-deps-before-run=false exec forgeax dev status --json
        if ($LASTEXITCODE -ne 0) { throw "Cannot inspect the existing session: $statusOutput" }
        $status = ($statusOutput -join "`n") | ConvertFrom-Json
        if (!$status.ok) { throw "Cannot inspect the existing session: $statusOutput" }
        if ($status.value.phase -notin @('stopped', 'unreachable') -and $status.value.bridgeConnected -eq $false) {
            Write-Host 'The previous browser is disconnected. Stopping its session before restarting...'
            & $pnpm --config.verify-deps-before-run=false exec forgeax dev stop --json
            if ($LASTEXITCODE -ne 0) { throw 'Could not stop the disconnected development session.' }
        }
        Write-Host 'Starting ForgeaX and its controlled browser. Waiting for engine readiness...'
        Write-Host 'If startup fails, keep the error output below. Check .forgeax/dev.log for backend details.'
    }
    $cliArguments = @('--config.verify-deps-before-run=false', 'exec', 'forgeax', 'dev', $Action, '--json')
    if ($Action -eq 'capture') { $cliArguments += @('--output', '.forgeax/diag/local-preview.png') }
    if ($Action -eq 'verify') { $cliArguments += @('--require-ui', '--timeout-ms', '120000') }
    # Keep the Engine CLI as the owner of services, browser, readiness and errors.
    & $pnpm @cliArguments
    $exitCode = $LASTEXITCODE
} catch {
    Write-Error -ErrorAction Continue $_
    $exitCode = 1
} finally {
    foreach ($entry in $savedEnvironment.GetEnumerator()) {
        [Environment]::SetEnvironmentVariable($entry.Key, $entry.Value, 'Process')
    }
    Pop-Location
}
exit $exitCode
