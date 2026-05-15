$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $projectRoot "backend\PetCareJordan.Api"
$frontendPath = Join-Path $projectRoot "frontend\petcare-jordan-client"
$logsPath = Join-Path $projectRoot ".run-logs"
$workspaceRoot = Split-Path -Parent $projectRoot

New-Item -ItemType Directory -Force -Path $logsPath | Out-Null

$backendCommand = @"
`$env:DOTNET_CLI_HOME = '$workspaceRoot\.dotnet-home'
`$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
`$env:APPDATA = '$workspaceRoot\.dotnet-home\AppData'
`$env:NUGET_PACKAGES = '$workspaceRoot\.nuget\packages'
`$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=PetCareJordanPart2RunDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True'
Set-Location '$backendPath'
& 'C:\Program Files\dotnet\dotnet.exe' run --urls 'http://localhost:5031' *>&1 | Tee-Object -FilePath '$logsPath\backend.log'
"@

$frontendCommand = @"
`$env:VITE_API_BASE_URL = 'http://localhost:5031/api'
Set-Location '$frontendPath'
& 'C:\Program Files\nodejs\npm.cmd' run dev -- --host 127.0.0.1 *>&1 | Tee-Object -FilePath '$logsPath\frontend.log'
"@

Write-Host "Starting PetCare Jordan backend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-ExecutionPolicy", "Bypass", "-Command", $backendCommand

Write-Host "Starting PetCare Jordan frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-ExecutionPolicy", "Bypass", "-Command", $frontendCommand

Write-Host ""
Write-Host "PetCare Jordan is starting in two terminal windows." -ForegroundColor Green
Write-Host "Backend: http://localhost:5031" -ForegroundColor Yellow
Write-Host "Frontend: http://127.0.0.1:5173" -ForegroundColor Yellow
Write-Host "Logs: $logsPath" -ForegroundColor Yellow
