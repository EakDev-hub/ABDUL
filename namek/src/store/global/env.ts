import IEnv from '@/models/global/env'
import { defineStore } from 'pinia'

export const useEnvStore = defineStore({
  id: 'env',
  state: () => ({
    env: {} as IEnv
  }),
  getters: {
    getEnv: (state) => state.env
  },
  actions: {
    setEnv(env: IEnv) {
      this.env = env
    }
  }
})
