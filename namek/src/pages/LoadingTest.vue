<template>
  <div class="loading-test-page">
    <div class="control-panel">
      <h1 class="title">🎨 Loading Components Test</h1>

      <div class="button-grid">
        <button
          v-for="option in loadingOptions"
          :key="option.mode"
          @click="selectLoading(option.mode)"
          :class="['test-button', { active: currentMode === option.mode }]"
        >
          <span class="icon">{{ option.icon }}</span>
          <span class="label">{{ option.label }}</span>
        </button>
      </div>

      <div class="controls">
        <button @click="toggleLoading" :class="['control-btn', { active: isLoading }]">
          {{ isLoading ? '⏸️ Stop Loading' : '▶️ Start Loading' }}
        </button>

        <button @click="requestStop" class="control-btn stop-btn" :disabled="!isLoading">
          🛑 Request Stop
        </button>

        <button @click="resetTest" class="control-btn reset-btn">
          🔄 Reset
        </button>
      </div>

      <div class="info-panel">
        <div class="info-item">
          <span class="info-label">Current Mode:</span>
          <span class="info-value">{{ getCurrentModeLabel() }}</span>
        </div>
        <div class="info-item">
          <span class="info-label">Status:</span>
          <span class="info-value">{{ isLoading ? '🟢 Active' : '🔴 Inactive' }}</span>
        </div>
        <div class="info-item">
          <span class="info-label">Stop Requested:</span>
          <span class="info-value">{{ stopRequested ? '✅ Yes' : '❌ No' }}</span>
        </div>
      </div>
    </div>

    <!-- Loading Component Display -->
    <RandomLoading
      v-if="isLoading"
      :isActive="isLoading"
      :stopRequested="stopRequested"
      :mode="currentMode"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import RandomLoading from '../components/RandomLoading.vue'

const isLoading = ref(false)
const stopRequested = ref(false)
const currentMode = ref<'random' | 'sequence' | 'terminal' | 'scifi' | 'logo'>('sequence')

const loadingOptions = [
  { mode: 'sequence' as const, icon: '🔄', label: 'Sequence' },
  { mode: 'random' as const, icon: '🎲', label: 'Random' },
  { mode: 'terminal' as const, icon: '💻', label: 'Terminal' },
  { mode: 'scifi' as const, icon: '🎯', label: 'Sci-Fi HUD' },
  { mode: 'logo' as const, icon: '🚀', label: 'Logo Animation' }
]

function selectLoading(mode: typeof currentMode.value) {
  currentMode.value = mode
  console.log(`Selected mode: ${mode}`)
}

function toggleLoading() {
  isLoading.value = !isLoading.value
  if (isLoading.value) {
    stopRequested.value = false
  }
}

function requestStop() {
  stopRequested.value = true
  setTimeout(() => {
    isLoading.value = false
    stopRequested.value = false
  }, 2000)
}

function resetTest() {
  isLoading.value = false
  stopRequested.value = false
  currentMode.value = 'sequence'
}

function getCurrentModeLabel() {
  const option = loadingOptions.find(opt => opt.mode === currentMode.value)
  return option ? option.label : 'Unknown'
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;700;900&family=Rajdhani:wght@300;400;500;600;700&display=swap');

.loading-test-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #0a0015 0%, #1a0520 50%, #000510 100%);
  padding: 2rem;
  position: relative;
  overflow-x: hidden;
}

.control-panel {
  max-width: 800px;
  margin: 0 auto;
  background: rgba(10, 5, 30, 0.95);
  border: 2px solid #FA4786;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 0 30px rgba(250, 71, 134, 0.3);
  backdrop-filter: blur(10px);
  position: relative;
  z-index: 10;
}

.title {
  font-family: 'Orbitron', sans-serif;
  font-size: 2rem;
  font-weight: 700;
  color: #FFB3D1;
  text-align: center;
  margin-bottom: 2rem;
  text-shadow: 0 0 20px rgba(250, 71, 134, 0.6);
}

.button-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.test-button {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1.5rem 1rem;
  background: rgba(250, 71, 134, 0.1);
  border: 2px solid rgba(250, 71, 134, 0.3);
  border-radius: 8px;
  color: #FFB3D1;
  font-family: 'Rajdhani', sans-serif;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.test-button:hover {
  background: rgba(250, 71, 134, 0.2);
  border-color: #FA4786;
  transform: translateY(-2px);
  box-shadow: 0 5px 20px rgba(250, 71, 134, 0.4);
}

.test-button.active {
  background: rgba(250, 71, 134, 0.3);
  border-color: #FA4786;
  box-shadow: 0 0 20px rgba(250, 71, 134, 0.6);
}

.test-button .icon {
  font-size: 2rem;
}

.test-button .label {
  letter-spacing: 1px;
}

.controls {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.control-btn {
  flex: 1;
  min-width: 150px;
  padding: 1rem 1.5rem;
  background: linear-gradient(135deg, #FA4786, #002D72);
  border: none;
  border-radius: 8px;
  color: white;
  font-family: 'Rajdhani', sans-serif;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(250, 71, 134, 0.3);
}

.control-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.control-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 25px rgba(250, 71, 134, 0.5);
}

.control-btn.active {
  background: linear-gradient(135deg, #6B8CFF, #FA4786);
  animation: pulse 2s ease-in-out infinite;
}

.control-btn.stop-btn {
  background: linear-gradient(135deg, #ff4444, #cc0000);
}

.control-btn.reset-btn {
  background: linear-gradient(135deg, #6B8CFF, #002D72);
}

@keyframes pulse {
  0%, 100% {
    box-shadow: 0 4px 15px rgba(250, 71, 134, 0.3);
  }
  50% {
    box-shadow: 0 4px 25px rgba(250, 71, 134, 0.6);
  }
}

.info-panel {
  background: rgba(0, 10, 30, 0.5);
  border: 1px solid rgba(250, 71, 134, 0.3);
  border-radius: 8px;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-bottom: 1px solid rgba(250, 71, 134, 0.1);
}

.info-item:last-child {
  border-bottom: none;
}

.info-label {
  font-family: 'Rajdhani', sans-serif;
  font-size: 1rem;
  color: #6B8CFF;
  font-weight: 500;
}

.info-value {
  font-family: 'Orbitron', monospace;
  font-size: 1rem;
  color: #FFB3D1;
  font-weight: 600;
  text-shadow: 0 0 10px rgba(255, 179, 209, 0.4);
}

/* Responsive */
@media (max-width: 768px) {
  .loading-test-page {
    padding: 1rem;
  }

  .control-panel {
    padding: 1.5rem;
  }

  .title {
    font-size: 1.5rem;
  }

  .button-grid {
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  }

  .controls {
    flex-direction: column;
  }

  .control-btn {
    min-width: 100%;
  }
}
</style>