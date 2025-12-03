<template>
  <div class="logo-overlay">
    <!-- Animated Background -->
    <div class="animated-bg"></div>
    <div class="grid-overlay"></div>
    <div class="particles">
      <div v-for="i in 30" :key="i" class="particle" :style="getParticleStyle(i)"></div>
    </div>
    <!-- Main Logo Container -->
    <div class="logo-container">
      <!-- Outer Rotating Rings -->
      <div class="outer-ring ring-1"></div>
      <div class="outer-ring ring-2"></div>
      <div class="outer-ring ring-3"></div>

      <!-- Energy Pulses -->
      <div class="energy-pulse pulse-1"></div>
      <div class="energy-pulse pulse-2"></div>
      <div class="energy-pulse pulse-3"></div>

      <!-- Logo Holder with Glow -->
      <div class="logo-holder">
        <div class="logo-glow"></div>
        <img
          src="/image-turbo.png"
          alt="Turbo Hackathon Logo"
          class="logo-image"
        />
      </div>

      <!-- Orbiting Data Points -->
      <div class="orbit-container">
        <div v-for="i in 8" :key="'orbit-'+i" class="orbit-dot" :style="getOrbitStyle(i)"></div>
      </div>

      <!-- Hexagon Frame -->
      <div class="hexagon-frame"></div>

      <!-- Corner Brackets -->
      <div class="corner-bracket top-left"></div>
      <div class="corner-bracket top-right"></div>
      <div class="corner-bracket bottom-left"></div>
      <div class="corner-bracket bottom-right"></div>
    </div>

    <!-- Status Display -->
    <div class="status-display">
      <div class="status-bar">
        <div class="status-progress" :style="{ width: progressWidth + '%' }"></div>
      </div>
      <div class="status-text">{{ statusText }}</div>
    </div>

    <!-- Side Data Streams -->
    <div class="data-stream left-stream">
      <div v-for="i in 12" :key="'left-'+i" class="data-line" :class="{ active: i <= Math.floor(progress / 8.33) }"></div>
    </div>
    <div class="data-stream right-stream">
      <div v-for="i in 12" :key="'right-'+i" class="data-line" :class="{ active: i <= Math.floor(progress / 8.33) }"></div>
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

// Orbit dots positioning
function getOrbitStyle(index: number) {
  const angle = (360 / 8) * index
  return {
    '--orbit-angle': `${angle}deg`,
    animationDelay: `${index * 0.2}s`
  }
}

const progress = ref(0)
const progressWidth = ref(0)
const statusText = ref('INITIALIZING SYSTEM...')
let progressInterval: number | null = null

const isAnimationRunning = ref(false)

const statuses = [
  'INITIALIZING SYSTEM...',
  'LOADING NEURAL CORE...',
  'CONNECTING TO AI MATRIX...',
  'ANALYZING DATA STREAMS...',
  'PROCESSING ALGORITHMS...',
  'OPTIMIZING PARAMETERS...',
  'SYNCHRONIZING MODULES...',
  'FINALIZING PROTOCOLS...',
]

function runLogoAnimation() {
  progress.value = 0
  progressWidth.value = 0
  isAnimationRunning.value = true

  let statusIndex = 0
  statusText.value = statuses[0]

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
      progressWidth.value = 100
      statusText.value = 'SYSTEM READY'
      if (progressInterval) {
        clearInterval(progressInterval)
        progressInterval = null
      }
      isAnimationRunning.value = false
      return
    }

    // Continuous progress
    progress.value += Math.random() * 2 + 0.5
    progressWidth.value = Math.min(progress.value, 100)

    // Cycle through status texts
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
    runLogoAnimation()
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
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;700;900&family=Rajdhani:wght@300;400;500;600;700&display=swap');

.logo-overlay {
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

/* Animated Background */
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
  width: 3px;
  height: 3px;
  background: #FA4786;
  border-radius: 50%;
  box-shadow: 0 0 10px #FA4786, 0 0 20px rgba(250, 71, 134, 0.5);
  animation: particleFloat linear infinite;
  opacity: 0.7;
}

@keyframes particleFloat {
  0% {
    transform: translateY(0) translateX(0);
    opacity: 0;
  }
  10% {
    opacity: 0.7;
  }
  90% {
    opacity: 0.7;
  }
  100% {
    transform: translateY(-100vh) translateX(50px);
    opacity: 0;
  }
}

/* Main Logo Container */
.logo-container {
  width: 500px;
  height: 500px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2;
}

/* Outer Rotating Rings */
.outer-ring {
  position: absolute;
  border-radius: 50%;
  border: 2px solid;
  animation: rotateRing linear infinite;
}

.ring-1 {
  width: 100%;
  height: 100%;
  border-color: rgba(250, 71, 134, 0.3);
  border-style: dashed;
  animation-duration: 15s;
}

.ring-2 {
  width: 85%;
  height: 85%;
  border-color: rgba(107, 140, 255, 0.3);
  animation-duration: 20s;
  animation-direction: reverse;
}

.ring-3 {
  width: 70%;
  height: 70%;
  border-color: rgba(250, 71, 134, 0.2);
  border-style: dotted;
  animation-duration: 25s;
}

@keyframes rotateRing {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* Energy Pulses */
.energy-pulse {
  position: absolute;
  width: 60%;
  height: 60%;
  border-radius: 50%;
  border: 2px solid;
  animation: energyPulse 3s ease-in-out infinite;
}

.pulse-1 {
  border-color: rgba(250, 71, 134, 0.6);
  animation-delay: 0s;
}

.pulse-2 {
  border-color: rgba(107, 140, 255, 0.6);
  animation-delay: 1s;
}

.pulse-3 {
  border-color: rgba(250, 71, 134, 0.4);
  animation-delay: 2s;
}

@keyframes energyPulse {
  0% {
    transform: scale(0.8);
    opacity: 0;
  }
  50% {
    opacity: 1;
  }
  100% {
    transform: scale(1.5);
    opacity: 0;
  }
}

/* Logo Holder */
.logo-holder {
  width: 250px;
  height: 250px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: logoFloat 4s ease-in-out infinite;
  z-index: 10;
}

@keyframes logoFloat {
  0%, 100% {
    transform: translateY(0) rotate(0deg);
  }
  25% {
    transform: translateY(-10px) rotate(5deg);
  }
  50% {
    transform: translateY(0) rotate(0deg);
  }
  75% {
    transform: translateY(-10px) rotate(-5deg);
  }
}

.logo-glow {
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(250, 71, 134, 0.4) 0%, rgba(107, 140, 255, 0.3) 50%, transparent 70%);
  border-radius: 50%;
  animation: glowPulse 2s ease-in-out infinite;
  filter: blur(20px);
}

@keyframes glowPulse {
  0%, 100% {
    transform: scale(1);
    opacity: 0.6;
  }
  50% {
    transform: scale(1.2);
    opacity: 1;
  }
}

.logo-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
  filter: drop-shadow(0 0 20px rgba(250, 71, 134, 0.8)) drop-shadow(0 0 30px rgba(107, 140, 255, 0.6));
  animation: logoFloat3D 6s ease-in-out infinite;
}

@keyframes logoFloat3D {
  0%, 100% {
    transform: perspective(1000px) rotateY(0deg) rotateX(0deg) scale(1);
  }
  25% {
    transform: perspective(1000px) rotateY(10deg) rotateX(5deg) scale(1.05);
  }
  50% {
    transform: perspective(1000px) rotateY(0deg) rotateX(0deg) scale(1);
  }
  75% {
    transform: perspective(1000px) rotateY(-10deg) rotateX(-5deg) scale(1.05);
  }
}

/* Orbiting Data Points */
.orbit-container {
  position: absolute;
  width: 100%;
  height: 100%;
}

.orbit-dot {
  position: absolute;
  width: 12px;
  height: 12px;
  background: linear-gradient(135deg, #FA4786, #6B8CFF);
  border-radius: 50%;
  box-shadow: 0 0 15px #FA4786, 0 0 25px rgba(107, 140, 255, 0.6);
  top: 50%;
  left: 50%;
  transform-origin: 0 0;
  animation: orbitRotate 8s linear infinite;
}

@keyframes orbitRotate {
  from {
    transform: rotate(var(--orbit-angle)) translateX(220px) rotate(calc(-1 * var(--orbit-angle)));
  }
  to {
    transform: rotate(calc(var(--orbit-angle) + 360deg)) translateX(220px) rotate(calc(-1 * (var(--orbit-angle) + 360deg)));
  }
}

/* Hexagon Frame */
.hexagon-frame {
  position: absolute;
  width: 350px;
  height: 350px;
  clip-path: polygon(50% 0%, 100% 25%, 100% 75%, 50% 100%, 0% 75%, 0% 25%);
  border: 2px solid rgba(250, 71, 134, 0.3);
  animation: hexagonPulse 4s ease-in-out infinite;
}

@keyframes hexagonPulse {
  0%, 100% {
    transform: scale(1) rotate(0deg);
    opacity: 0.3;
  }
  50% {
    transform: scale(1.05) rotate(180deg);
    opacity: 0.6;
  }
}

/* Corner Brackets */
.corner-bracket {
  position: absolute;
  width: 60px;
  height: 60px;
  border: 3px solid #FA4786;
  box-shadow: 0 0 15px rgba(250, 71, 134, 0.6);
}

.corner-bracket.top-left {
  top: 20px;
  left: 20px;
  border-right: none;
  border-bottom: none;
  animation: bracketGlow 2s ease-in-out infinite;
}

.corner-bracket.top-right {
  top: 20px;
  right: 20px;
  border-left: none;
  border-bottom: none;
  animation: bracketGlow 2s ease-in-out infinite 0.5s;
}

.corner-bracket.bottom-left {
  bottom: 20px;
  left: 20px;
  border-right: none;
  border-top: none;
  animation: bracketGlow 2s ease-in-out infinite 1s;
}

.corner-bracket.bottom-right {
  bottom: 20px;
  right: 20px;
  border-left: none;
  border-top: none;
  animation: bracketGlow 2s ease-in-out infinite 1.5s;
}

@keyframes bracketGlow {
  0%, 100% {
    box-shadow: 0 0 15px rgba(250, 71, 134, 0.6);
    border-color: #FA4786;
  }
  50% {
    box-shadow: 0 0 25px rgba(107, 140, 255, 0.8);
    border-color: #6B8CFF;
  }
}

/* Status Display */
.status-display {
  position: absolute;
  bottom: 100px;
  width: 500px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  z-index: 3;
}

.status-bar {
  width: 100%;
  height: 6px;
  background: rgba(250, 71, 134, 0.2);
  border-radius: 3px;
  overflow: hidden;
  box-shadow: inset 0 0 10px rgba(0, 0, 0, 0.5);
}

.status-progress {
  height: 100%;
  background: linear-gradient(90deg, #FA4786, #002D72, #6B8CFF);
  border-radius: 3px;
  box-shadow: 0 0 15px rgba(250, 71, 134, 0.8), 0 0 25px rgba(107, 140, 255, 0.6);
  transition: width 0.3s ease;
  animation: progressShimmer 2s linear infinite;
}

@keyframes progressShimmer {
  0% {
    filter: brightness(1);
  }
  50% {
    filter: brightness(1.3);
  }
  100% {
    filter: brightness(1);
  }
}

.status-text {
  font-family: 'Orbitron', 'Rajdhani', monospace;
  font-size: 1rem;
  font-weight: 600;
  color: #FFB3D1;
  letter-spacing: 3px;
  text-align: center;
  text-shadow:
    0 0 10px rgba(255, 179, 209, 0.6),
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 30px rgba(107, 140, 255, 0.3);
  animation: textFlicker 3s ease-in-out infinite;
}

@keyframes textFlicker {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.8;
  }
}

.status-percentage {
  font-family: 'Rajdhani', monospace;
  font-size: 2rem;
  font-weight: 700;
  color: #6B8CFF;
  text-shadow:
    0 0 15px rgba(107, 140, 255, 0.8),
    0 0 25px rgba(250, 71, 134, 0.5);
}

/* Side Data Streams */
.data-stream {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  flex-direction: column;
  gap: 8px;
  z-index: 2;
}

.left-stream {
  left: 50px;
}

.right-stream {
  right: 50px;
}

.data-line {
  width: 40px;
  height: 3px;
  background: rgba(250, 71, 134, 0.2);
  transition: all 0.3s ease;
}

.data-line.active {
  background: linear-gradient(90deg, #FA4786, #6B8CFF);
  box-shadow: 0 0 10px #FA4786, 0 0 15px rgba(107, 140, 255, 0.6);
  animation: dataFlow 1s ease-in-out infinite;
}

@keyframes dataFlow {
  0%, 100% {
    opacity: 1;
    transform: scaleX(1);
  }
  50% {
    opacity: 0.6;
    transform: scaleX(1.2);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .logo-container {
    width: 350px;
    height: 350px;
  }

  .logo-holder {
    width: 180px;
    height: 180px;
  }

  .hexagon-frame {
    width: 250px;
    height: 250px;
  }

  .status-display {
    width: 90%;
    bottom: 80px;
  }

  .status-text {
    font-size: 0.85rem;
    letter-spacing: 2px;
  }

  .status-percentage {
    font-size: 1.5rem;
  }

  .data-stream {
    display: none;
  }

  .corner-bracket {
    width: 40px;
    height: 40px;
  }
}
</style>