import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'
import CryptoJS from 'crypto-js'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  
  return {
    plugins: [
      vue(),
      {
        name: 'configure-server',
        configureServer(server) {
          server.middlewares.use((req, res, next) => {
            if (req.url === '/get-env') {
              const envConfig = {
                NODE_ENV: env.NODE_ENV || mode,
                API_GATEWAY_URL: env.VITE_API_GATEWAY_URL || ''
              }

              // Encrypt configuration
              const encryptEnv = CryptoJS.AES.encrypt(
                JSON.stringify(envConfig),
                'This is env'
              ).toString()

              // Prevent browser cache
              res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate, private')
              res.setHeader('Pragma', 'no-cache')
              res.setHeader('Expires', '0')
              res.setHeader('Content-Type', 'application/json')

              res.end(JSON.stringify({ env: encryptEnv }))
              return
            }

            next()
          })
        }
      }
    ],
    server: {
      host: '0.0.0.0',
      port: 5173,
      watch: {
        usePolling: true,
      },
    },
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    }
  }
})
