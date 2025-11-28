<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useEnvStore } from '@/store/global/env'
import getEnvConfig from '@/config/env'

const envStore = useEnvStore()

onMounted(async () => {
  // โหลด env config ถ้ายังไม่มี
  if (!Object.keys(envStore.env).length) {
    const env = await getEnvConfig()
    envStore.setEnv(env)
    console.log('[App] Environment config loaded:', env.NODE_ENV)
  }
})
</script>

<style>
#app {
  width: 100%;
  height: 100vh;
}
</style>