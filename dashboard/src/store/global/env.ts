import type { IEnv } from '@/models/global/env'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useEnvStore = defineStore('env', () => {
  const env = ref<IEnv>({} as IEnv)

  const getEnv = () => env.value

  function setEnv(newEnv: IEnv) {
    env.value = newEnv
  }

  return {
    env,
    getEnv,
    setEnv
  }
})