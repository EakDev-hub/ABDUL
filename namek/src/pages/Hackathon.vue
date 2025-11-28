<template>
  <div class="tw-min-h-screen tw-bg-gradient-to-br tw-from-gray-900 tw-via-blue-900 tw-to-gray-900 tw-p-8">
    <!-- Header -->
    <div class="tw-mb-8 tw-text-center">
      <h1 class="tw-text-5xl tw-font-bold tw-text-white tw-mb-2">
        Hackathon #2 2025
      </h1>
      <p class="tw-text-xl tw-text-blue-300">
        ระบบตรวจคำตอบและให้คะแนน
      </p>
    </div>

    <!-- Form Section -->
    <div class="tw-max-w-2xl tw-mx-auto tw-mb-8">
      <div class="tw-bg-gray-800 tw-rounded-lg tw-p-8 tw-shadow-2xl tw-border tw-border-blue-500">
        <form @submit.prevent="handleSubmit">
          <!-- Team Name -->
          <div class="tw-mb-6">
            <label class="tw-block tw-text-white tw-text-lg tw-font-semibold tw-mb-2">
              Team
            </label>
            <input
              v-model="formData.team"
              type="text"
              class="tw-w-full tw-px-4 tw-py-3 tw-bg-gray-700 tw-text-white tw-border tw-border-gray-600 tw-rounded-lg focus:tw-outline-none focus:tw-border-blue-500 focus:tw-ring-2 focus:tw-ring-blue-500"
              placeholder="ชื่อทีม"
              required
            />
          </div>

          <!-- Pass Key -->
          <div class="tw-mb-6">
            <label class="tw-block tw-text-white tw-text-lg tw-font-semibold tw-mb-2">
              Pass Key
            </label>
            <input
              v-model="formData.passKey"
              type="text"
              class="tw-w-full tw-px-4 tw-py-3 tw-bg-gray-700 tw-text-white tw-border tw-border-gray-600 tw-rounded-lg focus:tw-outline-none focus:tw-border-blue-500 focus:tw-ring-2 focus:tw-ring-blue-500"
              placeholder="AAAAMMMMNANSXASX"
              required
            />
          </div>

          <!-- API URL -->
          <div class="tw-mb-6">
            <label class="tw-block tw-text-white tw-text-lg tw-font-semibold tw-mb-2">
              API URL
            </label>
            <input
              v-model="formData.apiUrl"
              type="text"
              class="tw-w-full tw-px-4 tw-py-3 tw-bg-gray-700 tw-text-white tw-border tw-border-gray-600 tw-rounded-lg focus:tw-outline-none focus:tw-border-blue-500 focus:tw-ring-2 focus:tw-ring-blue-500"
              placeholder="aa.bb.cc:8088/api/test"
              required
            />
          </div>

          <!-- Submit Button -->
          <button
            type="submit"
            :disabled="isLoading"
            class="tw-w-full tw-bg-blue-600 hover:tw-bg-blue-700 tw-text-white tw-font-bold tw-py-4 tw-px-6 tw-rounded-lg tw-transition-all tw-duration-200 disabled:tw-opacity-50 disabled:tw-cursor-not-allowed tw-text-lg"
          >
            <span v-if="isLoading" class="tw-flex tw-items-center tw-justify-center">
              <svg class="tw-animate-spin tw-h-5 tw-w-5 tw-mr-3" viewBox="0 0 24 24">
                <circle class="tw-opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none"></circle>
                <path class="tw-opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Loading...
            </span>
            <span v-else>Submit</span>
          </button>
        </form>

        <!-- Error Message -->
        <div v-if="error" class="tw-mt-4 tw-p-4 tw-bg-red-900 tw-border tw-border-red-500 tw-rounded-lg">
          <p class="tw-text-red-200">{{ error }}</p>
        </div>
      </div>
    </div>

    <!-- Results Section -->
    <div v-if="result" class="tw-max-w-6xl tw-mx-auto">
      <!-- Summary -->
      <div class="tw-bg-gray-800 tw-rounded-lg tw-p-6 tw-mb-6 tw-shadow-2xl tw-border tw-border-blue-500">
        <div class="tw-grid tw-grid-cols-1 md:tw-grid-cols-3 tw-gap-4 tw-text-center">
          <div>
            <p class="tw-text-blue-300 tw-text-sm tw-mb-1">จำนวนข้อที่ทำได้</p>
            <p class="tw-text-white tw-text-3xl tw-font-bold">
              {{ result.answeredQuestion }} / {{ result.totalQuestion }} ข้อ
            </p>
          </div>
          <div>
            <p class="tw-text-blue-300 tw-text-sm tw-mb-1">คะแนนที่ได้</p>
            <p class="tw-text-white tw-text-3xl tw-font-bold">
              {{ result.score.toFixed(2) }} / {{ result.maximumScore }}
            </p>
          </div>
          <div>
            <p class="tw-text-blue-300 tw-text-sm tw-mb-1">UUID</p>
            <p class="tw-text-white tw-text-lg tw-font-mono tw-break-all">
              {{ result.uuid }}
            </p>
          </div>
        </div>
        <div class="tw-mt-4 tw-text-center">
          <span class="tw-inline-block tw-px-4 tw-py-2 tw-rounded-full tw-text-sm tw-font-semibold"
                :class="{
                  'tw-bg-green-900 tw-text-green-200': result.passKeyType === 'develop',
                  'tw-bg-blue-900 tw-text-blue-200': result.passKeyType === 'present',
                  'tw-bg-purple-900 tw-text-purple-200': result.passKeyType === 'finalist'
                }">
            {{ result.passKeyType.toUpperCase() }}
          </span>
          <span class="tw-ml-4 tw-text-gray-400">
            Max Duration: {{ result.maxDurationInSecs }} วินาที
          </span>
        </div>
      </div>

      <!-- Results Table -->
      <div class="tw-bg-gray-800 tw-rounded-lg tw-shadow-2xl tw-border tw-border-blue-500 tw-overflow-hidden">
        <div class="tw-overflow-x-auto">
          <table class="tw-w-full">
            <thead class="tw-bg-gray-900">
              <tr>
                <th class="tw-px-4 tw-py-3 tw-text-left tw-text-white tw-font-bold tw-border-b tw-border-gray-700">No.</th>
                <th class="tw-px-4 tw-py-3 tw-text-left tw-text-white tw-font-bold tw-border-b tw-border-gray-700">Question</th>
                <th class="tw-px-4 tw-py-3 tw-text-left tw-text-white tw-font-bold tw-border-b tw-border-gray-700">Expected Answer</th>
                <th class="tw-px-4 tw-py-3 tw-text-left tw-text-white tw-font-bold tw-border-b tw-border-gray-700">Answer</th>
                <th class="tw-px-4 tw-py-3 tw-text-center tw-text-white tw-font-bold tw-border-b tw-border-gray-700">Score<br/>(0.0 - 1.0)</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in result.results" :key="item.no"
                  class="tw-border-b tw-border-gray-700 hover:tw-bg-gray-750 tw-transition-colors">
                <td class="tw-px-4 tw-py-3 tw-text-white tw-font-semibold">{{ item.no }}</td>
                <td class="tw-px-4 tw-py-3 tw-text-gray-300">{{ item.question }}</td>
                <td class="tw-px-4 tw-py-3 tw-text-gray-300">{{ item.expectedAnswer }}</td>
                <td class="tw-px-4 tw-py-3 tw-text-gray-300">{{ item.actualAnswer }}</td>
                <td class="tw-px-4 tw-py-3 tw-text-center">
                  <span class="tw-inline-block tw-px-3 tw-py-1 tw-rounded-full tw-font-bold"
                        :class="{
                          'tw-bg-green-900 tw-text-green-200': item.score >= 0.8,
                          'tw-bg-yellow-900 tw-text-yellow-200': item.score >= 0.5 && item.score < 0.8,
                          'tw-bg-red-900 tw-text-red-200': item.score < 0.5
                        }">
                    {{ item.score.toFixed(1) }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useHackathonStore } from '@/store/hackathon.store'
import { hackathonService } from '@/services/hackathon.service'
import type { SubmitRequest } from '@/types/hackathon'

const hackathonStore = useHackathonStore()

const formData = ref<SubmitRequest>({
  team: '',
  passKey: '',
  apiUrl: ''
})

const isLoading = computed(() => hackathonStore.isLoading)
const error = computed(() => hackathonStore.error)
const result = computed(() => hackathonStore.result)

async function handleSubmit() {
  hackathonStore.setLoading(true)
  hackathonStore.setError(null)

  try {
    const response = await hackathonService.submitAnswer(formData.value)
    hackathonStore.setResult(response)
  } catch (err: any) {
    const errorMessage = err.response?.data?.message || err.message || 'เกิดข้อผิดพลาดในการส่งข้อมูล'
    hackathonStore.setError(errorMessage)
  } finally {
    hackathonStore.setLoading(false)
  }
}
</script>

<style scoped>
.tw-bg-gray-750 {
  background-color: #2d3748;
}
</style>