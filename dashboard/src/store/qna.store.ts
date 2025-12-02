import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Qna } from '@/types/qna'

export const useQnaStore = defineStore('qna', () => {
  const qnaItems = ref<Qna[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  function setQnaItems(data: Qna[]) {
    qnaItems.value = data
    error.value = null
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(errorMessage: string | null) {
    error.value = errorMessage
  }

  function clearQnaItems() {
    qnaItems.value = []
    error.value = null
  }

  return {
    qnaItems,
    isLoading,
    error,
    setQnaItems,
    setLoading,
    setError,
    clearQnaItems
  }
})