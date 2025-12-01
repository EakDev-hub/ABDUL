<template>
  <div class="hackathon-container">
    <!-- Animated Background -->
    <div class="animated-bg"></div>
    <div class="grid-overlay"></div>

    <!-- Floating Reset Button -->
    <button v-if="result" class="floating-reset-button" @click="goBack">
      <svg class="button-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8"/>
        <path d="M21 3v5h-5"/>
        <path d="M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16"/>
        <path d="M3 21v-5h5"/>
      </svg>
      <span class="button-text">ส่งใหม่</span>
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
          <div class="table-wrapper">
            <table class="results-table">
              <thead>
                <tr>
                  <th>No.</th>
                  <th>Question</th>
                  <th>Expected Answer</th>
                  <th>Answer</th>
                  <th class="text-center">Score<br/>(0.0 - 1.0)</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in result.results" :key="item.no" class="table-row">
                  <td class="row-number">{{ item.no }}</td>
                  <td>{{ item.question }}</td>
                  <td>{{ item.expectedAnswer }}</td>
                  <td>{{ item.actualAnswer }}</td>
                  <td class="text-center">
                    <span class="score-badge" :class="getScoreClass(item.score)">
                      {{ item.score.toFixed(1) }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
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
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useHackathonStore } from '@/store/hackathon.store'

const router = useRouter()
const hackathonStore = useHackathonStore()

const result = computed(() => hackathonStore.result)

function getScoreClass(score: number): string {
  if (score >= 0.8) return 'score-high'
  if (score >= 0.5) return 'score-medium'
  return 'score-low'
}

function goBack() {
  hackathonStore.clearResult()
  router.push('/hackathon')
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;700;900&family=Rajdhani:wght@300;500;700&family=Share+Tech+Mono&display=swap');

.hackathon-container {
  min-height: 100vh;
  position: relative;
  background: #000205;
  font-family: 'Rajdhani', sans-serif;
  overflow-x: hidden;
}

/* Animated Background */
.animated-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: radial-gradient(ellipse at center, #001a33 0%, #000205 70%);
  z-index: 0;
}

.animated-bg::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background:
    radial-gradient(circle at 20% 30%, rgba(0, 243, 255, 0.1) 0%, transparent 50%),
    radial-gradient(circle at 80% 70%, rgba(176, 0, 255, 0.08) 0%, transparent 50%);
  animation: pulseGlow 8s ease-in-out infinite;
}

@keyframes pulseGlow {
  0%, 100% { opacity: 0.3; }
  50% { opacity: 0.6; }
}

.grid-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 200%;
  height: 200%;
  background-image:
    linear-gradient(rgba(0, 243, 255, 0.06) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 243, 255, 0.06) 1px, transparent 1px);
  background-size: 40px 40px;
  transform: perspective(600px) rotateX(60deg) translateY(-150px) translateZ(-300px);
  animation: gridMove 25s linear infinite;
  z-index: 1;
  pointer-events: none;
}

@keyframes gridMove {
  0% { transform: perspective(600px) rotateX(60deg) translateY(-150px) translateZ(-300px); }
  100% { transform: perspective(600px) rotateX(60deg) translateY(-110px) translateZ(-300px); }
}

/* Floating Reset Button */
.floating-reset-button {
  position: fixed;
  top: 2rem;
  right: 2rem;
  z-index: 1000;
  padding: 0.9rem 1.5rem;
  background: rgba(0, 10, 20, 0.9);
  border: 2px solid #00f3ff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  font-size: 0.9rem;
  color: #00f3ff;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.6rem;
  letter-spacing: 1px;
  text-transform: uppercase;
  clip-path: polygon(10px 0, 100% 0, 100% calc(100% - 10px), calc(100% - 10px) 100%, 0 100%, 0 10px);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  box-shadow: 0 0 20px rgba(0, 243, 255, 0.3);
  animation: floatingPulse 3s ease-in-out infinite;
}

.floating-reset-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(0, 243, 255, 0.1), rgba(176, 0, 255, 0.1));
  opacity: 0;
  transition: opacity 0.3s ease;
}

.floating-reset-button:hover {
  transform: translateY(-3px);
  box-shadow: 0 0 35px rgba(0, 243, 255, 0.6), 0 0 50px rgba(0, 243, 255, 0.4);
  border-color: #00ffff;
}

.floating-reset-button:hover::before {
  opacity: 1;
}

.floating-reset-button .button-icon {
  width: 18px;
  height: 18px;
  filter: drop-shadow(0 0 5px #00f3ff);
  animation: rotateIcon 2s linear infinite paused;
}

.floating-reset-button:hover .button-icon {
  animation-play-state: running;
}

@keyframes rotateIcon {
  from { transform: rotate(0deg); }
  to { transform: rotate(-360deg); }
}

@keyframes floatingPulse {
  0%, 100% {
    box-shadow: 0 0 20px rgba(0, 243, 255, 0.3);
  }
  50% {
    box-shadow: 0 0 30px rgba(0, 243, 255, 0.5), 0 0 40px rgba(0, 243, 255, 0.3);
  }
}

.button-text {
  position: relative;
  z-index: 1;
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
  background: rgba(0, 10, 20, 0.85);
  border: 1px solid rgba(0, 243, 255, 0.3);
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
    0 0 40px rgba(0, 243, 255, 0.15),
    inset 0 0 80px rgba(0, 243, 255, 0.03);
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
  background: linear-gradient(135deg, #00f3ff, #b000ff) border-box;
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
  border-top: 3px solid #00f3ff;
  border-left: 3px solid #00f3ff;
  box-shadow: 0 0 15px rgba(0, 243, 255, 0.6);
}

@keyframes cardGlow {
  0%, 100% { box-shadow: 0 0 40px rgba(0, 243, 255, 0.15), inset 0 0 80px rgba(0, 243, 255, 0.03); }
  50% { box-shadow: 0 0 60px rgba(0, 243, 255, 0.25), inset 0 0 100px rgba(0, 243, 255, 0.05); }
}

/* Card Header */
.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem 2.5rem;
  background: rgba(0, 243, 255, 0.05);
  border-bottom: 1px solid rgba(0, 243, 255, 0.2);
}

.header-title {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  font-size: 1.1rem;
  color: #00f3ff;
  letter-spacing: 2px;
  text-transform: uppercase;
}

.title-icon {
  width: 24px;
  height: 24px;
  filter: drop-shadow(0 0 8px #00f3ff);
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
  color: #aaddff;
  font-family: 'Rajdhani', sans-serif;
  font-size: 0.95rem;
  margin-bottom: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  font-weight: 600;
}

.stat-value {
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  font-size: 2.8rem;
  font-weight: 900;
  background: linear-gradient(180deg, #fff, #00f3ff);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 20px rgba(0, 243, 255, 0.5));
  animation: statPulse 2s ease-in-out infinite;
}

@keyframes statPulse {
  0%, 100% { filter: drop-shadow(0 0 20px rgba(0, 243, 255, 0.5)); }
  50% { filter: drop-shadow(0 0 30px rgba(0, 243, 255, 0.8)); }
}

.stat-highlight {
  background: linear-gradient(180deg, #00f3ff, #b000ff);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 25px rgba(0, 243, 255, 0.8));
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
  color: #aaddff;
  font-family: 'Orbitron', sans-serif;
  font-size: 0.85rem;
  letter-spacing: 1.5px;
  font-weight: 600;
}

.stat-uuid {
  color: #00f3ff;
  font-family: 'Share Tech Mono', monospace;
  font-size: 0.95rem;
  text-shadow: 0 0 10px rgba(0, 243, 255, 0.5);
  padding: 0.5rem 1rem;
  background: rgba(0, 243, 255, 0.08);
  border: 1px solid rgba(0, 243, 255, 0.3);
  border-radius: 4px;
}

.pass-badge {
  padding: 0.5rem 1.5rem;
  font-family: 'Orbitron', sans-serif;
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
  background: rgba(0, 10, 20, 0.85);
  border: 1px solid rgba(0, 243, 255, 0.3);
  overflow: hidden;
  position: relative;
  backdrop-filter: blur(15px);
  clip-path: polygon(
    20px 0, 100% 0,
    100% calc(100% - 20px), calc(100% - 20px) 100%,
    0 100%, 0 20px
  );
  box-shadow:
    0 0 40px rgba(0, 243, 255, 0.15),
    inset 0 0 80px rgba(0, 243, 255, 0.03);
}

.results-table-card::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 20px;
  height: 20px;
  border-top: 3px solid #00f3ff;
  border-right: 3px solid #00f3ff;
  box-shadow: 0 0 15px rgba(0, 243, 255, 0.6);
  z-index: 10;
}

.table-wrapper {
  overflow-x: auto;
}

.results-table {
  width: 100%;
  border-collapse: collapse;
  font-family: 'Share Tech Mono', monospace;
}

.results-table thead {
  background: rgba(0, 243, 255, 0.08);
  position: relative;
}

.results-table thead::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 2px;
  background: linear-gradient(90deg, transparent, #00f3ff, transparent);
  box-shadow: 0 0 10px #00f3ff;
}

.results-table th {
  padding: 0.9rem 1rem;
  text-align: left;
  color: #00f3ff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  border-bottom: 1px solid rgba(0, 243, 255, 0.2);
}

.results-table th.text-center {
  text-align: center;
}

.table-row {
  border-bottom: 1px solid rgba(0, 243, 255, 0.1);
  transition: all 0.3s ease;
  position: relative;
}

.table-row::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 0;
  height: 100%;
  background: linear-gradient(90deg, rgba(0, 243, 255, 0.1), transparent);
  transition: width 0.3s ease;
}

.table-row:hover::before {
  width: 100%;
}

.table-row:hover {
  background: rgba(0, 243, 255, 0.03);
  border-left: 2px solid #00f3ff;
}

.results-table td {
  padding: 0.8rem 1rem;
  color: #e0f7ff;
  font-size: 0.9rem;
  line-height: 1.4;
}

.results-table td.text-center {
  text-align: center;
}

.row-number {
  color: #00f3ff;
  font-weight: 700;
  font-family: 'Orbitron', sans-serif;
  text-shadow: 0 0 10px rgba(0, 243, 255, 0.5);
}

.score-badge {
  display: inline-block;
  padding: 0.4rem 1rem;
  font-weight: 700;
  font-family: 'Orbitron', sans-serif;
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
  font-family: 'Orbitron', sans-serif;
  font-size: 2.5rem;
  background: linear-gradient(180deg, #fff, #00f3ff);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 25px rgba(0, 243, 255, 0.6));
  letter-spacing: 2px;
}

.no-results-button {
  padding: 1.2rem 2.5rem;
  background: rgba(0, 10, 20, 0.9);
  border: 2px solid #00f3ff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  font-size: 1.1rem;
  color: #00f3ff;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.8rem;
  letter-spacing: 1.5px;
  text-transform: uppercase;
  clip-path: polygon(15px 0, 100% 0, 100% calc(100% - 15px), calc(100% - 15px) 100%, 0 100%, 0 15px);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  box-shadow: 0 0 25px rgba(0, 243, 255, 0.3);
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
  background: linear-gradient(135deg, rgba(0, 243, 255, 0.15), rgba(176, 0, 255, 0.15));
  opacity: 0;
  transition: opacity 0.3s ease;
}

.no-results-button:hover {
  transform: translateY(-4px);
  box-shadow:
    0 0 40px rgba(0, 243, 255, 0.6),
    0 0 60px rgba(0, 243, 255, 0.4),
    0 5px 20px rgba(0, 243, 255, 0.3);
  border-color: #00ffff;
}

.no-results-button:hover::before {
  opacity: 1;
}

.no-results-icon {
  width: 22px;
  height: 22px;
  filter: drop-shadow(0 0 8px #00f3ff);
  transition: transform 0.3s ease;
  position: relative;
  z-index: 1;
}

.no-results-button:hover .no-results-icon {
  transform: scale(1.1);
  filter: drop-shadow(0 0 12px #00f3ff);
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
