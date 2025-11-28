<template>
  <div class="hackathon-container">
    <!-- Animated Background -->
    <div class="animated-bg"></div>
    <div class="grid-overlay"></div>

    <!-- Hacker Terminal Modal -->
    <transition name="terminal-fade">
      <div v-if="showTerminal" class="terminal-overlay">
        <div ref="terminalContentRef" class="terminal-content">
          <div class="terminal-header">
            <span class="terminal-title">{{ terminalTitle }}</span>
            <span class="terminal-cursor">_</span>
          </div>
          <pre ref="terminalTextRef" class="terminal-text">{{ displayedText }}<span class="cursor-blink">█</span></pre>
        </div>
        <div class="scanline"></div>
      </div>
    </transition>

    <!-- Main Content -->
    <div v-if="showTerminal" class="terminal-blocker"></div>
    <div class="main-content">
      <!-- Header -->
      <div v-if="!result" class="header-section">
        <h1 class="main-title">
          <span class="glitch" data-text="HACKATHON #2 2025">HACKATHON #2 2025</span>
        </h1>
        <p class="subtitle">
          <span class="ai-badge">AI</span> ระบบตรวจคำตอบและให้คะแนน
        </p>
      </div>

      <!-- Form Section -->
      <div v-if="!result" class="form-container">
        <div class="form-card">
          <form @submit.prevent="handleSubmit">
            <!-- Team Name -->
            <div class="input-group">
              <label class="input-label">
                <span class="label-icon">👥</span> Team
              </label>
              <input
                v-model="formData.team"
                type="text"
                class="cyber-input"
                placeholder="ชื่อทีม"
                required
              />
            </div>

            <!-- Pass Key -->
            <div class="input-group">
              <label class="input-label">
                <span class="label-icon">🔑</span> Pass Key
              </label>
              <input
                v-model="formData.passKey"
                type="text"
                class="cyber-input"
                placeholder="AAAAMMMMNANSXASX"
                required
              />
            </div>

            <!-- API URL -->
            <div class="input-group">
              <label class="input-label">
                <span class="label-icon">🌐</span> API URL
              </label>
              <input
                v-model="formData.apiUrl"
                type="text"
                class="cyber-input"
                placeholder="aa.bb.cc:8088/api/test"
                required
              />
            </div>

            <!-- Submit Button -->
            <button
              type="submit"
              :disabled="isLoading"
              class="cyber-button"
            >
              <span v-if="isLoading" class="button-loading">
                <span class="loading-spinner"></span>
                PROCESSING...
              </span>
              <span v-else class="button-text">
                <span class="button-icon">⚡</span>
                SUBMIT & ANALYZE
              </span>
            </button>
          </form>

          <!-- Error Message -->
          <div v-if="error" class="error-message">
            <span class="error-icon">⚠️</span>
            <p>{{ error }}</p>
          </div>
        </div>
      </div>

      <!-- Results Section -->
      <transition name="results-fade">
        <div v-if="result && !showTerminal" class="results-container">
          <!-- Summary -->
          <div class="summary-card">
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
                <p class="stat-label">UUID</p>
                <p class="stat-uuid">
                  {{ result.uuid }}
                </p>
              </div>
            </div>
            <div class="summary-footer">
              <span class="pass-badge" :class="`badge-${result.passKeyType}`">
                {{ result.passKeyType.toUpperCase() }}
              </span>
              <span class="duration-text">
                Max Duration: {{ result.maxDurationInSecs }} วินาที
              </span>
            </div>
            <!-- Reset Button -->
            <div class="reset-button-container">
              <button @click="resetForm" class="reset-button">
                <span class="button-icon">🔄</span>
                ส่งใหม่อีกครั้ง
              </button>
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
      </transition>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useHackathonStore } from '@/store/hackathon.store'
import { hackathonService } from '@/services/hackathon.service'
import type { SubmitRequest } from '@/types/hackathon'

const hackathonStore = useHackathonStore()

const formData = ref<SubmitRequest>({
  team: '',
  passKey: '',
  apiUrl: ''
})

const showTerminal = ref(false)
const displayedText = ref('')
const terminalTitle = ref('INITIALIZING AI ANALYSIS SYSTEM...')
const terminalContentRef = ref<HTMLElement | null>(null)
const terminalTextRef = ref<HTMLElement | null>(null)

const isLoading = computed(() => hackathonStore.isLoading)
const error = computed(() => hackathonStore.error)
const result = computed(() => hackathonStore.result)

// Hacker typer code samples
const codeSnippets = [
  `// Initializing neural network...\nconst model = tf.sequential();\nmodel.add(tf.layers.dense({units: 128, activation: 'relu'}));\n\n`,
  `// Loading AI models...\nimport { GPT4, BERT, Transformer } from '@ai/models';\nconst analyzer = new Transformer();\n\n`,
  `// Processing input data...\nfor (let i = 0; i < dataset.length; i++) {\n  const features = extractFeatures(dataset[i]);\n  predictions.push(model.predict(features));\n}\n\n`,
  `// Running semantic analysis...\nconst embeddings = await encoder.encode(inputText);\nconst similarity = cosineSimilarity(embeddings, targetEmbeddings);\n\n`,
  `// Calculating confidence scores...\nconst scores = predictions.map(p => {\n  return { value: p.value, confidence: p.probability * 100 };\n});\n\n`,
  `// Validating results...\nif (scores.every(s => s.confidence > 0.85)) {\n  console.log('[SUCCESS] Analysis complete');\n  return { status: 'COMPLETE', data: scores };\n}\n\n`,
  `// Generating final report...\nconst report = {\n  timestamp: Date.now(),\n  accuracy: calculateAccuracy(predictions, groundTruth),\n  summary: generateSummary(results)\n};\n\n`
]

function getScoreClass(score: number): string {
  if (score >= 0.8) return 'score-high'
  if (score >= 0.5) return 'score-medium'
  return 'score-low'
}

async function typeText(text: string, speed: number = 15): Promise<void> {
  for (let i = 0; i < text.length; i++) {
    displayedText.value += text[i]
    await new Promise(resolve => setTimeout(resolve, speed))
    // Auto-scroll to bottom
    if (terminalContentRef.value) {
      terminalContentRef.value.scrollTop = terminalContentRef.value.scrollHeight
    }
  }
}

async function runTerminalAnimation() {
  displayedText.value = ''
  // Type random code snippets
  for (const snippet of codeSnippets) {
    await typeText(snippet, 8)
  }
  // Final messages
  terminalTitle.value = 'ANALYSIS COMPLETE'
  await typeText('\n[✓] All systems operational\n', 30)
  await typeText('[✓] Results validated\n', 30)
  await typeText('[✓] Report generated\n\n', 30)
  await typeText('>>> DISPLAYING RESULTS...', 40)
  // Wait a bit before closing
  await new Promise(resolve => setTimeout(resolve, 800))
}

async function handleSubmit() {
  hackathonStore.setLoading(true)
  hackathonStore.setError(null)
  // Show terminal
  showTerminal.value = true
  // Start animation and API call in parallel
  const animationPromise = runTerminalAnimation()
  const apiPromise = (async () => {
    try {
      const response = await hackathonService.submitAnswer(formData.value)
      hackathonStore.setResult(response)
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'เกิดข้อผิดพลาดในการส่งข้อมูล'
      hackathonStore.setError(errorMessage)
    }
  })()
  // Wait for both to complete
  await Promise.all([animationPromise, apiPromise])
  // Hide terminal and show results
  showTerminal.value = false
  hackathonStore.setLoading(false)
}

function resetForm() {
  hackathonStore.setResult(undefined as any)
  hackathonStore.setError(null)
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;700;900&family=Courier+Prime:wght@400;700&display=swap');

.hackathon-container {
  min-height: 100vh;
  height: 100vh;
  position: relative;
  overflow-x: hidden;
  overflow-y: hidden;
  background: #000000;
}

/* Terminal Blocker - prevents scrolling when terminal is active */
.terminal-blocker {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 9998;
  overflow: hidden;
}

/* Animated Background */
.animated-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #000814 0%, #001d3d 25%, #003566 50%, #001d3d 75%, #000814 100%);
  background-size: 400% 400%;
  animation: gradientShift 15s ease infinite;
  z-index: 0;
}

@keyframes gradientShift {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}

.grid-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image:
    linear-gradient(rgba(0, 180, 216, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 180, 216, 0.03) 1px, transparent 1px);
  background-size: 50px 50px;
  animation: gridMove 20s linear infinite;
  z-index: 1;
}

@keyframes gridMove {
  0% { transform: translate(0, 0); }
  100% { transform: translate(50px, 50px); }
}

.main-content {
  position: relative;
  z-index: 2;
  padding: 2rem;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

/* Header */
.header-section {
  text-align: center;
  margin-bottom: 3rem;
}

.main-title {
  font-family: 'Orbitron', sans-serif;
  font-size: 4rem;
  font-weight: 900;
  margin-bottom: 1rem;
  position: relative;
}

.glitch {
  color: #00b4d8;
  text-shadow:
    0 0 10px #00b4d8,
    0 0 20px #00b4d8,
    0 0 30px #00b4d8,
    0 0 40px #0096c7,
    0 0 70px #0077b6,
    0 0 80px #023e8a;
  animation: glitchText 3s infinite;
}

@keyframes glitchText {
  0%, 90%, 100% {
    transform: translate(0);
  }
  92% {
    transform: translate(-2px, 2px);
  }
  94% {
    transform: translate(2px, -2px);
  }
  96% {
    transform: translate(-2px, -2px);
  }
}

.subtitle {
  font-family: 'Orbitron', sans-serif;
  font-size: 1.5rem;
  color: #90e0ef;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.ai-badge {
  background: linear-gradient(135deg, #00b4d8, #0096c7);
  color: #000;
  padding: 0.25rem 0.75rem;
  border-radius: 0.5rem;
  font-weight: 700;
  font-size: 1rem;
  animation: pulse 2s infinite;
  box-shadow: 0 0 10px rgba(0, 180, 216, 0.6);
}

@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

/* Form */
.form-container {
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  padding: 0 2rem;
}

.form-card {
  background: rgba(0, 13, 20, 0.95);
  border: 2px solid #00b4d8;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow:
    0 0 20px rgba(0, 180, 216, 0.4),
    0 0 40px rgba(0, 150, 199, 0.3),
    inset 0 0 60px rgba(0, 180, 216, 0.05);
  backdrop-filter: blur(10px);
}

.input-group {
  margin-bottom: 1.5rem;
}

.input-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #00b4d8;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  font-size: 1.1rem;
  margin-bottom: 0.5rem;
  text-shadow: 0 0 10px rgba(0, 180, 216, 0.6);
}

.label-icon {
  font-size: 1.2rem;
}

.cyber-input {
  width: 100%;
  padding: 1rem;
  background: rgba(0, 13, 20, 0.9);
  border: 2px solid #00b4d8;
  border-radius: 0.5rem;
  color: #90e0ef;
  font-family: 'Courier Prime', monospace;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.cyber-input:focus {
  outline: none;
  border-color: #0096c7;
  box-shadow:
    0 0 20px rgba(0, 180, 216, 0.6),
    inset 0 0 20px rgba(0, 180, 216, 0.1);
  transform: translateY(-2px);
}

.cyber-input::placeholder {
  color: rgba(0, 180, 216, 0.3);
}

.cyber-button {
  width: 100%;
  padding: 1.25rem;
  background: linear-gradient(135deg, #003566, #00b4d8);
  border: 2px solid #00b4d8;
  border-radius: 0.5rem;
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 900;
  font-size: 1.25rem;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  box-shadow: 0 0 20px rgba(0, 180, 216, 0.4);
}

.cyber-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.cyber-button:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow:
    0 0 30px rgba(0, 180, 216, 0.8),
    0 0 60px rgba(0, 150, 199, 0.6);
}

.button-text, .button-loading {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.button-icon {
  font-size: 1.5rem;
  animation: zap 1.5s infinite;
}

@keyframes zap {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.2); }
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(0, 0, 0, 0.3);
  border-top-color: #000;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Error Message */
.error-message {
  margin-top: 1rem;
  padding: 1rem;
  background: rgba(255, 0, 0, 0.1);
  border: 2px solid #ff0000;
  border-radius: 0.5rem;
  color: #ff6b6b;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-family: 'Courier Prime', monospace;
}

.error-icon {
  font-size: 1.5rem;
}

/* Terminal Modal */
.terminal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.95);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
}

.terminal-content {
  width: 90%;
  max-width: 1200px;
  height: 80vh;
  background: #000;
  border: 3px solid #00ff00;
  border-radius: 0.5rem;
  padding: 2rem;
  font-family: 'Courier Prime', monospace;
  overflow: auto;
  box-shadow:
    0 0 50px rgba(0, 255, 0, 0.5),
    inset 0 0 100px rgba(0, 255, 0, 0.05);
}

.terminal-header {
  color: #00ff00;
  font-size: 1.2rem;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.terminal-title {
  text-shadow: 0 0 10px #00ff00;
}

.terminal-cursor {
  animation: blink 1s infinite;
}

.terminal-text {
  color: #00ff00;
  font-size: 1rem;
  line-height: 1.6;
  white-space: pre-wrap;
  word-wrap: break-word;
  text-shadow: 0 0 5px rgba(0, 255, 0, 0.5);
}

.cursor-blink {
  animation: blink 0.7s infinite;
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
    rgba(0, 255, 0, 0.05) 50%,
    transparent 100%
  );
  animation: scan 8s linear infinite;
  pointer-events: none;
}

@keyframes scan {
  0% { transform: translateY(-100%); }
  100% { transform: translateY(100%); }
}

.terminal-fade-enter-active, .terminal-fade-leave-active {
  transition: opacity 0.5s;
}

.terminal-fade-enter-from, .terminal-fade-leave-to {
  opacity: 0;
}

/* Results */
.results-container {
  max-width: 1400px;
  margin: 0 auto;
}

.summary-card {
  background: rgba(0, 13, 20, 0.95);
  border: 2px solid #00b4d8;
  border-radius: 1rem;
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow:
    0 0 20px rgba(0, 180, 216, 0.4),
    inset 0 0 60px rgba(0, 180, 216, 0.05);
  backdrop-filter: blur(10px);
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 1.5rem;
}

.stat-box {
  text-align: center;
}

.stat-label {
  color: #90e0ef;
  font-family: 'Orbitron', sans-serif;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
  text-transform: uppercase;
}

.stat-value {
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  font-size: 2.5rem;
  font-weight: 900;
  text-shadow: 0 0 20px rgba(255, 255, 255, 0.5);
}

.stat-highlight {
  color: #00b4d8;
  text-shadow: 0 0 20px rgba(0, 180, 216, 0.8);
}

.stat-unit {
  font-size: 1.5rem;
  color: #90e0ef;
}

.stat-uuid {
  color: #90e0ef;
  font-family: 'Courier Prime', monospace;
  font-size: 1rem;
  word-break: break-all;
}

.summary-footer {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 2rem;
  flex-wrap: wrap;
}

.reset-button-container {
  margin-top: 2rem;
  text-align: center;
}

.reset-button {
  padding: 1rem 2rem;
  background: linear-gradient(135deg, #003566, #00b4d8);
  border: 2px solid #00b4d8;
  border-radius: 0.5rem;
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 900;
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 0 20px rgba(0, 180, 216, 0.4);
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.reset-button:hover {
  transform: translateY(-3px);
  box-shadow:
    0 0 30px rgba(0, 180, 216, 0.8),
    0 0 60px rgba(0, 150, 199, 0.6);
}

.pass-badge {
  padding: 0.5rem 1.5rem;
  border-radius: 2rem;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  font-size: 1rem;
}

.badge-develop {
  background: linear-gradient(135deg, #00ff00, #00cc00);
  color: #000;
}

.badge-present {
  background: linear-gradient(135deg, #ff6600, #ff0000);
  color: #fff;
}

.badge-finalist {
  background: linear-gradient(135deg, #ff0000, #cc0000);
  color: #fff;
}

.duration-text {
  color: #888;
  font-family: 'Courier Prime', monospace;
}

/* Results Table */
.results-table-card {
  background: rgba(0, 13, 20, 0.95);
  border: 2px solid #00b4d8;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow:
    0 0 20px rgba(0, 180, 216, 0.4),
    inset 0 0 60px rgba(0, 180, 216, 0.05);
  backdrop-filter: blur(10px);
}

.table-wrapper {
  overflow-x: auto;
}

.results-table {
  width: 100%;
  border-collapse: collapse;
  font-family: 'Courier Prime', monospace;
}

.results-table thead {
  background: rgba(0, 180, 216, 0.1);
}

.results-table th {
  padding: 1rem;
  text-align: left;
  color: #00b4d8;
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  border-bottom: 2px solid #00b4d8;
}

.results-table th.text-center {
  text-align: center;
}

.table-row {
  border-bottom: 1px solid rgba(0, 180, 216, 0.2);
  transition: all 0.3s ease;
}

.table-row:hover {
  background: rgba(0, 180, 216, 0.05);
  transform: translateX(5px);
}

.results-table td {
  padding: 1rem;
  color: #fff;
}

.results-table td.text-center {
  text-align: center;
}

.row-number {
  color: #00b4d8;
  font-weight: 700;
}

.score-badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 2rem;
  font-weight: 700;
}

.score-high {
  background: rgba(0, 255, 0, 0.2);
  color: #00ff00;
  border: 2px solid #00ff00;
}

.score-medium {
  background: rgba(255, 255, 0, 0.2);
  color: #ffff00;
  border: 2px solid #ffff00;
}

.score-low {
  background: rgba(255, 0, 0, 0.2);
  color: #ff0000;
  border: 2px solid #ff0000;
}

.results-fade-enter-active {
  animation: fadeInUp 0.8s ease;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .main-title {
    font-size: 2.5rem;
  }

  .subtitle {
    font-size: 1.2rem;
  }

  .terminal-content {
    padding: 1rem;
    font-size: 0.8rem;
  }

  .stat-value {
    font-size: 2rem;
  }
}
</style>