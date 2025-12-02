<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useAnnouncementStore } from '@/store/announcement.store'
import { announcementService } from '@/services/announcement.service'
import { useQnaStore } from '@/store/qna.store'
import { qnaService } from '@/services/qna.service'
import { useScoreStore } from '@/store/score.store'
import { scoreService } from '@/services/score.service'

// Hackathon finish time - Set your actual finish time here
const hackathonFinishTime = new Date('2025-11-19T12:00:00').getTime()

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

// Format time from ISO string
const formatTime = (isoString: string) => {
  const date = new Date(isoString)
  return `${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`
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
const qrCodeUrl = 'https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=https://forms.gle/yourQAform'

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
                <span class="announcement-time">{{ formatTime(announcement.postedAt) }}</span>
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
                <p class="answer"><strong>A:</strong> {{ qa.answer }}</p>
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
                <img src="./assets/images/sponsor/aws.png" alt="AWS" class="sponsor-logo" />
                <img src="./assets/images/sponsor/mtl.png" alt="MTL" class="sponsor-logo" />
                <img src="./assets/images/sponsor/ngernturbo.png" alt="Ngern Turbo" class="sponsor-logo" />
                <img src="./assets/images/sponsor/viriyah.png" alt="Viriyah" class="sponsor-logo" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;600;700;800;900&family=Chakra+Petch:wght@300;400;500;600;700&display=swap');

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.dashboard {
  height: 100vh;
  background: #000000;
  padding: 8px;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  position: relative;
  overflow: hidden;
}

.dashboard-logo {
  position: absolute;
  top: 1vh;
  right: 1.5vw;
  height: min(15vh, 15vw);
  width: min(15vh, 15vw);
  filter: drop-shadow(0 0 10px rgba(250, 71, 134, 0.3));
  z-index: 10;
  object-fit: contain;
}

.dashboard-content {
  display: grid;
  grid-template-columns: 40fr 60fr;
  gap: 8px;
  max-width: 100%;
  margin: 0 auto;
  height: calc(100vh - 16px);
  box-sizing: border-box;
}

/* Left Panel Styles */
.left-panel {
  display: flex;
  flex-direction: column;
  gap: 8px;
  height: 100%;
}

.timer-section {
  background: #0a0a0a;
  border-radius: 8px;
  padding: 10px;
  box-shadow: 0 5px 15px rgba(250, 71, 134, 0.3);
  text-align: center;
  border: 2px solid #fa4786;
  flex-shrink: 0;
}

.timer-section h2 {
  color: #fa4786;
  margin-bottom: 6px;
  font-size: 1rem;
}

.timer {
  font-size: 4rem;
  font-weight: 700;
  color: #ffffff;
  font-family: 'Courier New', monospace;
  padding: 10px;
  border-radius: 6px;
  text-shadow: 0 0 15px rgba(250, 71, 134, 0.5);
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
  background: #0a0a0a;
  border-radius: 8px;
  padding: 10px;
  box-shadow: 0 5px 15px rgba(250, 71, 134, 0.3);
  flex: 1;
  border: 2px solid #fa4786;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.scoreboard-section h2 {
  color: #fa4786;
  margin-bottom: 8px;
  font-size: 1rem;
  flex-shrink: 0;
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
  background: #fa4786;
  color: white;
  padding: 6px;
  text-align: left;
  font-weight: 600;
  font-size: 1rem;
}

.scoreboard-table thead th:first-child {
  border-top-left-radius: 10px;
}

.scoreboard-table thead th:last-child {
  border-top-right-radius: 10px;
}

.scoreboard-table tbody tr {
  border-bottom: 1px solid #1a1a1a;
  transition: background-color 0.3s;
}

.scoreboard-table tbody tr:hover {
  background-color: #1a1a1a;
}

.scoreboard-table tbody td {
  padding: 6px;
  font-size: 1rem;
  color: #e2e8f0;
}

.rank {
  font-weight: 700;
  font-size: 1rem;
}

.team-name {
  font-weight: 600;
  color: #cbd5e0;
}

.score {
  font-weight: 700;
  color: #fa4786;
  font-size: 1rem;
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
  background: #0a0a0a;
  border-radius: 8px;
  padding: 6px;
  box-shadow: 0 5px 15px rgba(250, 71, 134, 0.3);
  text-align: center;
  border: 2px solid #fa4786;
  flex-shrink: 0;
}

.datetime-section h2 {
  color: #fa4786;
  margin-bottom: 4px;
  font-size: 0.85rem;
}

.datetime {
  font-size: 0.9rem;
  font-weight: 600;
  color: #ffffff;
  font-family: 'Courier New', monospace;
  padding: 4px;
  border-radius: 6px;
  text-shadow: 0 0 10px rgba(250, 71, 134, 0.3);
}

.announcements-section {
  background: #0a0a0a;
  border-radius: 8px;
  padding: 10px;
  box-shadow: 0 5px 15px rgba(250, 71, 134, 0.3);
  border: 2px solid #fa4786;
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.announcements-section h2 {
  color: #fa4786;
  margin-bottom: 6px;
  font-size: 1rem;
  flex-shrink: 0;
}

.announcements-list {
  overflow-y: auto;
  flex: 1;
}

.announcement-item {
  padding: 6px;
  margin-bottom: 5px;
  background: #1a1a1a;
  border-left: 3px solid #fa4786;
  border-radius: 4px;
  transition: transform 0.2s;
}

.announcement-item:hover {
  transform: translateX(5px);
  background: #2a2a2a;
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
  background: #0a0a0a;
  border-radius: 8px;
  padding: 10px;
  box-shadow: 0 5px 15px rgba(250, 71, 134, 0.3);
  border: 2px solid #fa4786;
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.qa-section h2 {
  color: #fa4786;
  margin-bottom: 6px;
  font-size: 1rem;
  flex-shrink: 0;
}

.qa-list {
  overflow-y: auto;
  flex: 1;
}

.qa-item {
  padding: 6px;
  margin-bottom: 6px;
  background: #1a1a1a;
  border-radius: 4px;
  border: 1px solid #2a2a2a;
}

.qa-item:hover {
  background: #2a2a2a;
}

.question {
  color: #e2e8f0;
  margin-bottom: 3px;
  font-size: 1rem;
}

.answer {
  color: #a0aec0;
  font-size: 1rem;
  padding-left: 12px;
}

.qr-sponsor-wrapper {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  justify-content: space-between;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #2a2a2a;
  flex-shrink: 0;
  gap: 20px;
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
  box-shadow: 0 0 10px rgba(250, 71, 134, 0.3);
}

.qr-label {
  color: #fa4786;
  font-weight: 600;
  font-size: 0.7rem;
  text-align: center;
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
  height: 40px;
  width: auto;
  max-width: 70px;
  object-fit: contain;
  filter: drop-shadow(0 0 5px rgba(250, 71, 134, 0.2));
  transition: all 0.3s ease;
  background: white;
  padding: 5px;
  border-radius: 4px;
}

.sponsor-logo:hover {
  transform: scale(1.08);
  filter: drop-shadow(0 0 12px rgba(250, 71, 134, 0.5));
  background: white;
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
  background: #fa4786;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: #ff5a96;
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
  .dashboard-content {
    grid-template-columns: 1fr;
    height: auto;
  }
  
  .timer {
    font-size: 2rem;
  }
}
</style>
