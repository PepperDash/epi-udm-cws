<#
.SYNOPSIS
    Loads Essentials, the UdmCws plugin and the mock plugins onto a 4-Series processor
    for a demo, then checks that the roomstatus endpoint answers.

.DESCRIPTION
    SFTP upload with a size check on every file, then progload -p:N, which unpacks the
    .cpz and restarts the program. With -SkipEssentials there is no .cpz, so it runs
    progreset -p:N instead to pick up the new plugins and config.

    Credentials come from scripts/.env (see .env.example): PD_PROCESSOR_HOSTNAME,
    PD_USERNAME, PD_PASSWORD. Already-set environment variables win over the file.

    The config deployed is a copy of UdmCws-Test-Configs/configurationFile.json with:
      - the PSK removed (the Portal roomstatus lambda sends an empty PSK)
      - the real-hardware devices removed (codec-1, dsp-1, occupancy-1) and every
        mapping that points at them, unless -KeepHardware is passed
    The copy is written to .deploy/ (git-ignored); the repo file is not changed.

.EXAMPLE
    pwsh ./scripts/Deploy-UdmCwsDemo.ps1 -Build
.EXAMPLE
    pwsh ./scripts/Deploy-UdmCwsDemo.ps1 -SkipEssentials   # plugins + config only
.EXAMPLE
    pwsh ./scripts/Deploy-UdmCwsDemo.ps1 -ConfigOnly       # write the demo config, touch no processor
#>
[CmdletBinding()]
param(
    [ValidateRange(1, 10)][int]$ProgramSlot = 1,
    [string]$EnvFile = (Join-Path $PSScriptRoot '.env'),
    [string]$EssentialsVersion = '2.24.0',
    [switch]$Build,
    [switch]$SkipEssentials,
    [switch]$KeepHardware,
    [switch]$ConfigOnly,
    [int]$BootWaitSec = 240
)

$ErrorActionPreference = 'Stop'
$repoRoot   = Split-Path $PSScriptRoot -Parent
$deployDir  = Join-Path $repoRoot '.deploy'
$outputDir  = Join-Path $repoRoot 'output'
$hardware   = @('codec-1', 'dsp-1', 'occupancy-1')
$plugins    = @('UdmCws-Epi', 'UdmCws-Test-Room-Epi', 'UdmCws-Test-Occ-Epi', 'UdmCws-Test-Device-Epi')

function Write-Step([string]$Text) { Write-Host "`n==> $Text" -ForegroundColor Cyan }

New-Item -ItemType Directory -Force -Path $deployDir | Out-Null
$logPath = Join-Path $deployDir "deploy-$(Get-Date -Format yyyyMMdd-HHmmss).log"
Start-Transcript -Path $logPath | Out-Null
trap { Write-Host "`nFAILED: $_" -ForegroundColor Red; Write-Host "Full log: $logPath"; Stop-Transcript | Out-Null; break }

# --- Demo config -------------------------------------------------------------------------
Write-Step 'Writing demo config'
$config = Get-Content (Join-Path $repoRoot 'UdmCws-Test-Configs/configurationFile.json') -Raw | ConvertFrom-Json -AsHashtable
$template = $config.template
$udm = ($template.devices | Where-Object { $_.type -in 'udmcws', 'UdmCwsStateManager' } | Select-Object -First 1).properties
$udm.Remove('psk')
if (-not $KeepHardware) {
    $template.devices = @($template.devices | Where-Object { $_.key -notin $hardware })
    $udm.deviceMappings = @($udm.deviceMappings | Where-Object { $_.deviceKey -notin $hardware })
    $sp = $udm.standardProperties
    $sp.customPropertyMappings = @($sp.customPropertyMappings | Where-Object { $_.mapping.deviceKey -notin $hardware })
    foreach ($list in @($template.sourceLists.Values)) { foreach ($k in $hardware) { $list.Remove($k) } }
}
$configPath = Join-Path $deployDir 'configurationFile.json'
$config | ConvertTo-Json -Depth 32 | Set-Content -Path $configPath -Encoding utf8
if ($ConfigOnly) { Write-Host "Demo config: $configPath"; Stop-Transcript | Out-Null; return }

# --- Credentials from .env --------------------------------------------------------------
if (Test-Path $EnvFile) {
    foreach ($line in Get-Content $EnvFile) {
        if ($line -match '^\s*([A-Z_][A-Z0-9_]*)\s*=\s*(.*?)\s*$' -and -not [Environment]::GetEnvironmentVariable($Matches[1])) {
            [Environment]::SetEnvironmentVariable($Matches[1], $Matches[2].Trim('"').Trim("'"))
        }
    }
}
foreach ($name in 'PD_PROCESSOR_HOSTNAME', 'PD_USERNAME', 'PD_PASSWORD') {
    if (-not [Environment]::GetEnvironmentVariable($name)) { throw "$name is not set. Copy scripts/.env.example to scripts/.env and fill it in." }
}
$ip   = $env:PD_PROCESSOR_HOSTNAME
$cred = [pscredential]::new($env:PD_USERNAME, (ConvertTo-SecureString $env:PD_PASSWORD -AsPlainText -Force))

Import-Module Posh-SSH -MinimumVersion 3.0 -ErrorAction Stop

# --- Artifacts ---------------------------------------------------------------------------
if ($Build) {
    Write-Step 'Building plugins'
    # Native exit codes don't trip $ErrorActionPreference, so check each build.
    dotnet build (Join-Path $repoRoot 'UdmCws.4Series.sln') -c Release -v q
    if ($LASTEXITCODE) { throw 'Build failed: UdmCws.4Series.sln' }
    foreach ($p in $plugins | Where-Object { $_ -ne 'UdmCws-Epi' }) {
        dotnet build (Join-Path $repoRoot "$p/$p.csproj") -c Release -v q
        if ($LASTEXITCODE) { throw "Build failed: $p" }
    }
}
$cplz = foreach ($p in $plugins) {
    $f = Get-ChildItem $outputDir -Filter "$p.*.cplz" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $f) { throw "No $p .cplz in output/. Run with -Build." }
    $f
}

$cpz = $null
if (-not $SkipEssentials) {
    $cpz = Join-Path $deployDir "PepperDashEssentials.$EssentialsVersion.net472.cpz"
    if (-not (Test-Path $cpz)) {
        Write-Step "Downloading Essentials $EssentialsVersion"
        gh release download "v$EssentialsVersion" --repo PepperDash/Essentials --pattern '*.cpz' --dir $deployDir --clobber
        if ($LASTEXITCODE -or -not (Test-Path $cpz)) { throw "Could not download $cpz" }
    }
}

# --- Upload ------------------------------------------------------------------------------
$slotDir    = '/program{0:D2}' -f $ProgramSlot
$userDir    = "/user/program$ProgramSlot"
$pluginDir  = "$userDir/plugins"
$localCfg   = "$userDir/LocalConfig/configurationFile.json"

function Get-RemoteSize($Session, [string]$Dir, [string]$Name) {
    $item = Get-SFTPChildItem -SessionId $Session.SessionId -Path $Dir | Where-Object { $_.Name -eq $Name }
    if ($item) { $item.Length } else { $null }
}
function Send-File($Session, [string]$Local, [string]$Dir) {
    $file = Get-Item $Local
    Write-Host ('  {0} ({1:N0} bytes) -> {2}' -f $file.Name, $file.Length, $Dir)
    $sw = [Diagnostics.Stopwatch]::StartNew()
    Set-SFTPItem -SessionId $Session.SessionId -Path $file.FullName -Destination $Dir -Force
    $remote = Get-RemoteSize $Session $Dir $file.Name
    if ($remote -ne $file.Length) { throw "Upload of $($file.Name) to $Dir is incomplete: $remote of $($file.Length) bytes on the processor." }
    Write-Host ('    ok, {0:N1} s' -f $sw.Elapsed.TotalSeconds)
}
function Invoke-Console($Session, [string]$Command, [int]$TimeOut = 120) {
    Write-Host "  > $Command"
    $result = Invoke-SSHCommand -SessionId $Session.SessionId -Command $Command -TimeOut $TimeOut
    foreach ($line in $result.Output) { if ($line.Trim()) { Write-Host "    $line" } }
    $result.Output
}

Write-Step "Uploading to $ip"
$sftp = New-SFTPSession -ComputerName $ip -Credential $cred -Force -AcceptKey -ConnectionTimeout 30 -OperationTimeout 600
try {
    foreach ($dir in $userDir, $pluginDir) {
        if (-not (Test-SFTPPath -SessionId $sftp.SessionId -Path $dir)) { New-SFTPItem -SessionId $sftp.SessionId -Path $dir -ItemType Directory | Out-Null }
    }
    # A LocalConfig copy wins over an uploaded config; move it aside so the demo config loads.
    if (Test-SFTPPath -SessionId $sftp.SessionId -Path $localCfg) {
        $bak = "configurationFile.json.bak-$(Get-Date -Format yyyyMMddHHmmss)"
        Rename-SFTPFile -SessionId $sftp.SessionId -Path $localCfg -NewName $bak
        Write-Host "  Moved existing LocalConfig to $userDir/LocalConfig/$bak"
    }
    if ($cpz) { Send-File $sftp $cpz $slotDir }
    foreach ($f in $cplz) { Send-File $sftp $f.FullName $pluginDir }
    Send-File $sftp $configPath $userDir
} finally {
    Remove-SFTPSession -SessionId $sftp.SessionId | Out-Null
}

# --- Load and restart --------------------------------------------------------------------
Write-Step "Loading program $ProgramSlot"
$ssh = New-SSHSession -ComputerName $ip -Credential $cred -Force -AcceptKey -ConnectionTimeout 30
try {
    if ($cpz) {
        # progload unpacks the .cpz and restarts the program, so no progreset is needed after it.
        Invoke-Console $ssh "progload -p:$ProgramSlot" | Out-Null
        # It returns before it finishes; the processor removes the .cpz once it has unpacked it.
        $cpzName = Split-Path $cpz -Leaf
        $sftp = New-SFTPSession -ComputerName $ip -Credential $cred -Force -AcceptKey -ConnectionTimeout 30 -OperationTimeout 60
        try {
            $loadDeadline = (Get-Date).AddSeconds(180)
            while ((Get-RemoteSize $sftp $slotDir $cpzName) -and (Get-Date) -lt $loadDeadline) { Start-Sleep -Seconds 5 }
            if (Get-RemoteSize $sftp $slotDir $cpzName) { throw "progload has not unpacked $cpzName after 180 s; it is still in $slotDir." }
        } finally {
            Remove-SFTPSession -SessionId $sftp.SessionId | Out-Null
        }
        Write-Host '  progload unpacked the program'
    } else {
        # Essentials already loaded: restart it to pick up the new plugins and config.
        Invoke-Console $ssh "progreset -p:$ProgramSlot" | Out-Null
    }
} finally {
    Remove-SSHSession -SessionId $ssh.SessionId | Out-Null
}

# --- Check the endpoint ------------------------------------------------------------------
$route = $udm.routePrefix
$base  = 'https://{0}/cws/app{1:D2}/udmcws{2}' -f $ip, $ProgramSlot, ($(if ($route) { "/$route" } else { '' }))
Write-Step "Waiting up to $BootWaitSec s for $base/roomstatus"
$deadline = (Get-Date).AddSeconds($BootWaitSec)
$r = $null
while (-not $r -and (Get-Date) -lt $deadline) {
    Start-Sleep -Seconds 10
    try {
        $r = Invoke-WebRequest -Uri "$base/roomstatus" -SkipCertificateCheck -TimeoutSec 8 -Headers @{ 'PDT-API-VERSION' = '1.0' }
        if ($r.StatusCode -ne 200) { $r = $null }
    } catch { }
}
if (-not $r) {
    Write-Step 'roomstatus did not answer; processor state'
    $ssh = New-SSHSession -ComputerName $ip -Credential $cred -Force -AcceptKey -ConnectionTimeout 30
    try {
        Invoke-Console $ssh "appstat -p:$ProgramSlot" 30 | Out-Null
        Invoke-Console $ssh 'err' 30 | Out-Null
    } finally {
        Remove-SSHSession -SessionId $ssh.SessionId | Out-Null
    }
    throw "roomstatus did not answer at $base/roomstatus within $BootWaitSec s. Console output above; full log in $logPath."
}

Write-Host "`nroomstatus is up: $base/roomstatus" -ForegroundColor Green
Write-Host "Portal device Address: $($base -replace '^https://', '')   (method: https)" -ForegroundColor Green
($r.Content | ConvertFrom-Json).standard | Format-List
Write-Host "Full log: $logPath"
Stop-Transcript | Out-Null
