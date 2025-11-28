import IEnv from '@/models/global/env'
import CryptoJS from 'crypto-js'

export default async (): Promise<IEnv> => {
  const BUILD_STAGE = process.env.BUILD_STAGE || 'dev'
  const API_KEY_GOOGLE_MAP = CryptoJS.AES.decrypt(
    'U2FsdGVkX183H/Gra7cOGLXykqdBhFH6fMQA1COyIpNBLOUjey69o7lx5LeRNIm35r3fxEuoyAEZkqnLED9EIg==',
    'API_KEY_GOOGLE_MAP'
  )
    .toString(CryptoJS.enc.Utf8)
    .replace(/^"(.*)"$/, '$1')
  return {
    NODE_ENV: BUILD_STAGE,
    API_GATEWAY_URL: '',
  }
}