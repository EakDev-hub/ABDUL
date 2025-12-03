import type { IEnv } from '@/models/global/env'

const getEnvConfig = (): IEnv => {
  return {
    NODE_ENV: import.meta.env.MODE || 'development',
    API_GATEWAY_URL: import.meta.env.VITE_API_GATEWAY_URL || ''
  }
}

export default getEnvConfig