export interface Announcement {
  id: number
  text: string
  postedAt: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface AnnouncementResponse {
  success: boolean
  data: Announcement[]
}