# NGINX Reverse Proxy Deployment Guide

## Overview

This guide explains how to deploy and test the nginx reverse proxy configuration that routes traffic to your Dashboard, Namek, and Backend services.

## Architecture Summary

### NGINX Reverse Proxy (Port 80)
- **Dashboard**: Accessible at `/` (root path)
- **Namek**: Accessible at `/from`
- **Backend API**: Accessible at `/api`

### Direct Port Access
All services are also available directly on their individual ports:
- **Backend**: Port 5000
- **Namek**: Port 3000
- **Dashboard**: Port 8080

This dual-access configuration allows you to:
- Use nginx proxy for production (recommended)
- Access services directly for development/debugging
- Test each service independently

## Prerequisites

1. Docker and Docker Compose installed
2. All three services built as Docker images:
   - `abdul-backend:latest`
   - `abdul-namek:latest`
   - `abdul-dashboard:latest`

## Deployment Steps

### 1. Create the Docker Network

```bash
docker network create hackathon-network
```

### 2. Build the NGINX Image

```bash
cd nginx
docker build -t abdul-nginx:latest .
cd ..
```

### 3. Set Environment Variables

Create a `.env` file in the project root with the following variables:

```bash
# Backend Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000

# Namek Configuration
NODE_ENV=production
DOCKER_PORT=3000
API_GATEWAY_URL=/api

# Add any other required environment variables
```

### 4. Deploy the Services

```bash
docker-compose -f docker-compose.prod.yml up -d
```

### 5. Verify All Services Are Running

```bash
docker-compose -f docker-compose.prod.yml ps
```

Expected output:
```
NAME                IMAGE                   STATUS
abdul-nginx         abdul-nginx:latest      Up (healthy)
abdul-backend       abdul-backend:latest    Up (healthy)
abdul-namek         abdul-namek:latest      Up (healthy)
abdul-dashboard     abdul-dashboard:latest  Up (healthy)
```

## Testing the Configuration

### 1. Test NGINX Health

```bash
curl http://localhost/nginx-health
```

Expected response: `healthy`

### 2. Test Dashboard

Open your browser and navigate to:
```
http://localhost/
```

You should see the dashboard homepage.

### 3. Test Namek Application

Navigate to:
```
http://localhost/from
```

You should see the Namek hackathon submission interface.

**Important**: Verify that:
- Static assets load correctly (check browser console for 404 errors)
- Routing works (navigate between pages)
- The base path `/from` is preserved in the URL

### 4. Test Backend API

```bash
# Test health check
curl http://localhost/api/healthcheck

# Test other API endpoints (replace with your actual endpoints)
curl http://localhost/api/hackathon
curl http://localhost/api/scores
```

### 5. Test API Integration

#### From Dashboard:
- Open browser developer tools (F12)
- Navigate to Dashboard (`http://localhost/`)
- Check the Network tab
- Verify API calls are made to `/api/*` endpoints

#### From Namek:
- Navigate to Namek (`http://localhost/from`)
- Open developer tools
- Test any API-dependent features
- Verify calls go to `/api/*` endpoints

## Troubleshooting

### Issue: 502 Bad Gateway

**Cause**: Backend service is not responding or not healthy.

**Solutions**:
1. Check backend logs:
   ```bash
   docker logs abdul-backend
   ```

2. Verify backend is healthy:
   ```bash
   docker inspect abdul-backend | grep -A 10 Health
   ```

3. Test backend directly (if exposed):
   ```bash
   docker exec abdul-backend curl http://localhost:5000/healthcheck
   ```

### Issue: 404 Not Found for Static Assets

**Cause**: Static assets not found due to incorrect base path.

**Solutions for Dashboard**:
1. Verify dashboard nginx config serves static files correctly
2. Check build output in the dashboard container

**Solutions for Namek**:
1. Verify Vite config has `base: '/from'`
2. Rebuild namek image:
   ```bash
   docker build -t abdul-namek:latest ./namek
   ```

### Issue: CORS Errors

**Cause**: Backend not allowing requests from nginx proxy.

**Solutions**:
1. Check backend CORS configuration
2. Verify proxy headers are being forwarded
3. Add CORS headers in nginx if needed (see Advanced Configuration)

### Issue: Namek Routes Not Working

**Cause**: Browser routing not handled correctly.

**Solutions**:
1. Verify namek's production server (build/server.js) handles SPA routing
2. Check nginx proxies both `/from` and `/from/*` correctly

### Issue: Health Check Failing

**Solutions**:
1. Check individual service health:
   ```bash
   docker-compose -f docker-compose.prod.yml ps
   ```

2. Restart unhealthy services:
   ```bash
   docker-compose -f docker-compose.prod.yml restart <service-name>
   ```

## Viewing Logs

### All Services
```bash
docker-compose -f docker-compose.prod.yml logs -f
```

### Specific Service
```bash
docker-compose -f docker-compose.prod.yml logs -f nginx
docker-compose -f docker-compose.prod.yml logs -f backend
docker-compose -f docker-compose.prod.yml logs -f namek
docker-compose -f docker-compose.prod.yml logs -f dashboard
```

### NGINX Access Logs
```bash
docker exec abdul-nginx tail -f /var/log/nginx/access.log
```

### NGINX Error Logs
```bash
docker exec abdul-nginx tail -f /var/log/nginx/error.log
```

## Stopping the Services

```bash
docker-compose -f docker-compose.prod.yml down
```

To also remove volumes:
```bash
docker-compose -f docker-compose.prod.yml down -v
```

## Production Checklist

Before deploying to production:

- [ ] All services build successfully
- [ ] Environment variables configured correctly
- [ ] SSL/TLS certificates configured (if using HTTPS)
- [ ] Health checks passing for all services
- [ ] Dashboard accessible at `/`
- [ ] Namek accessible at `/from` with correct routing
- [ ] Backend API accessible at `/api/*`
- [ ] Static assets loading correctly
- [ ] API calls working from both frontends
- [ ] CORS configured properly
- [ ] Logs monitored for errors
- [ ] Resource limits set for containers
- [ ] Backup and rollback plan ready

## Advanced Configuration

### Adding SSL/TLS

1. Add SSL certificate files to nginx directory
2. Update `nginx/nginx.conf` to include:

```nginx
server {
    listen 443 ssl;
    server_name yourdomain.com;
    
    ssl_certificate /etc/nginx/ssl/cert.pem;
    ssl_certificate_key /etc/nginx/ssl/key.pem;
    
    # ... rest of configuration
}

server {
    listen 80;
    server_name yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

3. Update Dockerfile to copy certificates:
```dockerfile
COPY ssl/ /etc/nginx/ssl/
```

### Rate Limiting

Add to `nginx/nginx.conf`:

```nginx
limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;

location /api/ {
    limit_req zone=api_limit burst=20;
    # ... rest of configuration
}
```

### Custom Error Pages

Add to `nginx/nginx.conf`:

```nginx
error_page 404 /404.html;
error_page 500 502 503 504 /50x.html;

location = /404.html {
    root /usr/share/nginx/html;
    internal;
}

location = /50x.html {
    root /usr/share/nginx/html;
    internal;
}
```

## Performance Monitoring

### Monitor Resource Usage

```bash
docker stats abdul-nginx abdul-backend abdul-namek abdul-dashboard
```

### Check Connection Statistics

```bash
docker exec abdul-nginx nginx -T
```

## Support

For issues or questions:
1. Check logs first
2. Review troubleshooting section
3. Verify configuration matches this guide
4. Test each component individually

## Files Created/Modified

- `nginx/nginx.conf` - Main reverse proxy configuration
- `nginx/Dockerfile` - NGINX container definition
- `docker-compose.prod.yml` - Updated with nginx service
- `namek/vite.config.ts` - Updated base path to `/from`
- `dashboard/.env.example` - Updated API URL documentation
- `namek/.env.example` - Updated API URL documentation