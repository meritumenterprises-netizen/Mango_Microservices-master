param(
    [string]$Url = "http://myhost.local",
    [int]$TimeoutSeconds = 180,
    [int]$PollSeconds = 2
)

$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
$logPath = Join-Path $PSScriptRoot "browser-launcher.log"
$pidPath = Join-Path $PSScriptRoot "browser-launcher.pid"

"$(Get-Date -Format o) waiting for $Url" | Out-File -FilePath $logPath -Append -Encoding utf8

while ((Get-Date) -lt $deadline) {
    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec $PollSeconds
        if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 400) {
            "$(Get-Date -Format o) opening $Url" | Out-File -FilePath $logPath -Append -Encoding utf8
            Start-Process $Url
            Remove-Item -Path $pidPath -ErrorAction SilentlyContinue
            exit 0
        }
    }
    catch {
    }

    Start-Sleep -Seconds $PollSeconds
}

"$(Get-Date -Format o) timed out waiting for $Url" | Out-File -FilePath $logPath -Append -Encoding utf8
Remove-Item -Path $pidPath -ErrorAction SilentlyContinue
exit 1
