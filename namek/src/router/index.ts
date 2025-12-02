import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import { useHackathonStore } from '@/store/hackathon.store'
import type { SubmitResponse } from '@/types/hackathon'

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
  },
  {
    path: '/hackathon/result/mock',
    name: 'HackathonResultMock',
    component: () => import('@/pages/HackathonResult.vue'),
    beforeEnter: () => {
      const hackathonStore = useHackathonStore()

      // Mock data สำหรับทดสอบ
      const mockResult: SubmitResponse = {
        uuid: 'mock-uuid-12345-67890-abcdef',
        passKeyType: 'develop',
        maxDurationInSecs: 300,
        timeUsedInSeconds: 245,
        totalQuestion: 10,
        maximumScore: 10,
        answeredQuestion: 8,
        score: 7.5,
        results: [
          {
            no: 1,
            question: 'What is the capital of Thailand?',
            expectedAnswer: 'Bangkok',
            actualAnswer: 'Bangkok',
            score: 1.0
          },
          {
            no: 2,
            question: 'What is 2 + 2?',
            expectedAnswer: '4',
            actualAnswer: '4',
            score: 1.0
          },
          {
            no: 3,
            question: 'What is the largest planet in our solar system?',
            expectedAnswer: 'Jupiter',
            actualAnswer: 'Jupiter',
            score: 1.0
          },
          {
            no: 4,
            question: 'Who wrote Romeo and Juliet?',
            expectedAnswer: 'William Shakespeare',
            actualAnswer: 'Shakespeare',
            score: 0.8
          },
          {
            no: 5,
            question: 'What is the speed of light?',
            expectedAnswer: '299,792,458 m/s',
            actualAnswer: '300,000,000 m/s',
            score: 0.7
          },
          {
            no: 6,
            question: 'What is the chemical symbol for gold?',
            expectedAnswer: 'Au',
            actualAnswer: 'Au',
            score: 1.0
          },
          {
            no: 7,
            question: 'How many continents are there?',
            expectedAnswer: '7',
            actualAnswer: '7',
            score: 1.0
          },
          {
            no: 8,
            question: 'What is the smallest prime number?',
            expectedAnswer: '2',
            actualAnswer: '2',
            score: 1.0
          },
          {
            no: 9,
            question: 'What year did World War II end?',
            expectedAnswer: '1945',
            actualAnswer: '1944',
            score: 0.0
          },
          {
            no: 10,
            question: 'What is the boiling point of water in Celsius?',
            expectedAnswer: '100',
            actualAnswer: '',
            score: 0.0
          }
        ]
      }

      hackathonStore.setResult(mockResult)
      return true
    }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
