import axios, { AxiosInstance } from 'axios'
import getEnvConfig from '@/config/env'
import { useEnvStore } from '@/store/global/env'

// สร้าง axios instance สำหรับเชื่อมต่อ API Gateway
const apiClient: AxiosInstance = axios.create({
  timeout: 600000, // 10 minutes (600 seconds)
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor สำหรับเพิ่ม token หรือ headers อื่นๆ
apiClient.interceptors.request.use(
  async (config) => {
    // โหลด baseURL จาก env store
    const envStore = useEnvStore()
    let api = envStore.getEnv
    if (!Object.keys(api).length) {
      const envConfig = await getEnvConfig()
      envStore.setEnv(envConfig)
      api = envConfig
    }

    config.baseURL = api.API_GATEWAY_URL

    // เพิ่ม Authorization token ถ้ามี
    const token = localStorage.getItem('auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor สำหรับจัดการ error
apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    // จัดการ error ตาม status code
    if (error.response) {
      switch (error.response.status) {
        case 401:
          // Unauthorized - ให้ logout หรือ refresh token
          console.error('Unauthorized access')
          break
        case 403:
          // Forbidden
          console.error('Forbidden access')
          break
        case 404:
          // Not found
          console.error('Resource not found')
          break
        case 500:
          // Server error
          console.error('Server error')
          break
        default:
          console.error('API error:', error.response.data)
      }
    }
    return Promise.reject(error)
  }
)

export default apiClient
