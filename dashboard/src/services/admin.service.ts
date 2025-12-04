import api from './api'
import type { Qna } from '@/types/qna'
import type { Announcement } from '@/types/announcement'

export interface CreateQnaRequest {
  question: string
  answer?: string
}

export interface UpdateQnaRequest {
  question?: string
  answer?: string
}

export interface CreateAnnouncementRequest {
  text: string
  postedAt?: string
  isActive: boolean
}

export interface UpdateAnnouncementRequest {
  text?: string
  postedAt?: string
  isActive?: boolean
}

export const adminService = {
  // Q&A Management
  async getAllQna() {
    const response = await api.get<{ success: boolean; data: Qna[]; count: number }>('/api/admin/qna')
    return response.data
  },

  async getQnaById(id: number) {
    const response = await api.get<{ success: boolean; data: Qna }>(`/api/admin/qna/${id}`)
    return response.data
  },

  async createQna(data: CreateQnaRequest) {
    const response = await api.post<{ success: boolean; message: string; data: Qna }>('/api/admin/qna', data)
    return response.data
  },

  async updateQna(id: number, data: UpdateQnaRequest) {
    const response = await api.put<{ success: boolean; message: string; data: Qna }>(`/api/admin/qna/${id}`, data)
    return response.data
  },

  async deleteQna(id: number) {
    const response = await api.delete<{ success: boolean; message: string }>(`/api/admin/qna/${id}`)
    return response.data
  },

  // Announcement Management
  async getAllAnnouncements() {
    const response = await api.get<{ success: boolean; data: Announcement[]; count: number }>('/api/admin/announcements')
    return response.data
  },

  async getAnnouncementById(id: number) {
    const response = await api.get<{ success: boolean; data: Announcement }>(`/api/admin/announcements/${id}`)
    return response.data
  },

  async createAnnouncement(data: CreateAnnouncementRequest) {
    const response = await api.post<{ success: boolean; message: string; data: Announcement }>('/api/admin/announcements', data)
    return response.data
  },

  async updateAnnouncement(id: number, data: UpdateAnnouncementRequest) {
    const response = await api.put<{ success: boolean; message: string; data: Announcement }>(`/api/admin/announcements/${id}`, data)
    return response.data
  },

  async deleteAnnouncement(id: number) {
    const response = await api.delete<{ success: boolean; message: string }>(`/api/admin/announcements/${id}`)
    return response.data
  }
}