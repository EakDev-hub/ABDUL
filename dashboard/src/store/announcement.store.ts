import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Announcement } from '@/types/announcement'

export const useAnnouncementStore = defineStore('announcement', () => {
  const announcements = ref<Announcement[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  function setAnnouncements(data: Announcement[]) {
    announcements.value = data
    error.value = null
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(errorMessage: string | null) {
    error.value = errorMessage
  }

  function clearAnnouncements() {
    announcements.value = []
    error.value = null
  }

  return {
    announcements,
    isLoading,
    error,
    setAnnouncements,
    setLoading,
    setError,
    clearAnnouncements
  }
})