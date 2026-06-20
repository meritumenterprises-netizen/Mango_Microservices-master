$scriptPath = Join-Path $PSScriptRoot "launch-xango-web-browser.ps1"
$logPath = Join-Path $PSScriptRoot "browser-launcher.log"
$pidPath = Join-Path $PSScriptRoot "browser-launcher.pid"

if (Test-Path $pidPath) {
    $existingProcessId = Get-Content -Path $pidPath -ErrorAction SilentlyContinue
    if ($existingProcessId) {
        $existingProcess = Get-Process -Id $existingProcessId -ErrorAction SilentlyContinue
        if ($existingProcess) {
            "$(Get-Date -Format o) browser watcher already running as process $existingProcessId" | Out-File -FilePath $logPath -Append -Encoding utf8
            exit 0
        }
    }
}

"$(Get-Date -Format o) starting browser watcher for http://myhost.local" | Out-File -FilePath $logPath -Append -Encoding utf8

$process = Start-Process powershell -WindowStyle Hidden -PassThru -ArgumentList @(
    "-NoProfile",
    "-ExecutionPolicy",
    "Bypass",
    "-File",
    $scriptPath,
    "-Url",
    "http://myhost.local"
)

$process.Id | Out-File -FilePath $pidPath -Encoding ascii
