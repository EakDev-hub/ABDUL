import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    redirect: '/hackathon'
  },
  {
    path: '/hackathon',
    name: 'HackathonSubmit',
    component: () => import('@/pages/HackathonSubmit.vue')
  },
  {
    path: '/hackathon/result',
    name: 'HackathonResult',
    component: () => import('@/pages/HackathonResult.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
