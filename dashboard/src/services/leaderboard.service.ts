import apiClient from './api'
import type { ScoreResponse } from '@/types/score'

export const leaderboardService = {
  async getScores(passKeyType?: string): Promise<ScoreResponse> {
    try {
      console.log(`🚀 Fetching scores from API Gateway with passKeyType=${passKeyType || 'all'}...`)
      
      const params = passKeyType ? { passKeyType } : {}
      const response = await apiClient.get<ScoreResponse>('/score', { params })
      
      console.log('✅ API Response received:', response.data)
      return response.data
      
    } catch (error: any) {
      console.error('❌ API call failed:', error)
      throw error
    }
  }
}
