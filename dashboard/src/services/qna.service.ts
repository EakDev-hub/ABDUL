import apiClient from './api'
import type { QnaResponse } from '@/types/qna'

export const qnaService = {
  async getQna(): Promise<QnaResponse> {
    try {
      console.log('🚀 Fetching Q&A from API Gateway...')
      
      const response = await apiClient.get<QnaResponse>('/qna')
      
      console.log('✅ API Response received:', response.data)
      return response.data
      
    } catch (error: any) {
      console.error('❌ API call failed:', error)
      throw error
    }
  }
}