# Quick Start Guide - Dashboard

## Prerequisites

1. **Backend must be running** on `http://localhost:5000` (or update `.env` with your backend URL)
2. **Node.js 20+** installed
3. **npm** installed

## Steps to Run

### 1. Navigate to Dashboard Directory
```bash
cd dashboard
```

### 2. Install Dependencies
```bash
npm install
```
This will install all required packages including `crypto-js` for environment encryption.

### 3. Configure Backend URL
Edit `.env` file:
```bash
# dashboard/.env
VITE_API_GATEWAY_URL=http://localhost:5000
```

**Important**: Make sure this URL points to your running backend!

### 4. Start Development Server
```bash
npm run dev
```

The dashboard will start on `http://localhost:5173`

### 5. Verify It's Working

Open your browser to `http://localhost:5173` and check:

✅ **Console logs** (F12):
```
🚀 Fetching announcements from API Gateway...
✅ API Response received: {...}
```

✅ **Network tab** shows:
- GET `/get-env` → Status 200 (returns encrypted config)
- GET `http://localhost:5000/announcement` → Status 200
- GET `http://localhost:5000/qna` → Status 200  
- GET `http://localhost:5000/score` → Status 200

## Troubleshooting

### Problem: "Loading announcements..." never finishes

**Solution**:
1. Verify backend is running:
   ```bash
   curl http://localhost:5000/announcement
   ```

2. Check browser console for errors

3. Verify `.env` has correct URL:
   ```bash
   cat .env | grep VITE_API_GATEWAY_URL
   ```

### Problem: CORS errors in console

**Solution**: Backend must allow requests from `http://localhost:5173`

In your backend CORS configuration, add:
- Origin: `http://localhost:5173`
- Methods: `GET, POST, PUT, DELETE`
- Headers: `Content-Type, Authorization`

### Problem: "Failed to fetch env config"

**Solution**:
1. Stop the dev server (Ctrl+C)
2. Clear node_modules:
   ```bash
   rm -rf node_modules package-lock.json
   npm install
   ```
3. Start again:
   ```bash
   npm run dev
   ```

### Problem: Data shows in Network tab but not on screen

**Solution**: Check browser console for JavaScript errors. The data structure might not match the expected format.

Expected response format:
```json
{
  "success": true,
  "data": [...]
}
```

## Environment Variables Explained

### `VITE_API_GATEWAY_URL`
- **Required**: YES
- **Purpose**: Backend API base URL
- **Development**: `http://localhost:5000`
- **Production**: `https://your-backend-url.com`

This URL is:
1. Read by vite server
2. Encrypted and served at `/get-env`
3. Fetched by browser when app loads
4. Used as `baseURL` for all API calls

### How It Works

```
Browser loads app
    ↓
Fetches /get-env (encrypted config)
    ↓
Decrypts environment variables
    ↓
Stores in Pinia store
    ↓
API client uses VITE_API_GATEWAY_URL as baseURL
    ↓
Makes requests to backend
```

## Production Build

### Build for Production
```bash
npm run build
```

This creates optimized files in `dist/` directory.

### Preview Production Build Locally
```bash
npm run preview
```

**Note**: In production preview, `/get-env` endpoint won't work because it requires the dev server. For production deployment, use Docker (see `PRODUCTION_SETUP.md`).

## What Should Happen

When everything is configured correctly:

1. **Timer** shows countdown to hackathon finish
2. **Scoreboard** displays team scores (updates every 5 seconds)
3. **Announcements** appear in the right panel (updates every 5 seconds)
4. **Q&A** section shows questions and answers (updates every 5 seconds)
5. **Current Time** updates every second

## Need More Help?

See these files:
- `DEBUG_CHECKLIST.md` - Detailed debugging steps
- `PRODUCTION_SETUP.md` - Production deployment guide
- `ENV_CONFIG_CHANGES.md` - Technical details about environment setup