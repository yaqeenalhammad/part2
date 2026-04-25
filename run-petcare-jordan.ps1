$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $projectRoot "backend\PetCareJordan.Api"
$frontendPath = Join-Path $projectRoot "frontend\petcare-jordan-client"

Write-Host "Starting PetCare Jordan backend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$backendPath'; dotnet run"

Write-Host "Starting PetCare Jordan frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$frontendPath'; npm.cmd run dev"

Write-Host ""
Write-Host "PetCare Jordan is starting in two terminal windows." -ForegroundColor Green
Write-Host "Backend: http://localhost:5031" -ForegroundColor Yellow
Write-Host "Frontend: http://localhost:5173" -ForegroundColor Yellow
