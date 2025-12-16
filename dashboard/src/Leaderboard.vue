<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { leaderboardService } from '@/services/leaderboard.service'
import type { Score } from '@/types/score'

// Scores for both leaderboards
const scoreboardScores = ref<Score[]>([])
const bonusroundScores = ref<Score[]>([])
const isLoadingScoreboard = ref(false)
const isLoadingBonusround = ref(false)

// Format duration from seconds to HH:MM:SS
const formatDuration = (seconds: number): string => {
  const hours = Math.floor(seconds / 3600)
  const minutes = Math.floor((seconds % 3600) / 60)
  const secs = Math.floor(seconds % 60)
  
  return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(secs).padStart(2, '0')}`
}

// Sorted scores by totalScore descending
const sortedScoreboardScores = computed(() => {
  return [...scoreboardScores.value].sort((a, b) => b.totalScore - a.totalScore)
})

const sortedBonusroundScores = computed(() => {
  return [...bonusroundScores.value].sort((a, b) => b.totalScore - a.totalScore)
})

// Fetch scoreboard scores (passKeyType = 'present')
const fetchScoreboardScores = async () => {
  try {
    isLoadingScoreboard.value = true
    const response = await leaderboardService.getScores('present')
    
    if (response.success && response.data) {
      scoreboardScores.value = response.data
    }
  } catch (error: any) {
    console.error('Failed to fetch scoreboard scores:', error)
  } finally {
    isLoadingScoreboard.value = false
  }
}

// Fetch bonusround scores (passKeyType = 'finalist')
const fetchBonusroundScores = async () => {
  try {
    isLoadingBonusround.value = true
    const response = await leaderboardService.getScores('finalist')
    
    if (response.success && response.data) {
      bonusroundScores.value = response.data
    }
  } catch (error: any) {
    console.error('Failed to fetch bonusround scores:', error)
  } finally {
    isLoadingBonusround.value = false
  }
}

let scoreboardIntervalId: number
let bonusroundIntervalId: number

onMounted(() => {
  // Fetch both immediately
  fetchScoreboardScores()
  fetchBonusroundScores()
  
  // Poll every 1 second
  scoreboardIntervalId = setInterval(fetchScoreboardScores, 1000)
  bonusroundIntervalId = setInterval(fetchBonusroundScores, 1000)
})

onUnmounted(() => {
  if (scoreboardIntervalId) {
    clearInterval(scoreboardIntervalId)
  }
  if (bonusroundIntervalId) {
    clearInterval(bonusroundIntervalId)
  }
})
</script>

<template>
  <div class="leaderboard-page">
    <img src="./assets/images/logo.png" alt="Logo" class="page-logo" />
    
    <div class="leaderboard-container">
      <!-- Scoreboard Section -->
      <div class="leaderboard-section">
        <h1 class="section-title">📊 SCOREBOARD</h1>
        <div v-if="isLoadingScoreboard && scoreboardScores.length === 0" class="table-container">
          <div class="loading-message">Loading scoreboard...</div>
        </div>
        <div v-else-if="scoreboardScores.length === 0 && !isLoadingScoreboard" class="table-container">
          <div class="loading-message">No teams have submitted yet.</div>
        </div>
        <div v-else class="table-container">
          <table class="score-table">
            <thead>
              <tr>
                <th>Rank</th>
                <th>Team</th>
                <th>Questions</th>
                <th>Duration</th>
                <th>Total Score</th>
              </tr>
            </thead>
            <transition-group name="score-fade" tag="tbody">
              <tr v-for="(score, index) in sortedScoreboardScores" :key="score.team"
                  :class="{ 'first-place': index === 0, 'second-place': index === 1, 'third-place': index === 2 }">
                <td class="rank">{{ index + 1 }}</td>
                <td class="team-name">{{ score.team }}</td>
                <td class="questions">{{ score.answeredQuestion }}/{{ score.totalQuestion }}</td>
                <td>{{ formatDuration(score.timeUsedInSeconds) }}</td>
                <td class="score">{{ score.totalScore.toFixed(1) }}</td>
              </tr>
            </transition-group>
          </table>
        </div>
      </div>

      <!-- Bonusround Section -->
      <div class="leaderboard-section">
        <h1 class="section-title">🎯 BONUS ROUND</h1>
        <div v-if="isLoadingBonusround && bonusroundScores.length === 0" class="table-container">
          <div class="loading-message">Loading bonus round...</div>
        </div>
        <div v-else-if="bonusroundScores.length === 0 && !isLoadingBonusround" class="table-container">
          <div class="loading-message">No teams have submitted yet.</div>
        </div>
        <div v-else class="table-container">
          <table class="score-table">
            <thead>
              <tr>
                <th>Rank</th>
                <th>Team</th>
                <th>Questions</th>
                <th>Duration</th>
                <th>Total Score</th>
              </tr>
            </thead>
            <transition-group name="score-fade" tag="tbody">
              <tr v-for="(score, index) in sortedBonusroundScores" :key="score.team"
                  :class="{ 'first-place': index === 0, 'second-place': index === 1, 'third-place': index === 2 }">
                <td class="rank">{{ index + 1 }}</td>
                <td class="team-name">{{ score.team }}</td>
                <td class="questions">{{ score.answeredQuestion }}/{{ score.totalQuestion }}</td>
                <td>{{ formatDuration(score.timeUsedInSeconds) }}</td>
                <td class="score">{{ score.totalScore.toFixed(1) }}</td>
              </tr>
            </transition-group>
          </table>
        </div>
      </div>
    </div>

    <!-- Sponsor Section -->
    <div class="sponsor-footer">
      <p class="sponsor-title">Sponsored by</p>
      <div class="sponsor-logo-container">
        <img src="./assets/images/sponsor/viriyah.png" alt="Viriyah" class="sponsor-logo" />
        <img src="./assets/images/sponsor/mtl.png" alt="MTL" class="sponsor-logo" />
        <img src="./assets/images/sponsor/aws.png" alt="AWS" class="sponsor-logo" />
        <img src="./assets/images/sponsor/ngernturbo.png" alt="Ngern Turbo" class="sponsor-logo" />
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

.leaderboard-page {
  min-height: 100vh;
  height: 100vh;
  background:
    linear-gradient(0deg, rgba(0, 45, 114, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 45, 114, 0.05) 1px, transparent 1px),
    radial-gradient(circle at 50% 50%, #000814 0%, #000000 100%);
  background-size: 50px 50px, 50px 50px, 100% 100%;
  background-position: 0 0, 0 0, center;
  background-attachment: fixed;
  padding: 1rem;
  font-family: 'Orbitron', 'Chakra Petch', sans-serif;
  position: relative;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.leaderboard-page::before {
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
}

.page-logo {
  position: absolute;
  top: 1rem;
  right: 1.5rem;
  height: min(12vh, 12vw);
  width: min(12vh, 12vw);
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

.leaderboard-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  flex: 1;
  position: relative;
  z-index: 2;
  overflow: hidden;
  max-height: calc(100vh - 10rem);
}

.leaderboard-section {
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 10px;
  padding: 1rem;
  box-shadow:
    0 0 20px rgba(250, 71, 134, 0.4),
    0 0 40px rgba(0, 45, 114, 0.3),
    inset 0 0 20px rgba(0, 45, 114, 0.1);
  border: 3px solid #fa4786;
  border-image: linear-gradient(45deg, #fa4786, #002d72, #fa4786) 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative;
}

.leaderboard-section::before {
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

.section-title {
  color: #fa4786;
  margin-bottom: 0.8rem;
  font-size: 0.85rem;
  text-align: center;
  flex-shrink: 0;
  text-shadow:
    0 0 10px rgba(250, 71, 134, 0.8),
    0 0 20px rgba(250, 71, 134, 0.6),
    0 0 30px rgba(0, 45, 114, 0.4);
  position: relative;
  z-index: 1;
  font-weight: 900;
  letter-spacing: 2px;
}

.table-container {
  overflow-x: auto;
  overflow-y: auto;
  flex: 1;
  position: relative;
  z-index: 1;
}

.score-table {
  width: 100%;
  border-collapse: collapse;
}

.score-table thead th {
  background: linear-gradient(135deg, #fa4786 0%, #002d72 100%);
  color: white;
  padding: 0.1rem;
  text-align: left;
  font-weight: 700;
  font-size: 0.9rem;
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.5);
  box-shadow: 0 2px 10px rgba(250, 71, 134, 0.3);
  position: sticky;
  top: 0;
  z-index: 10;
}

.score-table thead th:first-child {
  border-top-left-radius: 10px;
}

.score-table thead th:last-child {
  border-top-right-radius: 10px;
}

.score-table tbody tr {
  border-bottom: 1px solid rgba(0, 45, 114, 0.3);
  transition: all 0.3s;
}

.score-table tbody tr:hover {
  background: linear-gradient(90deg, rgba(250, 71, 134, 0.15) 0%, rgba(0, 45, 114, 0.15) 100%);
  box-shadow: 0 0 15px rgba(250, 71, 134, 0.3);
  transform: translateX(5px);
}

.score-table tbody td {
  padding: 0.2rem;
  font-size: 1.0rem;
  color: #e2e8f0;
  font-family: 'Sarabun', sans-serif;
}

.rank {
  font-weight: 700;
  font-size: 1.0rem;
  font-family: 'Sarabun', sans-serif;
}

.team-name {
  font-weight: 600;
  color: #cbd5e0;
  font-size: 1.0rem;
}

.questions {
  font-weight: 600;
  color: #e2e8f0;
  font-size: 1.0rem;
  font-family: 'Sarabun', sans-serif;
}

.score {
  font-weight: 700;
  color: #fa4786;
  font-size: 1.0rem;
  text-shadow: 0 0 5px rgba(250, 71, 134, 0.5);
  font-family: 'Sarabun', sans-serif;
  text-align: center;
}

.first-place {
  background: rgba(255, 215, 0, 0.15);
  border-left: 6px solid #ffd700;
}

.first-place .rank {
  color: #ffd700;
  text-shadow: 0 0 10px rgba(255, 215, 0, 0.6);
}

.second-place {
  background: rgba(192, 192, 192, 0.15);
  border-left: 6px solid #c0c0c0;
}

.second-place .rank {
  color: #c0c0c0;
  text-shadow: 0 0 10px rgba(192, 192, 192, 0.6);
}

.third-place {
  background: rgba(205, 127, 50, 0.15);
  border-left: 6px solid #cd7f32;
}

.third-place .rank {
  color: #cd7f32;
  text-shadow: 0 0 10px rgba(205, 127, 50, 0.6);
}

/* Sponsor Footer */
.sponsor-footer {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 0.5rem;
  gap: 0.5rem;
  background: linear-gradient(135deg, rgba(0, 45, 114, 0.2) 0%, rgba(10, 10, 10, 0.9) 100%);
  border-radius: 8px;
  border: 2px solid;
  border-image: linear-gradient(90deg, #fa4786, #002d72, #fa4786) 1;
  position: relative;
  z-index: 2;
  margin-top: 0.5rem;
}

.sponsor-title {
  color: #fa4786;
  font-weight: 700;
  font-size: 0.8rem;
  text-align: center;
  text-shadow: 0 0 8px rgba(250, 71, 134, 0.6);
}

.sponsor-logo-container {
  display: flex;
  flex-direction: row;
  gap: 0.8rem;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
}

.sponsor-logo {
  height: 50px;
  width: auto;
  max-width: 150px;
  object-fit: contain;
  filter: drop-shadow(0 0 10px rgba(250, 71, 134, 0.3)) drop-shadow(0 0 20px rgba(0, 45, 114, 0.2));
  transition: all 0.3s ease;
  background: white;
  padding: 4px;
  border-radius: 6px;
  border: 2px solid rgba(0, 45, 114, 0.2);
}

.sponsor-logo:hover {
  transform: scale(1.1);
  filter: drop-shadow(0 0 20px rgba(250, 71, 134, 0.6)) drop-shadow(0 0 30px rgba(0, 45, 114, 0.4));
  border-color: rgba(250, 71, 134, 0.4);
}

/* Loading Message */
.loading-message {
  color: #a0aec0;
  text-align: center;
  padding: 3rem;
  font-size: 1.5rem;
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

/* Scrollbar Styles */
::-webkit-scrollbar {
  width: 10px;
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

/* Responsive Design */
@media (max-width: 1200px) {
  .leaderboard-container {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .section-title {
    font-size: 2rem;
  }
  
  .score-table tbody td {
    font-size: 1rem;
    padding: 0.8rem;
  }
}

@media (max-width: 768px) {
  .leaderboard-page {
    padding: 0.5rem;
  }
  
  .section-title {
    font-size: 1.5rem;
  }
  
  .page-logo {
    height: min(8vh, 8vw);
    width: min(8vh, 8vw);
  }
  
  .sponsor-logo {
    height: 40px;
  }
}
</style>
