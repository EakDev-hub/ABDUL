# Dashboard Debug Checklist - Backend Data Not Showing

## Quick Diagnosis Steps

### 1. Install Dependencies First
```bash
cd dashboard
npm install
```

### 2. Check Backend is Running
The backend must be running on the URL specified in `.env`:
```bash
# Check if backend is accessible
curl http://localhost:5000/health
# or
curl http://localhost:5000/announcement
```

If backend is not running, start it first.

### 3. Start Dashboard in Development
```bash
npm run dev
```

### 4. Open Browser Console
1. Open http://localhost:5173 in your browser
2. Press F12 to open Developer Tools
3. Go to Console tab
4. Look for these messages:

**Expected Success:**
```
🚀 Fetching announcements from API Gateway...
✅ API Response received: {success: true, data: [...]}
```

**Common Errors:**

#### Error: "Failed to fetch env config"
**Cause**: `/get-env` endpoint not responding
**Solution**: 
- Check vite dev server is running
- Verify `vite.config.ts` has encryption middleware
- Check browser Network tab for `/get-env` request

#### Error: "ERR_CONNECTION_REFUSED" or "Network Error"
**Cause**: Backend is not running or wrong URL
**Solution**:
```bash
# Update .env with correct backend URL
VITE_API_GATEWAY_URL=http://localhost:5000

# Verify backend is accessible:
curl http://localhost:5000/health
```

#### Error: "CORS policy" 
**Cause**: Backend not allowing requests from dashboard
**Solution**: Backend must allow `http://localhost:5173` in CORS settings

#### Error: API returns but data doesn't show
**Cause**: Data structure mismatch
**Solution**: Check console logs for response structure

### 5. Network Tab Inspection

Open browser DevTools → Network tab:

1. **Check `/get-env` request**:
   - Should return: `{env: "encrypted_string"}`
   - Status: 200 OK
   - Response has encrypted configuration

2. **Check API requests** (e.g., `/announcement`, `/qna`, `/score`):
   - URL should be: `http://localhost:5000/announcement`
   - Method: GET
   - Status: 200 OK
   - Response structure:
     ```json
     {
       "success": true,
       "data": [...]
     }
     ```

### 6. Verify Environment Loading

Add this to browser console:
```javascript
// Check if environment is loaded
localStorage.clear() // Clear any cached data
location.reload() // Reload page

// After reload, check console for environment fetch
```

## Step-by-Step Testing

### Test 1: Environment Configuration
```bash
# Visit: http://localhost:5173/get-env
# Should return encrypted config
```

### Test 2: Backend Health
```bash
curl http://localhost:5000/health
# Should return: {"status": "ok"} or similar
```

### Test 3: Backend Endpoints
```bash
# Test announcements
curl http://localhost:5000/announcement

# Test Q&A
curl http://localhost:5000/qna

# Test scores
curl http://localhost:5000/score
```

### Test 4: Full Flow
1. Clear browser cache (Ctrl+Shift+Del)
2. Open http://localhost:5173
3. Open Console (F12)
4. Watch for:
   - Environment config fetch
   - API calls to backend
   - Data loading into store

## Common Fixes

### Fix 1: Reinstall Dependencies
```bash
cd dashboard
rm -rf node_modules package-lock.json
npm install
npm run dev
```

### Fix 2: Clear Browser Cache
```bash
# In browser
Ctrl+Shift+Del → Clear all

# Or hard reload
Ctrl+Shift+R
```

### Fix 3: Verify Backend URL
```bash
# Check .env file
cat .env | grep VITE_API_GATEWAY_URL

# Should match where your backend is running
# Local development: http://localhost:5000
# Production: https://your-backend-url.com
```

### Fix 4: Check Backend CORS
Backend must allow requests from dashboard origin.

In your backend (e.g., .NET):
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

app.UseCors("AllowDashboard");
```

## Production Debugging

For production (Docker):

### Check Container Logs
```bash
docker logs dashboard-container-name
```

### Check Environment Variables
```bash
docker exec dashboard-container-name env | grep VITE
```

### Test /get-env Endpoint
```bash
curl http://your-domain/get-env
# Should return encrypted config
```

### Check Nginx Proxy
```bash
docker exec dashboard-container-name cat /etc/nginx/conf.d/default.conf
# Verify /get-env proxy configuration
```

## Quick Fix Command

If nothing works, try this complete reset:

```bash
# Stop all processes
# Clear everything and restart

cd dashboard

# Clean install
rm -rf node_modules package-lock.json
npm install

# Start fresh
npm run dev
```

Then:
1. Ensure backend is running on http://localhost:5000
2. Open http://localhost:5173
3. Check browser console for errors
4. Verify API calls in Network tab

## Still Not Working?

Check these files for correct configuration:

1. **`dashboard/.env`** - Has correct `VITE_API_GATEWAY_URL`
2. **`dashboard/src/config/env.ts`** - Fetches and decrypts environment
3. **`dashboard/src/services/api.ts`** - Uses environment for baseURL
4. **`dashboard/vite.config.ts`** - Serves `/get-env` endpoint

Run this diagnostic:
```bash
cd dashboard
npm run dev

# In another terminal
curl http://localhost:5173/get-env
# Should return: {"env":"U2Fsd..."}  (encrypted data)

curl http://localhost:5000/announcement
# Should return your backend data