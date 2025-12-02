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

    <div class="terminal-frame">
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

const displayedText = ref('')
const terminalTitle = ref('INITIALIZING SYSTEM...')
const terminalContentRef = ref<HTMLElement | null>(null)
const terminalTextRef = ref<HTMLElement | null>(null)

const isAnimationRunning = ref(false)

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
    if (!isAnimationRunning.value) break
    displayedText.value += text[i]
    await new Promise(resolve => setTimeout(resolve, speed))
    // Auto-scroll to bottom
    if (terminalContentRef.value) {
      terminalContentRef.value.scrollTop = terminalContentRef.value.scrollHeight
    }
  }
}

async function runTerminalAnimation() {
  isAnimationRunning.value = true

  // Loop animation until stop is requested
  while (isAnimationRunning.value && !props.stopRequested) {
    // Type random code snippets
    for (const snippet of codeSnippets) {
      if (!isAnimationRunning.value || props.stopRequested) break
      await typeText(snippet, 8)
    }

    // If we should stop, show completion messages
    if (props.stopRequested) {
      terminalTitle.value = 'ANALYSIS COMPLETE'
      await typeText('\n[✓] SYSTEM DIAGNOSTICS: GREEN\n', 30)
      await typeText('[✓] DATA INTEGRITY: VERIFIED\n', 30)
      await typeText('[✓] REPORT GENERATED\n\n', 30)
      await typeText('>>> REDIRECTING TO DASHBOARD...', 40)
      await new Promise(resolve => setTimeout(resolve, 800))
      break
    }

    // Continue adding more text instead of clearing
    if (isAnimationRunning.value && !props.stopRequested) {
      await typeText('\n', 10)
    }
  }

  isAnimationRunning.value = false
}

// Watch for activation
watch(() => props.isActive, (newVal) => {
  if (newVal && !isAnimationRunning.value) {
    displayedText.value = ''
    terminalTitle.value = 'INITIALIZING SYSTEM...'
    runTerminalAnimation()
  } else if (!newVal) {
    isAnimationRunning.value = false
  }
}, { immediate: true })

onUnmounted(() => {
  isAnimationRunning.value = false
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
  background: #000205;
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
  z-index: 2;
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

.cursor-blink {
  animation: blink 1s step-end infinite;
}

@keyframes blink {
  0%, 49% { opacity: 1; }
  50%, 100% { opacity: 0; }
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
</style>