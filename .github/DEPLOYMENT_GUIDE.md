# GitHub Actions Deployment Guide

## Overview

This project includes automated CI/CD workflows using GitHub Actions to build and push Docker images to Docker Hub.

## Workflow: Build and Push Docker Image

**File**: `.github/workflows/docker-build-push.yml`

### What It Does

1. **Triggers**:
   - On push to `main` branch
   - On push to any `feature/**` branch
   - On pull requests to `main` branch
   - Only when `dashboard/` or workflow files change

2. **Actions**:
   - Sets up Docker Buildx for multi-platform builds
   - Logs into Docker Hub
   - Builds production Docker image
   - Pushes image to Docker Hub with automatic tagging
   - Caches layers for faster builds
   - Tests development image on PRs

3. **Image Tagging**:
   - `latest` - Latest version on main branch
   - `main` - Latest from main branch
   - `feature/branch-name` - Feature branch builds
   - `v1.0.0` - Semantic version tags
   - `sha-abc123def` - Git commit SHA

## Setup Instructions

### 1. Create Docker Hub Account

If you don't have one:
- Go to [Docker Hub](https://hub.docker.com)
- Sign up for a free account
- Create a repository named `dashboard`

### 2. Generate Docker Hub Access Token

1. Log in to Docker Hub
2. Go to Account Settings → Security
3. Click "New Access Token"
4. Name it: `github-actions`
5. Copy the token (you'll need it next)

### 3. Add GitHub Secrets

1. Go to your GitHub repository
2. Settings → Secrets and variables → Actions
3. Click "New repository secret"
4. Add two secrets:

   **Secret 1**:
   - Name: `DOCKER_USERNAME`
   - Value: Your Docker Hub username

   **Secret 2**:
   - Name: `DOCKER_PASSWORD`
   - Value: Your Docker Hub access token

### 4. Verify Workflow

1. Push code to `main` or a `feature/` branch
2. Go to GitHub repository → Actions tab
3. Watch the workflow run
4. Check Docker Hub for the new image

## Workflow Details

### Triggers

```yaml
on:
  push:
    branches:
      - main
      - feature/**
    paths:
      - 'dashboard/**'
      - '.github/workflows/docker-build-push.yml'
  pull_request:
    branches:
      - main
    paths:
      - 'dashboard/**'
```

**Explanation**:
- Runs on push to `main` or `feature/*` branches
- Only runs if `dashboard/` files or workflow changes
- Runs on PRs to `main` for testing

### Build Steps

1. **Checkout**: Clones the repository
2. **Setup Buildx**: Enables Docker multi-platform builds
3. **Login**: Authenticates with Docker Hub using secrets
4. **Extract Metadata**: Generates image tags and labels
5. **Build & Push**: Builds and pushes production image
6. **Dev Build**: Tests development image on PRs

### Image Naming

Images are pushed to:
```
docker.io/YOUR_USERNAME/dashboard:TAG
```

Example tags:
- `docker.io/myusername/dashboard:latest`
- `docker.io/myusername/dashboard:main`
- `docker.io/myusername/dashboard:feature-new-ui`
- `docker.io/myusername/dashboard:v1.0.0`

## Using the Pushed Images

### Pull from Docker Hub

```bash
# Latest version
docker pull YOUR_USERNAME/dashboard:latest

# Specific branch
docker pull YOUR_USERNAME/dashboard:main

# Specific feature
docker pull YOUR_USERNAME/dashboard:feature-dashboard-vue-docker

# Run the image
docker run -p 80:80 YOUR_USERNAME/dashboard:latest
```

### In Docker Compose

```yaml
version: '3.8'
services:
  dashboard:
    image: YOUR_USERNAME/dashboard:latest
    ports:
      - "80:80"
    restart: unless-stopped
```

## Monitoring Builds

### GitHub Actions Dashboard

1. Go to your repository
2. Click "Actions" tab
3. View workflow runs
4. Click on a run to see details
5. Check logs for any errors

### Docker Hub Dashboard

1. Log in to Docker Hub
2. Go to your `dashboard` repository
3. View all pushed images and tags
4. Check image details and layers

## Troubleshooting

### Build Fails

**Check logs**:
1. Go to Actions tab
2. Click the failed workflow
3. Expand the failed step
4. Read the error message

**Common issues**:
- Docker Hub credentials incorrect
- Dockerfile syntax error
- Missing files in context
- Node.js version incompatibility

### Image Not Pushed

**Verify**:
1. Secrets are set correctly
2. Branch matches trigger conditions
3. Files in `dashboard/` were changed
4. Workflow file is valid YAML

### Slow Builds

**Optimize**:
- GitHub Actions cache is enabled
- First build is slower (builds cache)
- Subsequent builds use cache layers
- Multi-platform builds take longer

## Advanced Configuration

### Add Semantic Versioning

To automatically tag releases:

1. Create a release on GitHub
2. Workflow automatically tags with version
3. Example: `v1.0.0` creates image tag `1.0.0`

### Multi-Platform Builds

Current workflow supports:
- `linux/amd64` (Intel/AMD)
- `linux/arm64` (Apple Silicon, ARM servers)

To enable multi-platform:
```yaml
platforms: linux/amd64,linux/arm64
```

### Custom Registry

To use different registry (AWS ECR, GCP, etc.):

1. Update `REGISTRY` variable
2. Change login action
3. Update image naming

## Security Best Practices

✅ **Implemented**:
- Secrets stored securely
- No credentials in code
- Read-only permissions for checkout
- Cache isolation per branch

✅ **Recommended**:
- Rotate Docker Hub token regularly
- Use branch protection rules
- Require PR reviews before merge
- Sign commits with GPG

## CI/CD Pipeline Flow

```
Push to GitHub
    ↓
Trigger Workflow
    ↓
Checkout Code
    ↓
Setup Docker Buildx
    ↓
Login to Docker Hub
    ↓
Build Docker Image
    ↓
Push to Docker Hub
    ↓
Complete ✓
```

## Next Steps

1. **Set up secrets** in GitHub repository
2. **Push code** to trigger workflow
3. **Monitor** the Actions tab
4. **Verify** image on Docker Hub
5. **Deploy** using the pushed image

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [Docker Hub Documentation](https://docs.docker.com/docker-hub/)
- [Semantic Versioning](https://semver.org/)

## Support

For issues:
1. Check GitHub Actions logs
2. Verify Docker Hub credentials
3. Review workflow YAML syntax
4. Check file paths and contexts