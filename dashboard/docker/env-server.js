const http = require('http');
const CryptoJS = require('crypto-js');

// Read environment variables
const envConfig = {
  NODE_ENV: process.env.NODE_ENV || 'production',
  API_GATEWAY_URL: process.env.VITE_API_GATEWAY_URL || ''
};

// Encrypt configuration
const encryptEnv = CryptoJS.AES.encrypt(
  JSON.stringify(envConfig),
  'This is env'
).toString();

const server = http.createServer((req, res) => {
  if (req.url === '/get-env' && req.method === 'GET') {
    // Prevent browser cache
    res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate, private');
    res.setHeader('Pragma', 'no-cache');
    res.setHeader('Expires', '0');
    res.setHeader('Content-Type', 'application/json');
    res.setHeader('Access-Control-Allow-Origin', '*');
    
    res.writeHead(200);
    res.end(JSON.stringify({ env: encryptEnv }));
  } else {
    res.writeHead(404);
    res.end('Not Found');
  }
});

const PORT = 3001;
server.listen(PORT, '127.0.0.1', () => {
  console.log(`Environment config server running on http://127.0.0.1:${PORT}`);
  console.log('Serving encrypted environment configuration');
});