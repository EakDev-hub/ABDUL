# Dual Access Configuration Guide

## Overview

Your services are now configured to be accessible in TWO ways:
1. **Through NGINX Reverse Proxy** (Port 80) - Recommended for production
2. **Direct Port Access** - Useful for development and debugging

## Access Methods

### Dashboard

| Method | URL | Notes |
|--------|-----|-------|
| **NGINX Proxy** | `http://localhost/` | ✅ Production recommended |
| **Direct Port** | `http://localhost:8080/` | 🔧 Development/Debug |

### Namek

| Method | URL | Notes |
|--------|-----|-------|
| **NGINX Proxy** | `http://localhost/from` | ✅ Production recommended (respects `/from` base path) |
| **Direct Port** | `http://localhost:3000/` | ⚠️ May not work correctly (built with `/from` base) |

### Backend API

| Method | URL Format | Example |
|--------|------------|---------|
| **NGINX Proxy** | `http://localhost/api/*` | `http://localhost/api/healthcheck` |
| **Direct Port** | `http://localhost:5000/*` | `http://localhost:5000/healthcheck` |

⚠️ **Note**: The path structure differs between proxy and direct access for the API!

## Quick Test Commands

### Test All Services via NGINX Proxy

```bash
# Dashboard
curl -I http://localhost/

# Namek
curl -I http://localhost/from

# Backend
curl http://localhost/api/healthcheck
```

### Test All Services via Direct Ports

```bash
# Dashboard
curl -I http://localhost:8080/

# Namek
curl -I http://localhost:3000/

# Backend
curl http://localhost:5000/healthcheck
```

## Port Summary

| Service | NGINX | Direct | Container Internal |
|---------|-------|--------|-------------------|
| NGINX | 80 | - | 80 |
| Backend | 80 → /api/* | 5000 | 5000 |
| Namek | 80 → /from | 3000 | 3000 |
| Dashboard | 80 → / | 8080 | 80 |

## When to Use Each Method

### Use NGINX Proxy (Port 80) When:
- ✅ Running in production
- ✅ Want clean URLs with path-based routing
- ✅ Need a single entry point for all services
- ✅ Want proper routing for Namek (`/from` base path)
- ✅ Testing the complete system as users will access it

### Use Direct Ports When:
- 🔧 Debugging a specific service
- 🔧 Testing service in isolation
- 🔧 Service won't start and you need to check logs
- 🔧 Bypassing nginx for troubleshooting
- 🔧 Development and rapid testing

## Important Notes

### Namek Direct Access Limitation
Namek is built with `base: '/from'` in vite.config.ts, so:
- ✅ Works correctly at `http://localhost/from` (via nginx)
- ⚠️ May have issues at `http://localhost:3000/` (direct)
- Assets will try to load from `/from/assets/*` even on direct port

### API Path Differences
When making API calls:
- Via Proxy: Use `/api/endpoint` (nginx strips `/api` prefix)
- Direct: Use `/endpoint` (no `/api` prefix)

Example:
```javascript
// Via NGINX Proxy
fetch('http://localhost/api/healthcheck')

// Direct Port
fetch('http://localhost:5000/healthcheck')
```

### CORS Considerations
- NGINX Proxy: Same origin, no CORS issues
- Direct Ports: May encounter CORS errors if frontend calls backend directly

## Docker Compose Configuration

Your current `docker-compose.prod.yml` exposes all ports:

```yaml
nginx:
  ports:
    - "80:80"

backend:
  ports:
    - "5000:5000"

namek:
  ports:
    - "3000:3000"

dashboard:
  ports:
    - "8080:80"
```

## Verifying Deployment

After running `docker-compose -f docker-compose.prod.yml up -d`:

```bash
# Check all ports are listening
netstat -an | grep LISTEN | grep -E ':(80|3000|5000|8080)'

# Or on macOS:
lsof -i -P | grep LISTEN | grep -E ':(80|3000|5000|8080)'

# Check service health
curl http://localhost/nginx-health
curl http://localhost/api/healthcheck
curl http://localhost:5000/healthcheck
```

## Firewall Configuration (Production)

If deploying to production server, consider:

```bash
# Allow NGINX proxy (required)
sudo ufw allow 80/tcp

# Optionally block direct ports from external access
sudo ufw deny 3000/tcp
sudo ufw deny 5000/tcp
sudo ufw deny 8080/tcp

# This allows internal Docker network but blocks external access
```

Or in AWS Security Group:
- Inbound rule: Port 80 from 0.0.0.0/0 ✅
- Inbound rule: Port 3000 from 172.31.0.0/16 only (internal)
- Inbound rule: Port 5000 from 172.31.0.0/16 only (internal)
- Inbound rule: Port 8080 from 172.31.0.0/16 only (internal)

## Switching Between Configurations

### To Expose Only NGINX (More Secure)

Modify `docker-compose.prod.yml`:
```yaml
backend:
  expose:
    - "5000"  # Internal only

namek:
  expose:
    - "3000"  # Internal only

dashboard:
  expose:
    - "80"    # Internal only
```

### To Expose All Ports (Current Configuration)

Keep current configuration:
```yaml
backend:
  ports:
    - "5000:5000"  # External access

namek:
  ports:
    - "3000:3000"  # External access

dashboard:
  ports:
    - "8080:80"    # External access
```

## Troubleshooting

### Port Already in Use

```bash
# Check what's using a port
lsof -i :80
lsof -i :3000
lsof -i :5000
lsof -i :8080

# Stop conflicting service or change port in docker-compose.prod.yml
```

### Service Works on Direct Port but Not via NGINX

1. Check nginx logs:
   ```bash
   docker logs abdul-nginx
   ```

2. Test nginx configuration:
   ```bash
   docker exec abdul-nginx nginx -t
   ```

3. Verify service is reachable from nginx container:
   ```bash
   docker exec abdul-nginx wget -O- http://backend:5000/healthcheck
   ```

### Service Works via NGINX but Not on Direct Port

1. Verify port mapping:
   ```bash
   docker ps | grep abdul
   ```

2. Check if port is actually listening:
   ```bash
   docker exec abdul-backend netstat -an | grep :5000
   ```

## Summary

You now have maximum flexibility:
- 🌐 **Production users** → Use nginx proxy on port 80
- 🔧 **Developers** → Can access services directly on individual ports
- 🐛 **Debugging** → Test each service independently
- 🚀 **Deployment** → Can lock down direct ports via firewall when ready

Both access methods work simultaneously without any conflicts!