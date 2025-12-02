import apiClient from './api'
import type { AnnouncementResponse } from '@/types/announcement'

export const announcementService = {
  async getAnnouncements(): Promise<AnnouncementResponse> {
    try {
      console.log('🚀 Fetching announcements from API Gateway...')
      
      const response = await apiClient.get<AnnouncementResponse>('/announcement')
      
      console.log('✅ API Response received:', response.data)
      return response.data
      
    } catch (error: any) {
      console.error('❌ API call failed:', error)
      throw error
    }
  }
}