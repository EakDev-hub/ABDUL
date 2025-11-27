# GitHub Actions Quick Start

## 🚀 Get Started in 5 Minutes

### Step 1: Create Docker Hub Account
- Go to [Docker Hub](https://hub.docker.com)
- Sign up (free)
- Create a repository named `dashboard`

### Step 2: Generate Access Token
1. Log in to Docker Hub
2. Account Settings → Security
3. New Access Token → Name: `github-actions`
4. Copy the token

### Step 3: Add GitHub Secrets
1. Go to GitHub repository
2. Settings → Secrets and variables → Actions
3. Add two secrets:
   - `DOCKER_USERNAME`: Your Docker Hub username
   - `DOCKER_PASSWORD`: Your access token

### Step 4: Push Code
```bash
git add .
git commit -m "Add GitHub Actions workflows"
git push origin feature/dashboard-vue-docker
```

### Step 5: Watch It Build
1. Go to GitHub → Actions tab
2. Watch the workflows run
3. Check Docker Hub for your image

## 📋 Available Workflows

### 1. Build and Push to Docker Hub
**File**: `.github/workflows/docker-build-push.yml`

**Triggers**:
- Push to `main` or `feature/**` branches
- Pull requests to `main`

**What it does**:
- Builds Docker image
- Pushes to Docker Hub
- Auto-tags with branch/version

**Image location**:
```
docker.io/YOUR_USERNAME/dashboard:TAG
```

### 2. CI - Build and Test
**File**: `.github/workflows/ci.yml`

**Triggers**:
- Push to `main` or `feature/**` branches
- Pull requests to `main`

**What it does**:
- Installs dependencies
- Type checks with TypeScript
- Builds production bundle
- Tests Docker images
- Checks security vulnerabilities

## 🔍 Monitor Your Builds

### GitHub Actions Dashboard
```
Repository → Actions → View workflow runs
```

### Docker Hub Dashboard
```
Docker Hub → Your Repository → Tags
```

## 📦 Pull Your Image

```bash
# Latest version
docker pull YOUR_USERNAME/dashboard:latest

# Specific branch
docker pull YOUR_USERNAME/dashboard:main

# Run it
docker run -p 80:80 YOUR_USERNAME/dashboard:latest
```

## 🐛 Troubleshooting

### Workflow Not Running?
- Check branch name matches trigger
- Verify files in `dashboard/` were changed
- Check workflow YAML syntax

### Build Fails?
- Check GitHub Actions logs
- Verify Docker Hub credentials
- Check Dockerfile syntax

### Image Not Pushed?
- Verify secrets are set correctly
- Check Docker Hub credentials
- Review workflow logs

## 📚 Full Documentation

See [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) for detailed setup and advanced configuration.

## ✅ Checklist

- [ ] Docker Hub account created
- [ ] Access token generated
- [ ] GitHub secrets added
- [ ] Code pushed to GitHub
- [ ] Workflows running successfully
- [ ] Image visible on Docker Hub
- [ ] Image can be pulled locally

## 🎯 Next Steps

1. **Set up secrets** (Step 3 above)
2. **Push code** to trigger workflows
3. **Monitor** Actions tab
4. **Deploy** using the image

---

**Need help?** Check the full [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)