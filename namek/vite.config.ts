import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import eslint from 'vite-plugin-eslint'
import stylelint from 'vite-plugin-stylelint'
import checker from 'vite-plugin-checker'
import { fileURLToPath, URL } from 'node:url'
import dns from 'dns'

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
      })
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
