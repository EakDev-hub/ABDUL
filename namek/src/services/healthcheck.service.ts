import apiClient from './api'

export interface HealthCheckResponse {
  status: string
  timestamp: string
  message?: string
}

export const healthCheckService = {
  async checkHealth(): Promise<HealthCheckResponse> {
    const response = await apiClient.get<HealthCheckResponse>('/health')
    return response.data
  }
}