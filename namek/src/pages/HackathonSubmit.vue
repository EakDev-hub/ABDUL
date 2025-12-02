<template>
  <div class="hackathon-container">
    <!-- Animated Background -->
    <div class="animated-bg"></div>
    <div class="grid-overlay"></div>
    <div class="particles">
      <div v-for="i in 20" :key="i" class="particle" :style="getParticleStyle(i)"></div>
    </div>
    <div class="holo-lines">
      <div v-for="i in 5" :key="i" class="holo-line"></div>
    </div>

    <!-- Loading Modal - Random between Terminal and Sci-Fi -->
    <transition name="terminal-fade">
      <TerminalLoading
        v-if="showTerminal && loadingMode === 'terminal'"
        :is-active="showTerminal"
        :stop-requested="animationStopRequested"
      />
      <SciFiLoading
        v-else-if="showTerminal && loadingMode === 'scifi'"
        :is-active="showTerminal"
        :stop-requested="animationStopRequested"
      />
    </transition>

    <!-- Main Content -->
    <div v-if="showTerminal" class="terminal-blocker"></div>
    <div class="main-content">
      <!-- Header -->
      <div class="header-section">
        <h1 class="main-title">
          <span class="glitch" data-text="HACKATHON #2 2025">HACKATHON #2 2025</span>
        </h1>
        <div class="subtitle-wrapper">
          <div class="tech-line left"></div>
          <p class="subtitle">
            <span class="ai-badge">
              <svg class="ai-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M12 2a10 10 0 1 0 10 10A10 10 0 0 0 12 2zm0 14a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                <path d="M12 12v.01" />
              </svg>
              AI SYSTEM
            </span>
            <span class="subtitle-text">AUTOMATED SCORING PROTOCOL</span>
          </p>
          <div class="tech-line right"></div>
        </div>
      </div>

      <!-- Form Section -->
      <div class="form-container">
        <div class="form-card hud-panel">
          <div class="panel-corner top-left"></div>
          <div class="panel-corner top-right"></div>
          <div class="panel-corner bottom-left"></div>
          <div class="panel-corner bottom-right"></div>

          <div class="panel-header">
            <span class="panel-label">INPUT PARAMETERS</span>
            <div class="panel-line"></div>
          </div>

          <form novalidate @submit.prevent="handleSubmit">
            <!-- Team Name -->
            <div class="input-group">
              <label class="input-label">
                <svg class="label-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
                  <circle cx="9" cy="7" r="4"></circle>
                  <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
                  <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                </svg>
                TEAM IDENTIFIER
              </label>
              <div class="input-wrapper">
                <input
                  v-model="formData.team"
                  type="text"
                  class="cyber-input"
                  placeholder="ENTER TEAM NAME"
                  required
                />
                <div class="input-border"></div>
              </div>
              <div v-if="errors.team" class="field-error">{{ errors.team }}</div>
            </div>

            <!-- Pass Key -->
            <div class="input-group">
              <label class="input-label">
                <svg class="label-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                </svg>
                ACCESS KEY
              </label>
              <div class="input-wrapper">
                <input
                  v-model="formData.passKey"
                  type="text"
                  class="cyber-input"
                  placeholder="ENTER PASS KEY"
                  required
                />
                <div class="input-border"></div>
              </div>
              <div v-if="errors.passKey" class="field-error">{{ errors.passKey }}</div>
            </div>

            <!-- API URL -->
            <div class="input-group">
              <label class="input-label">
                <svg class="label-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10"></circle>
                  <line x1="2" y1="12" x2="22" y2="12"></line>
                  <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z"></path>
                </svg>
                ENDPOINT URL
              </label>
              <div class="input-wrapper">
                <input
                  v-model="formData.apiUrl"
                  type="text"
                  class="cyber-input"
                  placeholder="ENTER API URL"
                  required
                />
                <div class="input-border"></div>
              </div>
              <div v-if="errors.apiUrl" class="field-error">{{ errors.apiUrl }}</div>
            </div>

            <!-- Submit Button -->
            <button
              type="submit"
              :disabled="isLoading"
              class="cyber-button"
            >
              <div class="button-content">
                <span v-if="isLoading" class="button-loading">
                  <svg class="loading-spinner" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                  </svg>
                  PROCESSING...
                </span>
                <span v-else class="button-text">
                  <svg class="button-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"></path>
                  </svg>
                  INITIATE ANALYSIS
                </span>
              </div>
              <div class="button-glint"></div>
            </button>
          </form>

          <!-- Error Message -->
          <div v-if="error" class="error-message">
            <svg class="error-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path>
              <line x1="12" y1="9" x2="12" y2="13"></line>
              <line x1="12" y1="17" x2="12.01" y2="17"></line>
            </svg>
            <p>{{ error }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useHackathonStore } from '@/store/hackathon.store'
import { hackathonService } from '@/services/hackathon.service'
import type { SubmitRequest } from '@/types/hackathon'
import TerminalLoading from '@/components/TerminalLoading.vue'
import SciFiLoading from '@/components/SciFiLoading.vue'

const router = useRouter()
const hackathonStore = useHackathonStore()

// Loading mode - randomly choose between 'terminal' and 'scifi'
const loadingMode = ref<'terminal' | 'scifi'>('terminal')

// Particle animation
function getParticleStyle(index: number) {
  const randomX = Math.random() * 100
  const randomY = Math.random() * 100
  const randomDelay = Math.random() * 5
  const randomDuration = 10 + Math.random() * 10

  return {
    left: `${randomX}%`,
    top: `${randomY}%`,
    animationDelay: `${randomDelay}s`,
    animationDuration: `${randomDuration}s`
  }
}

const formData = ref<SubmitRequest>({
  team: '',
  passKey: '',
  apiUrl: ''
})

const errors = ref({
  team: '',
  passKey: '',
  apiUrl: ''
})

const showTerminal = ref(false)
const animationStopRequested = ref(false)

const isLoading = computed(() => hackathonStore.isLoading)
const error = computed(() => hackathonStore.error)

function stopAnimation() {
  animationStopRequested.value = true
}

function isValidUrl(urlString: string) {
  try {
    const url = new URL(urlString)
    return url.protocol === 'http:' || url.protocol === 'https:'
  } catch (_) {
    return false
  }
}

async function handleSubmit() {
  // Reset errors
  errors.value = {
    team: '',
    passKey: '',
    apiUrl: ''
  }
  hackathonStore.setError(null)

  let hasError = false

  if (!formData.value.team) {
    errors.value.team = 'REQUIRED FIELD'
    hasError = true
  }
  if (!formData.value.passKey) {
    errors.value.passKey = 'REQUIRED FIELD'
    hasError = true
  }
  if (!formData.value.apiUrl) {
    errors.value.apiUrl = 'REQUIRED FIELD'
    hasError = true
  } else if (!isValidUrl(formData.value.apiUrl)) {
    errors.value.apiUrl = 'INVALID PROTOCOL (HTTP/HTTPS ONLY)'
    hasError = true
  }

  if (hasError) return

  hackathonStore.setLoading(true)

  // Randomly choose loading mode
  loadingMode.value = Math.random() > 0.5 ? 'terminal' : 'scifi'

  // Show loading
  showTerminal.value = true

  // Random minimum delay between 5-10 seconds (5000-10000 ms)
  const minDelay = Math.floor(Math.random() * 5000) + 5000
  const startTime = Date.now()

  // API call (runs in parallel with animation)
  try {
    const response = await hackathonService.submitAnswer(formData.value)
    hackathonStore.setResult(response)

    // Calculate remaining time to meet minimum delay
    const elapsedTime = Date.now() - startTime
    const remainingTime = Math.max(0, minDelay - elapsedTime)

    // Wait for remaining time if needed
    if (remainingTime > 0) {
      await new Promise(resolve => setTimeout(resolve, remainingTime))
    }

    // Stop animation gracefully
    stopAnimation()

    // Wait for animation to complete gracefully
    await new Promise(resolve => setTimeout(resolve, 1000))

    // Hide loading
    showTerminal.value = false
    hackathonStore.setLoading(false)

    // Navigate to results page immediately
    router.push('/hackathon/result')
  } catch (err: any) {
    console.error('API Error:', err)

    // Extract error information from response
    const errorData = err.response?.data
    const statusCode = err.response?.status

    let errorMessage = 'SYSTEM ERROR: CONNECTION FAILED'
    let errorDetails = ''

    if (errorData) {
      // Handle different error response formats
      errorMessage = errorData.error || errorData.message || errorMessage
      errorDetails = errorData.details || errorData.detail || err.message || ''
    } else if (err.message) {
      errorMessage = err.message
    }

    hackathonStore.setError(errorMessage, statusCode, errorDetails)

    // ถ้า error แสดงทันทีเลย ไม่ต้องรอเวลาและไม่ต้อง stop animation gracefully
    // Hide loading immediately without stopping animation (just hide it)
    showTerminal.value = false
    hackathonStore.setLoading(false)

    // แสดง error ในหน้า submit เลย ไม่ต้อง navigate ไปหน้า result
  }
}

</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Chakra+Petch:wght@300;400;500;600;700&family=Orbitron:wght@400;500;700;900&family=Rajdhani:wght@300;500;700&family=Share+Tech+Mono&display=swap');

:root {
  --primary-cyan: #00f3ff;
  --deep-blue: #000a1f;
  --glass-bg: rgba(0, 20, 40, 0.7);
  --neon-glow: 0 0 10px rgba(0, 243, 255, 0.5);
}

.hackathon-container {
  min-height: 100vh;
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
  background: #000205;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  color: #00f3ff;
}

/* Animated Background */
.animated-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle at 50% 50%, #001a33 0%, #000205 100%);
  z-index: 0;
}

.grid-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 200%;
  height: 200%;
  background-image:
    linear-gradient(rgba(0, 243, 255, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 243, 255, 0.05) 1px, transparent 1px);
  background-size: 40px 40px;
  transform: perspective(500px) rotateX(60deg) translateY(-100px) translateZ(-200px);
  animation: gridMove 20s linear infinite;
  z-index: 1;
  pointer-events: none;
}

@keyframes gridMove {
  0% { transform: perspective(500px) rotateX(60deg) translateY(0) translateZ(-200px); }
  100% { transform: perspective(500px) rotateX(60deg) translateY(40px) translateZ(-200px); }
}

/* Particles */
.particles {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1;
  pointer-events: none;
  overflow: hidden;
}

.particle {
  position: absolute;
  width: 2px;
  height: 2px;
  background: #00f3ff;
  border-radius: 50%;
  box-shadow: 0 0 10px #00f3ff;
  animation: particleFloat linear infinite;
  opacity: 0.6;
}

@keyframes particleFloat {
  0% {
    transform: translateY(0) translateX(0);
    opacity: 0;
  }
  10% {
    opacity: 0.6;
  }
  90% {
    opacity: 0.6;
  }
  100% {
    transform: translateY(-100vh) translateX(50px);
    opacity: 0;
  }
}

/* Holographic Lines */
.holo-lines {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1;
  pointer-events: none;
  overflow: hidden;
}

.holo-line {
  position: absolute;
  width: 100%;
  height: 1px;
  background: linear-gradient(90deg, transparent, #00f3ff, transparent);
  box-shadow: 0 0 10px #00f3ff;
  opacity: 0.3;
  animation: holoScan 8s linear infinite;
}

.holo-line:nth-child(1) { top: 20%; animation-delay: 0s; }
.holo-line:nth-child(2) { top: 40%; animation-delay: 1.6s; }
.holo-line:nth-child(3) { top: 60%; animation-delay: 3.2s; }
.holo-line:nth-child(4) { top: 80%; animation-delay: 4.8s; }
.holo-line:nth-child(5) { top: 100%; animation-delay: 6.4s; }

@keyframes holoScan {
  0% {
    transform: translateY(0);
    opacity: 0;
  }
  10% {
    opacity: 0.3;
  }
  90% {
    opacity: 0.3;
  }
  100% {
    transform: translateY(100vh);
    opacity: 0;
  }
}

.main-content {
  position: relative;
  z-index: 2;
  padding: 1.5rem;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  overflow: hidden;
  gap: 1rem;
}

/* Header */
.header-section {
  text-align: center;
  position: relative;
  flex-shrink: 0;
}

.main-title {
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 2.5rem;
  font-weight: 900;
  letter-spacing: 3px;
  margin-bottom: 0.3rem;
  text-transform: uppercase;
  background: linear-gradient(180deg, #fff, #00f3ff);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 15px rgba(0, 243, 255, 0.6));
}

.subtitle-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

.tech-line {
  height: 1px;
  width: 50px;
  background: linear-gradient(90deg, transparent, #00f3ff, transparent);
}

.subtitle {
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 0.9rem;
  color: #aaddff;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.15rem;
}

.ai-badge {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  color: #00f3ff;
  font-size: 0.85rem;
  letter-spacing: 1.5px;
}

.ai-icon {
  width: 16px;
  height: 16px;
  filter: drop-shadow(0 0 5px #00f3ff);
}

.subtitle-text {
  font-size: 0.75rem;
  letter-spacing: 2px;
  opacity: 0.8;
}

/* HUD Panel Form */
.form-container {
  width: 100%;
  max-width: 900px;
  perspective: 1000px;
  animation: formSlideIn 0.8s ease-out;
  flex-shrink: 0;
  overflow: visible;
}

@keyframes formSlideIn {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.hud-panel {
  background: rgba(0, 10, 20, 0.85);
  border: 1px solid rgba(0, 243, 255, 0.3);
  padding: 2rem 2.5rem;
  position: relative;
  backdrop-filter: blur(15px);
  clip-path: polygon(
    25px 0, 100% 0,
    100% calc(100% - 25px), calc(100% - 25px) 100%,
    0 100%, 0 25px
  );
  box-shadow:
    0 0 40px rgba(0, 243, 255, 0.15),
    inset 0 0 80px rgba(0, 243, 255, 0.03);
  animation: panelGlow 3s ease-in-out infinite;
}

@keyframes panelGlow {
  0%, 100% {
    box-shadow:
      0 0 40px rgba(0, 243, 255, 0.15),
      inset 0 0 80px rgba(0, 243, 255, 0.03);
  }
  50% {
    box-shadow:
      0 0 60px rgba(0, 243, 255, 0.25),
      inset 0 0 100px rgba(0, 243, 255, 0.05);
  }
}

.hud-panel::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  border: 2px solid transparent;
  background: linear-gradient(135deg, #00f3ff, #b000ff) border-box;
  -webkit-mask: linear-gradient(#fff 0 0) padding-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  opacity: 0.4;
  pointer-events: none;
}

.panel-corner {
  position: absolute;
  width: 25px;
  height: 25px;
  border: 3px solid #00f3ff;
  z-index: 10;
  box-shadow: 0 0 15px rgba(0, 243, 255, 0.6);
  animation: cornerPulse 2s ease-in-out infinite;
}

@keyframes cornerPulse {
  0%, 100% { opacity: 0.6; }
  50% { opacity: 1; }
}

.top-left {
  top: 0;
  left: 0;
  border-right: none;
  border-bottom: none;
  clip-path: polygon(0 0, 100% 0, 100% 3px, 3px 3px, 3px 100%, 0 100%);
}

.top-right {
  top: 0;
  right: 0;
  border-left: none;
  border-bottom: none;
  clip-path: polygon(0 0, 100% 0, 100% 100%, calc(100% - 3px) 100%, calc(100% - 3px) 3px, 0 3px);
}

.bottom-left {
  bottom: 0;
  left: 0;
  border-right: none;
  border-top: none;
  clip-path: polygon(0 0, 3px 0, 3px calc(100% - 3px), 100% calc(100% - 3px), 100% 100%, 0 100%);
}

.bottom-right {
  bottom: 0;
  right: 0;
  border-left: none;
  border-top: none;
  clip-path: polygon(0 calc(100% - 3px), calc(100% - 3px) calc(100% - 3px), calc(100% - 3px) 0, 100% 0, 100% 100%, 0 100%);
}

.panel-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
  position: relative;
}

.panel-header::before {
  content: '';
  position: absolute;
  left: -2.5rem;
  top: 50%;
  width: 10px;
  height: 10px;
  background: #00f3ff;
  transform: translateY(-50%) rotate(45deg);
  box-shadow: 0 0 15px #00f3ff;
}

.field-error {
  color: #ff003c;
  font-size: 0.75rem;
  margin-top: 0.5rem;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  letter-spacing: 1px;
  text-shadow: 0 0 8px rgba(255, 0, 60, 0.8);
  display: flex;
  align-items: center;
  gap: 0.5rem;
  animation: errorGlitch 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94) both;
  background: rgba(255, 0, 60, 0.1);
  padding: 4px 8px;
  border-left: 2px solid #ff003c;
  clip-path: polygon(0 0, 100% 0, 100% 100%, 10px 100%, 0 calc(100% - 10px));
}

.field-error::before {
  content: '!';
  display: flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  background: #ff003c;
  color: #000;
  font-weight: 900;
  font-size: 0.8em;
  clip-path: polygon(20% 0%, 80% 0%, 100% 20%, 100% 80%, 80% 100%, 20% 100%, 0% 80%, 0% 20%);
}

@keyframes errorGlitch {
  0% {
    opacity: 0;
    transform: translateX(-10px);
    clip-path: inset(0 100% 0 0);
  }
  20% {
    opacity: 1;
    clip-path: inset(0 60% 0 0);
  }
  40% {
    opacity: 1;
    clip-path: inset(0 20% 0 0);
    transform: translateX(5px);
  }
  60% {
    transform: translateX(-2px);
  }
  80% {
    transform: translateX(1px);
  }
  100% {
    opacity: 1;
    transform: translateX(0);
    clip-path: inset(0 0 0 0);
  }
}

@keyframes markerPulse {
  0%, 100% { opacity: 0.5; transform: translateY(-50%) rotate(45deg) scale(1); }
  50% { opacity: 1; transform: translateY(-50%) rotate(45deg) scale(1.2); }
}

.panel-label {
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 0.85rem;
  color: #00f3ff;
  letter-spacing: 1.5px;
  text-shadow: 0 0 10px rgba(0, 243, 255, 0.5);
  white-space: nowrap;
}

.panel-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, #00f3ff, transparent);
  box-shadow: 0 0 5px rgba(0, 243, 255, 0.5);
  position: relative;
  overflow: hidden;
}

.panel-line::after {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 50%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.6), transparent);
  animation: lineGlint 3s ease-in-out infinite;
}

@keyframes lineGlint {
  0% { left: -100%; }
  100% { left: 200%; }
}

/* Inputs */
.input-group {
  margin-bottom: 1.3rem;
}

.input-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #aaddff;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 600;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
  letter-spacing: 1.2px;
  text-transform: uppercase;
  position: relative;
  padding-left: 1rem;
}

.input-label::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  width: 4px;
  height: 60%;
  background: #00f3ff;
  transform: translateY(-50%);
  box-shadow: 0 0 10px #00f3ff;
}

.label-icon {
  width: 18px;
  height: 18px;
  color: #00f3ff;
  filter: drop-shadow(0 0 5px #00f3ff);
  animation: iconFloat 2s ease-in-out infinite;
}

@keyframes iconFloat {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-3px); }
}

.input-wrapper {
  position: relative;
}

.cyber-input {
  width: 100%;
  padding: 1.1rem 1.5rem;
  background: rgba(0, 20, 40, 0.5);
  border: none;
  border-bottom: 2px solid rgba(0, 243, 255, 0.2);
  color: #fff;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 1.1rem;
  transition: all 0.3s ease;
  clip-path: polygon(0 0, 100% 0, 100% 100%, 12px 100%, 0 calc(100% - 12px));
  position: relative;
}

.cyber-input:focus {
  outline: none;
  background: rgba(0, 30, 60, 0.7);
  border-bottom-color: transparent;
  padding-left: 2rem;
  box-shadow:
    inset 0 0 20px rgba(0, 243, 255, 0.1),
    0 0 20px rgba(0, 243, 255, 0.2);
}

.cyber-input:focus::before {
  content: '>';
  position: absolute;
  left: 1rem;
  color: #00f3ff;
  animation: cursorBlink 1s infinite;
}

.input-border {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, #00f3ff, #b000ff);
  transition: width 0.4s ease;
  box-shadow: 0 0 15px #00f3ff;
}

.cyber-input:focus + .input-border {
  width: 100%;
}

.cyber-input::placeholder {
  color: rgba(0, 243, 255, 0.25);
  letter-spacing: 1px;
}

@keyframes cursorBlink {
  0%, 49% { opacity: 1; }
  50%, 100% { opacity: 0; }
}

/* Button */
.cyber-button {
  width: 100%;
  padding: 1.2rem;
  background: transparent;
  border: 2px solid #00f3ff;
  color: #00f3ff;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  font-size: 1.15rem;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  clip-path: polygon(18px 0, 100% 0, 100% calc(100% - 18px), calc(100% - 18px) 100%, 0 100%, 0 18px);
  margin-top: 1rem;
  letter-spacing: 2px;
  text-transform: uppercase;
}

.cyber-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, rgba(0, 243, 255, 0.1), rgba(176, 0, 255, 0.1));
  opacity: 0;
  transition: opacity 0.3s ease;
}

.cyber-button::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(0, 243, 255, 0.3);
  transform: translate(-50%, -50%);
  transition: width 0.5s ease, height 0.5s ease;
}

.cyber-button:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  border-color: #444;
  color: #444;
}

.cyber-button:hover:not(:disabled)::before {
  opacity: 1;
}

.cyber-button:hover:not(:disabled)::after {
  width: 300px;
  height: 300px;
}

.cyber-button:hover:not(:disabled) {
  background: rgba(0, 243, 255, 0.05);
  box-shadow:
    0 0 30px rgba(0, 243, 255, 0.5),
    inset 0 0 30px rgba(0, 243, 255, 0.1);
  letter-spacing: 3px;
  transform: translateY(-2px);
}

.cyber-button:active:not(:disabled) {
  transform: translateY(0);
  box-shadow:
    0 0 20px rgba(0, 243, 255, 0.3),
    inset 0 0 20px rgba(0, 243, 255, 0.2);
}

.button-content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.8rem;
  position: relative;
  z-index: 2;
}

.button-text,
.button-loading {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.button-icon {
  width: 26px;
  height: 26px;
  filter: drop-shadow(0 0 5px currentColor);
  animation: iconPulse 2s ease-in-out infinite;
}

@keyframes iconPulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

.button-glint {
  position: absolute;
  top: 0;
  left: -100%;
  width: 50%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  transform: skewX(-20deg);
  transition: left 0.6s ease;
  z-index: 1;
}

.cyber-button:hover .button-glint {
  left: 200%;
  transition: left 1s ease;
}

.loading-spinner {
  width: 24px;
  height: 24px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Error Message */
.error-message {
  margin-top: 1rem;
  padding: 1rem;
  background: rgba(255, 68, 68, 0.1);
  border: 1px solid rgba(255, 68, 68, 0.3);
  border-left: 4px solid #ff4444;
  color: #ff6666;
  display: flex;
  align-items: center;
  gap: 0.8rem;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 0.85rem;
  clip-path: polygon(10px 0, 100% 0, 100% calc(100% - 10px), calc(100% - 10px) 100%, 0 100%, 0 10px);
  box-shadow: 0 0 20px rgba(255, 68, 68, 0.2);
  animation: errorPulse 2s ease-in-out infinite;
}

@keyframes errorPulse {
  0%, 100% {
    box-shadow: 0 0 20px rgba(255, 68, 68, 0.2);
    border-left-color: #ff4444;
  }
  50% {
    box-shadow: 0 0 30px rgba(255, 68, 68, 0.4);
    border-left-color: #ff6666;
  }
}

.error-icon {
  width: 18px;
  height: 18px;
  filter: drop-shadow(0 0 5px currentColor);
  animation: shake 0.5s ease-in-out infinite;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-2px); }
  75% { transform: translateX(2px); }
}

/* Terminal Fade Animation */
.terminal-fade-enter-active, .terminal-fade-leave-active {
  transition: opacity 0.3s, transform 0.3s;
}

.terminal-fade-enter-from, .terminal-fade-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* Responsive */
@media (max-width: 768px) {
  .main-content {
    padding: 1rem;
  }
  .main-title {
    font-size: 1.8rem;
    letter-spacing: 2px;
  }
  .hud-panel {
    padding: 1.5rem;
  }
  .cyber-input {
    padding: 1rem 1.2rem;
    font-size: 1rem;
  }
  .cyber-button {
    padding: 1.1rem;
    font-size: 1rem;
  }
  .form-container {
    max-width: 100%;
  }
  .input-group {
    margin-bottom: 1rem;
  }
  .panel-header {
    margin-bottom: 1.2rem;
  }
  .scifi-hud {
    width: 400px;
    height: 400px;
  }
  .hud-main-circle {
    width: 300px;
    height: 300px;
  }
  .center-frame {
    width: 140px;
    height: 100px;
  }
  .progress-text {
    font-size: 2rem;
  }
}
</style>

