import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { SubmitResponse, ErrorState } from '@/types/hackathon'

const STORAGE_KEY = 'hackathon_result'
const ERROR_STORAGE_KEY = 'hackathon_error'

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

function saveErrorToStorage(errorState: ErrorState | null) {
  try {
    if (errorState && errorState.hasError) {
      localStorage.setItem(ERROR_STORAGE_KEY, JSON.stringify(errorState))
    } else {
      localStorage.removeItem(ERROR_STORAGE_KEY)
    }
  } catch (error) {
    console.error('Failed to save error to localStorage:', error)
  }
}

function loadErrorFromStorage(): ErrorState | null {
  try {
    const stored = localStorage.getItem(ERROR_STORAGE_KEY)
    return stored ? JSON.parse(stored) : null
  } catch (error) {
    console.error('Failed to load error from localStorage:', error)
    return null
  }
}

export const useHackathonStore = defineStore('hackathon', () => {
  // Initialize from localStorage
  const result = ref<SubmitResponse | null>(loadFromStorage())
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const errorState = ref<ErrorState | null>(loadErrorFromStorage())

  function setResult(data: SubmitResponse) {
    result.value = data
    errorState.value = null
    saveToStorage(data)
    saveErrorToStorage(null)
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(errorMessage: string | null, errorCode?: number, errorDetails?: string) {
    error.value = errorMessage
    if (errorMessage) {
      errorState.value = {
        hasError: true,
        errorCode,
        errorMessage,
        errorDetails
      }
      saveErrorToStorage(errorState.value)
    } else {
      errorState.value = null
      saveErrorToStorage(null)
    }
  }

  function clearResult() {
    result.value = null
    error.value = null
    errorState.value = null
    saveToStorage(null)
    saveErrorToStorage(null)
  }

  return {
    result,
    isLoading,
    error,
    errorState,
    setResult,
    setLoading,
    setError,
    clearResult
  }
})