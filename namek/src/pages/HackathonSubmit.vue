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
      <div v-if="showTerminal" class="terminal-overlay">
        <!-- Terminal Mode -->
        <div v-if="loadingMode === 'terminal'" class="terminal-frame">
          <div class="terminal-header-bar">
            <div class="terminal-title-box">
              <svg class="terminal-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M4 17l6-6-6-6M12 19h8" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
              <span class="terminal-title">{{ terminalTitle }}</span>
            </div>
            <div class="terminal-controls">
              <div class="control-dot red"></div>
              <div class="control-dot yellow"></div>
              <div class="control-dot green"></div>
            </div>
          </div>
          <div ref="terminalContentRef" class="terminal-content">
            <pre ref="terminalTextRef" class="terminal-text">{{ displayedText }}<span class="cursor-blink">█</span></pre>
          </div>
          <div class="scanline"></div>
          <div class="holo-glint"></div>
        </div>

        <!-- Sci-Fi AI HUD Mode -->
        <div v-else class="scifi-hud">
          <div class="hud-background"></div>

          <!-- Main Circle -->
          <div class="hud-main-circle">
            <!-- Rotating Rings -->
            <div class="hud-ring ring-1"></div>
            <div class="hud-ring ring-2"></div>
            <div class="hud-ring ring-3"></div>

            <!-- Progress Arc -->
            <svg class="progress-arc" viewBox="0 0 200 200">
              <circle cx="100" cy="100" r="85" class="arc-bg"/>
              <circle cx="100" cy="100" r="85" class="arc-progress" :style="{ strokeDashoffset: arcProgress }"/>
            </svg>

            <!-- Center Display -->
            <div class="hud-center">
              <div class="center-frame">
                <div class="frame-corner tl"></div>
                <div class="frame-corner tr"></div>
                <div class="frame-corner bl"></div>
                <div class="frame-corner br"></div>
                <div class="progress-text">{{ Math.round(progress) }}%</div>
                <div class="status-text">{{ statusText }}</div>
              </div>
            </div>

            <!-- Side Indicators -->
            <div class="side-indicator left">
              <div v-for="i in 8" :key="'l'+i" class="indicator-bar" :class="{ active: i <= Math.floor(progress / 12.5) }"></div>
            </div>
            <div class="side-indicator right">
              <div v-for="i in 8" :key="'r'+i" class="indicator-bar" :class="{ active: i <= Math.floor(progress / 12.5) }"></div>
            </div>

            <!-- Bottom Stats -->
            <div class="hud-stats">
              <div class="stat-item">
                <div class="stat-label">NEURAL NET</div>
                <div class="stat-bars">
                  <div v-for="i in 6" :key="'n'+i" class="stat-bar" :class="{ active: i <= 6 }"></div>
                </div>
              </div>
              <div class="stat-item">
                <div class="stat-label">AI CORE</div>
                <div class="stat-bars">
                  <div v-for="i in 6" :key="'a'+i" class="stat-bar" :class="{ active: i <= 6 }"></div>
                </div>
              </div>
            </div>

            <!-- Orbiting Dots -->
            <div class="orbit-dot dot-1"></div>
            <div class="orbit-dot dot-2"></div>
          </div>

          <!-- Corner Decorations -->
          <div class="corner-deco top-left"></div>
          <div class="corner-deco top-right"></div>
          <div class="corner-deco bottom-left"></div>
          <div class="corner-deco bottom-right"></div>
        </div>
      </div>
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

          <form @submit.prevent="handleSubmit" novalidate>
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
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useHackathonStore } from '@/store/hackathon.store'
import { hackathonService } from '@/services/hackathon.service'
import type { SubmitRequest } from '@/types/hackathon'

const router = useRouter()
const hackathonStore = useHackathonStore()

// Loading mode - randomly choose between 'terminal' and 'scifi'
const loadingMode = ref<'terminal' | 'scifi'>('terminal')
const progress = ref(0)
const statusText = ref('INITIALIZING...')
const arcProgress = ref(534) // Full circle circumference ≈ 534
let progressInterval: number | null = null

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
const displayedText = ref('')
const terminalTitle = ref('INITIALIZING SYSTEM...')
const terminalContentRef = ref<HTMLElement | null>(null)
const terminalTextRef = ref<HTMLElement | null>(null)

const isLoading = computed(() => hackathonStore.isLoading)
const error = computed(() => hackathonStore.error)

// Hacker typer code samples
const codeSnippets = [
  `> CONNECTING TO NEURAL NET...\n> ESTABLISHING SECURE LINK...\n> HANDSHAKE COMPLETE.\n\n`,
  `> LOADING MODULES:\n  [+] TENSORFLOW.JS\n  [+] NATURAL.JS\n  [+] SENTIMENT_ANALYZER\n  [+] PATTERN_RECOGNITION\n\n`,
  `> ANALYZING INPUT STREAM...\n> DECODING PACKETS...\n> VERIFYING INTEGRITY...\n\n`,
  `> RUNNING HEURISTIC SCAN...\n  - VECTORIZING DATA...\n  - CALCULATING LOSS FUNCTION...\n  - OPTIMIZING WEIGHTS...\n\n`,
  `> GENERATING PREDICTIONS...\n  CONFIDENCE: 98.4%\n  ACCURACY: 99.1%\n  LATENCY: 12ms\n\n`,
  `> COMPILING RESULTS...\n> ENCRYPTING PAYLOAD...\n> TRANSMITTING TO CORE...\n\n`
]

async function typeText(text: string, speed: number = 15): Promise<void> {
  for (let i = 0; i < text.length; i++) {
    displayedText.value += text[i]
    await new Promise(resolve => setTimeout(resolve, speed))
    // Auto-scroll to bottom
    if (terminalContentRef.value) {
      terminalContentRef.value.scrollTop = terminalContentRef.value.scrollHeight
    }
  }
}

async function runTerminalAnimation() {
  displayedText.value = ''
  // Type random code snippets
  for (const snippet of codeSnippets) {
    await typeText(snippet, 8)
  }
  // Final messages
  terminalTitle.value = 'ANALYSIS COMPLETE'
  await typeText('\n[✓] SYSTEM DIAGNOSTICS: GREEN\n', 30)
  await typeText('[✓] DATA INTEGRITY: VERIFIED\n', 30)
  await typeText('[✓] REPORT GENERATED\n\n', 30)
  await typeText('>>> REDIRECTING TO DASHBOARD...', 40)
  // Wait a bit before closing
  await new Promise(resolve => setTimeout(resolve, 800))
}

async function runSciFiAnimation() {
  progress.value = 0
  const statuses = [
    'INITIALIZING...',
    'CONNECTING TO AI CORE...',
    'ANALYZING DATA...',
    'PROCESSING NEURAL NET...',
    'OPTIMIZING ALGORITHMS...',
    'FINALIZING RESULTS...',
    'COMPLETE'
  ]

  let statusIndex = 0
  statusText.value = statuses[0]

  // Progress animation
  progressInterval = window.setInterval(() => {
    if (progress.value < 100) {
      progress.value += Math.random() * 3 + 1
      if (progress.value > 100) progress.value = 100

      // Update arc progress (534 is full circle, 0 is complete)
      arcProgress.value = 534 - (534 * progress.value / 100)

      // Update status text
      const newIndex = Math.floor(progress.value / (100 / statuses.length))
      if (newIndex !== statusIndex && newIndex < statuses.length) {
        statusIndex = newIndex
        statusText.value = statuses[statusIndex]
      }
    }
  }, 100)
}

function stopSciFiAnimation() {
  if (progressInterval) {
    clearInterval(progressInterval)
    progressInterval = null
  }
  progress.value = 100
  arcProgress.value = 0
  statusText.value = 'COMPLETE'
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

  // Start animation based on mode
  let animationPromise: Promise<void>
  if (loadingMode.value === 'terminal') {
    animationPromise = runTerminalAnimation()
  } else {
    runSciFiAnimation()
    // Create a promise that resolves when API is done
    animationPromise = new Promise(resolve => {
      setTimeout(resolve, 100) // Small delay to start animation
    })
  }

  // API call
  const apiPromise = (async () => {
    try {
      const response = await hackathonService.submitAnswer(formData.value)
      hackathonStore.setResult(response)
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'SYSTEM ERROR: CONNECTION FAILED'
      hackathonStore.setError(errorMessage)
    }
  })()

  // Wait for animation and API to complete
  await Promise.all([animationPromise, apiPromise])

  // Calculate remaining time to meet minimum delay
  const elapsedTime = Date.now() - startTime
  const remainingTime = Math.max(0, minDelay - elapsedTime)

  // Wait for remaining time if needed
  if (remainingTime > 0) {
    await new Promise(resolve => setTimeout(resolve, remainingTime))
  }

  // Stop sci-fi animation if active
  if (loadingMode.value === 'scifi') {
    stopSciFiAnimation()
    await new Promise(resolve => setTimeout(resolve, 800)) // Show 100% briefly
  }

  // Hide loading
  showTerminal.value = false
  hackathonStore.setLoading(false)

  // Navigate to results page if successful
  if (hackathonStore.result) {
    router.push('/hackathon/result')
  }
}

onUnmounted(() => {
  if (progressInterval) {
    clearInterval(progressInterval)
  }
})
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

/* Terminal Modal */
.terminal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.9);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(5px);
}

.terminal-frame {
  width: 90%;
  max-width: 1000px;
  height: 70vh;
  background: rgba(0, 10, 20, 0.95);
  border: 1px solid #00f3ff;
  box-shadow: 0 0 50px rgba(0, 243, 255, 0.3);
  display: flex;
  flex-direction: column;
  position: relative;
  overflow: hidden;
}

.terminal-header-bar {
  background: rgba(0, 30, 50, 0.5);
  padding: 0.8rem 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #00f3ff;
}

.terminal-title-box {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #00f3ff;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
}

.terminal-icon {
  width: 18px;
  height: 18px;
}

.terminal-controls {
  display: flex;
  gap: 0.5rem;
}

.control-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}

.red { background: #ff5f56; }
.yellow { background: #ffbd2e; }
.green { background: #27c93f; }

.terminal-content {
  flex: 1;
  padding: 1.5rem;
  overflow-y: auto;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
}

.terminal-text {
  color: #00f3ff;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 1.1rem;
  line-height: 1.6;
  white-space: pre-wrap;
  text-shadow: 0 0 5px rgba(0, 243, 255, 0.5);
}

.scanline {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    to bottom,
    transparent 0%,
    rgba(0, 243, 255, 0.05) 50%,
    transparent 100%
  );
  animation: scan 6s linear infinite;
  pointer-events: none;
}

.holo-glint {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle at 50% 50%, rgba(0, 243, 255, 0.05), transparent 70%);
  pointer-events: none;
}

@keyframes scan {
  0% { transform: translateY(-100%); }
  100% { transform: translateY(100%); }
}

.terminal-fade-enter-active, .terminal-fade-leave-active {
  transition: opacity 0.3s, transform 0.3s;
}

.terminal-fade-enter-from, .terminal-fade-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* Sci-Fi HUD Loading */
.scifi-hud {
  width: 600px;
  height: 600px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hud-background {
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(0, 50, 100, 0.3) 0%, transparent 70%);
  animation: pulse 3s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

.hud-main-circle {
  width: 400px;
  height: 400px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Rotating Rings */
.hud-ring {
  position: absolute;
  border-radius: 50%;
  border: 2px solid rgba(0, 243, 255, 0.3);
}

.ring-1 {
  width: 100%;
  height: 100%;
  animation: rotate 10s linear infinite;
  border-style: dashed;
}

.ring-2 {
  width: 85%;
  height: 85%;
  animation: rotate 15s linear infinite reverse;
}

.ring-3 {
  width: 70%;
  height: 70%;
  animation: rotate 20s linear infinite;
  border-color: rgba(0, 243, 255, 0.2);
}

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* Progress Arc */
.progress-arc {
  position: absolute;
  width: 90%;
  height: 90%;
  transform: rotate(-90deg);
}

.arc-bg {
  fill: none;
  stroke: rgba(0, 243, 255, 0.1);
  stroke-width: 3;
}

.arc-progress {
  fill: none;
  stroke: #00f3ff;
  stroke-width: 3;
  stroke-linecap: round;
  stroke-dasharray: 534;
  filter: drop-shadow(0 0 10px #00f3ff);
  transition: stroke-dashoffset 0.3s ease;
}

/* Center Display */
.hud-center {
  position: relative;
  z-index: 10;
}

.center-frame {
  width: 180px;
  height: 120px;
  background: rgba(0, 10, 30, 0.9);
  border: 2px solid #00f3ff;
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  clip-path: polygon(15px 0, 100% 0, 100% calc(100% - 15px), calc(100% - 15px) 100%, 0 100%, 0 15px);
  box-shadow: 0 0 30px rgba(0, 243, 255, 0.3), inset 0 0 30px rgba(0, 243, 255, 0.1);
}

.frame-corner {
  position: absolute;
  width: 15px;
  height: 15px;
  border: 2px solid #ffff00;
  box-shadow: 0 0 10px #ffff00;
}

.frame-corner.tl {
  top: -2px;
  left: -2px;
  border-right: none;
  border-bottom: none;
}

.frame-corner.tr {
  top: -2px;
  right: -2px;
  border-left: none;
  border-bottom: none;
}

.frame-corner.bl {
  bottom: -2px;
  left: -2px;
  border-right: none;
  border-top: none;
}

.frame-corner.br {
  bottom: -2px;
  right: -2px;
  border-left: none;
  border-top: none;
}

.progress-text {
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 2.5rem;
  font-weight: 900;
  color: #00f3ff;
  text-shadow: 0 0 20px rgba(0, 243, 255, 0.8);
  line-height: 1;
}

.status-text {
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 0.7rem;
  color: #aaddff;
  letter-spacing: 1px;
  text-align: center;
}

/* Side Indicators */
.side-indicator {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.side-indicator.left {
  left: -40px;
}

.side-indicator.right {
  right: -40px;
}

.indicator-bar {
  width: 25px;
  height: 4px;
  background: rgba(0, 243, 255, 0.2);
  transition: all 0.3s ease;
}

.indicator-bar.active {
  background: #ffff00;
  box-shadow: 0 0 10px #ffff00;
}

/* Bottom Stats */
.hud-stats {
  position: absolute;
  bottom: -80px;
  display: flex;
  gap: 3rem;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.stat-label {
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 0.65rem;
  color: #aaddff;
  letter-spacing: 1px;
}

.stat-bars {
  display: flex;
  gap: 3px;
}

.stat-bar {
  width: 4px;
  height: 20px;
  background: rgba(0, 243, 255, 0.2);
}

.stat-bar.active {
  background: #ffff00;
  box-shadow: 0 0 8px #ffff00;
  animation: barPulse 1s ease-in-out infinite;
}

@keyframes barPulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}

/* Orbiting Dots */
.orbit-dot {
  position: absolute;
  width: 12px;
  height: 12px;
  background: #00f3ff;
  border-radius: 50%;
  box-shadow: 0 0 15px #00f3ff;
}

.dot-1 {
  top: 10%;
  left: 50%;
  animation: orbit1 4s linear infinite;
}

.dot-2 {
  bottom: 10%;
  right: 50%;
  animation: orbit2 4s linear infinite;
}

@keyframes orbit1 {
  from { transform: rotate(0deg) translateX(180px) rotate(0deg); }
  to { transform: rotate(360deg) translateX(180px) rotate(-360deg); }
}

@keyframes orbit2 {
  from { transform: rotate(180deg) translateX(180px) rotate(-180deg); }
  to { transform: rotate(540deg) translateX(180px) rotate(-540deg); }
}

/* Corner Decorations */
.corner-deco {
  position: absolute;
  width: 80px;
  height: 80px;
  border: 2px solid rgba(0, 243, 255, 0.3);
}

.corner-deco.top-left {
  top: 20px;
  left: 20px;
  border-right: none;
  border-bottom: none;
  clip-path: polygon(0 0, 100% 0, 100% 2px, 2px 2px, 2px 100%, 0 100%);
}

.corner-deco.top-right {
  top: 20px;
  right: 20px;
  border-left: none;
  border-bottom: none;
  clip-path: polygon(0 0, 100% 0, 100% 100%, calc(100% - 2px) 100%, calc(100% - 2px) 2px, 0 2px);
}

.corner-deco.bottom-left {
  bottom: 20px;
  left: 20px;
  border-right: none;
  border-top: none;
  clip-path: polygon(0 0, 2px 0, 2px calc(100% - 2px), 100% calc(100% - 2px), 100% 100%, 0 100%);
}

.corner-deco.bottom-right {
  bottom: 20px;
  right: 20px;
  border-left: none;
  border-top: none;
  clip-path: polygon(0 calc(100% - 2px), calc(100% - 2px) calc(100% - 2px), calc(100% - 2px) 0, 100% 0, 100% 100%, 0 100%);
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

