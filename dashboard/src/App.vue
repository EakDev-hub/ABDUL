<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'

// Hackathon finish time - Set your actual finish time here
const hackathonFinishTime = new Date('2025-11-30T18:00:00').getTime()

// Current time
const currentTime = ref(new Date())

// Timer countdown
const timeRemaining = ref({
  hours: 0,
  minutes: 0,
  seconds: 0,
  isFinished: false
})

// Scoreboard data
const teams = ref([
  { team: 'Team Alpha', duration: '02:45:30', score: 850 },
  { team: 'Team Beta', duration: '03:12:15', score: 920 },
  { team: 'Team Gamma', duration: '02:58:42', score: 785 },
  { team: 'Team Delta', duration: '03:05:20', score: 890 },
  { team: 'Team Epsilon', duration: '02:30:55', score: 950 }
])

// Announcements
const announcements = ref([
  { id: 1, time: '14:00', message: 'Hackathon has officially started! Good luck to all teams!' },
  { id: 2, time: '15:30', message: 'Reminder: Lunch break at 16:00-17:00' },
  { id: 3, time: '16:45', message: 'API documentation has been updated. Check your email!' }
])

// Q&A items
const qaItems = ref([
  { id: 1, question: 'Can we use external APIs?', answer: 'Yes, any public API is allowed.' },
  { id: 2, question: 'What is the submission deadline?', answer: 'Submissions close at 18:00 today.' },
  { id: 3, question: 'Is there a prize for best UI/UX?', answer: 'Yes! Special category prizes will be announced later.' }
])

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

onMounted(() => {
  updateTime()
  intervalId = setInterval(updateTime, 1000)
})

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId)
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
            {{ timeRemaining.isFinished ? 'FINISHED!' : formattedTimer }}
          </div>
        </div>

        <!-- Scoreboard Section -->
        <div class="scoreboard-section">
          <h2>📊 Scoreboard</h2>
          <div class="table-container">
            <table class="scoreboard-table">
              <thead>
                <tr>
                  <th>Rank</th>
                  <th>Team</th>
                  <th>Duration</th>
                  <th>Total Score</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(team, index) in teams.sort((a, b) => b.score - a.score)" :key="team.team" 
                    :class="{ 'first-place': index === 0, 'second-place': index === 1, 'third-place': index === 2 }">
                  <td class="rank">{{ index + 1 }}</td>
                  <td class="team-name">{{ team.team }}</td>
                  <td>{{ team.duration }}</td>
                  <td class="score">{{ team.score }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Sponsor Logo Section -->
        <div class="sponsor-section">
          <h2>🤝 Sponsors</h2>
          <div class="sponsor-logo-container">
            <img src="./assets/images/logo.png" alt="Sponsor Logo" class="sponsor-logo" />
            <img src="./assets/images/logo.png" alt="Sponsor Logo" class="sponsor-logo" />
            <img src="./assets/images/logo.png" alt="Sponsor Logo" class="sponsor-logo" />
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
          <div class="announcements-list">
            <div v-for="announcement in announcements" :key="announcement.id" class="announcement-item">
              <span class="announcement-time">{{ announcement.time }}</span>
              <p class="announcement-message">{{ announcement.message }}</p>
            </div>
          </div>
        </div>

        <!-- Q&A Section -->
        <div class="qa-section">
          <h2>❓ Q&A</h2>
          <div class="qa-list">
            <div v-for="qa in qaItems" :key="qa.id" class="qa-item">
              <p class="question"><strong>Q:</strong> {{ qa.question }}</p>
              <p class="answer"><strong>A:</strong> {{ qa.answer }}</p>
            </div>
          </div>
          <div class="qr-container">
            <p class="qr-label">📱 Scan to submit your question</p>
            <img :src="qrCodeUrl" alt="Q&A QR Code" class="qr-code" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.dashboard {
  height: 100vh;
  background: #000000;
  padding: 8px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  position: relative;
  overflow: hidden;
}

.dashboard-logo {
  position: absolute;
  top: 10px;
  right: 15px;
  height: 200px;
  width: 200px;
  filter: drop-shadow(0 0 10px rgba(250, 71, 134, 0.3));
  z-index: 10;
}

.dashboard-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
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
  font-size: 0.8rem;
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
  font-size: 0.8rem;
  color: #e2e8f0;
}

.rank {
  font-weight: 700;
  font-size: 0.9rem;
}

.team-name {
  font-weight: 600;
  color: #cbd5e0;
}

.score {
  font-weight: 700;
  color: #fa4786;
  font-size: 0.9rem;
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

/* Sponsor Section Styles */
.sponsor-section {
  background: #0a0a0a;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 5px 20px rgba(250, 71, 134, 0.4);
  border: 2px solid #fa4786;
  flex-shrink: 0;
  text-align: center;
}

.sponsor-section h2 {
  color: #fa4786;
  margin-bottom: 15px;
  font-size: 1.2rem;
}

.sponsor-logo-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 15px;
  flex-wrap: wrap;
  padding: 15px;
}

.sponsor-logo {
  width: 100px;
  height: 100px;
  object-fit: contain;
  filter: drop-shadow(0 0 10px rgba(250, 71, 134, 0.5));
  transition: transform 0.3s ease, filter 0.3s ease;
}

.sponsor-logo:hover {
  transform: scale(1.05);
  filter: drop-shadow(0 0 30px rgba(250, 71, 134, 0.7));
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
  font-size: 0.7rem;
}

.announcement-message {
  color: #cbd5e0;
  margin-top: 2px;
  line-height: 1.2;
  font-size: 0.75rem;
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
  font-size: 0.75rem;
}

.answer {
  color: #a0aec0;
  font-size: 0.7rem;
  padding-left: 12px;
}

.qr-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #2a2a2a;
  flex-shrink: 0;
}

.qr-code {
  width: 90px;
  height: 90px;
  border: 2px solid #fa4786;
  border-radius: 4px;
  padding: 5px;
  background: white;
  box-shadow: 0 0 10px rgba(250, 71, 134, 0.3);
  margin-top: 5px;
}

.qr-label {
  color: #fa4786;
  font-weight: 600;
  font-size: 0.7rem;
  text-align: center;
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
