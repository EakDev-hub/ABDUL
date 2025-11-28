import IEnv from '@/models/global/env'
import CryptoJS from 'crypto-js'

export default async (): Promise<IEnv> => {
  try {
    // ดึง environment configuration จาก server endpoint
    const response = await fetch('/get-env')

    if (!response.ok) {
      throw new Error(`Failed to fetch environment config: ${response.statusText}`)
    }

    const data = await response.json()

    // Decrypt environment configuration
    const decryptedEnv = CryptoJS.AES.decrypt(data.env, 'This is env')
      .toString(CryptoJS.enc.Utf8)

    const envConfig: IEnv = JSON.parse(decryptedEnv)

    // Validate required fields
    if (!envConfig.API_GATEWAY_URL) {
      console.warn('API_GATEWAY_URL is not configured. API calls may fail.')
    }

    return envConfig
  } catch (error) {
    console.error('Error loading environment configuration:', error)

    // Fallback configuration for development
    return {
      NODE_ENV: process.env.BUILD_STAGE || 'development',
      API_GATEWAY_URL: process.env.API_GATEWAY_URL || '',
    }
  }
}