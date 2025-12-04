import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import Admin from './Admin.vue'

const app = createApp(Admin)

app.use(createPinia())

app.mount('#app')