# ABDUL Deployment Guide

This guide explains how to deploy the ABDUL project (Backend + Namek Frontend) to AWS EC2 using GitHub Actions.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [GitHub Secrets Configuration](#github-secrets-configuration)
4. [EC2 Setup](#ec2-setup)
5. [Deployment Process](#deployment-process)
6. [Monitoring & Troubleshooting](#monitoring--troubleshooting)
7. [Rollback Procedures](#rollback-procedures)

---

## Architecture Overview

The deployment uses the following architecture:

```
GitHub Repository
    ↓
GitHub Actions Workflow
    ├─ Transfer docker-compose config
    ├─ Transfer deployment script
    └─ Execute deployment via SSH
         ↓
    EC2 Instance
    ├─ Git clone/pull repository (using GitHub PAT)
    ├─ Build Backend Docker Image (C#)
    ├─ Build Namek Docker Image (Node.js)
    ├─ Run docker-compose
    └─ Start Services
         ├─ Backend API (port 5000)
         └─ Namek Frontend (port 3000)
```

### Key Features

- **Remote Build**: Docker images are built directly on EC2 (no registry needed)
- **Git-based Deployment**: Always builds from latest git commit
- **Faster Deployment**: No large image transfers, only code changes via git
- **Build Caching**: Docker layer caching on EC2 speeds up rebuilds
- **Secure Transfer**: SSH/SCP for secure file transfer to EC2
- **Atomic Deployment**: Both services deployed together using docker-compose
- **Health Checks**: Automatic verification of service health
- **Environment Management**: Secrets handled securely via GitHub Secrets

---

## Prerequisites

### GitHub Repository Requirements

- Repository must be on GitHub
- GitHub Actions must be enabled
- You need write access to configure secrets

### AWS EC2 Instance Requirements

- **OS**: Ubuntu 20.04 LTS or later (or Amazon Linux 2)
- **Instance Type**: t3.small or larger recommended for building (t3.micro may be too slow)
- **Storage**: At least 30GB free disk space (for git repo + Docker builds)
- **Security Group**: Ports 5000, 3000, and 22 open for inbound traffic
- **Software**:
  - Docker installed and running
  - Docker Compose installed
  - Git installed
  - SSH access configured
  - curl installed (for health checks)
- **Network**: Outbound internet access to GitHub and Docker Hub

### Local Machine Requirements

- SSH key pair for EC2 access
- Git installed
- (Optional) Docker installed for local testing

---

## GitHub Secrets Configuration

You need to configure the following secrets in your GitHub repository:

### Steps to Add Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add each secret below

### Required Secrets

#### 1. `EC2_HOST`
- **Description**: Public IP address or hostname of your EC2 instance
- **Example**: `ec2-54-123-45-67.compute-1.amazonaws.com` or `54.123.45.67`
- **Type**: String

#### 2. `EC2_USERNAME`
- **Description**: SSH username for EC2 instance
- **Example**: `ubuntu` (for Ubuntu AMI) or `ec2-user` (for Amazon Linux)
- **Type**: String

#### 3. `EC2_SSH_PRIVATE_KEY`
- **Description**: Private SSH key for EC2 access (supports both PuTTY .ppk and OpenSSH .pem formats)
- **How to get it**:
  1. Locate your EC2 key pair file (e.g., `my-key.pem` or `my-key.ppk`)
  2. Open it in a text editor
  3. Copy the entire content (including all headers and footers)
  4. Paste into this secret
- **Type**: Secret (multi-line)
- **Format**: Both .ppk (PuTTY) and .pem (OpenSSH) formats are supported
- **Important**: Keep this secure and never commit to repository

#### 4. `EC2_SSH_PASSPHRASE` (Optional)
- **Description**: Passphrase for your SSH private key (if encrypted)
- **Example**: Your key passphrase
- **Type**: Secret
- **Required**: Only if your SSH key is encrypted with a passphrase

#### 5. `PAT_GITHUB`
- **Description**: GitHub Personal Access Token for cloning repository on EC2
- **Note**: GitHub reserves the `GITHUB_` prefix for system secrets, so we use `PAT_GITHUB`
- **How to get it**:
  1. Go to GitHub Settings → Developer settings → Personal access tokens
  2. Generate new token (classic)
  3. Select scope: `repo` (Full control of private repositories)
  4. Copy the token immediately
- **Type**: Secret
- **Important**: This token allows read access to your repository

#### 6. `GITHUB_REPO`
- **Description**: GitHub repository URL without https://
- **Example**: `github.com/username/ABDUL.git`
- **Type**: String
- **Format**: `github.com/username/repository.git`

#### 7. `BACKEND_ENV`
- **Description**: Environment variables for the backend service
- **Example**:
  ```
  ASPNETCORE_ENVIRONMENT=Production
  ASPNETCORE_URLS=http://+:5000
  Logging__LogLevel__Default=Information
  AllowedHosts=*
  OPENROUTER_API_KEY=your_api_key_here
  ```
- **Type**: Secret (multi-line)
- **Reference**: See `backend/.env.example`

#### 8. `NAMEK_ENV`
- **Description**: Environment variables for the Namek frontend service
- **Example**:
  ```
  NODE_ENV=production
  VITE_API_GATEWAY_URL=http://your-ec2-ip:5000
  VITE_APP_VERSION=1.0.0
  ```
- **Type**: Secret (multi-line)
- **Reference**: See `namek/.env.example`
- **Important**: `VITE_API_GATEWAY_URL` must be accessible from the browser

#### 9. `API_GATEWAY_URL`
- **Description**: Backend API URL for Namek to communicate with backend
- **Example**: `http://localhost:5000` or `http://your-ec2-ip:5000`
- **Type**: String
- **Note**: Used during Namek Docker build

### Example Secret Configuration

```yaml
EC2_HOST: ec2-54-123-45-67.compute-1.amazonaws.com
EC2_USERNAME: ubuntu
EC2_SSH_PRIVATE_KEY: |
  -----BEGIN RSA PRIVATE KEY-----
  MIIEpAIBAAKCAQEA2x5q...
  ... (rest of key content)
  -----END RSA PRIVATE KEY-----
PAT_GITHUB: ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
GITHUB_REPO: github.com/username/ABDUL.git
BACKEND_ENV: |
  ASPNETCORE_ENVIRONMENT=Production
  ASPNETCORE_URLS=http://+:5000
  Logging__LogLevel__Default=Information
  AllowedHosts=*
NAMEK_ENV: |
  NODE_ENV=production
  VITE_API_GATEWAY_URL=http://54.123.45.67:5000
  VITE_APP_VERSION=1.0.0
API_GATEWAY_URL: http://localhost:5000
```

---

## EC2 Setup

### 1. Launch EC2 Instance

```bash
# Using AWS CLI
aws ec2 run-instances \
  --image-id ami-0c55b159cbfafe1f0 \
  --instance-type t3.small \
  --key-name your-key-pair \
  --security-groups default \
  --region us-east-1
```

### 2. Connect to EC2

```bash
ssh -i /path/to/your-key.pem ubuntu@your-ec2-ip
```

### 3. Install Docker and Git

```bash
# Update system
sudo apt-get update
sudo apt-get upgrade -y

# Install Docker and Git
sudo apt-get install -y docker.io git

# Start Docker service
sudo systemctl start docker
sudo systemctl enable docker

# Add current user to docker group (optional, for sudo-less commands)
sudo usermod -aG docker $USER
newgrp docker

# Verify Git installation
git --version
```

### 4. Install Docker Compose

```bash
# Download Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# Make it executable
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker-compose --version
```

### 5. Configure Security Group

In AWS Console:
1. Go to EC2 → Security Groups
2. Select your instance's security group
3. Add inbound rules:
   - **Port 5000** (Backend API) - Source: 0.0.0.0/0 or your IP
   - **Port 3000** (Namek Frontend) - Source: 0.0.0.0/0 or your IP
   - **Port 22** (SSH) - Source: Your IP (already configured)

### 6. Create Deployment Directory

```bash
sudo mkdir -p /opt/abdul
sudo chown $USER:$USER /opt/abdul
```

---

## Deployment Process

### Automatic Deployment (Recommended)

Deployment is triggered automatically when you push to the `main` branch:

```bash
git add .
git commit -m "Deploy to EC2"
git push origin main
```

### Manual Deployment

To manually trigger deployment:

1. Go to your GitHub repository
2. Click **Actions** tab
3. Select **Build and Deploy to EC2** workflow
4. Click **Run workflow** → **Run workflow**

### Deployment Workflow Steps

The GitHub Actions workflow performs these steps:

1. **Checkout Code**: Pulls the latest code from the repository (to get deployment scripts)
2. **Setup SSH**: Configures SSH connection to EC2 using OpenSSH key
3. **Transfer Files**: Copies docker-compose and deployment script to EC2
4. **Create .env**: Sets up environment variables on EC2
5. **Execute Deployment**: Runs deployment script on EC2 which:
   - Clones/pulls the latest code from GitHub using PAT
   - Builds Docker images for backend and frontend
   - Stops old containers
   - Starts new containers with docker-compose
6. **Verify Deployment**: Checks if services are running and healthy
7. **Cleanup**: Removes SSH keys and temporary files

### Monitoring Deployment

1. Go to **Actions** tab in GitHub
2. Click on the running workflow
3. View real-time logs of each step
4. Check for any errors or warnings

---

## Monitoring & Troubleshooting

### Check Service Status

```bash
# SSH into EC2
ssh -i /path/to/your-key.pem ubuntu@your-ec2-ip

# View running containers
docker ps

# View container logs
docker logs abdul-backend
docker logs abdul-namek

# View docker-compose status
cd /opt/abdul
docker-compose ps
```

### Common Issues

#### 1. SSH Connection Failed

**Error**: `Permission denied (publickey)`

**Solution**:
```bash
# Check SSH key permissions
chmod 600 ~/.ssh/your-key.pem

# Verify EC2 security group allows SSH (port 22)
# Verify EC2_SSH_PRIVATE_KEY secret is correctly formatted
```

#### 2. Git Clone Failed

**Error**: `Failed to clone repository` or `Authentication failed`

**Solution**:
```bash
# Check if git is installed
git --version

# Verify GitHub PAT has correct permissions
# Ensure GITHUB_REPO format is correct (github.com/user/repo.git)

# Test git clone manually on EC2
git clone https://YOUR_PAT@github.com/user/repo.git /tmp/test-clone
```

#### 3. Docker Build Failed

**Error**: `Failed to build backend image` or `Failed to build namek image`

**Solution**:
```bash
# Check disk space
df -h

# Check Docker daemon status
sudo systemctl status docker

# View build logs
docker logs abdul-backend
docker logs abdul-namek

# Restart Docker if needed
sudo systemctl restart docker
```

#### 4. Containers Not Starting

**Error**: `Container exited with code 1`

**Solution**:
```bash
# Check container logs
docker logs abdul-backend

# Verify environment variables
docker inspect abdul-backend | grep -A 20 "Env"

# Check port conflicts
sudo netstat -tlnp | grep 5000
sudo netstat -tlnp | grep 3000
```

#### 5. Health Check Failures

**Error**: `Health check failed`

**Solution**:
```bash
# Test backend connectivity
curl http://localhost:5000/health

# Test namek connectivity
curl http://localhost:3000/

# Check container logs for errors
docker logs abdul-backend --tail 50
docker logs abdul-namek --tail 50
```

#### 6. Out of Disk Space

**Error**: `No space left on device`

**Solution**:
```bash
# Clean up old Docker images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove old git clones if any
rm -rf /tmp/test-clone

# Check disk usage
du -sh /var/lib/docker/
du -sh /opt/abdul/

# Increase EBS volume size (AWS Console)
```

### View Deployment Logs

```bash
# On EC2 instance
tail -f /var/log/abdul-deployment.log

# Or view last deployment
cat /var/log/abdul-deployment.log
```

---

## Rollback Procedures

### Quick Rollback (Last 24 Hours)

If deployment fails or causes issues:

```bash
# SSH into EC2
ssh -i /path/to/your-key.pem ubuntu@your-ec2-ip

# Stop current containers
cd /opt/abdul
docker-compose down

# List available images
docker images | grep abdul

# Restart with previous image (if available)
docker-compose up -d
```

### Full Rollback

If you need to revert to a previous version:

1. **Identify Previous Commit**:
   ```bash
   git log --oneline | head -10
   ```

2. **Checkout Previous Version**:
   ```bash
   git checkout <commit-hash>
   ```

3. **Push to Trigger Redeployment**:
   ```bash
   git push origin main --force-with-lease
   ```

4. **Monitor Deployment**:
   - Go to GitHub Actions
   - Watch the workflow complete

### Manual Rollback on EC2

```bash
# SSH into EC2
ssh -i /path/to/your-key.pem ubuntu@your-ec2-ip

# Stop current containers
cd /opt/abdul
docker-compose down

# Remove current images
docker rmi abdul-backend:latest
docker rmi abdul-namek:latest

# Manually load previous images (if backed up)
docker load -i /backup/backend-image-v1.tar
docker load -i /backup/namek-image-v1.tar

# Start containers
docker-compose up -d
```

---

## Performance Optimization

### Reduce Build Time

1. **Use Docker layer caching**: Images are built on EC2, subsequent builds use cache
2. **Optimize Dockerfiles**: Ensure Dockerfiles are optimized for layer caching
3. **Smaller base images**: Consider using alpine variants
4. **Faster EC2 Instance**: Use t3.medium or larger for faster builds

### Reduce Clone Time

1. **Shallow clone**: Use git shallow clone if repository is large
2. **Optimize network**: Ensure EC2 has good internet connectivity
3. **Git cache**: Subsequent deployments use `git pull` which is much faster

### Monitor Resource Usage

```bash
# Check CPU and memory usage
docker stats

# Check disk usage
df -h
du -sh /var/lib/docker/

# Check network bandwidth
iftop
```

---

## Security Best Practices

1. **SSH Key Management**:
   - Never commit SSH keys to repository
   - Rotate keys regularly
   - Use strong key passphrases

2. **Environment Variables**:
   - Use GitHub Secrets for sensitive data
   - Never log secrets in deployment scripts
   - Rotate API keys periodically

3. **EC2 Security**:
   - Restrict Security Group to necessary ports
   - Use VPC for additional isolation
   - Enable CloudWatch monitoring
   - Regular security updates: `sudo apt-get update && sudo apt-get upgrade`

4. **Docker Security**:
   - Use specific image versions (not `latest`)
   - Scan images for vulnerabilities
   - Run containers as non-root user
   - Use read-only filesystems where possible

---

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [AWS EC2 Documentation](https://docs.aws.amazon.com/ec2/)
- [SSH Key Pair Documentation](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ec2-key-pairs.html)

---

## Support & Troubleshooting

For issues or questions:

1. Check the deployment logs in GitHub Actions
2. Review EC2 instance logs: `/var/log/abdul-deployment.log`
3. Check Docker container logs: `docker logs <container-name>`
4. Verify all GitHub Secrets are correctly configured
5. Ensure EC2 instance has sufficient resources

---

**Last Updated**: 2025-12-01  
**Version**: 1.0.0