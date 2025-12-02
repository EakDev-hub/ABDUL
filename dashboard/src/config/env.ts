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
    // Fallback to default values
    return {
      NODE_ENV: 'development',
      API_GATEWAY_URL: 'http://localhost:5000'
    }
  }
}

export default getEnvConfig