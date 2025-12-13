import apiClient from './api'
import type { ScoreResponse } from '@/types/score'

export const scoreService = {
  async getScores(): Promise<ScoreResponse> {
    try {
      console.log('🚀 Fetching scores from API Gateway with passKeyType=develop...')
      
      const response = await apiClient.get<ScoreResponse>('/score', {
        params: {
          passKeyType: 'develop'
        }
      })
      
      console.log('✅ API Response received:', response.data)
      return response.data
      
    } catch (error: any) {
      console.error('❌ API call failed:', error)
      throw error
    }
  }
}