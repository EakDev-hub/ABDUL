# GitHub Secrets Setup Guide

This document contains instructions for setting up GitHub Secrets required for the ABDUL deployment workflow.

## Required Secrets

The deployment process requires the following GitHub Secrets to be configured in your repository:

### 1. EC2_HOST

**Description:** The public IP address or hostname of your EC2 instance.

**Example:**
- `ec2-54-123-45-67.compute-1.amazonaws.com`
- `54.123.45.67`

**How to get it:**
1. Go to AWS EC2 Console
2. Select your EC2 instance
3. Copy the "Public IPv4 address" or "Public IPv4 DNS"

---

### 2. EC2_USERNAME

**Description:** The SSH username for your EC2 instance.

**Common values:**
- `ubuntu` (for Ubuntu AMI)
- `ec2-user` (for Amazon Linux)
- `admin` (for Debian)

**Example:** `ubuntu`

---

### 3. EC2_SSH_PRIVATE_KEY

**Description:** Your SSH private key for accessing the EC2 instance. Supports both PuTTY (.ppk) and OpenSSH (.pem) formats.

**Supported Formats:**
- PuTTY .ppk format (recommended if you're already using it)
- OpenSSH .pem format

**How to get it:**

#### If you have a .ppk file (PuTTY format):
1. Open your `.ppk` file in a text editor
2. Copy the entire content including all headers and footers:
   ```
   PuTTY-User-Key-File-2: ssh-rsa
   Encryption: aes256-cbc
   ... (all key content) ...
   ```
3. Paste into the GitHub Secret

#### If you have a .pem file (OpenSSH format):
1. Open your `.pem` file in a text editor
2. Copy the entire content including the header and footer:
   ```
   -----BEGIN RSA PRIVATE KEY-----
   ... (key content) ...
   -----END RSA PRIVATE KEY-----
   ```
3. Paste into the GitHub Secret

**Important:**
- Both .ppk and .pem formats are supported (no conversion needed)
- Include the entire key content with all headers/footers
- Keep this secret secure and never commit it to your repository
- Ensure proper line breaks are preserved

---

### 4. EC2_SSH_PASSPHRASE

**Description:** The passphrase for your SSH private key (if your key is encrypted).

**Format:** Plain text string

**How to get it:**
- This is the passphrase you created when generating your SSH key pair
- If your key is not encrypted, you can skip this secret or leave it empty

**Example:** `MySecurePassphrase123!`

**Important:**
- Only needed if your SSH private key is encrypted with a passphrase
- Keep this secret secure
- If you don't have a passphrase on your key, this secret is optional

---

### 5. GITHUB_PAT

**Description:** GitHub Personal Access Token for cloning the repository on EC2.

**Required Scopes:**
- `repo` (Full control of private repositories)

**How to create:**

1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click **"Generate new token (classic)"**
3. Give it a descriptive name (e.g., "ABDUL EC2 Deployment")
4. Set expiration (recommended: 90 days)
5. Select scopes:
   - ✅ `repo` (Full control of private repositories)
6. Click **"Generate token"**
7. **Copy the token immediately** (you won't see it again!)
8. Add it as a GitHub Secret named `GITHUB_PAT`

**Important:**
- Store this token securely
- If using a public repository, this token is still needed for the deployment script
- Rotate this token regularly for security
- If the token expires, you'll need to generate a new one

---

### 6. GITHUB_REPO

**Description:** Your GitHub repository URL (without https://).

**Format:** `github.com/username/repository.git`

**Examples:**
- `github.com/apisitthaosri/ABDUL.git`
- `github.com/yourorg/your-repo.git`

**How to get it:**
1. Go to your GitHub repository
2. Click the green **"Code"** button
3. Copy the HTTPS URL (e.g., `https://github.com/user/repo.git`)
4. Remove the `https://` prefix
5. The result should be: `github.com/user/repo.git`

---

### 7. BACKEND_ENV

**Description:** Environment variables for the backend .NET service.

**Format:** Multi-line string with KEY=VALUE pairs

**Example:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft.AspNetCore=Warning
AllowedHosts=*
OPENROUTER_API_KEY=sk-or-v1-xxxxxxxxxxxxxxxxxxxxx
TEAM_API_BASE_URL=https://your-api-endpoint.com
TEAM_API_PASS_KEY=your-passkey-here
```

**Reference:** See `backend/.env.example` for all available options

**Important:**
- Do not include quotes around values
- Each variable should be on its own line
- Sensitive values (API keys) should only be stored here, never in code

---

### 8. NAMEK_ENV

**Description:** Environment variables for the Namek frontend service.

**Format:** Multi-line string with KEY=VALUE pairs

**Example:**
```
NODE_ENV=production
VITE_API_GATEWAY_URL=http://54.123.45.67:5000
VITE_APP_VERSION=1.0.0
DOCKER_PORT=3000
```

**Reference:** See `namek/.env.example` for all available options

**Important:**
- `VITE_API_GATEWAY_URL` must be accessible from users' browsers
- If EC2 is behind a load balancer, use the load balancer URL
- Use your EC2's public IP or domain name for `VITE_API_GATEWAY_URL`

---

### 9. API_GATEWAY_URL

**Description:** The backend API URL used during the Docker build process.

**Format:** Full URL including protocol

**Examples:**
- `http://localhost:5000` (for internal communication)
- `http://54.123.45.67:5000` (for external access)
- `https://api.yourdomain.com` (for production with domain)

**Important:**
- This is used as a build argument for the Namek Docker image
- Can be different from `VITE_API_GATEWAY_URL` in `NAMEK_ENV`
- If unsure, use `http://localhost:5000`

---

## How to Add Secrets to GitHub

1. Go to your GitHub repository
2. Navigate to **Settings**
3. In the left sidebar, click **Secrets and variables** → **Actions**
4. Click **New repository secret**
5. Enter the **Name** (must match exactly as listed above)
6. Enter the **Value** (paste the content)
7. Click **Add secret**
8. Repeat for all required secrets

---

## Secrets Summary Table

| Secret Name | Type | Required | Description |
|-------------|------|----------|-------------|
| `EC2_HOST` | String | ✅ Yes | EC2 instance IP or hostname |
| `EC2_USERNAME` | String | ✅ Yes | SSH username (ubuntu/ec2-user) |
| `EC2_SSH_PRIVATE_KEY` | Secret | ✅ Yes | SSH private key (.ppk or .pem format) |
| `EC2_SSH_PASSPHRASE` | Secret | ⚠️ If encrypted | Passphrase for encrypted SSH key |
| `GITHUB_PAT` | Secret | ✅ Yes | GitHub Personal Access Token |
| `GITHUB_REPO` | String | ✅ Yes | Repository URL (github.com/user/repo.git) |
| `BACKEND_ENV` | Secret | ✅ Yes | Backend environment variables |
| `NAMEK_ENV` | Secret | ✅ Yes | Frontend environment variables |
| `API_GATEWAY_URL` | String | ✅ Yes | API URL for Docker build |

---

## Verification

After adding all secrets, verify they are set correctly:

1. Go to **Settings** → **Secrets and variables** → **Actions**
2. You should see all 9 secrets listed (or 8 if SSH key has no passphrase)
3. Click on **Actions** tab
4. Manually trigger the workflow using **Run workflow**
5. Monitor the workflow execution for any errors

---

## Troubleshooting

### Error: Permission denied (publickey)
- **Check:** `EC2_SSH_PRIVATE_KEY` contains the complete key file (.ppk or .pem)
- **Check:** If using encrypted key, `EC2_SSH_PASSPHRASE` is correct
- **Check:** The key matches the one configured in EC2 instance
- **Check:** `EC2_USERNAME` is correct for your AMI

### Error: Repository not found or authentication failed
- **Check:** `GITHUB_PAT` has `repo` scope
- **Check:** `GITHUB_REPO` format is correct (github.com/user/repo.git)
- **Check:** Token hasn't expired

### Error: Docker build failed
- **Check:** `BACKEND_ENV` and `NAMEK_ENV` have correct syntax
- **Check:** `API_GATEWAY_URL` is a valid URL
- **Check:** All required environment variables are present

### Error: Container failed to start
- **Check:** Port 5000 and 3000 are not already in use on EC2
- **Check:** Environment variables in `BACKEND_ENV` and `NAMEK_ENV` are valid
- **Check:** Docker has sufficient resources on EC2

---

## Security Best Practices

1. **Rotate Secrets Regularly**
   - Update `GITHUB_PAT` every 90 days
   - Rotate SSH keys annually
   - Update API keys when team members change

2. **Limit Access**
   - Only give repository admin access to trusted team members
   - Use the principle of least privilege for GitHub PAT scopes

3. **Monitor Usage**
   - Review GitHub Actions logs regularly
   - Check for failed authentication attempts
   - Monitor EC2 access logs

4. **Backup**
   - Keep a secure backup of your SSH keys
   - Document all secret values in a secure password manager
   - Don't store secrets in code or commit history

---

## Support

If you encounter issues with secret configuration:
1. Check this documentation thoroughly
2. Review the GitHub Actions workflow logs
3. Check EC2 system logs: `tail -f /var/log/abdul-deployment.log`
4. Verify EC2 security group allows SSH (port 22)

For more information, see [DEPLOYMENT.md](./DEPLOYMENT.md)