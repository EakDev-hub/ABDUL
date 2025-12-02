import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Score } from '@/types/score'

export const useScoreStore = defineStore('score', () => {
  const scores = ref<Score[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  function setScores(data: Score[]) {
    scores.value = data
    error.value = null
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(errorMessage: string | null) {
    error.value = errorMessage
  }

  function clearScores() {
    scores.value = []
    error.value = null
  }

  return {
    scores,
    isLoading,
    error,
    setScores,
    setLoading,
    setError,
    clearScores
  }
})