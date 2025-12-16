<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useAnnouncementStore } from '@/store/announcement.store'
import { announcementService } from '@/services/announcement.service'
import { useQnaStore } from '@/store/qna.store'
import { qnaService } from '@/services/qna.service'
import { useScoreStore } from '@/store/score.store'
import { scoreService } from '@/services/score.service'

// Hackathon finish time - Set your actual finish time here
const hackathonFinishTime = new Date('2025-12-19T12:00:00').getTime()

// Announcement store
const announcementStore = useAnnouncementStore()

// QnA store
const qnaStore = useQnaStore()

// Score store
const scoreStore = useScoreStore()

// Current time
const currentTime = ref(new Date())

// Timer countdown
const timeRemaining = ref({
  hours: 0,
  minutes: 0,
  seconds: 0,
  isFinished: false
})

// Fetch scores from API
const fetchScores = async () => {
  try {
    scoreStore.setLoading(true)
    const response = await scoreService.getScores()
    
    if (response.success && response.data) {
      scoreStore.setScores(response.data)
    }
  } catch (error: any) {
    console.error('Failed to fetch scores:', error)
    scoreStore.setError(error.message || 'Failed to fetch scores')
  } finally {
    scoreStore.setLoading(false)
  }
}

// Format duration from seconds to HH:MM:SS
const formatDuration = (seconds: number): string => {
  const hours = Math.floor(seconds / 3600)
  const minutes = Math.floor((seconds % 3600) / 60)
  const secs = Math.floor(seconds % 60)
  
  return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(secs).padStart(2, '0')}`
}

// Sorted scores by totalScore descending
const sortedScores = computed(() => {
  return [...scoreStore.scores].sort((a, b) => b.totalScore - a.totalScore)
})

// Fetch announcements from API
const fetchAnnouncements = async () => {
  try {
    announcementStore.setLoading(true)
    const response = await announcementService.getAnnouncements()
    
    if (response.success && response.data) {
      announcementStore.setAnnouncements(response.data)
    }
  } catch (error: any) {
    console.error('Failed to fetch announcements:', error)
    announcementStore.setError(error.message || 'Failed to fetch announcements')
  } finally {
    announcementStore.setLoading(false)
  }
}

// Fetch QnA from API
const fetchQna = async () => {
  try {
    qnaStore.setLoading(true)
    const response = await qnaService.getQna()
    
    if (response.success && response.data) {
      qnaStore.setQnaItems(response.data)
    }
  } catch (error: any) {
    console.error('Failed to fetch Q&A:', error)
    qnaStore.setError(error.message || 'Failed to fetch Q&A')
  } finally {
    qnaStore.setLoading(false)
  }
}

// QR Code URL for Q&A
const qrCodeUrl = 'https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=https://forms.gle/d1fbLviWCx57SbUp7'

// Update current time and countdown
const updateTime = () => {
  currentTime.value = new Date()
  
  const now = Date.now()
  const diff = hackathonFinishTime - now
  
  if (diff <= 0) {
    timeRemaining.value = {
      hours: 0,
      minutes: 0,
      seconds: 0,
      isFinished: true
    }
  } else {
    const hours = Math.floor(diff / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
    const seconds = Math.floor((diff % (1000 * 60)) / 1000)
    
    timeRemaining.value = {
      hours,
      minutes,
      seconds,
      isFinished: false
    }
  }
}

// Format current datetime
const formattedDateTime = computed(() => {
  const date = currentTime.value
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  
  return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`
})

// Format timer display
const formattedTimer = computed(() => {
  const { hours, minutes, seconds } = timeRemaining.value
  return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`
})

// Format date for Q&A
const formatQnaDate = (dateString: string): string => {
  const date = new Date(dateString)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  
  return `${day}/${month}/${year} ${hours}:${minutes}`
}

let intervalId: number
let announcementIntervalId: number
let qnaIntervalId: number
let scoreIntervalId: number

onMounted(() => {
  updateTime()
  intervalId = setInterval(updateTime, 1000)
  
  // Fetch announcements immediately
  fetchAnnouncements()
  
  // Poll announcements every 5 seconds
  announcementIntervalId = setInterval(fetchAnnouncements, 5000)
  
  // Fetch Q&A immediately
  fetchQna()
  
  // Poll Q&A every 5 seconds
  qnaIntervalId = setInterval(fetchQna, 5000)
  
  // Fetch scores immediately
  fetchScores()
  
  // Poll scores every 5 seconds
  scoreIntervalId = setInterval(fetchScores, 5000)
})

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId)
  }
  if (announcementIntervalId) {
    clearInterval(announcementIntervalId)
  }
  if (qnaIntervalId) {
    clearInterval(qnaIntervalId)
  }
  if (scoreIntervalId) {
    clearInterval(scoreIntervalId)
  }
})
</script>

<template>
  <div class="dashboard">
    <img src="./assets/images/logo.png" alt="Logo" class="dashboard-logo" />
    <div class="dashboard-content">
      <!-- Left Side -->
      <div class="left-panel">
        <!-- Timer Section -->
        <div class="timer-section">
          <h2>⏱️ Time Remaining</h2>
          <div class="timer" :class="{ 'finished': timeRemaining.isFinished }">
            {{ timeRemaining.isFinished ? "Time's up" : formattedTimer }}
          </div>
        </div>

        <!-- Scoreboard Section -->
        <div class="scoreboard-section">
          <h2>📊 Scoreboard</h2>
          <div v-if="scoreStore.isLoading && scoreStore.scores.length === 0" class="table-container">
            <div class="loading-message">Loading scoreboard...</div>
          </div>
          <div v-else-if="scoreStore.scores.length === 0 && !scoreStore.isLoading" class="table-container">
            <div class="loading-message">No teams have submitted yet.</div>
          </div>
          <div v-else class="table-container">
            <table class="scoreboard-table">
              <thead>
                <tr>
                  <th>Rank</th>
                  <th>Team</th>
                  <th>Duration</th>
                  <th>Total Score</th>
                </tr>
              </thead>
              <transition-group name="score-fade" tag="tbody">
                <tr v-for="(score, index) in sortedScores" :key="score.team"
                    :class="{ 'first-place': index === 0, 'second-place': index === 1, 'third-place': index === 2 }">
                  <td class="rank">{{ index + 1 }}</td>
                  <td class="team-name">{{ score.team }}</td>
                  <td>{{ formatDuration(score.timeUsedInSeconds) }}</td>
                  <td class="score">{{ score.totalScore.toFixed(1) }}</td>
                </tr>
              </transition-group>
            </table>
          </div>
        </div>

      </div>

      <!-- Right Side -->
      <div class="right-panel">
        <!-- Current DateTime -->
        <div class="datetime-section">
          <h2>📅 Current Time</h2>
          <div class="datetime">{{ formattedDateTime }}</div>
        </div>

        <!-- Announcements Section -->
        <div class="announcements-section">
          <h2>📢 Announcements</h2>
          <div v-if="announcementStore.isLoading && announcementStore.announcements.length === 0" class="announcements-list">
            <div class="announcement-item">
              <p class="announcement-message">Loading announcements...</p>
            </div>
          </div>
          <div v-else-if="announcementStore.announcements.length === 0 && !announcementStore.isLoading" class="announcements-list">
            <div class="announcement-item">
              <p class="announcement-message">No announcements yet.</p>
            </div>
          </div>
          <div v-else class="announcements-list">
            <transition-group name="announcement-fade" tag="div">
              <div v-for="announcement in announcementStore.announcements" :key="announcement.id" class="announcement-item">
                <p class="announcement-message">{{ announcement.text }}</p>
              </div>
            </transition-group>
          </div>
        </div>

        <!-- Q&A Section -->
        <div class="qa-section">
          <h2>❓ Q&A</h2>
          <div v-if="qnaStore.isLoading && qnaStore.qnaItems.length === 0" class="qa-list">
            <div class="qa-item">
              <p class="question">Loading Q&A...</p>
            </div>
          </div>
          <div v-else-if="qnaStore.qnaItems.length === 0 && !qnaStore.isLoading" class="qa-list">
            <div class="qa-item">
              <p class="question">No Q&A items yet.</p>
            </div>
          </div>
          <div v-else class="qa-list">
            <transition-group name="qa-fade" tag="div">
              <div v-for="qa in qnaStore.qnaItems" :key="qa.id" class="qa-item">
                <p class="question"><strong>Q:</strong> {{ qa.question }}</p>
                <div class="answer-row">
                  <p class="answer"><strong>A:</strong> {{ qa.answer }}</p>
                  <span class="posted-at">Posted at: {{ formatQnaDate(qa.createdAt) }}</span>
                </div>
              </div>
            </transition-group>
          </div>
          <div class="qr-sponsor-wrapper">
            <div class="qr-container">
              <p class="qr-label">📱 Scan to submit your question</p>
              <img :src="qrCodeUrl" alt="Q&A QR Code" class="qr-code" />
            </div>
            <div class="sponsor-section">
              <p class="sponsor-title">Sponsored by</p>
              <div class="sponsor-logo-container">
                <img src="./assets/images/sponsor/viriyah.png" alt="Viriyah" class="sponsor-logo" />
                <img src="./assets/images/sponsor/mtl.png" alt="MTL" class="sponsor-logo" />
                <img src="./assets/images/sponsor/aws.png" alt="AWS" class="sponsor-logo" />
                <img src="./assets/images/sponsor/ngernturbo.png" alt="Ngern Turbo" class="sponsor-logo" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;600;700;800;900&family=Chakra+Petch:wght@300;400;500;600;700&family=Sarabun:wght@300;400;500;600;700;800&display=swap');

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.dashboard {
  min-height: 100vh;
  background:
    linear-gradient(0deg, rgba(0, 45, 114, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 45, 114, 0.05) 1px, transparent 1px),
    radial-gradient(circle at 50% 50%, #000814 0%, #000000 100%);
  background-size: 50px 50px, 50px 50px, 100% 100%;
  background-position: 0 0, 0 0, center;
  background-attachment: fixed;
  padding: 8px;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  position: relative;
  overflow-y: auto;
  overflow-x: hidden;
  scrollbar-width: none; /* Firefox */
  -ms-overflow-style: none; /* IE and Edge */
}

.dashboard::-webkit-scrollbar {
  display: none; /* Chrome, Safari, Opera */
}

.dashboard::before {
  content: '';
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
    linear-gradient(180deg,
      rgba(250, 71, 134, 0.03) 0%,
      transparent 50%,
      rgba(0, 45, 114, 0.03) 100%
    );
  pointer-events: none;
  z-index: 1;
  will-change: opacity;
}

@keyframes scanline {
  0% {
    transform: translate3d(0, 0, 0);
  }
  100% {
    transform: translate3d(0, 100vh, 0);
  }
}

.dashboard-logo {
  position: absolute;
  top: 1vh;
  right: 1.5vw;
  height: min(15vh, 15vw);
  width: min(15vh, 15vw);
  filter: drop-shadow(0 0 20px rgba(250, 71, 134, 0.6)) drop-shadow(0 0 40px rgba(0, 45, 114, 0.4));
  z-index: 10;
  object-fit: contain;
  animation: logoPulse 3s ease-in-out infinite;
}

@keyframes logoPulse {
  0%, 100% {
    filter: drop-shadow(0 0 20px rgba(250, 71, 134, 0.6)) drop-shadow(0 0 40px rgba(0, 45, 114, 0.4));
  }
  50% {
    filter: drop-shadow(0 0 30px rgba(250, 71, 134, 0.8)) drop-shadow(0 0 50px rgba(0, 45, 114, 0.6));
  }
}

.dashboard-content {
  display: grid;
  grid-template-columns: 40fr 60fr;
  gap: 8px;
  max-width: 100%;
  margin: 0 auto;
  min-height: calc(100vh - 16px);
  box-sizing: border-box;
  position: relative;
  z-index: 2;
}

/* Left Panel Styles */
.left-panel {
  display: flex;
  flex-direction: column;
  gap: 8px;
  height: 100%;
}

.timer-section {
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 8px;
  padding: 8px;
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 40px rgba(0, 45, 114, 0.3),
    inset 0 0 20px rgba(0, 45, 114, 0.1);
  text-align: center;
  border: 2px solid #fa4786;
  position: relative;
  flex-shrink: 0;
  overflow: hidden;
}

.timer-section::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(
    45deg,
    transparent 30%,
    rgba(250, 71, 134, 0.1) 50%,
    transparent 70%
  );
  animation: shimmer 3s linear infinite;
}

@keyframes shimmer {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.timer-section h2 {
  color: #fa4786;
  margin-bottom: 4px;
  font-size: 0.85rem;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.8);
  position: relative;
  z-index: 1;
}

.timer {
  font-size: 2.5rem;
  font-weight: 700;
  color: #ffffff;
  font-family: 'Orbitron', 'Courier New', monospace;
  padding: 8px;
  border-radius: 6px;
  text-shadow:
    0 0 10px rgba(250, 71, 134, 0.8),
    0 0 20px rgba(250, 71, 134, 0.6),
    0 0 30px rgba(0, 45, 114, 0.4);
  position: relative;
  z-index: 1;
}

.timer.finished {
  color: #fc8181;
  border-color: #fc8181;
  animation: pulse 1.5s infinite;
  text-shadow: 0 0 20px rgba(252, 129, 129, 0.5);
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
}

.timer-label {
  color: #a0aec0;
  font-size: 1.1rem;
  font-weight: 500;
}

.scoreboard-section {
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 8px;
  padding: 10px;
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 40px rgba(0, 45, 114, 0.3),
    inset 0 0 20px rgba(0, 45, 114, 0.1);
  flex: 1;
  border: 2px solid #fa4786;
  border-image: linear-gradient(45deg, #fa4786, #002d72, #fa4786) 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative;
}

.scoreboard-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(250, 71, 134, 0.1),
    transparent
  );
  animation: slideLight 3s infinite;
}

@keyframes slideLight {
  0% {
    left: -100%;
  }
  100% {
    left: 100%;
  }
}

.scoreboard-section h2 {
  color: #fa4786;
  margin-bottom: 8px;
  font-size: 1rem;
  flex-shrink: 0;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.8);
  position: relative;
  z-index: 1;
}

.table-container {
  overflow-x: auto;
  overflow-y: auto;
  flex: 1;
}

.scoreboard-table {
  width: 100%;
  border-collapse: collapse;
}

.scoreboard-table thead th {
  background: linear-gradient(135deg, #fa4786 0%, #002d72 100%);
  color: white;
  padding: 6px;
  text-align: left;
  font-weight: 600;
  font-size: 0.8rem;
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.5);
  box-shadow: 0 2px 10px rgba(250, 71, 134, 0.3);
}

.scoreboard-table thead th:first-child {
  border-top-left-radius: 10px;
}

.scoreboard-table thead th:last-child {
  border-top-right-radius: 10px;
}

.scoreboard-table tbody tr {
  border-bottom: 1px solid rgba(0, 45, 114, 0.3);
  transition: all 0.3s;
}

.scoreboard-table tbody tr:hover {
  background: linear-gradient(90deg, rgba(250, 71, 134, 0.1) 0%, rgba(0, 45, 114, 0.1) 100%);
  box-shadow: 0 0 15px rgba(250, 71, 134, 0.2);
}

.scoreboard-table tbody td {
  padding: 6px;
  font-size: 0.8rem;
  color: #e2e8f0;
  font-family: 'Sarabun', sans-serif;
}

.rank {
  font-weight: 700;
  font-size: 0.8rem;
  font-family: 'Sarabun', sans-serif;
}

.team-name {
  font-weight: 600;
  color: #cbd5e0;
}

.score {
  font-weight: 700;
  color: #fa4786;
  font-size: 0.8rem;
  font-family: 'Sarabun', sans-serif;
  text-align: center;
}

.first-place {
  background: rgba(255, 215, 0, 0.1);
  border-left: 4px solid #ffd700;
}

.first-place .rank {
  color: #ffd700;
}

.second-place {
  background: rgba(192, 192, 192, 0.1);
  border-left: 4px solid #c0c0c0;
}

.second-place .rank {
  color: #c0c0c0;
}

.third-place {
  background: rgba(205, 127, 50, 0.1);
  border-left: 4px solid #cd7f32;
}

.third-place .rank {
  color: #cd7f32;
}


/* Right Panel Styles */
.right-panel {
  display: flex;
  flex-direction: column;
  gap: 8px;
  height: 100%;
}

.datetime-section {
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 8px;
  padding: 6px;
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 40px rgba(0, 45, 114, 0.3),
    inset 0 0 20px rgba(0, 45, 114, 0.1);
  text-align: center;
  border: 2px solid;
  border-image: linear-gradient(90deg, #fa4786, #002d72, #fa4786) 1;
  flex-shrink: 0;
}

.datetime-section h2 {
  color: #fa4786;
  margin-bottom: 4px;
  font-size: 0.85rem;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.8);
}

.datetime {
  font-size: 0.9rem;
  font-weight: 600;
  color: #ffffff;
  font-family: 'Courier New', monospace;
  padding: 4px;
  border-radius: 6px;
  text-shadow:
    0 0 5px rgba(250, 71, 134, 0.6),
    0 0 10px rgba(0, 45, 114, 0.4);
}

.announcements-section {
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 8px;
  padding: 10px;
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 40px rgba(0, 45, 114, 0.3),
    inset 0 0 20px rgba(0, 45, 114, 0.1);
  border: 2px solid;
  border-image: linear-gradient(135deg, #fa4786, #002d72) 1;
  flex: 0.6;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative;
}

.announcements-section h2 {
  color: #fa4786;
  margin-bottom: 6px;
  font-size: 1rem;
  flex-shrink: 0;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.8);
  position: relative;
  z-index: 1;
}

.announcements-list {
  overflow-y: auto;
  flex: 1;
}

.announcement-item {
  padding: 6px;
  margin-bottom: 5px;
  background: linear-gradient(90deg, rgba(0, 45, 114, 0.2) 0%, rgba(26, 26, 26, 0.8) 100%);
  border-left: 3px solid #fa4786;
  border-radius: 4px;
  transition: all 0.3s;
  box-shadow: 0 2px 5px rgba(0, 45, 114, 0.2);
}

.announcement-item:hover {
  transform: translateX(5px);
  background: linear-gradient(90deg, rgba(250, 71, 134, 0.2) 0%, rgba(42, 42, 42, 0.9) 100%);
  box-shadow:
    0 0 10px rgba(250, 71, 134, 0.3),
    0 0 20px rgba(0, 45, 114, 0.2);
}

.announcement-time {
  font-weight: 700;
  color: #fa4786;
  margin-right: 6px;
  font-size: 1rem;
}

.announcement-message {
  color: #cbd5e0;
  margin-top: 2px;
  line-height: 1.2;
  font-size: 1rem;
}

.qa-section {
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 8px;
  padding: 10px;
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 40px rgba(0, 45, 114, 0.3),
    inset 0 0 20px rgba(0, 45, 114, 0.1);
  border: 2px solid;
  border-image: linear-gradient(135deg, #002d72, #fa4786) 1;
  flex: 2.0;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative;
}

.qa-section h2 {
  color: #fa4786;
  margin-bottom: 6px;
  font-size: 1rem;
  flex-shrink: 0;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.8);
  position: relative;
  z-index: 1;
}

.qa-list {
  overflow-y: auto;
  flex: 1;
}

.qa-item {
  padding: 6px;
  margin-bottom: 6px;
  background: linear-gradient(90deg, rgba(0, 45, 114, 0.15) 0%, rgba(26, 26, 26, 0.8) 100%);
  border-radius: 4px;
  border: 1px solid rgba(0, 45, 114, 0.4);
  transition: all 0.3s;
}

.qa-item:hover {
  background: linear-gradient(90deg, rgba(250, 71, 134, 0.15) 0%, rgba(42, 42, 42, 0.9) 100%);
  border-color: rgba(250, 71, 134, 0.4);
  box-shadow: 0 0 10px rgba(250, 71, 134, 0.2);
}

.question {
  color: #e2e8f0;
  margin-bottom: 3px;
  font-size: 0.75rem;
}

.answer-row {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  gap: 10px;
}

.answer {
  color: #a0aec0;
  font-size: 0.75rem;
  padding-left: 12px;
  margin: 0;
  flex: 1;
}

.posted-at {
  font-size: 0.6rem;
  color: #666;
  white-space: nowrap;
  margin-left: auto;
}

.qr-sponsor-wrapper {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  justify-content: space-between;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid rgba(0, 45, 114, 0.5);
  flex-shrink: 0;
  gap: 20px;
  position: relative;
  z-index: 1;
}

.qr-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 5px;
}

.qr-code {
  width: 90px;
  height: 90px;
  border: 2px solid #fa4786;
  border-radius: 4px;
  padding: 5px;
  background: white;
  box-shadow:
    0 0 15px rgba(250, 71, 134, 0.5),
    0 0 30px rgba(0, 45, 114, 0.3);
}

.qr-label {
  color: #fa4786;
  font-weight: 600;
  font-size: 0.7rem;
  text-align: center;
  text-shadow: 0 0 8px rgba(250, 71, 134, 0.6);
}

/* Sponsor Section */
.sponsor-section {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  align-self: flex-end;
  gap: 8px;
}

.sponsor-title {
  color: #fa4786;
  font-weight: 600;
  font-size: 0.8rem;
  text-align: center;
  text-shadow: 0 0 8px rgba(250, 71, 134, 0.6);
}

.sponsor-logo-container {
  display: flex;
  flex-direction: row;
  gap: 12px;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
}

.sponsor-logo {
  height: 60px;
  width: auto;
  max-width: 150px;
  object-fit: contain;
  filter: drop-shadow(0 0 8px rgba(250, 71, 134, 0.3)) drop-shadow(0 0 15px rgba(0, 45, 114, 0.2));
  transition: all 0.3s ease;
  background: white;
  padding: 5px;
  border-radius: 4px;
  border: 1px solid rgba(0, 45, 114, 0.2);
}

.sponsor-logo:hover {
  transform: scale(1.08);
  filter: drop-shadow(0 0 15px rgba(250, 71, 134, 0.6)) drop-shadow(0 0 25px rgba(0, 45, 114, 0.4));
  background: white;
  border-color: rgba(250, 71, 134, 0.4);
}

/* Scrollbar Styles */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: #1a1a1a;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb {
  background: linear-gradient(180deg, #fa4786 0%, #002d72 100%);
  border-radius: 10px;
  box-shadow: 0 0 5px rgba(250, 71, 134, 0.5);
}

::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(180deg, #ff5a96 0%, #003d92 100%);
  box-shadow: 0 0 10px rgba(250, 71, 134, 0.7);
}

/* Announcement Transitions */
.announcement-fade-enter-active,
.announcement-fade-leave-active {
  transition: all 0.5s ease;
}

.announcement-fade-enter-from {
  opacity: 0;
  transform: translateX(-20px);
}

.announcement-fade-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

.announcement-fade-move {
  transition: transform 0.5s ease;
}

/* Q&A Transitions */
.qa-fade-enter-active,
.qa-fade-leave-active {
  transition: all 0.5s ease;
}

.qa-fade-enter-from {
  opacity: 0;
  transform: translateY(-20px);
}

.qa-fade-leave-to {
  opacity: 0;
  transform: translateY(20px);
}

.qa-fade-move {
  transition: transform 0.5s ease;
}
/* Score Transitions */
.score-fade-enter-active,
.score-fade-leave-active {
  transition: all 0.5s ease;
}

.score-fade-enter-from {
  opacity: 0;
  transform: translateX(-20px);
}

.score-fade-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

.score-fade-move {
  transition: transform 0.5s ease;
}

/* Loading Message */
.loading-message {
  color: #a0aec0;
  text-align: center;
  padding: 20px;
  font-size: 1rem;
}


/* Responsive Design */
@media (max-width: 1200px) {
  .dashboard {
    height: auto;
    min-height: 100vh;
    overflow-y: auto;
  }

  .dashboard-content {
    grid-template-columns: 1fr;
    height: auto;
    min-height: calc(100vh - 16px);
  }
  
  .left-panel,
  .right-panel {
    height: auto;
    min-height: 50vh;
  }

  .scoreboard-section,
  .announcements-section,
  .qa-section {
    min-height: 400px;
    max-height: 600px;
  }
  
  .timer {
    font-size: 2rem;
  }
}

@media (max-width: 768px) {
  .dashboard {
    padding: 4px;
    overflow-y: auto;
  }

  .dashboard-content {
    gap: 4px;
  }

  .timer {
    font-size: 1.5rem;
  }

  .dashboard-logo {
    height: min(10vh, 10vw);
    width: min(10vh, 10vw);
  }

  .scoreboard-section,
  .announcements-section,
  .qa-section {
    min-height: 300px;
    max-height: 500px;
  }
}
</style>
