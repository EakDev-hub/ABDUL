import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Hackathon',
    component: () => import('@/pages/Hackathon.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
