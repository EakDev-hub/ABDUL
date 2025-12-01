# GitHub Secrets Setup Guide

This guide provides step-by-step instructions for configuring GitHub Secrets required for automated deployment to AWS EC2.

## Quick Reference

| Secret Name | Required | Type | Example |
|-------------|----------|------|---------|
| `EC2_HOST` | ✅ Yes | String | `ec2-54-123-45-67.compute-1.amazonaws.com` |
| `EC2_USERNAME` | ✅ Yes | String | `ubuntu` |
| `EC2_SSH_PRIVATE_KEY` | ✅ Yes | Secret | (SSH private key content) |
| `BACKEND_ENV` | ✅ Yes | Secret | (Environment variables) |
| `NAMEK_ENV` | ✅ Yes | Secret | (Environment variables) |
| `API_GATEWAY_URL` | ⚠️ Optional | String | `http://localhost:5000` |

---

## Step-by-Step Setup

### 1. Access GitHub Secrets Settings

1. Go to your GitHub repository
2. Click **Settings** (top navigation)
3. In the left sidebar, click **Secrets and variables** → **Actions**
4. You should see "Repository secrets" section

### 2. Add `EC2_HOST` Secret

**Purpose**: Public IP or hostname of your EC2 instance

**Steps**:
1. Click **New repository secret**
2. **Name**: `EC2_HOST`
3. **Value**: Your EC2 instance's public IP or hostname
   - Example: `54.123.45.67`
   - Or: `ec2-54-123-45-67.compute-1.amazonaws.com`
4. Click **Add secret**

**How to find your EC2 Host**:
- AWS Console → EC2 → Instances
- Look for "Public IPv4 address" or "Public IPv4 DNS"

### 3. Add `EC2_USERNAME` Secret

**Purpose**: SSH username for your EC2 instance

**Steps**:
1. Click **New repository secret**
2. **Name**: `EC2_USERNAME`
3. **Value**: SSH username
   - For Ubuntu AMI: `ubuntu`
   - For Amazon Linux 2: `ec2-user`
   - For other OS: check your AMI documentation
4. Click **Add secret**

### 4. Add `EC2_SSH_PRIVATE_KEY` Secret

**Purpose**: Private SSH key for EC2 authentication

**⚠️ IMPORTANT**: This is sensitive data. Handle with care!

**Steps**:

1. **Locate your SSH key file**:
   - Usually in `~/.ssh/` directory
   - File name like `my-key.pem` or `my-key.ppk`
   - This is the file you downloaded when creating the EC2 key pair

2. **Open the key file in a text editor**:
   ```bash
   cat ~/.ssh/my-key.pem
   ```
   
   Or on Windows:
   ```powershell
   Get-Content C:\Users\YourUsername\.ssh\my-key.pem
   ```

3. **Copy the entire content**:
   - Include the `-----BEGIN RSA PRIVATE KEY-----` line
   - Include the `-----END RSA PRIVATE KEY-----` line
   - Include all lines in between

4. **Add to GitHub**:
   - Click **New repository secret**
   - **Name**: `EC2_SSH_PRIVATE_KEY`
   - **Value**: Paste the entire key content
   - Click **Add secret**

**Example SSH Key Format**:
```
-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA2x5q7vZ8kL9mN2pQ3rS4tU5vW6xY7zA8bC9dE0fG1hI2jK3l
M4nO5pQ6rS7tU8vW9xY0zA1bC2dE3fG4hI5jK6lM7nO8pQ9rS0tU1vW2xY3zA4bC5
... (more key content)
-----END RSA PRIVATE KEY-----
```

### 5. Add `BACKEND_ENV` Secret

**Purpose**: Environment variables for the C# backend service

**Steps**:

1. **Prepare environment variables**:
   - Reference: `backend/.env.example`
   - Customize for your production environment

2. **Example content**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:5000
   Logging__LogLevel__Default=Information
   Logging__LogLevel__Microsoft.AspNetCore=Warning
   AllowedHosts=*
   OPENROUTER_API_KEY=sk-or-v1-xxxxxxxxxxxxx
   TEAM_API_URL=https://api.example.com
   TEAM_API_KEY=your_team_api_key
   ```

3. **Add to GitHub**:
   - Click **New repository secret**
   - **Name**: `BACKEND_ENV`
   - **Value**: Paste all environment variables (multi-line)
   - Click **Add secret**

**Common Backend Variables**:
- `ASPNETCORE_ENVIRONMENT`: Set to `Production`
- `ASPNETCORE_URLS`: Set to `http://+:5000`
- `Logging__LogLevel__Default`: Set to `Information`
- API keys and credentials for external services
- Database connection strings (if applicable)

### 6. Add `NAMEK_ENV` Secret

**Purpose**: Environment variables for the Node.js frontend service

**Steps**:

1. **Prepare environment variables**:
   - Reference: `namek/.env.example`
   - Customize for your production environment

2. **Example content**:
   ```
   NODE_ENV=production
   VITE_API_GATEWAY_URL=http://54.123.45.67:5000
   VITE_APP_VERSION=1.0.0
   VITE_APP_TITLE=ABDUL - Hackathon Platform
   ```

3. **Add to GitHub**:
   - Click **New repository secret**
   - **Name**: `NAMEK_ENV`
   - **Value**: Paste all environment variables (multi-line)
   - Click **Add secret**

**Important Variables**:
- `NODE_ENV`: Set to `production`
- `VITE_API_GATEWAY_URL`: **Must be accessible from browser**
  - Use EC2 public IP: `http://54.123.45.67:5000`
  - Or use domain name: `https://api.yourdomain.com`
  - NOT `http://localhost:5000` (won't work from browser)

### 7. Add `API_GATEWAY_URL` Secret (Optional)

**Purpose**: Backend API URL used during Namek Docker build

**Steps**:

1. Click **New repository secret**
2. **Name**: `API_GATEWAY_URL`
3. **Value**: Backend API URL
   - Example: `http://localhost:5000`
   - Or: `http://54.123.45.67:5000`
4. Click **Add secret**

---

## Verification Checklist

After adding all secrets, verify they're correctly configured:

- [ ] `EC2_HOST` is set to your EC2 public IP or hostname
- [ ] `EC2_USERNAME` matches your EC2 AMI (ubuntu, ec2-user, etc.)
- [ ] `EC2_SSH_PRIVATE_KEY` contains the full private key (with BEGIN/END lines)
- [ ] `BACKEND_ENV` contains all required backend environment variables
- [ ] `NAMEK_ENV` contains all required frontend environment variables
- [ ] `VITE_API_GATEWAY_URL` in `NAMEK_ENV` is accessible from browser
- [ ] No secrets are committed to the repository
- [ ] All secrets are marked as "Secret" type (not visible in logs)

---

## Testing Secrets

### Test SSH Connection

After adding secrets, test if GitHub Actions can connect to EC2:

1. Go to **Actions** tab
2. Select **Build and Deploy to EC2** workflow
3. Click **Run workflow** → **Run workflow**
4. Watch the "Setup SSH key" step
5. If it succeeds, your SSH secrets are correct

### Test Environment Variables

Check if environment variables are correctly passed:

1. SSH into EC2:
   ```bash
   ssh -i /path/to/key.pem ubuntu@your-ec2-ip
   ```

2. Check container environment:
   ```bash
   docker inspect abdul-backend | grep -A 20 "Env"
   docker inspect abdul-namek | grep -A 20 "Env"
   ```

3. Verify variables are set correctly

---

## Troubleshooting

### "Permission denied (publickey)"

**Cause**: SSH key is incorrect or not properly formatted

**Solution**:
1. Verify the private key file is correct
2. Check that you copied the entire key (including BEGIN/END lines)
3. Ensure no extra spaces or line breaks were added
4. Try the key locally first: `ssh -i key.pem ubuntu@your-ec2-ip`

### "Host key verification failed"

**Cause**: EC2 host not in known_hosts

**Solution**:
- The workflow automatically handles this with `ssh-keyscan`
- If it fails, manually add the host:
  ```bash
  ssh-keyscan -H your-ec2-ip >> ~/.ssh/known_hosts
  ```

### "Secrets not available in workflow"

**Cause**: Secrets not properly saved or wrong repository

**Solution**:
1. Verify you're in the correct repository
2. Check that secret names match exactly (case-sensitive)
3. Try re-adding the secret
4. Wait a few minutes for GitHub to sync

### "Environment variables not set in container"

**Cause**: `BACKEND_ENV` or `NAMEK_ENV` not properly formatted

**Solution**:
1. Verify each line is a valid environment variable: `KEY=VALUE`
2. No extra spaces or special characters
3. Multi-line format should have each variable on a new line
4. Check deployment logs: `/var/log/abdul-deployment.log`

### "API Gateway URL not accessible from browser"

**Cause**: Using `localhost` or internal IP in `VITE_API_GATEWAY_URL`

**Solution**:
1. Use EC2 public IP: `http://54.123.45.67:5000`
2. Or use domain name: `https://api.yourdomain.com`
3. Ensure port 5000 is open in Security Group
4. Test from browser: `http://your-ec2-ip:5000/health`

---

## Security Best Practices

1. **Never commit secrets to repository**:
   - Use `.gitignore` for `.env` files
   - Always use GitHub Secrets for sensitive data

2. **Rotate secrets regularly**:
   - Change API keys every 90 days
   - Rotate SSH keys annually
   - Update database passwords periodically

3. **Limit secret access**:
   - Only share secrets with team members who need them
   - Use branch protection rules
   - Require code reviews before deployment

4. **Monitor secret usage**:
   - Check GitHub Actions logs for errors
   - Review deployment history
   - Set up alerts for failed deployments

5. **SSH Key Management**:
   - Keep private keys secure
   - Never share private keys
   - Use strong passphrases
   - Disable old keys when no longer needed

---

## Reference Files

- **Backend Environment**: `backend/.env.example`
- **Frontend Environment**: `namek/.env.example`
- **Deployment Guide**: `DEPLOYMENT.md`
- **GitHub Actions Workflow**: `.github/workflows/deploy-to-ec2.yml`

---

## Next Steps

1. ✅ Add all required secrets
2. ✅ Verify secrets are correctly configured
3. ✅ Test SSH connection via GitHub Actions
4. ✅ Monitor first deployment
5. ✅ Verify services are running on EC2

---

**Last Updated**: 2025-12-01  
**Version**: 1.0.0