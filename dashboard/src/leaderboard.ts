import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import Leaderboard from './Leaderboard.vue'

const app = createApp(Leaderboard)

app.use(createPinia())

app.mount('#leaderboard')
