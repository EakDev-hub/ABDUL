<template>
  <div class="hackathon-container" :class="{ 'page-exit': isExiting }">
    <!-- Animated Background -->
    <div class="animated-bg"></div>
    <div class="grid-overlay"></div>
    <div class="particles">
      <div v-for="i in 20" :key="i" class="particle" :style="getParticleStyle(i)"></div>
    </div>
    <div class="holo-lines">
      <div v-for="i in 5" :key="i" class="holo-line"></div>
    </div>

    <!-- Transition Overlay -->
    <div v-if="isExiting" class="transition-overlay">
      <div class="warp-lines">
        <div v-for="i in 20" :key="i" class="warp-line" :style="getWarpLineStyle(i)"></div>
      </div>
      <div class="transition-text">RETURNING TO TERMINAL...</div>
    </div>

    <!-- Floating Reset Button -->
    <button v-if="result" class="floating-reset-button" @click="goBack">
      <svg class="button-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M19 12H5M5 12l7 7M5 12l7-7" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
      <span class="button-text">RESUBMIT</span>
    </button>

    <!-- Main Content -->
    <div class="main-content">
      <!-- Results Section -->
      <div v-if="result" class="results-container">
        <!-- Summary -->
        <div class="summary-card">
          <!-- Card Header with Badge -->
          <div class="card-header">
            <div class="header-title">
              <svg class="title-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M9 11l3 3L22 4"/>
                <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/>
              </svg>
              <span>RESULTS SUMMARY</span>
            </div>
            <div class="header-badge">
              <span class="pass-badge" :class="`badge-${result.passKeyType}`">
                {{ result.passKeyType.toUpperCase() }}
              </span>
            </div>
          </div>

          <div class="summary-grid">
            <div class="stat-box">
              <p class="stat-label">จำนวนข้อที่ทำได้</p>
              <p class="stat-value">
                {{ result.answeredQuestion }} / {{ result.totalQuestion }} <span class="stat-unit">ข้อ</span>
              </p>
            </div>
            <div class="stat-box">
              <p class="stat-label">คะแนนที่ได้</p>
              <p class="stat-value stat-highlight">
                {{ result.score.toFixed(2) }} / {{ result.maximumScore }}
              </p>
            </div>
            <div class="stat-box">
              <p class="stat-label">Max Duration</p>
              <p class="stat-value stat-duration">
                {{ result.maxDurationInSecs }} <span class="stat-unit">วินาที</span>
              </p>
            </div>
          </div>

          <!-- UUID Section -->
          <div class="uuid-section">
            <span class="uuid-label">SUBMISSION ID:</span>
            <span class="stat-uuid">{{ result.uuid }}</span>
          </div>
        </div>

        <!-- Results Table -->
        <div class="results-table-card">
          <div class="table-container">
            <!-- Table Header -->
            <div class="table-header">
              <div class="table-cell header-cell cell-no">No.</div>
              <div class="table-cell header-cell cell-question">Question</div>
              <div class="table-cell header-cell cell-expected">Expected Answer</div>
              <div class="table-cell header-cell cell-answer">Answer</div>
              <div class="table-cell header-cell cell-score">Score<br/>(0.0 - 1.0)</div>
            </div>

            <!-- Table Body -->
            <div class="table-body">
              <div v-for="item in result.results" :key="item.no" class="table-row">
                <div class="table-cell cell-no">{{ item.no }}</div>
                <div class="table-cell cell-question">{{ item.question }}</div>
                <div class="table-cell cell-expected">{{ item.expectedAnswer }}</div>
                <div class="table-cell cell-answer">{{ item.actualAnswer }}</div>
                <div class="table-cell cell-score">
                  <span class="score-badge" :class="getScoreClass(item.score)">
                    {{ item.score.toFixed(1) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- No Results Message -->
      <div v-else class="no-results">
        <p class="no-results-text">ไม่พบผลลัพธ์</p>
        <button class="no-results-button" @click="goBack">
          <svg class="no-results-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
            <polyline points="9 22 9 12 15 12 15 22"/>
          </svg>
          <span>กลับหน้าแรก</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useHackathonStore } from '@/store/hackathon.store'

const router = useRouter()
const hackathonStore = useHackathonStore()

const result = computed(() => hackathonStore.result)
const errorState = computed(() => hackathonStore.errorState)

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

function getScoreClass(score: number): string {
  if (score >= 0.8) return 'score-high'
  if (score >= 0.5) return 'score-medium'
  return 'score-low'
}

const isExiting = ref(false)

function getWarpLineStyle(index: number) {
  const randomY = Math.random() * 100
  const randomDelay = Math.random() * 0.3
  const randomDuration = 0.5 + Math.random() * 0.5

  return {
    top: `${randomY}%`,
    animationDelay: `${randomDelay}s`,
    animationDuration: `${randomDuration}s`
  }
}

function goBack() {
  isExiting.value = true
  setTimeout(() => {
    hackathonStore.clearResult()
    router.push('/hackathon')
  }, 1500)
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Chakra+Petch:wght@300;400;500;600;700&family=Orbitron:wght@400;500;700;900&family=Rajdhani:wght@300;500;700&family=Share+Tech+Mono&display=swap');

.hackathon-container {
  min-height: 100vh;
  position: relative;
  background: #000205;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  overflow-x: hidden;
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

/* Transition Overlay - Enhanced Dramatic Effect */
.transition-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 2, 5, 0);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: overlayDarkFadeIn 3s ease forwards;
  pointer-events: none;
}

@keyframes overlayDarkFadeIn {
  0% {
    background: rgba(0, 2, 5, 0.98);
  }
  100% {
    background: rgba(0, 2, 5, 0.98);
  }
}

.warp-lines {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 1;
}

.warp-line {
  position: absolute;
  left: -200%;
  width: 300%;
  height: 3px;
  background: linear-gradient(90deg,
    transparent 0%,
    transparent 20%,
    rgba(250, 71, 134, 0.3) 35%,
    rgba(250, 71, 134, 1) 45%,
    rgba(107, 140, 255, 1) 55%,
    rgba(107, 140, 255, 0.3) 65%,
    transparent 80%,
    transparent 100%
  );
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.8),
    0 0 40px rgba(250, 71, 134, 0.6),
    0 0 60px rgba(250, 71, 134, 0.4),
    0 0 30px rgba(107, 140, 255, 0.5);
  animation: none;
  filter: blur(0.5px);
}

/* Different speeds and timings for each line */
.warp-line:nth-child(1) { animation-delay: 3s; height: 2px; }
.warp-line:nth-child(2) { animation-delay: 3.03s; height: 3px; }
.warp-line:nth-child(3) { animation-delay: 3.06s; height: 2px; }
.warp-line:nth-child(4) { animation-delay: 3.09s; height: 4px; }
.warp-line:nth-child(5) { animation-delay: 3.12s; height: 2px; }
.warp-line:nth-child(6) { animation-delay: 3.15s; height: 3px; }
.warp-line:nth-child(7) { animation-delay: 3.18s; height: 2px; }
.warp-line:nth-child(8) { animation-delay: 3.21s; height: 5px; }
.warp-line:nth-child(9) { animation-delay: 3.24s; height: 2px; }
.warp-line:nth-child(10) { animation-delay: 3.27s; height: 3px; }
.warp-line:nth-child(11) { animation-delay: 3.05s; height: 2px; }
.warp-line:nth-child(12) { animation-delay: 3.08s; height: 4px; }
.warp-line:nth-child(13) { animation-delay: 3.11s; height: 2px; }
.warp-line:nth-child(14) { animation-delay: 3.14s; height: 3px; }
.warp-line:nth-child(15) { animation-delay: 3.17s; height: 2px; }
.warp-line:nth-child(16) { animation-delay: 3.20s; height: 5px; }
.warp-line:nth-child(17) { animation-delay: 3.23s; height: 2px; }
.warp-line:nth-child(18) { animation-delay: 3.26s; height: 3px; }
.warp-line:nth-child(19) { animation-delay: 3.29s; height: 2px; }
.warp-line:nth-child(20) { animation-delay: 3.32s; height: 4px; }

@keyframes warpSpeed {
  0% {
    left: -200%;
    opacity: 0;
    transform: scaleX(0.5);
  }
  20% {
    opacity: 1;
    transform: scaleX(1);
  }
  80% {
    opacity: 1;
    transform: scaleX(1.2);
  }
  100% {
    left: 200%;
    opacity: 0;
    transform: scaleX(2);
  }
}

.transition-text {
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 2.5rem;
  font-weight: 900;
  color: #FA4786;
  text-shadow:
    0 0 20px rgba(250, 71, 134, 1),
    0 0 40px rgba(250, 71, 134, 0.8),
    0 0 60px rgba(250, 71, 134, 0.6);
  letter-spacing: 5px;
  animation:
    textGlitchIntense 0.15s ease infinite,
    textPulse 1s ease-in-out infinite;
  z-index: 2;
  position: relative;
  text-transform: uppercase;
}

.transition-text::before,
.transition-text::after {
  content: 'RETURNING TO TERMINAL...';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0.8;
}

.transition-text::before {
  color: #ff00ff;
  animation: glitchBefore 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94) infinite;
  text-shadow:
    -2px 0 rgba(255, 0, 255, 0.8),
    0 0 20px rgba(255, 0, 255, 0.6);
}

.transition-text::after {
  color: #00ffff;
  animation: glitchAfter 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94) infinite reverse;
  text-shadow:
    2px 0 rgba(0, 255, 255, 0.8),
    0 0 20px rgba(0, 255, 255, 0.6);
}

@keyframes textGlitchIntense {
  0%, 100% {
    transform: translate(0, 0) skew(0deg);
  }
  20% {
    transform: translate(-3px, 2px) skew(-2deg);
  }
  40% {
    transform: translate(3px, -2px) skew(2deg);
  }
  60% {
    transform: translate(-2px, -1px) skew(1deg);
  }
  80% {
    transform: translate(2px, 1px) skew(-1deg);
  }
}

@keyframes glitchBefore {
  0%, 100% {
    clip-path: inset(0 0 0 0);
    transform: translateX(0);
  }
  20% {
    clip-path: inset(20% 0 60% 0);
    transform: translateX(-5px);
  }
  40% {
    clip-path: inset(60% 0 20% 0);
    transform: translateX(5px);
  }
  60% {
    clip-path: inset(40% 0 40% 0);
    transform: translateX(-3px);
  }
  80% {
    clip-path: inset(10% 0 80% 0);
    transform: translateX(3px);
  }
}

@keyframes glitchAfter {
  0%, 100% {
    clip-path: inset(0 0 0 0);
    transform: translateX(0);
  }
  20% {
    clip-path: inset(40% 0 40% 0);
    transform: translateX(5px);
  }
  40% {
    clip-path: inset(10% 0 70% 0);
    transform: translateX(-5px);
  }
  60% {
    clip-path: inset(70% 0 10% 0);
    transform: translateX(3px);
  }
  80% {
    clip-path: inset(30% 0 50% 0);
    transform: translateX(-3px);
  }
}

@keyframes textPulse {
  0%, 100% {
    filter: brightness(1);
  }
  50% {
    filter: brightness(1.3);
  }
}

.page-exit {
  animation: none;
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
  box-shadow: 0 0 10px #FA4786;
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

/* Floating Reset Button - Enhanced Epic Effects */
.floating-reset-button {
  position: fixed;
  top: 2rem;
  right: 2rem;
  z-index: 1000;
  padding: 1rem 2rem;
  background: rgba(10, 0, 10, 0.95);
  border: 2px solid #FA4786;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  font-size: 1rem;
  color: #FA4786;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.8rem;
  letter-spacing: 2px;
  text-transform: uppercase;
  clip-path: polygon(12px 0, 100% 0, 100% calc(100% - 12px), calc(100% - 12px) 100%, 0 100%, 0 12px);
  backdrop-filter: blur(15px);
  transition: all 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  box-shadow:
    0 0 25px rgba(250, 71, 134, 0.4),
    0 0 50px rgba(250, 71, 134, 0.2),
    inset 0 0 20px rgba(250, 71, 134, 0.05);
  animation: floatingPulse 3s ease-in-out infinite;
  overflow: hidden;
}

.floating-reset-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(250, 71, 134, 0.15), rgba(184, 79, 209, 0.15));
  opacity: 0;
  transition: opacity 0.4s ease;
  z-index: 0;
}

.floating-reset-button::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(250, 71, 134, 0.6), transparent 70%);
  transform: translate(-50%, -50%);
  opacity: 0;
  transition: all 0.6s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  z-index: 0;
}

.floating-reset-button:hover {
  transform: translateY(-5px) scale(1.05);
  box-shadow:
    0 0 40px rgba(250, 71, 134, 0.7),
    0 0 80px rgba(250, 71, 134, 0.5),
    0 0 120px rgba(250, 71, 134, 0.3),
    inset 0 0 30px rgba(250, 71, 134, 0.1);
  border-color: #FF6BA3;
  letter-spacing: 3px;
}

.floating-reset-button:hover::before {
  opacity: 1;
}

.floating-reset-button:hover::after {
  width: 400px;
  height: 400px;
  opacity: 0.3;
}

.floating-reset-button:active {
  transform: translateY(-3px) scale(0.98);
  animation:
    buttonGlitchIntense 0.4s ease,
    buttonShockwave 0.6s ease;
}

.floating-reset-button:active::after {
  width: 500px;
  height: 500px;
  opacity: 0;
  transition: all 0.4s ease;
}

@keyframes buttonGlitchIntense {
  0%, 100% {
    transform: translateY(-3px) scale(0.98);
    filter: hue-rotate(0deg);
  }
  10% {
    transform: translateY(-3px) scale(0.98) translateX(-4px) skew(-2deg);
    filter: hue-rotate(90deg);
  }
  20% {
    transform: translateY(-3px) scale(0.98) translateX(4px) skew(2deg);
    filter: hue-rotate(-90deg);
  }
  30% {
    transform: translateY(-3px) scale(0.98) translateX(-2px) skew(1deg);
    filter: hue-rotate(45deg);
  }
  40% {
    transform: translateY(-3px) scale(0.98) translateX(2px) skew(-1deg);
    filter: hue-rotate(-45deg);
  }
  50% {
    transform: translateY(-3px) scale(0.98);
    filter: hue-rotate(0deg);
  }
}

@keyframes buttonShockwave {
  0% {
    box-shadow:
      0 0 40px rgba(250, 71, 134, 0.7),
      0 0 80px rgba(250, 71, 134, 0.5);
  }
  50% {
    box-shadow:
      0 0 80px rgba(250, 71, 134, 1),
      0 0 160px rgba(250, 71, 134, 0.8),
      0 0 240px rgba(250, 71, 134, 0.6);
  }
  100% {
    box-shadow:
      0 0 40px rgba(250, 71, 134, 0.7),
      0 0 80px rgba(250, 71, 134, 0.5);
  }
}

.floating-reset-button .button-icon {
  width: 20px;
  height: 20px;
  filter: drop-shadow(0 0 8px #FA4786);
  transition: all 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  position: relative;
  z-index: 1;
}

.floating-reset-button:hover .button-icon {
  transform: translateX(-5px) rotate(-10deg);
  filter: drop-shadow(0 0 15px #FA4786) drop-shadow(0 0 25px #FA4786);
  animation: iconFloat 0.6s ease-in-out infinite;
}

.floating-reset-button:active .button-icon {
  animation: iconPulseIntense 0.4s ease;
}

@keyframes iconFloat {
  0%, 100% {
    transform: translateX(-5px) rotate(-10deg) translateY(0);
  }
  50% {
    transform: translateX(-5px) rotate(-10deg) translateY(-3px);
  }
}

@keyframes iconPulseIntense {
  0%, 100% {
    transform: translateX(-5px) scale(1) rotate(-10deg);
  }
  25% {
    transform: translateX(-8px) scale(1.3) rotate(-15deg);
  }
  50% {
    transform: translateX(-3px) scale(0.9) rotate(-5deg);
  }
  75% {
    transform: translateX(-7px) scale(1.2) rotate(-12deg);
  }
}

@keyframes floatingPulse {
  0%, 100% {
    box-shadow:
      0 0 25px rgba(250, 71, 134, 0.4),
      0 0 50px rgba(250, 71, 134, 0.2),
      inset 0 0 20px rgba(250, 71, 134, 0.05);
  }
  50% {
    box-shadow:
      0 0 35px rgba(250, 71, 134, 0.6),
      0 0 70px rgba(250, 71, 134, 0.4),
      0 0 100px rgba(250, 71, 134, 0.2),
      inset 0 0 30px rgba(250, 71, 134, 0.08);
  }
}

.button-text {
  position: relative;
  z-index: 1;
  transition: all 0.3s ease;
}

.floating-reset-button:hover .button-text {
  text-shadow:
    0 0 10px rgba(250, 71, 134, 0.8),
    0 0 20px rgba(250, 71, 134, 0.6);
}

.main-content {
  position: relative;
  z-index: 2;
  padding: 2rem;
  padding-top: 5rem;
  min-height: 100vh;
}

/* Results */
.results-container {
  max-width: 1400px;
  margin: 0 auto;
}

.summary-card {
  background: rgba(10, 0, 10, 0.85);
  border: 1px solid rgba(250, 71, 134, 0.3);
  padding: 0;
  margin-bottom: 2.5rem;
  position: relative;
  backdrop-filter: blur(15px);
  clip-path: polygon(
    25px 0, 100% 0,
    100% calc(100% - 25px), calc(100% - 25px) 100%,
    0 100%, 0 25px
  );
  box-shadow:
    0 0 40px rgba(250, 71, 134, 0.15),
    inset 0 0 80px rgba(250, 71, 134, 0.03);
  animation: cardGlow 3s ease-in-out infinite;
  overflow: hidden;
}

.summary-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  border: 2px solid transparent;
  background: linear-gradient(135deg, #FA4786, #002D72, #6B8CFF) border-box;
  -webkit-mask: linear-gradient(#fff 0 0) padding-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask: linear-gradient(#fff 0 0) padding-box, linear-gradient(#fff 0 0);
  mask-composite: exclude;
  opacity: 0.4;
  pointer-events: none;
}

.summary-card::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 25px;
  height: 25px;
  border-top: 3px solid #FA4786;
  border-left: 3px solid #FA4786;
  box-shadow: 0 0 15px rgba(250, 71, 134, 0.6);
}

@keyframes cardGlow {
  0%, 100% { box-shadow: 0 0 40px rgba(250, 71, 134, 0.15), inset 0 0 80px rgba(250, 71, 134, 0.03); }
  50% { box-shadow: 0 0 60px rgba(250, 71, 134, 0.25), inset 0 0 100px rgba(250, 71, 134, 0.05); }
}

/* Card Header */
.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem 2.5rem;
  background: rgba(250, 71, 134, 0.05);
  border-bottom: 1px solid rgba(250, 71, 134, 0.2);
}

.header-title {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  font-size: 1.1rem;
  color: #FA4786;
  letter-spacing: 2px;
  text-transform: uppercase;
}

.title-icon {
  width: 24px;
  height: 24px;
  filter: drop-shadow(0 0 8px #FA4786);
}

.header-badge {
  display: flex;
  align-items: center;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  padding: 2.5rem;
}

.stat-box {
  text-align: center;
}

.stat-label {
  color: #FFB3D1;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 0.95rem;
  margin-bottom: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  font-weight: 600;
}

.stat-value {
  color: #fff;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 2.8rem;
  font-weight: 900;
  background: linear-gradient(180deg, #fff, #FA4786);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 20px rgba(250, 71, 134, 0.5));
  animation: statPulse 2s ease-in-out infinite;
}

@keyframes statPulse {
  0%, 100% { filter: drop-shadow(0 0 20px rgba(250, 71, 134, 0.5)); }
  50% { filter: drop-shadow(0 0 30px rgba(250, 71, 134, 0.8)); }
}

.stat-highlight {
  background: linear-gradient(180deg, #FA4786, #002D72, #6B8CFF);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 25px rgba(250, 71, 134, 0.8));
}

.stat-duration {
  background: linear-gradient(180deg, #ffcc00, #ff6600);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 20px rgba(255, 153, 0, 0.6));
}

.stat-unit {
  font-size: 1.3rem;
  color: #90e0ef;
}

/* UUID Section */
.uuid-section {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 1.5rem 2.5rem;
  padding-top: 0;
  flex-wrap: wrap;
}

.uuid-label {
  color: #FFB3D1;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 0.85rem;
  letter-spacing: 1.5px;
  font-weight: 600;
}

.stat-uuid {
  color: #FA4786;
  font-family: 'Orbitron', 'Chakra Petch', monospace;
  font-size: 0.95rem;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.5);
  padding: 0.5rem 1rem;
  background: rgba(250, 71, 134, 0.08);
  border: 1px solid rgba(250, 71, 134, 0.3);
  border-radius: 4px;
}

.pass-badge {
  padding: 0.5rem 1.5rem;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  font-size: 0.85rem;
  letter-spacing: 2px;
  clip-path: polygon(6px 0, 100% 0, calc(100% - 6px) 100%, 0 100%);
  border: 1px solid;
  box-shadow: 0 0 15px;
  animation: badgePulse 2s ease-in-out infinite;
}

@keyframes badgePulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

.badge-develop {
  background: rgba(0, 255, 136, 0.15);
  color: #00ff88;
  border-color: #00ff88;
  box-shadow: 0 0 15px rgba(0, 255, 136, 0.4);
}

.badge-present {
  background: rgba(255, 102, 0, 0.15);
  color: #ff6600;
  border-color: #ff6600;
  box-shadow: 0 0 15px rgba(255, 102, 0, 0.4);
}

.badge-finalist {
  background: rgba(255, 0, 102, 0.15);
  color: #ff0066;
  border-color: #ff0066;
  box-shadow: 0 0 15px rgba(255, 0, 102, 0.4);
}

/* Results Table */
.results-table-card {
  background: rgba(10, 0, 10, 0.85);
  border: 1px solid rgba(250, 71, 134, 0.3);
  overflow: hidden;
  position: relative;
  backdrop-filter: blur(15px);
  clip-path: polygon(
    20px 0, 100% 0,
    100% calc(100% - 20px), calc(100% - 20px) 100%,
    0 100%, 0 20px
  );
  box-shadow:
    0 0 40px rgba(250, 71, 134, 0.15),
    inset 0 0 80px rgba(250, 71, 134, 0.03);
}

.results-table-card::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 20px;
  height: 20px;
  border-top: 3px solid #FA4786;
  border-right: 3px solid #FA4786;
  box-shadow: 0 0 15px rgba(250, 71, 134, 0.6);
  z-index: 10;
}

.table-container {
  width: 100%;
  overflow-x: auto;
}

.table-header,
.table-row {
  display: grid;
  grid-template-columns: 80px 1fr 1fr 1fr 140px;
  gap: 0;
  min-height: 50px;
}

.table-header {
  background: rgba(250, 71, 134, 0.08);
  border-bottom: 2px solid rgba(250, 71, 134, 0.3);
  position: relative;
}

.table-header::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 100%;
  height: 2px;
  background: linear-gradient(90deg, transparent, #FA4786, #002D72, #6B8CFF, transparent);
  box-shadow: 0 0 10px #FA4786, 0 0 8px rgba(107, 140, 255, 0.5);
}

.table-cell {
  padding: 1rem;
  display: flex;
  align-items: flex-start;
  word-break: break-word;
  overflow-wrap: break-word;
  border-right: 1px solid rgba(250, 71, 134, 0.05);
}

.table-cell:last-child {
  border-right: none;
}

.header-cell {
  color: #FA4786;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  align-items: center;
}

.cell-no,
.cell-score {
  justify-content: center;
  text-align: center;
}

.cell-score {
  align-items: center;
}

.table-row {
  border-bottom: 1px solid rgba(250, 71, 134, 0.1);
  transition: all 0.3s ease;
  position: relative;
  color: #e0f7ff;
  font-size: 0.9rem;
  line-height: 1.5;
}

.table-row::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 0;
  height: 100%;
  background: linear-gradient(90deg, rgba(250, 71, 134, 0.1), rgba(0, 45, 114, 0.05), transparent);
  transition: width 0.3s ease;
  pointer-events: none;
}

.table-row:hover::before {
  width: 100%;
}

.table-row:hover {
  background: rgba(250, 71, 134, 0.03);
  border-left: 2px solid #FA4786;
}

.table-row .cell-no {
  color: #FA4786;
  font-weight: 700;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.5);
}

.score-badge {
  display: inline-block;
  padding: 0.4rem 1rem;
  font-weight: 700;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 0.85rem;
  clip-path: polygon(5px 0, 100% 0, calc(100% - 5px) 100%, 0 100%);
  border: 1px solid;
  box-shadow: 0 0 12px;
  letter-spacing: 0.5px;
}

.score-high {
  background: rgba(0, 255, 136, 0.15);
  color: #00ff88;
  border-color: #00ff88;
  box-shadow: 0 0 15px rgba(0, 255, 136, 0.4);
}

.score-medium {
  background: rgba(255, 204, 0, 0.15);
  color: #ffcc00;
  border-color: #ffcc00;
  box-shadow: 0 0 15px rgba(255, 204, 0, 0.4);
}

.score-low {
  background: rgba(255, 68, 68, 0.15);
  color: #ff4444;
  border-color: #ff4444;
  box-shadow: 0 0 15px rgba(255, 68, 68, 0.4);
}

/* No Results */
.no-results {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 80vh;
  gap: 2.5rem;
}

.no-results-text {
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-size: 2.5rem;
  background: linear-gradient(180deg, #fff, #FA4786);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 25px rgba(250, 71, 134, 0.6));
  letter-spacing: 2px;
}

.no-results-button {
  padding: 1.2rem 2.5rem;
  background: rgba(10, 0, 10, 0.9);
  border: 2px solid #FA4786;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  font-weight: 700;
  font-size: 1.1rem;
  color: #FA4786;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.8rem;
  letter-spacing: 1.5px;
  text-transform: uppercase;
  clip-path: polygon(15px 0, 100% 0, 100% calc(100% - 15px), calc(100% - 15px) 100%, 0 100%, 0 15px);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  box-shadow: 0 0 25px rgba(250, 71, 134, 0.3);
  position: relative;
  overflow: hidden;
}

.no-results-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(250, 71, 134, 0.15), rgba(0, 45, 114, 0.15), rgba(107, 140, 255, 0.1));
  opacity: 0;
  transition: opacity 0.3s ease;
}

.no-results-button:hover {
  transform: translateY(-4px);
  box-shadow:
    0 0 40px rgba(250, 71, 134, 0.6),
    0 0 60px rgba(250, 71, 134, 0.4),
    0 5px 20px rgba(250, 71, 134, 0.3);
  border-color: #FF6BA3;
}

.no-results-button:hover::before {
  opacity: 1;
}

.no-results-icon {
  width: 22px;
  height: 22px;
  filter: drop-shadow(0 0 8px #FA4786);
  transition: transform 0.3s ease;
  position: relative;
  z-index: 1;
}

.no-results-button:hover .no-results-icon {
  transform: scale(1.1);
  filter: drop-shadow(0 0 12px #FA4786);
}

.no-results-button span {
  position: relative;
  z-index: 1;
}

/* Responsive */
@media (max-width: 768px) {
  .floating-reset-button {
    top: 1rem;
    right: 1rem;
    padding: 0.7rem 1.2rem;
    font-size: 0.8rem;
  }

  .floating-reset-button .button-text {
    display: none;
  }

  .main-content {
    padding: 1rem;
    padding-top: 4rem;
  }

  .card-header {
    flex-direction: column;
    gap: 1rem;
    padding: 1.2rem 1.5rem;
  }

  .header-title {
    font-size: 0.9rem;
  }

  .summary-grid {
    padding: 1.5rem;
    gap: 1.5rem;
  }

  .stat-value {
    font-size: 2rem;
  }

  .uuid-section {
    flex-direction: column;
    gap: 0.5rem;
    padding: 1.2rem 1.5rem;
    padding-top: 0;
  }

  .results-table th,
  .results-table td {
    padding: 0.6rem 0.8rem;
    font-size: 0.8rem;
  }
}
</style>
