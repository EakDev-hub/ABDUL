import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import eslint from 'vite-plugin-eslint'
import stylelint from 'vite-plugin-stylelint'
import checker from 'vite-plugin-checker'
import { fileURLToPath, URL } from 'node:url'
import dns from 'dns'
import CryptoJS from 'crypto-js'

dns.setDefaultResultOrder('verbatim')

export default ({ mode, command }) => {
  console.log('Executing vite %s in %s mode...', command, mode)
  const env = loadEnv(mode, process.cwd(), '')
  const isDev = mode === 'development'
  return defineConfig({
    base: '/',
    server: {
      port: +env.npm_package_config_port || 3000
    },
    build: {
      rollupOptions: {
        // input: isDev ? 'index.html' : 'src/singleSpaEntry.ts',
        preserveEntrySignatures: isDev ? false : 'exports-only',
        output: {
          exports: 'auto',
          format: 'es',
          assetFileNames: (assetInfo) => {
            let extType = assetInfo.name.split('.').at(-1)
            if (/png|jpe?g|svg|gif|tiff|bmp|ico/i.test(extType)) {
              extType = 'img'
            }
            if (/ttf|otf|woff2?|eot/i.test(extType)) {
              extType = 'font'
            }
            if (/css/i.test(extType)) {
              return 'assets/[name][extname]'
            }
            return `assets/${extType}/[name][extname]`
          },
          chunkFileNames: 'assets/js/[name]-[hash].js',
          entryFileNames: '[name].js'
        }
      }
    },
    plugins: [
      vue(),
      eslint({
        emitWarning: false
      }),
      stylelint(),
      checker({
        vueTsc: true
      }),
      {
        name: 'configure-server',
        configureServer(server) {
          server.middlewares.use((req, res, next) => {
            if (req.url === '/get-env') {
              const BUILD_STAGE = env.BUILD_STAGE || 'local'
              const API_GATEWAY_URL = env.API_GATEWAY_URL || ''

              // สร้าง environment configuration object
              const envConfig = {
                NODE_ENV: BUILD_STAGE === 'prod' ? 'production' : BUILD_STAGE,
                API_GATEWAY_URL: API_GATEWAY_URL
              }

              // Encrypt configuration
              const encryptEnv = CryptoJS.AES.encrypt(
                JSON.stringify(envConfig),
                'This is env'
              ).toString()

              // ป้องกัน browser cache
              res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate, private')
              res.setHeader('Pragma', 'no-cache')
              res.setHeader('Expires', '0')
              res.setHeader('Content-Type', 'application/json')

              res.end(JSON.stringify({ env: encryptEnv }))
              return
            }

            if (req.url === '/healthcheck') {
              // ป้องกัน browser cache
              res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate, private')
              res.setHeader('Pragma', 'no-cache')
              res.setHeader('Expires', '0')
              res.setHeader('Content-Type', 'application/json')

              res.end(
                JSON.stringify({
                  status: 'UP',
                  buildState: env.BUILD_STAGE || 'development',
                  version: env.PROJECT_VERSION || '1.0.0'
                })
              )
              return
            }

            next()
          })
        }
      }
    ],
    resolve: {
      alias: [
        {
          find: '@',
          replacement: fileURLToPath(new URL('./src', import.meta.url))
        }
      ]
    },
    define: {
      'process.env.NODE_ENV': JSON.stringify(env.NODE_ENV),
      'process.env.BUILD_STAGE': JSON.stringify(env.BUILD_STAGE),
      'process.env.DEPARTMENT_PATH': JSON.stringify(env.npm_package_config_departmentPath),
      'process.env.PROJECT_NAME': JSON.stringify(env.npm_package_name)
    }
  })
}
