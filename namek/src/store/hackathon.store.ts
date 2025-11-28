import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { SubmitResponse } from '@/types/hackathon'

export const useHackathonStore = defineStore('hackathon', () => {
  const result = ref<SubmitResponse | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  function setResult(data: SubmitResponse) {
    result.value = data
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