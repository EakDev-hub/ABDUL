# Dashboard Deployment Guide

This document outlines the integration of the `/dashboard` project into the ABDUL GitHub Actions deployment pipeline.

## 🎯 Overview

The dashboard has been successfully integrated into the existing CI/CD pipeline alongside the backend and namek services. When deployed, it will run on **port 8080** and connect to the backend API on port 5000.

## 📋 What Changed

### 1. Docker Compose Configuration (`docker-compose.prod.yml`)
Added a new `dashboard` service:
- **Container name**: `abdul-dashboard`
- **Port mapping**: `8080:80` (host:container)
- **Image**: `abdul-dashboard:latest`
- **Health check**: Configured to verify service availability
- **Dependencies**: Depends on backend service
- **Network**: Connected to `abdul-network`

### 2. Deployment Script (`scripts/deploy-ec2.sh`)
Enhanced the deployment script to:
- Build the `abdul-dashboard` Docker image
- Verify dashboard container status
- Check dashboard health at port 8080
- Include dashboard in deployment logs

### 3. GitHub Actions Workflow (`.github/workflows/deploy-to-ec2.yml`)
Updated to include `DASHBOARD_ENV` secret in the environment file creation step.

### 4. Documentation (`GITHUB_SECRETS_SETUP.md`)
- Added documentation for the new `DASHBOARD_ENV` secret
- Updated secrets summary table (now 10 secrets total)
- Updated troubleshooting sections to include dashboard
- Updated port references to include 8080

### 5. Dashboard Environment Configuration (`dashboard/.env.example`)
Updated with production-ready settings:
- `VITE_API_BASE_URL=http://localhost:5000`
- `NODE_ENV=production`
- `VITE_APP_ENV=production`

## 🔧 Required Action: Add GitHub Secret

You need to add **ONE** new GitHub Secret before deploying:

### Secret Name: `DASHBOARD_ENV`

**Content:**
```
NODE_ENV=production
VITE_API_BASE_URL=http://localhost:5000
VITE_APP_ENV=production
```

**How to Add:**
1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `DASHBOARD_ENV`
5. Value: Paste the content above
6. Click **Add secret**

> **Note:** The `VITE_API_BASE_URL` uses `http://localhost:5000` because the dashboard container communicates with the backend container through Docker's internal network. Users will access the dashboard via `http://<EC2_IP>:8080`.

## 🚀 Deployment

### Automatic Deployment
The dashboard will automatically deploy when you:
- Push to the `main` branch
- Manually trigger the workflow via GitHub Actions

### Manual Trigger
1. Go to **Actions** tab in GitHub
2. Select **Deploy to EC2 (Remote Build)** workflow
3. Click **Run workflow**
4. Select `main` branch
5. Click **Run workflow**

## ✅ Verification

After deployment, verify all three services are running:

### Service URLs
- **Backend API**: `http://<EC2_IP>:5000`
- **Namek Frontend**: `http://<EC2_IP>:3000`
- **Dashboard**: `http://<EC2_IP>:8080` ⭐ NEW

### Check via GitHub Actions
The workflow will automatically verify all containers are running and display their status.

### Check via SSH
Connect to your EC2 instance and run:
```bash
# Check all running containers
docker ps --filter 'name=abdul'

# Check specific dashboard container
docker ps --filter 'name=abdul-dashboard'

# View dashboard logs
docker logs abdul-dashboard

# Test dashboard endpoint
curl http://localhost:8080/
```

## 📊 Architecture

```
┌─────────────────────────────────────────────────────┐
│              EC2 Instance                           │
│                                                     │
│  ┌─────────────────────────────────────────────┐  │
│  │         Docker Network (abdul-network)      │  │
│  │                                             │  │
│  │  ┌──────────────┐  ┌──────────────┐       │  │
│  │  │   backend    │  │    namek     │       │  │
│  │  │  Port 5000   │  │  Port 3000   │       │  │
│  │  └──────────────┘  └──────────────┘       │  │
│  │         ▲                                   │  │
│  │         │ API calls                         │  │
│  │  ┌──────────────┐                          │  │
│  │  │  dashboard   │                          │  │
│  │  │  Port 8080   │                          │  │
│  │  └──────────────┘                          │  │
│  └─────────────────────────────────────────────┘  │
│         │           │           │                  │
│      :5000       :3000       :8080                 │
└─────────────────────────────────────────────────────┘
         │           │           │
         ▼           ▼           ▼
    External Access via EC2 Public IP
```

## 🔒 Security Considerations

- **Firewall**: Ensure EC2 Security Group allows inbound traffic on port 8080
- **Environment Variables**: Store sensitive data only in GitHub Secrets
- **API Communication**: Dashboard uses Docker internal network to communicate with backend

## 🐛 Troubleshooting

### Dashboard container not starting
```bash
# Check container logs
docker logs abdul-dashboard

# Verify image was built
docker images | grep abdul-dashboard

# Check if port is available
sudo netstat -tlnp | grep 8080
```

### Cannot access dashboard from browser
1. Verify EC2 Security Group allows port 8080
2. Check dashboard container is running: `docker ps | grep dashboard`
3. Test locally on EC2: `curl http://localhost:8080/`

### Dashboard shows blank page
1. Check browser console for errors
2. Verify `VITE_API_BASE_URL` in `DASHBOARD_ENV` secret
3. Check if backend is accessible from dashboard container

### API calls failing
1. Verify backend container is running
2. Check Docker network connectivity:
   ```bash
   docker exec abdul-dashboard ping abdul-backend
   ```
3. Verify `VITE_API_BASE_URL` environment variable

## 📝 Deployment Checklist

- [ ] Add `DASHBOARD_ENV` GitHub Secret
- [ ] Ensure EC2 Security Group allows port 8080
- [ ] Trigger deployment (push to main or manual trigger)
- [ ] Verify workflow completes successfully
- [ ] Access dashboard at `http://<EC2_IP>:8080`
- [ ] Test dashboard functionality
- [ ] Verify API calls to backend work

## 🎉 Success Indicators

After successful deployment, you should see:

**In GitHub Actions logs:**
```
✓ Dashboard image built successfully
✓ Dashboard container status: Up X seconds (healthy)
✓ Dashboard service is responding on port 8080
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✓ Deployment completed successfully!
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Backend: http://localhost:5000
Namek: http://localhost:3000
Dashboard: http://localhost:8080
```

**In Docker:**
```bash
$ docker ps --filter 'name=abdul'
NAMES              STATUS
abdul-backend      Up 2 minutes (healthy)
abdul-namek        Up 2 minutes (healthy)
abdul-dashboard    Up 2 minutes (healthy)
```

## 📚 Related Documentation

- [GITHUB_SECRETS_SETUP.md](./GITHUB_SECRETS_SETUP.md) - Complete secrets configuration guide
- [DEPLOYMENT.md](./DEPLOYMENT.md) - General deployment documentation
- [dashboard/README.md](./dashboard/README.md) - Dashboard project documentation

---

**Need Help?** Check the troubleshooting section above or review the deployment logs in GitHub Actions.