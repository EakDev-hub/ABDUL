#!/bin/bash

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
DOCKER_COMPOSE_FILE="/tmp/docker-compose.yml"
ENV_FILE="/tmp/.env"
BACKEND_IMAGE_TAR="/tmp/backend-image.tar"
NAMEK_IMAGE_TAR="/tmp/namek-image.tar"
DEPLOYMENT_DIR="/opt/abdul"
LOG_FILE="/var/log/abdul-deployment.log"

# Logging function
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1" | tee -a "$LOG_FILE"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1" | tee -a "$LOG_FILE"
    exit 1
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1" | tee -a "$LOG_FILE"
}

# Check if running as root or with sudo
if [[ $EUID -ne 0 ]]; then
    error "This script must be run as root or with sudo"
fi

log "Starting ABDUL deployment process..."

# Create deployment directory
log "Creating deployment directory..."
mkdir -p "$DEPLOYMENT_DIR"
cd "$DEPLOYMENT_DIR"

# Check if Docker is running
log "Checking Docker daemon..."
if ! docker ps > /dev/null 2>&1; then
    error "Docker daemon is not running. Please start Docker and try again."
fi

# Load Docker images
log "Loading backend Docker image..."
if [ -f "$BACKEND_IMAGE_TAR" ]; then
    docker load -i "$BACKEND_IMAGE_TAR" || error "Failed to load backend image"
    log "Backend image loaded successfully"
else
    error "Backend image tar file not found at $BACKEND_IMAGE_TAR"
fi

log "Loading namek Docker image..."
if [ -f "$NAMEK_IMAGE_TAR" ]; then
    docker load -i "$NAMEK_IMAGE_TAR" || error "Failed to load namek image"
    log "Namek image loaded successfully"
else
    error "Namek image tar file not found at $NAMEK_IMAGE_TAR"
fi

# Stop existing containers
log "Stopping existing containers..."
if docker-compose -f "$DOCKER_COMPOSE_FILE" ps 2>/dev/null | grep -q "abdul"; then
    docker-compose -f "$DOCKER_COMPOSE_FILE" down --remove-orphans || warning "Failed to stop some containers"
    log "Existing containers stopped"
else
    log "No existing containers found"
fi

# Copy docker-compose and .env files to deployment directory
log "Setting up configuration files..."
cp "$DOCKER_COMPOSE_FILE" "$DEPLOYMENT_DIR/docker-compose.yml" || error "Failed to copy docker-compose.yml"
cp "$ENV_FILE" "$DEPLOYMENT_DIR/.env" || error "Failed to copy .env file"
chmod 600 "$DEPLOYMENT_DIR/.env"

# Start new containers
log "Starting containers with docker-compose..."
docker-compose -f "$DEPLOYMENT_DIR/docker-compose.yml" up -d || error "Failed to start containers"

# Wait for containers to be healthy
log "Waiting for containers to become healthy..."
sleep 10

# Check container status
log "Verifying container status..."
BACKEND_STATUS=$(docker ps --filter "name=abdul-backend" --format "{{.Status}}" 2>/dev/null || echo "")
NAMEK_STATUS=$(docker ps --filter "name=abdul-namek" --format "{{.Status}}" 2>/dev/null || echo "")

if [ -z "$BACKEND_STATUS" ]; then
    error "Backend container is not running"
fi

if [ -z "$NAMEK_STATUS" ]; then
    error "Namek container is not running"
fi

log "Backend container status: $BACKEND_STATUS"
log "Namek container status: $NAMEK_STATUS"

# Verify services are accessible
log "Verifying service connectivity..."
if curl -f http://localhost:5000/health > /dev/null 2>&1; then
    log "Backend service is responding on port 5000"
else
    warning "Backend health check failed, but container is running"
fi

if curl -f http://localhost:3000/ > /dev/null 2>&1; then
    log "Namek service is responding on port 3000"
else
    warning "Namek health check failed, but container is running"
fi

# Cleanup old images (keep last 2 versions)
log "Cleaning up old Docker images..."
docker image prune -f --filter "until=72h" > /dev/null 2>&1 || warning "Failed to prune old images"

# Cleanup temporary files
log "Cleaning up temporary files..."
rm -f "$BACKEND_IMAGE_TAR" "$NAMEK_IMAGE_TAR" "$DOCKER_COMPOSE_FILE" "$ENV_FILE"

log "Deployment completed successfully!"
log "Backend: http://localhost:5000"
log "Namek: http://localhost:3000"
log "Deployment logs: $LOG_FILE"

exit 0