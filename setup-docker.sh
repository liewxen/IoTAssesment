#!/bin/bash
# Docker Setup Script for IoT Device Management System
# For Linux/Mac

set -e

echo "=================================================="
echo "  IoT Device Management System - Docker Setup"
echo "=================================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check if Docker is installed
echo -e "${YELLOW}Checking prerequisites...${NC}"
if ! command -v docker &> /dev/null; then
    echo -e "${RED}Docker is not installed!${NC}"
    echo "Please install Docker Desktop from: https://www.docker.com/products/docker-desktop"
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    echo -e "${RED}Docker Compose is not installed!${NC}"
    echo "Please install Docker Compose"
    exit 1
fi

echo -e "${GREEN}✓ Docker is installed${NC}"
echo -e "${GREEN}✓ Docker Compose is installed${NC}"
echo ""

# Create required directories
echo -e "${YELLOW}Creating required directories...${NC}"
mkdir -p mosquitto/data
mkdir -p mosquitto/log
echo -e "${GREEN}✓ Directories created${NC}"
echo ""

# Set permissions for Mosquitto directories
echo -e "${YELLOW}Setting permissions...${NC}"
chmod -R 755 mosquitto
echo -e "${GREEN}✓ Permissions set${NC}"
echo ""

# Check if containers are already running
if docker-compose ps | grep -q "Up"; then
    echo -e "${YELLOW}Containers are already running. Stopping them first...${NC}"
    docker-compose down
    echo ""
fi

# Build and start containers
echo -e "${YELLOW}Building and starting containers...${NC}"
echo "This may take a few minutes on first run..."
echo ""

docker-compose up -d --build

echo ""
echo -e "${YELLOW}Waiting for services to become healthy...${NC}"
echo "This usually takes 30-60 seconds..."
echo ""

# Wait for services to be healthy
MAX_WAIT=120
ELAPSED=0
while [ $ELAPSED -lt $MAX_WAIT ]; do
    POSTGRES_STATUS=$(docker-compose ps postgres | grep -o "healthy" || echo "starting")
    MOSQUITTO_STATUS=$(docker-compose ps mosquitto | grep -o "healthy" || echo "starting")
    APP_STATUS=$(docker-compose ps iot-app | grep -o "healthy\|running" || echo "starting")
    
    if [ "$POSTGRES_STATUS" = "healthy" ] && [ "$MOSQUITTO_STATUS" = "healthy" ] && [ "$APP_STATUS" != "starting" ]; then
        break
    fi
    
    echo -n "."
    sleep 5
    ELAPSED=$((ELAPSED + 5))
done

echo ""
echo ""

# Check final status
if docker-compose ps | grep -q "unhealthy\|Exit"; then
    echo -e "${RED}Some services failed to start properly!${NC}"
    echo ""
    echo "Run the following command to see logs:"
    echo "  docker-compose logs"
    exit 1
fi

echo -e "${GREEN}=================================================="
echo -e "  ✓ Setup Complete!"
echo -e "==================================================${NC}"
echo ""
echo -e "${GREEN}Services Status:${NC}"
docker-compose ps
echo ""
echo -e "${GREEN}Application URLs:${NC}"
echo "  Web Application:  http://localhost:5212"
echo "  Health Check:     http://localhost:5212/health"
echo "  PostgreSQL:       localhost:5432 (user: postgres, password: postgres)"
echo "  MQTT Broker:      localhost:1883"
echo ""
echo -e "${GREEN}Useful Commands:${NC}"
echo "  View logs:        docker-compose logs -f"
echo "  Stop services:    docker-compose stop"
echo "  Restart:          docker-compose restart"
echo "  Clean up:         docker-compose down"
echo ""
echo -e "${YELLOW}Opening application in browser...${NC}"
sleep 2

# Try to open browser (works on most Linux systems and Mac)
if command -v xdg-open &> /dev/null; then
    xdg-open http://localhost:5212
elif command -v open &> /dev/null; then
    open http://localhost:5212
fi

echo ""
echo -e "${GREEN}🚀 Your IoT Device Management System is ready!${NC}"
echo ""

