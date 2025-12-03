import CryptoJS from 'crypto-js'
import type { IEnv } from '@/models/global/env'

const getEnvConfig = async (): Promise<IEnv> => {
  try {
    const response = await fetch('/get-env', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })

    if (!response.ok) {
      throw new Error(`Failed to fetch env config: ${response.statusText}`)
    }

    const data = await response.json()
    const decryptedBytes = CryptoJS.AES.decrypt(data.env, 'This is env')
    const decryptedEnv = JSON.parse(decryptedBytes.toString(CryptoJS.enc.Utf8))

    return decryptedEnv
  } catch (error) {
    console.error('Error loading environment config:', error)
    // Fallback to environment variables from Vite
    return {
      NODE_ENV: import.meta.env.MODE || 'development',
      API_GATEWAY_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'
    }
  }
}

export default getEnvConfig