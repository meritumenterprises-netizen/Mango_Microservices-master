# Get only one matching certificate (the first one)
$cert = Get-ChildItem -Path Cert:\CurrentUser\My |
    Where-Object { $_.Subject -match "CN=myhost" } |
    Sort-Object NotAfter -Descending |
    Select-Object -First 1

# Set the password
$pwd = ConvertTo-SecureString -String "P@ssw0rd!" -Force -AsPlainText

# Export to PFX
Export-PfxCertificate -Cert $cert -FilePath "$env:USERPROFILE\lXango.pfx" -Password $pwd
