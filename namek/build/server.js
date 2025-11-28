import express from 'express'
import bodyParser from 'body-parser'
import path from 'path'
import cors from 'cors'
import { fileURLToPath } from 'url'
import CryptoJS from 'crypto-js'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

const app = express()
const DIST_DIR = path.join(__dirname, '../dist')
const HTML_FILE = path.join(DIST_DIR, 'index.html')
const PORT = process.env.DOCKER_PORT || 3000

app.disable('x-powered-by')

app.use(bodyParser.json({ limit: '50mb' }))
app.use(bodyParser.urlencoded({ limit: '50mb', extended: true, parameterLimit: 50000 }))
app.use(cors())
app.use(express.static(DIST_DIR))

// Healthcheck endpoint
app.get('/healthcheck', (req, res) => {

  if (req) {
    res.json({
      status: 'UP',
      buildState: process.env.BUILD_STAGE || 'development',
      version: process.env.PROJECT_VERSION || '1.0.0'
    })
  }
})

// Get encrypted environment configuration
app.get('/get-env', (req, res) => {
  const BUILD_STAGE = process.env.BUILD_STAGE || 'local'
  const API_GATEWAY_URL = process.env.API_GATEWAY_URL || ''

  // สร้าง environment configuration object
  const envConfig = {
    NODE_ENV: BUILD_STAGE === 'prod' ? 'production' : BUILD_STAGE,
    API_GATEWAY_URL: API_GATEWAY_URL
  }

  // Encrypt configuration
  const encryptEnv = CryptoJS.AES.encrypt(JSON.stringify(envConfig), 'This is env').toString()

  res.json({
    env: encryptEnv
  })
})

// SPA fallback - ต้องอยู่ล่างสุด
app.get('*', (req, res) => res.sendFile(HTML_FILE))

app.listen(PORT, function (err) {
  if (err) {
    console.log(err)
  } else {
    console.log('> Namek is listening on port ' + (process.env.PORT || PORT) + '\n')
  }
})
