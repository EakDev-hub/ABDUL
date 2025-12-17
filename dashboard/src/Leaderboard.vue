<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed, watch } from 'vue'
import { leaderboardService } from '@/services/leaderboard.service'
import type { Score } from '@/types/score'
import api from '@/services/api'

// Scores for both leaderboards
const scoreboardScores = ref<Score[]>([])
const bonusroundScores = ref<Score[]>([])
const isLoadingScoreboard = ref(false)
const isLoadingBonusround = ref(false)

// On stage control flag
const onStage = ref(true) // true = show scoreboard, false = show bonus round

// Track score changes for animations
const changedTeams = ref<Set<string>>(new Set())
const previousScores = ref<Map<string, number>>(new Map())

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

// Split into Top 3 and Rest
const scoreboardTop3 = computed(() => sortedScoreboardScores.value.slice(0, 3))
const scoreboardRest = computed(() => sortedScoreboardScores.value.slice(3))
const bonusTop3 = computed(() => sortedBonusroundScores.value.slice(0, 3))
const bonusRest = computed(() => sortedBonusroundScores.value.slice(3))

const fetchScoreboardScores = async () => {
  try {
    isLoadingScoreboard.value = true
    const response = await leaderboardService.getScores('present')
    
    if (response.success && response.data) {
      // Detect score changes
      detectScoreChanges(response.data)
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
      // Detect score changes for bonus round too
      detectScoreChanges(response.data)
      bonusroundScores.value = response.data
    }
  } catch (error: any) {
    console.error('Failed to fetch bonus round scores:', error)
  } finally {
    isLoadingBonusround.value = false
  }
}

// Fetch on_stage flag from DisplayControl API
const fetchOnStageFlag = async () => {
  try {
    const response = await api.get<{
      success: boolean;
      data: {
        onStage: boolean
      }
    }>('/api/DisplayControl')
    
    if (response.data?.success && response.data.data) {
      // Directly use the onStage boolean value from API
      onStage.value = response.data.data.onStage
    }
  } catch (error: any) {
    console.error('Failed to fetch on_stage flag:', error)
    // Keep current value on error to avoid flickering
  }
}

// Detect score changes and trigger animations
const detectScoreChanges = (newScores: Score[]) => {
  const currentChangedTeams = new Set<string>()
  
  newScores.forEach(score => {
    const previousScore = previousScores.value.get(score.team)
    
    if (previousScore !== undefined && previousScore !== score.totalScore) {
      // Score changed!
      currentChangedTeams.add(score.team)
      
      // Remove highlight after 3 seconds
      setTimeout(() => {
        changedTeams.value.delete(score.team)
      }, 3000)
    }
    
    // Update previous score
    previousScores.value.set(score.team, score.totalScore)
  })
  
  // Update changed teams
  currentChangedTeams.forEach(team => changedTeams.value.add(team))
}

// Check if a team has changed score
const hasScoreChanged = (teamName: string): boolean => {
  return changedTeams.value.has(teamName)
}

let scoreboardIntervalId: number
let bonusroundIntervalId: number

let onStageIntervalId: number

onMounted(() => {
  // Fetch both immediately
  fetchScoreboardScores()
  fetchBonusroundScores()
  fetchOnStageFlag()
  
  // Poll every 1 second
  scoreboardIntervalId = setInterval(fetchScoreboardScores, 1000)
  bonusroundIntervalId = setInterval(fetchBonusroundScores, 1000)
  onStageIntervalId = setInterval(fetchOnStageFlag, 2000) // Check on_stage every 2 seconds
})

onUnmounted(() => {
  if (scoreboardIntervalId) {
    clearInterval(scoreboardIntervalId)
  }
  if (bonusroundIntervalId) {
    clearInterval(bonusroundIntervalId)
  }
  if (onStageIntervalId) {
    clearInterval(onStageIntervalId)
  }
})
</script>

<template>
  <div class="leaderboard-page">
    <div class="bg-grid"></div>
    <div class="vignette"></div>
    
    <!-- Floating Logo -->
    <img src="./assets/images/logo.png" alt="Logo" class="floating-logo" />
    
    <main class="main-content">
      <!-- SCOREBOARD - shown when on_stage is true -->
      <section v-if="onStage" class="board navy-board single-board">
        <h2 class="board-title">SCOREBOARD</h2>
        
        <!-- Loading / Empty States -->
        <div v-if="isLoadingScoreboard && scoreboardScores.length === 0" class="loading-container">
          <div class="loading-message">Loading scoreboard...</div>
        </div>
        <div v-else-if="scoreboardScores.length === 0 && !isLoadingScoreboard" class="loading-container">
          <div class="loading-message">No teams have submitted yet.</div>
        </div>
        
        <!-- Content -->
        <div v-else class="board-content">
          <!-- Podium Top 3 -->
          <div class="podium">
            <!-- 2nd Place -->
            <div class="podium-card silver" v-if="scoreboardTop3[1]" :class="{ 'score-changed': hasScoreChanged(scoreboardTop3[1].team) }">
              <div class="podium-rank">2</div>
              <div class="podium-team">{{ scoreboardTop3[1].team }}</div>
              <div class="podium-score">{{ scoreboardTop3[1].totalScore.toFixed(1) }} <span class="pts">pts.</span></div>
              <div class="podium-stats">Q: {{ scoreboardTop3[1].answeredQuestion }}/{{ scoreboardTop3[1].totalQuestion }} · Time: {{ formatDuration(scoreboardTop3[1].timeUsedInSeconds) }}</div>
            </div>
            
            <!-- 1st Place (Center, Tallest) -->
            <div class="podium-card gold" v-if="scoreboardTop3[0]" :class="{ 'score-changed': hasScoreChanged(scoreboardTop3[0].team) }">
              <div class="podium-rank">1</div>
              <div class="podium-team">{{ scoreboardTop3[0].team }}</div>
              <div class="podium-score">{{ scoreboardTop3[0].totalScore.toFixed(1) }} <span class="pts">pts.</span></div>
              <div class="podium-stats">Q: {{ scoreboardTop3[0].answeredQuestion }}/{{ scoreboardTop3[0].totalQuestion }} · Time: {{ formatDuration(scoreboardTop3[0].timeUsedInSeconds) }}</div>
            </div>
            
            <!-- 3rd Place -->
            <div class="podium-card bronze" v-if="scoreboardTop3[2]" :class="{ 'score-changed': hasScoreChanged(scoreboardTop3[2].team) }">
              <div class="podium-rank">3</div>
              <div class="podium-team">{{ scoreboardTop3[2].team }}</div>
              <div class="podium-score">{{ scoreboardTop3[2].totalScore.toFixed(1) }} <span class="pts">pts.</span></div>
              <div class="podium-stats">Q: {{ scoreboardTop3[2].answeredQuestion }}/{{ scoreboardTop3[2].totalQuestion }} · Time: {{ formatDuration(scoreboardTop3[2].timeUsedInSeconds) }}</div>
            </div>
          </div>
          
          <!-- Rows for 4-15 -->
          <div class="rest-rows">
            <div class="row-card" v-for="(score, idx) in scoreboardRest" :key="score.team" :class="{ 'score-changed': hasScoreChanged(score.team) }">
              <span class="row-rank">{{ idx + 4 }}</span>
              <span class="row-team">{{ score.team }}</span>
              <span class="row-stats">Q: {{ score.answeredQuestion }}/{{ score.totalQuestion }} · Time: {{ formatDuration(score.timeUsedInSeconds) }}</span>
              <span class="row-score">{{ score.totalScore.toFixed(1) }} <span class="pts">pts.</span></span>
            </div>
          </div>
        </div>
      </section>

      <!-- BONUS ROUND - shown when on_stage is false -->
      <section v-if="!onStage" class="board pink-board single-board">
        <h2 class="board-title">BONUS ROUND</h2>
        
        <!-- Loading / Empty States -->
        <div v-if="isLoadingBonusround && bonusroundScores.length === 0" class="loading-container">
          <div class="loading-message">Loading bonus round...</div>
        </div>
        <div v-else-if="bonusroundScores.length === 0 && !isLoadingBonusround" class="loading-container">
          <div class="loading-message">No teams have submitted yet.</div>
        </div>

        <div v-else class="board-content">
          <!-- Podium Top 3 -->
          <div class="podium">
            <div class="podium-card silver" v-if="bonusTop3[1]" :class="{ 'score-changed': hasScoreChanged(bonusTop3[1].team) }">
              <div class="podium-rank">2</div>
              <div class="podium-team">{{ bonusTop3[1].team }}</div>
              <div class="podium-score">{{ bonusTop3[1].totalScore.toFixed(1) }} <span class="pts">pts.</span></div>
              <div class="podium-stats">Q: {{ bonusTop3[1].answeredQuestion }}/{{ bonusTop3[1].totalQuestion }} · Time: {{ formatDuration(bonusTop3[1].timeUsedInSeconds) }}</div>
            </div>
            
            <div class="podium-card gold" v-if="bonusTop3[0]" :class="{ 'score-changed': hasScoreChanged(bonusTop3[0].team) }">
              <div class="podium-rank">1</div>
              <div class="podium-team">{{ bonusTop3[0].team }}</div>
              <div class="podium-score">{{ bonusTop3[0].totalScore.toFixed(1) }} <span class="pts">pts.</span></div>
              <div class="podium-stats">Q: {{ bonusTop3[0].answeredQuestion }}/{{ bonusTop3[0].totalQuestion }} · Time: {{ formatDuration(bonusTop3[0].timeUsedInSeconds) }}</div>
            </div>
            
            <div class="podium-card bronze" v-if="bonusTop3[2]" :class="{ 'score-changed': hasScoreChanged(bonusTop3[2].team) }">
              <div class="podium-rank">3</div>
              <div class="podium-team">{{ bonusTop3[2].team }}</div>
              <div class="podium-score">{{ bonusTop3[2].totalScore.toFixed(1) }} <span class="pts">pts.</span></div>
              <div class="podium-stats">Q: {{ bonusTop3[2].answeredQuestion }}/{{ bonusTop3[2].totalQuestion }} · Time: {{ formatDuration(bonusTop3[2].timeUsedInSeconds) }}</div>
            </div>
          </div>
          
          <!-- Rows for 4-15 -->
          <div class="rest-rows">
            <div class="row-card bonus" v-for="(score, idx) in bonusRest" :key="score.team" :class="{ 'score-changed': hasScoreChanged(score.team) }">
              <span class="row-rank">{{ idx + 4 }}</span>
              <span class="row-team">{{ score.team }}</span>
              <span class="row-stats">Q: {{ score.answeredQuestion }}/{{ score.totalQuestion }} · Time: {{ formatDuration(score.timeUsedInSeconds) }}</span>
              <span class="row-score">{{ score.totalScore.toFixed(1) }} <span class="pts">pts.</span></span>
            </div>
          </div>
        </div>
      </section>
    </main>

    <!-- Footer -->
    <footer class="sponsor-bar">
      <span class="sponsor-text">SPONSORED BY</span>
      <div class="logos">
        <img src="./assets/images/sponsor/viriyah.png" alt="Viriyah" class="logo" />
        <img src="./assets/images/sponsor/mtl.png" alt="MTL" class="logo" />
        <img src="./assets/images/sponsor/aws.png" alt="AWS" class="logo" />
        <img src="./assets/images/sponsor/ngernturbo.png" alt="Ngern Turbo" class="logo" />
      </div>
    </footer>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;700;900&family=Rajdhani:wght@500;600;700&display=swap');

:root {
  --navy: #002d72;
  --pink: #ff0066;
  --gold: #ffd700;
  --silver: #c0c0c0;
  --bronze: #cd7f32;
}

* { box-sizing: border-box; margin: 0; padding: 0; }

.leaderboard-page {
  width: 100vw;
  height: 100vh;
  background: linear-gradient(160deg, #050010 0%, #0a0020 50%, #050010 100%);
  color: #fff;
  font-family: 'Rajdhani', sans-serif;
  overflow: hidden;
  display: grid;
  grid-template-rows: 1fr auto;
  position: relative;
}

.bg-grid {
  position: absolute;
  inset: 0;
  background: 
    linear-gradient(rgba(255,0,102,0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,0,102,0.03) 1px, transparent 1px);
  background-size: 50px 50px;
  z-index: 0;
}

.vignette {
  position: absolute;
  inset: 0;
  background: radial-gradient(ellipse at 50% 50%, transparent 40%, #000 100%);
  pointer-events: none;
  z-index: 1;
}

/* FLOATING LOGO */
.floating-logo {
  position: absolute;
  top: 1.5vh;
  right: 2vw;
  height: 10vh;
  z-index: 100;
  filter: drop-shadow(0 0 8px #ff0066) drop-shadow(0 0 20px #ff006660);
  animation: floatPulse 4s ease-in-out infinite, glowPulse 3s ease-in-out infinite alternate;
}

@keyframes floatPulse {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

@keyframes glowPulse {
  0% { filter: drop-shadow(0 0 6px #ff0066) drop-shadow(0 0 15px #ff006650); }
  100% { filter: drop-shadow(0 0 12px #ff0066) drop-shadow(0 0 30px #ff006680); }
}

/* MAIN */
.main-content {
  z-index: 10;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1vw;
  padding: 0.5vh 1vw;
  overflow: hidden;
}

/* Single board takes full width when only one is shown */
.single-board {
  grid-column: 1 / -1;
  width: 100%;
  max-width: 100%;
  margin: 0;
}

/* BOARD */
.board {
  display: flex;
  flex-direction: column;
  gap: 0.5vh; /* Reduced from 1vh */
}

.board-title {
  font-family: 'Orbitron', sans-serif;
  font-size: 2.5vh;
  text-align: center;
  letter-spacing: 4px;
}

.board-content {
  display: flex;
  flex-direction: column;
  flex: 1;
  gap: 0.5vh;
  overflow: hidden;
}

.loading-container {
  display: flex;
  align-items: center;
  justify-content: center;
  flex: 1;
}

.loading-message {
  font-family: 'Orbitron';
  font-size: 2vh;
  color: rgba(255,255,255,0.5);
  text-align: center;
  letter-spacing: 2px;
}

.navy-board .board-title {
  color: #fff;
  text-shadow: 
    0 0 5px #00aaff,
    0 0 10px #00aaff,
    0 0 20px #00aaff,
    0 0 40px #002d72;
  font-weight: 900;
  border-bottom: 2px solid #00aaff;
  box-shadow: 0 10px 20px -10px rgba(0, 170, 255, 0.5);
  display: inline-block;
  padding-bottom: 5px;
}

.pink-board .board-title {
  color: #fff;
  text-shadow: 
    0 0 5px #ff0066,
    0 0 10px #ff0066,
    0 0 20px #ff0066,
    0 0 40px #ff0066;
  font-weight: 900;
  border-bottom: 2px solid #ff0066;
  box-shadow: 0 10px 20px -10px rgba(255, 0, 102, 0.5);
  display: inline-block;
  padding-bottom: 5px;
}

/* PODIUM - Top 3 Cards */
.podium {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  gap: 0.8vw;
  height: 22vh; /* Reverted to 22vh */
  padding: 0;
}

.podium-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 1vh 0.8vw; /* Reverted to 1vh */
  text-align: center;
  position: relative;
  border-radius: 8px;
  border: none;
}

.podium-card.gold {
  width: 30%;
  height: 100%;
  background: rgba(255, 215, 0, 0.15);
  color: #ffd700;
  text-shadow: 0 0 10px rgba(255, 215, 0, 0.7), 0 0 25px rgba(255, 215, 0, 0.4);
}

.podium-card.silver {
  width: 26%;
  height: 80%;
  background: rgba(192, 192, 192, 0.15);
  color: #e0e0e0;
  text-shadow: 0 0 10px rgba(192, 192, 192, 0.7), 0 0 25px rgba(192, 192, 192, 0.4);
}

.podium-card.bronze {
  width: 26%;
  height: 75%;
  background: rgba(205, 127, 50, 0.15);
  color: #e8a86c;
  text-shadow: 0 0 10px rgba(205, 127, 50, 0.7), 0 0 25px rgba(205, 127, 50, 0.4);
}

.podium-rank {
  font-family: 'Orbitron';
  font-weight: 700;
  font-size: 1.5vh;
  opacity: 0.7;
}

.podium-team {
  font-weight: 700;
  font-size: 2vh;
  margin: 0.3vh 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
}

.podium-score {
  font-family: 'Orbitron';
  font-weight: 900;
  font-size: 3.5vh;
  /* Use parent card color/glow */
  color: inherit;
  text-shadow: inherit;
}

.pink-board .podium-score {
  font-size: 3.5vh;
  /* Use parent card color/glow */
  color: inherit;
  text-shadow: inherit;
}

.podium-stats {
  font-size: 1.6vh;
  font-weight: 700;
  opacity: 1;
  margin-top: 0.5vh;
  color: #ddd;
}

.pts {
  font-size: 0.6em;
  opacity: 0.7;
}

/* REST ROWS - Teams 4-15 */
.rest-rows {
  display: flex;
  flex-direction: column;
  gap: 0.4vh;
  flex: 1;
  overflow: hidden;
}

.row-card {
  display: flex;
  align-items: center;
  gap: 1vw;
  padding: 0.7vh 0.6vw;
  background: rgba(0,45,114,0.2);
  border-radius: 4px;
  border-left: 3px solid #8eb8ff;
}

.row-card.bonus {
  background: rgba(255,0,102,0.12);
  border-left: 3px solid #ff0066;
}

.row-rank {
  font-family: 'Orbitron';
  font-weight: 700;
  font-size: 1.8vh;
  color: #888;
  min-width: 2vw;
  text-align: center;
}

.row-team {
  flex: 1;
  font-weight: 700;
  font-size: 2vh;
  color: #8eb8ff;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-shadow: 0 0 8px rgba(142, 184, 255, 0.8), 0 0 15px rgba(142, 184, 255, 0.4);
}

.row-card.bonus .row-team {
  color: #fff;
  text-shadow: 0 0 8px rgba(255, 0, 102, 0.8), 0 0 15px rgba(255, 0, 102, 0.4);
}

.row-stats {
  font-size: 1.7vh;
  color: #ccc;
  min-width: 14vw;
  text-align: center;
  font-weight: 600;
}

.row-score {
  font-family: 'Orbitron';
  font-weight: 900;
  font-size: 2.2vh;
  color: #8eb8ff;
  text-shadow: 0 0 8px rgba(142, 184, 255, 0.8), 0 0 15px rgba(142, 184, 255, 0.4);
  min-width: 6vw;
  text-align: right;
}

.row-card.bonus .row-score {
  color: var(--pink);
  font-size: 2.2vh;
  text-shadow: 0 0 8px rgba(255, 0, 102, 0.8), 0 0 15px rgba(255, 0, 102, 0.4);
}

/* FOOTER */
.sponsor-bar {
  z-index: 20;
  background: rgba(0,0,0,0.9);
  padding: 0.4vh 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.2vh;
  border-top: 1px solid rgba(255,0,102,0.3);
}

.sponsor-text {
  font-size: 0.9vh;
  color: var(--pink);
  letter-spacing: 3px;
}

.logos {
  display: flex;
  gap: 2vw;
  align-items: center;
}

.logo {
  height: 5vh; /* Reduced from 6vh to save vertical space */
  width: 5vh;
  object-fit: contain;
  background: #fff;
  padding: 4px;
  border-radius: 5px;
  box-shadow: 0 0 8px rgba(255, 255, 255, 0.15);
}

/* Score change animation */
@keyframes scoreChange {
  0%, 100% {
    transform: scale(1);
    filter: brightness(1);
  }
  50% {
    transform: scale(1.05);
    filter: brightness(1.3);
  }
}

@keyframes pulseGlow {
  0%, 100% {
    box-shadow: 0 0 10px currentColor;
  }
  50% {
    box-shadow: 0 0 25px currentColor, 0 0 40px currentColor;
  }
}

.score-changed {
  animation: scoreChange 0.8s ease-in-out, pulseGlow 1.5s ease-in-out 3;
}

.score-changed .podium-score,
.score-changed .row-score {
  animation: scoreChange 0.8s ease-in-out;
}
</style>
