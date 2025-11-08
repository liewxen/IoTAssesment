# Docker Setup Script for IoT Device Management System
# For Windows PowerShell

$ErrorActionPreference = "Stop"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  IoT Device Management System - Docker Setup" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is installed
Write-Host "Checking prerequisites..." -ForegroundColor Yellow
$dockerInstalled = Get-Command docker -ErrorAction SilentlyContinue
$dockerComposeInstalled = Get-Command docker-compose -ErrorAction SilentlyContinue

if (-not $dockerInstalled) {
    Write-Host "Docker is not installed!" -ForegroundColor Red
    Write-Host "Please install Docker Desktop from: https://www.docker.com/products/docker-desktop"
    exit 1
}

if (-not $dockerComposeInstalled) {
    Write-Host "Docker Compose is not installed!" -ForegroundColor Red
    Write-Host "Please install Docker Compose"
    exit 1
}

Write-Host "✓ Docker is installed" -ForegroundColor Green
Write-Host "✓ Docker Compose is installed" -ForegroundColor Green
Write-Host ""

# Create required directories
Write-Host "Creating required directories..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "mosquitto\data" | Out-Null
New-Item -ItemType Directory -Force -Path "mosquitto\log" | Out-Null
Write-Host "✓ Directories created" -ForegroundColor Green
Write-Host ""

# Check if containers are already running
Write-Host "Checking for existing containers..." -ForegroundColor Yellow
$runningContainers = docker-compose ps -q
if ($runningContainers) {
    Write-Host "Containers are already running. Stopping them first..." -ForegroundColor Yellow
    docker-compose down
    Write-Host ""
}

# Build and start containers
Write-Host "Building and starting containers..." -ForegroundColor Yellow
Write-Host "This may take a few minutes on first run..." -ForegroundColor Gray
Write-Host ""

docker-compose up -d --build

Write-Host ""
Write-Host "Waiting for services to become healthy..." -ForegroundColor Yellow
Write-Host "This usually takes 30-60 seconds..." -ForegroundColor Gray
Write-Host ""

# Wait for services to be healthy
$maxWait = 120
$elapsed = 0
while ($elapsed -lt $maxWait) {
    $status = docker-compose ps
    
    if ($status -match "healthy.*healthy.*healthy|healthy.*healthy.*Up") {
        break
    }
    
    Write-Host "." -NoNewline
    Start-Sleep -Seconds 5
    $elapsed += 5
}

Write-Host ""
Write-Host ""

# Check final status
$finalStatus = docker-compose ps
if ($finalStatus -match "unhealthy|Exit") {
    Write-Host "Some services failed to start properly!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Run the following command to see logs:"
    Write-Host "  docker-compose logs" -ForegroundColor Yellow
    exit 1
}

Write-Host "==================================================" -ForegroundColor Green
Write-Host "  ✓ Setup Complete!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Services Status:" -ForegroundColor Green
docker-compose ps
Write-Host ""
Write-Host "Application URLs:" -ForegroundColor Green
Write-Host "  Web Application:  http://localhost:5212"
Write-Host "  Health Check:     http://localhost:5212/health"
Write-Host "  PostgreSQL:       localhost:5432 (user: postgres, password: postgres)"
Write-Host "  MQTT Broker:      localhost:1883"
Write-Host ""
Write-Host "Useful Commands:" -ForegroundColor Green
Write-Host "  View logs:        docker-compose logs -f"
Write-Host "  Stop services:    docker-compose stop"
Write-Host "  Restart:          docker-compose restart"
Write-Host "  Clean up:         docker-compose down"
Write-Host ""
Write-Host "Opening application in browser..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Open browser
Start-Process "http://localhost:5212"

Write-Host ""
Write-Host "🚀 Your IoT Device Management System is ready!" -ForegroundColor Green
Write-Host ""

