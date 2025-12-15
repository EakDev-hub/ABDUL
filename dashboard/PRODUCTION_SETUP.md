# Dashboard Production Setup with Encrypted Environment Configuration

## Overview

The dashboard uses encrypted environment configuration that works in both development and production:
- **Development**: Vite dev server provides `/get-env` endpoint via middleware
- **Production**: Node.js server provides `/get-env` endpoint, proxied through nginx

## Architecture

```
Browser Request (/get-env)
    ↓
Nginx (port 80)
    ↓
Node.js Env Server (port 3001)
    ↓
Returns encrypted environment config
```

## Files Involved

### Production Infrastructure
- [`docker/env-server.js`](docker/env-server.js) - Node.js server that encrypts and serves environment variables
- [`docker/nginx/nginx.conf`](docker/nginx/nginx.conf) - Nginx config with proxy to env server
- [`docker/Dockerfile`](docker/Dockerfile) - Multi-stage build with nginx + Node.js
- [`docker/start.sh`](docker/start.sh) - Startup script for both nginx and env server

### Application Code
- [`src/config/env.ts`](src/config/env.ts) - Fetches and decrypts environment config
- [`vite.config.ts`](vite.config.ts) - Development server with `/get-env` endpoint

## How It Works

### 1. Development Mode

```bash
npm run dev
```

- Vite dev server starts on port 5173
- `vite.config.ts` middleware provides `/get-env` endpoint
- Environment variables from `.env` are encrypted and served
- Client-side code fetches and decrypts configuration

### 2. Production Build

```bash
# Step 1: Create/update .env file with production values
cp .env.production .env
# Edit .env with your actual backend URL

# Step 2: Build the application
npm run build
```

- Vite builds the static files (HTML, JS, CSS)
- Environment variables are NOT embedded in the build
- Configuration will be loaded at runtime from `/get-env` endpoint

### 3. Production Deployment (Docker)

```bash
# Build Docker image
docker build -f docker/Dockerfile -t dashboard:latest .

# Run container
docker run -d \
  -p 80:80 \
  -e NODE_ENV=production \
  -e VITE_API_GATEWAY_URL=https://your-backend-url.com \
  --name dashboard \
  dashboard:latest
```

**Container Startup Sequence:**
1. `start.sh` starts the Node.js env server on port 3001
2. `start.sh` starts nginx on port 80
3. Nginx proxies `/get-env` requests to the Node.js server
4. Browser requests `/get-env` when the app loads
5. Encrypted config is returned and decrypted client-side

## Environment Variables

### Required Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `NODE_ENV` | Environment mode | `production` |
| `VITE_API_GATEWAY_URL` | Backend API URL | `https://api.yourdomain.com` |

### Optional Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_APP_TITLE` | Application title | `ABDUL - Hackathon Dashboard` |
| `VITE_APP_VERSION` | Application version | `1.0.0` |

## Security

### Encryption Details
- Algorithm: AES encryption via CryptoJS
- Key: `'This is env'` (hardcoded in both server and client)
- Purpose: Obscure environment values from casual inspection, not cryptographic security

**Important**: The encryption key is included in the client-side JavaScript, so this approach provides obfuscation rather than true security. Do not store sensitive secrets this way.

### Best Practices
1. Use HTTPS in production to encrypt all traffic
2. Set `VITE_API_GATEWAY_URL` to actual backend URL
3. Never commit `.env` with production values to git
4. Use environment variables or secrets management in CI/CD

## Docker Compose

Example `docker-compose.yml`:

```yaml
version: '3.8'

services:
  dashboard:
    build:
      context: .
      dockerfile: docker/Dockerfile
    ports:
      - "80:80"
    environment:
      - NODE_ENV=production
      - VITE_API_GATEWAY_URL=https://api.yourdomain.com
      - VITE_APP_TITLE=ABDUL - Hackathon Dashboard
      - VITE_APP_VERSION=1.0.0
    restart: unless-stopped
```

## GitHub Actions Deployment

Example workflow snippet:

```yaml
- name: Build and push Docker image
  env:
    VITE_API_GATEWAY_URL: ${{ secrets.VITE_API_GATEWAY_URL }}
  run: |
    docker build \
      --build-arg NODE_ENV=production \
      -f dashboard/docker/Dockerfile \
      -t dashboard:latest \
      dashboard/
```

Then set environment variables in the container at runtime.

## Troubleshooting

### /get-env returns 404
- Check that env-server.js is running: `docker exec <container> ps aux | grep node`
- Check nginx config: `docker exec <container> cat /etc/nginx/conf.d/default.conf`
- Check nginx error logs: `docker logs <container>`

### Environment variables not updating
- Environment variables are set at container start time
- Restart the container after changing environment variables
- Check env-server.js is receiving correct values: `docker exec <container> node -e "console.log(process.env.VITE_API_GATEWAY_URL)"`

### CORS errors
- Ensure `/get-env` is served from the same origin as the app
- Check nginx proxy configuration
- Verify no trailing slashes in configuration

### Encryption/Decryption errors
- Ensure encryption key matches on server and client
- Check browser console for decryption errors
- Verify JSON structure of encrypted data

## Development vs Production Comparison

| Aspect | Development | Production |
|--------|-------------|------------|
| Server | Vite dev server | Nginx + Node.js |
| Port | 5173 | 80 |
| /get-env | Vite middleware | Node.js → Nginx proxy |
| Hot reload | Yes | No |
| Environment source | `.env` file | Container env vars |
| Build required | No | Yes |

## Migration from Simple Approach

If you previously used the simplified environment approach:

1. **Install dependencies**:
   ```bash
   cd dashboard
   npm install
   ```

2. **Update Docker images**:
   ```bash
   docker-compose down
   docker-compose build --no-cache
   docker-compose up -d
   ```

3. **Set environment variables** via docker-compose or container environment

## Testing

### Test Development Mode
```bash
cd dashboard
npm install
npm run dev
# Visit http://localhost:5173
# Open browser console and check for encrypted config fetch
```

### Test Production Build Locally
```bash
cd dashboard
npm run build
npm run preview
# Visit http://localhost:4173
```

### Test Docker Build
```bash
cd dashboard
docker build -f docker/Dockerfile -t dashboard-test .
docker run -d -p 8080:80 \
  -e VITE_API_GATEWAY_URL=http://localhost:5000 \
  dashboard-test
# Visit http://localhost:8080
```

## Additional Notes

- The Node.js env server is lightweight and runs alongside nginx in the same container
- The server only responds to `/get-env` requests, everything else returns 404
- Nginx efficiently serves static files while proxying only `/get-env` to Node.js
- This approach allows runtime configuration without rebuilding the application
- All configuration is encrypted in transit (over HTTPS in production)

## Support

For issues or questions about the production setup, check:
1. Container logs: `docker logs <container-name>`
2. Nginx access/error logs
3. Browser network tab for `/get-env` request/response
4. Browser console for decryption errors