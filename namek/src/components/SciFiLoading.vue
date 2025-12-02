<template>
  <div class="terminal-overlay">
    <!-- Animated Background - Same as Submit Page -->
    <div class="animated-bg"></div>
    <div class="grid-overlay"></div>
    <div class="particles">
      <div v-for="i in 20" :key="i" class="particle" :style="getParticleStyle(i)"></div>
    </div>
    <div class="holo-lines">
      <div v-for="i in 5" :key="i" class="holo-line"></div>
    </div>

    <div class="scifi-hud">
      <div class="hud-background"></div>

      <!-- Main Circle -->
      <div class="hud-main-circle">
        <!-- Rotating Rings -->
        <div class="hud-ring ring-1"></div>
        <div class="hud-ring ring-2"></div>
        <div class="hud-ring ring-3"></div>

        <!-- Progress Arc -->
        <svg class="progress-arc" viewBox="0 0 200 200">
          <defs>
            <linearGradient id="pinkBlueGradient" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" style="stop-color:#FA4786;stop-opacity:1" />
              <stop offset="50%" style="stop-color:#B84FD1;stop-opacity:1" />
              <stop offset="100%" style="stop-color:#6B8CFF;stop-opacity:1" />
            </linearGradient>
          </defs>
          <circle cx="100" cy="100" r="85" class="arc-bg"/>
          <circle cx="100" cy="100" r="85" class="arc-progress"/>
        </svg>

        <!-- Center Display -->
        <div class="hud-center">
          <div class="center-frame">
            <div class="frame-corner tl"></div>
            <div class="frame-corner tr"></div>
            <div class="frame-corner bl"></div>
            <div class="frame-corner br"></div>
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
</template>

<script setup lang="ts">
import { ref, watch, onUnmounted } from 'vue'

const props = defineProps<{
  isActive: boolean
  stopRequested: boolean
}>()

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

const progress = ref(0)
const statusText = ref('INITIALIZING...')
let progressInterval: number | null = null

const isAnimationRunning = ref(false)

const statuses = [
  'INITIALIZING...',
  'CONNECTING TO AI CORE...',
  'ANALYZING DATA...',
  'PROCESSING NEURAL NET...',
  'OPTIMIZING ALGORITHMS...',
  'FINALIZING RESULTS...',
]

function runSciFiAnimation() {
  progress.value = 0
  isAnimationRunning.value = true

  let statusIndex = 0
  statusText.value = statuses[0]

  // Progress animation - loop until stop is requested
  progressInterval = window.setInterval(() => {
    if (!isAnimationRunning.value) {
      if (progressInterval) {
        clearInterval(progressInterval)
        progressInterval = null
      }
      return
    }

    // If stop requested and we're at 100%, complete
    if (props.stopRequested && progress.value >= 100) {
      progress.value = 100
      statusText.value = 'COMPLETE'
      if (progressInterval) {
        clearInterval(progressInterval)
        progressInterval = null
      }
      isAnimationRunning.value = false
      return
    }

    // Continuous progress - keep incrementing without limit
    progress.value += Math.random() * 2 + 0.5

    // Cycle through status texts continuously
    const cycleIndex = Math.floor(progress.value / (100 / statuses.length)) % statuses.length
    if (cycleIndex !== statusIndex) {
      statusIndex = cycleIndex
      statusText.value = statuses[statusIndex]
    }
  }, 100)
}

// Watch for activation
watch(() => props.isActive, (newVal) => {
  if (newVal && !isAnimationRunning.value) {
    runSciFiAnimation()
  } else if (!newVal) {
    isAnimationRunning.value = false
    if (progressInterval) {
      clearInterval(progressInterval)
      progressInterval = null
    }
  }
}, { immediate: true })

onUnmounted(() => {
  isAnimationRunning.value = false
  if (progressInterval) {
    clearInterval(progressInterval)
    progressInterval = null
  }
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;700;900&family=Chakra+Petch:wght@300;400;500;600;700&display=swap');

.terminal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #0a0015 0%, #1a0520 50%, #000510 100%);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

/* Animated Background - Same as Submit Page */
.animated-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle at 50% 50%, #2a0a3a 0%, #0a0015 100%);
  z-index: 0;
}

.grid-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 200%;
  height: 200%;
  background-image:
    linear-gradient(rgba(250, 71, 134, 0.08) 1px, transparent 1px),
    linear-gradient(90deg, rgba(107, 140, 255, 0.05) 1px, transparent 1px);
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
  background: #FA4786;
  border-radius: 50%;
  box-shadow: 0 0 10px #FA4786, 0 0 20px rgba(250, 71, 134, 0.5);
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
  background: linear-gradient(90deg, transparent, #FA4786, #6B8CFF, transparent);
  box-shadow: 0 0 10px #FA4786, 0 0 15px rgba(107, 140, 255, 0.5);
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

.scifi-hud {
  width: 600px;
  height: 600px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2;
}

.hud-background {
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(250, 71, 134, 0.2) 0%, rgba(107, 140, 255, 0.1) 50%, transparent 70%);
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
  border: 2px solid rgba(250, 71, 134, 0.4);
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
  border-color: rgba(107, 140, 255, 0.3);
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
  stroke: rgba(250, 71, 134, 0.15);
  stroke-width: 3;
}

.arc-progress {
  fill: none;
  stroke: url(#pinkBlueGradient);
  stroke-width: 3;
  stroke-linecap: round;
  stroke-dasharray: 200 534;
  stroke-dashoffset: 0;
  filter: drop-shadow(0 0 10px #FA4786) drop-shadow(0 0 15px rgba(107, 140, 255, 0.5));
  animation: arcFlow 3s linear infinite;
}

@keyframes arcFlow {
  0% {
    stroke-dashoffset: 0;
  }
  100% {
    stroke-dashoffset: -534;
  }
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
  border: 2px solid #FA4786;
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  clip-path: polygon(15px 0, 100% 0, 100% calc(100% - 15px), calc(100% - 15px) 100%, 0 100%, 0 15px);
  box-shadow: 0 0 30px rgba(250, 71, 134, 0.4), 0 0 20px rgba(107, 140, 255, 0.3), inset 0 0 30px rgba(250, 71, 134, 0.1);
}

.frame-corner {
  position: absolute;
  width: 15px;
  height: 15px;
  border: 2px solid #6B8CFF;
  box-shadow: 0 0 10px #6B8CFF, 0 0 15px rgba(107, 140, 255, 0.5);
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

.status-text {
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 0.7rem;
  color: #FFB3D1;
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
  background: rgba(250, 71, 134, 0.2);
  transition: all 0.3s ease;
}

.indicator-bar.active {
  background: linear-gradient(135deg, #FA4786, #6B8CFF);
  box-shadow: 0 0 10px #FA4786, 0 0 15px rgba(107, 140, 255, 0.5);
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
  color: #FFB3D1;
  letter-spacing: 1px;
}

.stat-bars {
  display: flex;
  gap: 3px;
}

.stat-bar {
  width: 4px;
  height: 20px;
  background: rgba(250, 71, 134, 0.2);
}

.stat-bar.active {
  background: linear-gradient(135deg, #FA4786, #6B8CFF);
  box-shadow: 0 0 8px #FA4786, 0 0 12px rgba(107, 140, 255, 0.5);
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
  background: radial-gradient(circle, #FA4786, #6B8CFF);
  border-radius: 50%;
  box-shadow: 0 0 15px #FA4786, 0 0 20px rgba(107, 140, 255, 0.5);
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
  border: 2px solid rgba(250, 71, 134, 0.3);
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
}
</style>