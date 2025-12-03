#!/bin/bash

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Parameters from GitHub Actions
GITHUB_PAT="$1"
GITHUB_REPO="$2"
API_GATEWAY_URL="$3"

# Configuration
DEPLOYMENT_DIR="/opt/abdul"
LOG_FILE="/var/log/abdul-deployment.log"
DOCKER_COMPOSE_FILE="/tmp/docker-compose.yml"
ENV_FILE="/tmp/.env"

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

# Validate parameters
if [ -z "$GITHUB_PAT" ]; then
    error "GITHUB_PAT parameter is required"
fi

if [ -z "$GITHUB_REPO" ]; then
    error "GITHUB_REPO parameter is required"
fi

if [ -z "$API_GATEWAY_URL" ]; then
    warning "API_GATEWAY_URL not provided, using default"
    API_GATEWAY_URL="http://localhost:5000"
fi

log "Starting ABDUL deployment process..."
log "Repository: ${GITHUB_REPO}"
log "Deployment directory: ${DEPLOYMENT_DIR}"

# Check if Docker is running
log "Checking Docker daemon..."
if ! docker ps > /dev/null 2>&1; then
    error "Docker daemon is not running. Please start Docker and try again."
fi

# Check and setup Docker Compose
log "Checking Docker Compose installation..."
DOCKER_COMPOSE_CMD=""

# Check for docker compose plugin (V2)
if docker compose version > /dev/null 2>&1; then
    DOCKER_COMPOSE_CMD="docker compose"
    log "Using Docker Compose V2 (plugin)"
# Check for standalone docker-compose (V1)
elif command -v docker-compose > /dev/null 2>&1; then
    DOCKER_COMPOSE_CMD="docker-compose"
    log "Using Docker Compose V1 (standalone)"
else
    # Install Docker Compose plugin
    log "Docker Compose not found. Installing Docker Compose plugin..."
    COMPOSE_VERSION=$(curl -s https://api.github.com/repos/docker/compose/releases/latest | grep 'tag_name' | cut -d\" -f4)
    
    # Create CLI plugins directory if it doesn't exist
    mkdir -p /usr/local/lib/docker/cli-plugins
    
    # Download and install Docker Compose plugin
    curl -SL "https://github.com/docker/compose/releases/download/${COMPOSE_VERSION}/docker-compose-linux-x86_64" \
        -o /usr/local/lib/docker/cli-plugins/docker-compose || error "Failed to download Docker Compose"
    
    chmod +x /usr/local/lib/docker/cli-plugins/docker-compose
    
    # Verify installation
    if docker compose version > /dev/null 2>&1; then
        DOCKER_COMPOSE_CMD="docker compose"
        log "Docker Compose plugin installed successfully"
    else
        error "Failed to install Docker Compose plugin"
    fi
fi

log "Docker Compose command: ${DOCKER_COMPOSE_CMD}"

# Check if git is installed
log "Checking Git installation..."
if ! command -v git &> /dev/null; then
    error "Git is not installed. Please install git and try again."
fi

# Create deployment directory if it doesn't exist
log "Setting up deployment directory..."
mkdir -p "$DEPLOYMENT_DIR"

# Git repository management
cd "$DEPLOYMENT_DIR"

if [ ! -d ".git" ]; then
    log "Cloning repository for the first time..."
    # Clone repository using PAT
    git clone "https://${GITHUB_PAT}@${GITHUB_REPO}" . || error "Failed to clone repository"
    log "Repository cloned successfully"
else
    log "Repository exists, pulling latest changes..."
    # Reset any local changes
    git reset --hard HEAD || warning "Failed to reset repository"
    # Pull latest changes
    git pull origin main || error "Failed to pull latest changes"
    log "Repository updated successfully"
fi

# Clear git credentials from memory
unset GITHUB_PAT

# Show current commit
CURRENT_COMMIT=$(git rev-parse --short HEAD)
log "Current commit: ${CURRENT_COMMIT}"

# Copy configuration files
log "Setting up configuration files..."
cp "$DOCKER_COMPOSE_FILE" "$DEPLOYMENT_DIR/docker-compose.prod.yml" || error "Failed to copy docker-compose.yml"
cp "$ENV_FILE" "$DEPLOYMENT_DIR/.env" || error "Failed to copy .env file"
chmod 600 "$DEPLOYMENT_DIR/.env"

# Copy nginx configuration if provided
if [ -d "/tmp/nginx-config" ]; then
    log "Copying nginx configuration files..."
    mkdir -p "$DEPLOYMENT_DIR/nginx"
    cp /tmp/nginx-config/nginx.conf "$DEPLOYMENT_DIR/nginx/" || warning "Failed to copy nginx.conf"
    cp /tmp/nginx-config/Dockerfile "$DEPLOYMENT_DIR/nginx/" || warning "Failed to copy nginx Dockerfile"
    rm -rf /tmp/nginx-config
    log "NGINX configuration copied successfully"
else
    log "Using existing nginx configuration from repository"
fi

# Stop existing containers
log "Stopping existing containers..."
if ${DOCKER_COMPOSE_CMD} -f "$DEPLOYMENT_DIR/docker-compose.prod.yml" ps 2>/dev/null | grep -q "abdul"; then
    ${DOCKER_COMPOSE_CMD} -f "$DEPLOYMENT_DIR/docker-compose.prod.yml" down --remove-orphans || warning "Failed to stop some containers"
    log "Existing containers stopped"
else
    log "No existing containers found"
fi

# Build Backend Docker image
log "Building backend Docker image..."
docker build -t abdul-backend:latest \
    -f "$DEPLOYMENT_DIR/Backend/Dockerfile" \
    "$DEPLOYMENT_DIR/Backend" || error "Failed to build backend image"
log "Backend image built successfully"

# Build Namek Docker image
log "Building namek Docker image..."
docker build -t abdul-namek:latest \
    --build-arg BUILD_STAGE=production \
    --build-arg environment=production \
    --build-arg namekVersion="${CURRENT_COMMIT}" \
    --build-arg dockerport=3000 \
    --build-arg API_GATEWAY_URL="${API_GATEWAY_URL}" \
    -f "$DEPLOYMENT_DIR/namek/Dockerfile" \
    "$DEPLOYMENT_DIR/namek" || error "Failed to build namek image"
log "Namek image built successfully"

# Build Dashboard Docker image
log "Building dashboard Docker image..."
docker build -t abdul-dashboard:latest \
    -f "$DEPLOYMENT_DIR/dashboard/docker/Dockerfile" \
    "$DEPLOYMENT_DIR/dashboard" || error "Failed to build dashboard image"
log "Dashboard image built successfully"

# Build NGINX Docker image
log "Building nginx reverse proxy Docker image..."
docker build -t abdul-nginx:latest \
    -f "$DEPLOYMENT_DIR/nginx/Dockerfile" \
    "$DEPLOYMENT_DIR/nginx" || error "Failed to build nginx image"
log "NGINX image built successfully"

# Create Docker network if it doesn't exist
log "Setting up Docker network..."
if ! docker network inspect hackathon-network >/dev/null 2>&1; then
    docker network create hackathon-network || error "Failed to create Docker network"
    log "Docker network created"
else
    log "Docker network already exists"
fi

# Start new containers
log "Starting containers with ${DOCKER_COMPOSE_CMD}..."
${DOCKER_COMPOSE_CMD} -f "$DEPLOYMENT_DIR/docker-compose.prod.yml" up -d || error "Failed to start containers"

# Wait for containers to be healthy
log "Waiting for containers to become healthy..."
sleep 10

# Check container status
log "Verifying container status..."
NGINX_STATUS=$(docker ps --filter "name=abdul-nginx" --format "{{.Status}}" 2>/dev/null || echo "")
BACKEND_STATUS=$(docker ps --filter "name=abdul-backend" --format "{{.Status}}" 2>/dev/null || echo "")
NAMEK_STATUS=$(docker ps --filter "name=abdul-namek" --format "{{.Status}}" 2>/dev/null || echo "")
DASHBOARD_STATUS=$(docker ps --filter "name=abdul-dashboard" --format "{{.Status}}" 2>/dev/null || echo "")

if [ -z "$NGINX_STATUS" ]; then
    error "NGINX container is not running"
fi

if [ -z "$BACKEND_STATUS" ]; then
    error "Backend container is not running"
fi

if [ -z "$NAMEK_STATUS" ]; then
    error "Namek container is not running"
fi

if [ -z "$DASHBOARD_STATUS" ]; then
    error "Dashboard container is not running"
fi

log "NGINX container status: $NGINX_STATUS"
log "Backend container status: $BACKEND_STATUS"
log "Namek container status: $NAMEK_STATUS"
log "Dashboard container status: $DASHBOARD_STATUS"

# Verify services are accessible
log "Verifying service connectivity..."

# Check NGINX reverse proxy
if curl -f http://localhost/nginx-health > /dev/null 2>&1; then
    log "✓ NGINX reverse proxy is responding on port 80"
else
    warning "NGINX health check failed, but container is running"
fi

# Check services via NGINX proxy
if curl -f http://localhost/api/healthcheck > /dev/null 2>&1; then
    log "✓ Backend service is responding via NGINX proxy (/api)"
else
    warning "Backend health check via proxy failed"
fi

if curl -f http://localhost/from > /dev/null 2>&1; then
    log "✓ Namek service is responding via NGINX proxy (/from)"
else
    warning "Namek health check via proxy failed"
fi

if curl -f http://localhost/ > /dev/null 2>&1; then
    log "✓ Dashboard service is responding via NGINX proxy (/)"
else
    warning "Dashboard health check via proxy failed"
fi

# Check direct port access
if curl -f http://localhost:5000/healthcheck > /dev/null 2>&1; then
    log "✓ Backend service is responding on direct port 5000"
else
    warning "Backend health check on port 5000 failed"
fi

if curl -f http://localhost:3000/ > /dev/null 2>&1; then
    log "✓ Namek service is responding on direct port 3000"
else
    warning "Namek health check on port 3000 failed"
fi

if curl -f http://localhost:8080/ > /dev/null 2>&1; then
    log "✓ Dashboard service is responding on direct port 8080"
else
    warning "Dashboard health check on port 8080 failed"
fi

# Cleanup old images (keep last 2 versions)
log "Cleaning up old Docker images..."
docker image prune -f --filter "until=72h" > /dev/null 2>&1 || warning "Failed to prune old images"

# Cleanup temporary files
log "Cleaning up temporary files..."
rm -f "$DOCKER_COMPOSE_FILE" "$ENV_FILE" /tmp/deploy-ec2.sh

log "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
log "✓ Deployment completed successfully!"
log "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
log "Commit: ${CURRENT_COMMIT}"
log ""
log "Access via NGINX Reverse Proxy (Recommended):"
log "  Dashboard:  http://localhost/"
log "  Namek:      http://localhost/from"
log "  Backend:    http://localhost/api/*"
log ""
log "Direct Port Access (Development/Debug):"
log "  Dashboard:  http://localhost:8080/"
log "  Namek:      http://localhost:3000/"
log "  Backend:    http://localhost:5000/*"
log ""
log "Deployment logs: $LOG_FILE"
log "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

exit 0