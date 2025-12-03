# GitHub Actions NGINX Deployment Guide

## Overview

The GitHub Actions workflow has been updated to automatically build and deploy the NGINX reverse proxy along with all other services.

## What's Automated

When you push to the `main` branch, the workflow will:

1. ✅ Build the Backend Docker image
2. ✅ Build the Namek Docker image (with `/from` base path)
3. ✅ Build the Dashboard Docker image
4. ✅ **Build the NGINX reverse proxy Docker image**
5. ✅ **Create the Docker network (hackathon-network)**
6. ✅ Deploy all services via docker-compose
7. ✅ Verify all services are healthy (via proxy AND direct ports)

## Updated Files

### 1. GitHub Actions Workflow (`.github/workflows/deploy-to-ec2.yml`)

**New Step Added:**
```yaml
- name: Transfer nginx configuration to EC2
  run: |
    ssh -i ~/.ssh/id_rsa -o StrictHostKeyChecking=no \
      ${{ secrets.EC2_USERNAME }}@${{ secrets.EC2_HOST }} \
      "mkdir -p /tmp/nginx-config"
    scp -i ~/.ssh/id_rsa -o StrictHostKeyChecking=no \
      nginx/nginx.conf \
      ${{ secrets.EC2_USERNAME }}@${{ secrets.EC2_HOST }}:/tmp/nginx-config/
    scp -i ~/.ssh/id_rsa -o StrictHostKeyChecking=no \
      nginx/Dockerfile \
      ${{ secrets.EC2_USERNAME }}@${{ secrets.EC2_HOST }}:/tmp/nginx-config/
```

**Enhanced Verification:**
- Checks NGINX proxy health (`/nginx-health`)
- Tests all services via proxy routes
- Tests direct port access
- Displays HTTP status codes

### 2. Deployment Script (`scripts/deploy-ec2.sh`)

**New Features:**
- Copies NGINX configuration files from `/tmp/nginx-config`
- Builds `abdul-nginx:latest` Docker image
- Creates Docker network `hackathon-network` if not exists
- Checks NGINX container status
- Verifies services via both proxy and direct ports
- Enhanced deployment summary showing both access methods

## Deployment Flow

```
1. GitHub Actions Triggered (push to main)
   ↓
2. SSH to EC2 Server
   ↓
3. Transfer Files:
   - docker-compose.prod.yml
   - nginx/nginx.conf
   - nginx/Dockerfile
   - scripts/deploy-ec2.sh
   - .env file
   ↓
4. Execute Deployment Script on EC2:
   a. Pull latest code from GitHub
   b. Copy transferred files to /opt/abdul
   c. Build Docker images:
      - abdul-backend:latest
      - abdul-namek:latest
      - abdul-dashboard:latest
      - abdul-nginx:latest (NEW!)
   d. Create Docker network (NEW!)
   e. Deploy with docker-compose
   f. Verify all containers running
   g. Health check all services
   ↓
5. Display Deployment Summary
```

## Health Checks Performed

### Via NGINX Proxy (Port 80)
```bash
✓ NGINX:     http://localhost/nginx-health
✓ Dashboard: http://localhost/
✓ Namek:     http://localhost/from
✓ Backend:   http://localhost/api/healthcheck
```

### Via Direct Ports
```bash
✓ Dashboard: http://localhost:8080/
✓ Namek:     http://localhost:3000/
✓ Backend:   http://localhost:5000/healthcheck
```

## Expected GitHub Actions Output

```
====================================
Container Status:
====================================
NAME                STATUS
abdul-nginx         Up (healthy)
abdul-backend       Up (healthy)
abdul-namek         Up (healthy)
abdul-dashboard     Up (healthy)

====================================
Service Health Checks:
====================================
NGINX Proxy:  healthy
Dashboard:    200
Namek:        200
Backend API:  200
```

## Required GitHub Secrets

No new secrets needed! The workflow uses existing secrets:

- `EC2_SSH_PRIVATE_KEY` - Your PPK private key
- `EC2_SSH_PASSPHRASE` - Passphrase for the PPK
- `EC2_HOST` - EC2 server hostname/IP
- `EC2_USERNAME` - SSH username (usually ubuntu)
- `PAT_GITHUB` - GitHub Personal Access Token
- `REPO_GITHUB` - Repository URL
- `API_GATEWAY_URL` - API Gateway URL
- `BACKEND_ENV` - Backend environment variables
- `NAMEK_ENV` - Namek environment variables
- `DASHBOARD_ENV` - Dashboard environment variables

## Deployment Logs Location

On EC2 server: `/var/log/abdul-deployment.log`

View logs:
```bash
ssh your-user@your-ec2-host
sudo tail -f /var/log/abdul-deployment.log
```

## Manual Deployment (If Needed)

If you need to deploy manually on EC2:

```bash
# SSH to EC2
ssh your-user@your-ec2-host

# Navigate to deployment directory
cd /opt/abdul

# Pull latest changes
git pull origin main

# Build images
docker build -t abdul-backend:latest -f Backend/Dockerfile Backend/
docker build -t abdul-namek:latest -f namek/Dockerfile namek/
docker build -t abdul-dashboard:latest -f dashboard/docker/Dockerfile dashboard/
docker build -t abdul-nginx:latest -f nginx/Dockerfile nginx/

# Create network
docker network create hackathon-network || true

# Deploy
docker compose -f docker-compose.prod.yml up -d

# Verify
docker ps
curl http://localhost/nginx-health
```

## Troubleshooting

### Issue: NGINX container fails to start

**Check logs:**
```bash
docker logs abdul-nginx
```

**Common causes:**
1. Port 80 already in use
2. Invalid nginx.conf syntax
3. Missing configuration files

**Solution:**
```bash
# Test nginx config
docker exec abdul-nginx nginx -t

# Check if port 80 is in use
sudo lsof -i :80

# Rebuild nginx image
cd /opt/abdul
docker build -t abdul-nginx:latest -f nginx/Dockerfile nginx/
docker compose -f docker-compose.prod.yml restart nginx
```

### Issue: Services not accessible via proxy

**Check nginx logs:**
```bash
docker logs abdul-nginx --tail 100
```

**Check if services are running:**
```bash
docker ps | grep abdul
```

**Test service connectivity from nginx container:**
```bash
docker exec abdul-nginx wget -O- http://dashboard:80/
docker exec abdul-nginx wget -O- http://namek:3000/
docker exec abdul-nginx wget -O- http://backend:5000/healthcheck
```

### Issue: GitHub Actions workflow fails

**Common issues:**

1. **SSH connection failed:**
   - Verify EC2_SSH_PRIVATE_KEY secret
   - Check EC2_HOST is correct
   - Ensure EC2 security group allows SSH

2. **File transfer failed:**
   - Check disk space on EC2: `df -h`
   - Verify /tmp directory is writable

3. **Docker build failed:**
   - Check EC2 has enough memory
   - View full logs in GitHub Actions

4. **Health checks fail:**
   - Services might need more time to start
   - Check container logs on EC2

## Monitoring Deployment

### Watch GitHub Actions

1. Go to your repository on GitHub
2. Click "Actions" tab
3. Click on the running workflow
4. Expand each step to see details

### Monitor on EC2

```bash
# Watch container status
watch docker ps

# Follow deployment logs
tail -f /var/log/abdul-deployment.log

# Monitor resource usage
docker stats abdul-nginx abdul-backend abdul-namek abdul-dashboard
```

## Rollback Procedure

If deployment fails:

```bash
# SSH to EC2
ssh your-user@your-ec2-host

# Navigate to deployment directory
cd /opt/abdul

# Check git log
git log --oneline -10

# Rollback to previous commit
git reset --hard <previous-commit-hash>

# Rebuild and redeploy
docker compose -f docker-compose.prod.yml down
# Rebuild images...
docker compose -f docker-compose.prod.yml up -d
```

## Success Indicators

✅ All containers show "Up (healthy)" status
✅ NGINX health check returns "healthy"
✅ Dashboard accessible at `http://your-ec2-ip/`
✅ Namek accessible at `http://your-ec2-ip/from`
✅ Backend API accessible at `http://your-ec2-ip/api/healthcheck`
✅ All HTTP status codes return 200
✅ No errors in deployment logs

## Next Steps After Deployment

1. **Test all features:**
   - Navigate to dashboard
   - Test Namek submission flow
   - Verify API endpoints

2. **Configure domain (if applicable):**
   - Point domain to EC2 IP
   - Update nginx.conf with actual domain
   - Add SSL certificates

3. **Set up monitoring:**
   - Configure CloudWatch (AWS)
   - Set up log aggregation
   - Create health check alarms

4. **Security hardening:**
   - Configure firewall to allow only port 80
   - Block direct port access from outside
   - Set up SSL/TLS certificates

## Getting Help

If you encounter issues:

1. Check GitHub Actions logs
2. Review `/var/log/abdul-deployment.log` on EC2
3. Check Docker container logs: `docker logs <container-name>`
4. Verify all configuration files are correct
5. Ensure all GitHub secrets are set correctly

## Summary

Your GitHub Actions workflow now automatically:
- ✅ Builds NGINX reverse proxy
- ✅ Creates Docker network
- ✅ Deploys all services
- ✅ Verifies health on both proxy and direct ports
- ✅ Provides detailed deployment summary

Every push to `main` triggers a complete deployment with NGINX reverse proxy!