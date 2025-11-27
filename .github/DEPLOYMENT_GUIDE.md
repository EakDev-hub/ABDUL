# GitHub Actions Deployment Guide

## Overview

This project includes automated CI/CD workflows using GitHub Actions to build and push Docker images to GitHub Container Registry (GHCR).

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
   - Logs into GitHub Container Registry (GHCR)
   - Builds production Docker image
   - Pushes image to GHCR with automatic tagging
   - Caches layers for faster builds
   - Tests development image on PRs

3. **Image Tagging**:
   - `latest` - Latest version on main branch
   - `main` - Latest from main branch
   - `feature/branch-name` - Feature branch builds
   - `v1.0.0` - Semantic version tags
   - `sha-abc123def` - Git commit SHA

## Setup Instructions

### 1. GitHub Container Registry (GHCR) Setup

GHCR is automatically available with your GitHub account. No additional setup is required!

**Key Points**:
- Uses your GitHub account for authentication
- Automatically available in all repositories
- No separate credentials needed
- Images are private by default

### 2. Verify Repository Settings

1. Go to your GitHub repository
2. Settings → Actions → General
3. Ensure "Read and write permissions" is enabled for workflows
4. This allows the workflow to push images to GHCR

### 3. Verify Workflow

1. Push code to `main` or a `feature/` branch
2. Go to GitHub repository → Actions tab
3. Watch the workflow run
4. Check GHCR for the new image (repository → Packages)

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
3. **Login**: Authenticates with GitHub Container Registry using GITHUB_TOKEN
4. **Extract Metadata**: Generates image tags and labels
5. **Build & Push**: Builds and pushes production image
6. **Dev Build**: Tests development image on PRs

### Image Naming

Images are pushed to:
```
ghcr.io/OWNER/REPOSITORY/dashboard:TAG
```

Example tags:
- `ghcr.io/your-username/abdul/dashboard:latest`
- `ghcr.io/your-username/abdul/dashboard:main`
- `ghcr.io/your-username/abdul/dashboard:feature-new-ui`
- `ghcr.io/your-username/abdul/dashboard:v1.0.0`

## Using the Pushed Images

### Authentication

**For Public Images** (if visibility is set to public):
```bash
# No authentication needed
docker pull ghcr.io/your-username/abdul/dashboard:latest
```

**For Private Images** (default):
```bash
# Option 1: Using GitHub Personal Access Token (PAT)
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
docker pull ghcr.io/your-username/abdul/dashboard:latest

# Option 2: Using GitHub CLI
gh auth login
docker pull ghcr.io/your-username/abdul/dashboard:latest
```

### Pull from GitHub Container Registry

```bash
# Latest version
docker pull ghcr.io/your-username/abdul/dashboard:latest

# Specific branch
docker pull ghcr.io/your-username/abdul/dashboard:main

# Specific feature
docker pull ghcr.io/your-username/abdul/dashboard:feature-new-ui

# Run the image
docker run -p 80:80 ghcr.io/your-username/abdul/dashboard:latest
```

### In Docker Compose

```yaml
version: '3.8'
services:
  dashboard:
    image: ghcr.io/your-username/abdul/dashboard:latest
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

### GitHub Container Registry Dashboard

1. Go to your repository main page
2. Click "Packages" on the right sidebar
3. Click on the `dashboard` package
4. View all pushed images and tags
5. Check image details and visibility settings

## Troubleshooting

### Build Fails

**Check logs**:
1. Go to Actions tab
2. Click the failed workflow
3. Expand the failed step
4. Read the error message

**Common issues**:
- Workflow permissions not set correctly
- Dockerfile syntax error
- Missing files in context
- Node.js version incompatibility
- GITHUB_TOKEN not available

### Image Not Pushed

**Verify**:
1. Workflow has `packages: write` permission
2. Branch matches trigger conditions
3. Files in `dashboard/` were changed
4. Workflow file is valid YAML
5. Repository allows package writes

### Slow Builds

**Optimize**:
- GitHub Actions cache is enabled
- First build is slower (builds cache)
- Subsequent builds use cache layers
- Multi-platform builds take longer
- GHCR is on same infrastructure as GitHub Actions

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

### Image Visibility

To change image visibility from private to public:

1. Go to repository → Packages
2. Click on `dashboard` package
3. Click "Package settings"
4. Scroll to "Danger Zone"
5. Change visibility to "Public"

Or use GitHub CLI:
```bash
gh api repos/{owner}/{repo}/packages/container/dashboard/access --input -
```

## Security Best Practices

✅ **Implemented**:
- Uses GitHub's native authentication (GITHUB_TOKEN)
- No external credentials needed
- Automatic token rotation
- Read-only permissions for checkout
- Cache isolation per branch
- Fine-grained access control via GitHub permissions

✅ **Recommended**:
- Enable branch protection rules
- Require PR reviews before merge
- Sign commits with GPG
- Enable GitHub Advanced Security for vulnerability scanning
- Set up image retention policies

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
Auto Login to GHCR
    ↓
Build Docker Image
    ↓
Push to GHCR
    ↓
Complete ✓
```

## Next Steps

1. **Verify workflow** runs successfully on next push
2. **Check Packages** tab for new images
3. **Test image pull** locally if needed
4. **Configure visibility** (public/private) as needed
5. **Deploy** using the pushed image

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [GitHub Container Registry Documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
- [Semantic Versioning](https://semver.org/)
- [GitHub Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)

## Support

For issues:
1. Check GitHub Actions logs
2. Verify workflow permissions are set correctly
3. Review workflow YAML syntax
4. Check file paths and contexts
5. Ensure GITHUB_TOKEN is available in workflow