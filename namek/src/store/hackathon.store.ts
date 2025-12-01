import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { SubmitResponse } from '@/types/hackathon'

const STORAGE_KEY = 'hackathon_result'

// Helper functions for localStorage
function saveToStorage(data: SubmitResponse | null) {
  try {
    if (data) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(data))
    } else {
      localStorage.removeItem(STORAGE_KEY)
    }
  } catch (error) {
    console.error('Failed to save to localStorage:', error)
  }
}

function loadFromStorage(): SubmitResponse | null {
  try {
    const stored = localStorage.getItem(STORAGE_KEY)
    return stored ? JSON.parse(stored) : null
  } catch (error) {
    console.error('Failed to load from localStorage:', error)
    return null
  }
}

export const useHackathonStore = defineStore('hackathon', () => {
  // Initialize from localStorage
  const result = ref<SubmitResponse | null>(loadFromStorage())
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  function setResult(data: SubmitResponse) {
    result.value = data
    saveToStorage(data)
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(errorMessage: string | null) {
    error.value = errorMessage
  }

  function clearResult() {
    result.value = null
    error.value = null
    saveToStorage(null)
  }

  return {
    result,
    isLoading,
    error,
    setResult,
    setLoading,
    setError,
    clearResult
  }
})