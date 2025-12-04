<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { adminService } from '@/services/admin.service'
import type { Qna } from '@/types/qna'
import type { Announcement } from '@/types/announcement'

// Authentication
const isAuthenticated = ref(false)
const loginForm = ref({
  username: '',
  password: ''
})
const loginError = ref('')

// Check if already logged in
const checkAuth = () => {
  const authToken = localStorage.getItem('admin_auth')
  if (authToken === 'authenticated') {
    isAuthenticated.value = true
  }
}

// Login function
const login = () => {
  loginError.value = ''
  
  if (loginForm.value.username === 'nury' && loginForm.value.password === 'pastelsoftcream') {
    isAuthenticated.value = true
    localStorage.setItem('admin_auth', 'authenticated')
    loginForm.value = { username: '', password: '' }
  } else {
    loginError.value = 'Invalid username or password'
  }
}

// Logout function
const logout = () => {
  isAuthenticated.value = false
  localStorage.removeItem('admin_auth')
  loginForm.value = { username: '', password: '' }
}

// Active tab
const activeTab = ref<'qna' | 'announcements'>('qna')

// Q&A State
const qnaList = ref<Qna[]>([])
const qnaLoading = ref(false)
const qnaError = ref('')

// Announcement State
const announcementList = ref<Announcement[]>([])
const announcementLoading = ref(false)
const announcementError = ref('')

// Modal States
const showQnaModal = ref(false)
const showAnnouncementModal = ref(false)
const editingQna = ref<Qna | null>(null)
const editingAnnouncement = ref<Announcement | null>(null)

// Form States
const qnaForm = ref({
  question: '',
  answer: ''
})

const announcementForm = ref({
  text: '',
  postedAt: '',
  isActive: true
})

// Auto-refresh interval
let refreshInterval: number | null = null

// Fetch Q&A
const fetchQna = async () => {
  try {
    qnaLoading.value = true
    qnaError.value = ''
    const response = await adminService.getAllQna()
    if (response.success) {
      qnaList.value = response.data
    }
  } catch (error: any) {
    qnaError.value = error.message || 'Failed to fetch Q&A'
    console.error('Failed to fetch Q&A:', error)
  } finally {
    qnaLoading.value = false
  }
}

// Fetch Announcements
const fetchAnnouncements = async () => {
  try {
    announcementLoading.value = true
    announcementError.value = ''
    const response = await adminService.getAllAnnouncements()
    if (response.success) {
      announcementList.value = response.data
    }
  } catch (error: any) {
    announcementError.value = error.message || 'Failed to fetch announcements'
    console.error('Failed to fetch announcements:', error)
  } finally {
    announcementLoading.value = false
  }
}

// Refresh data
const refreshData = async () => {
  if (activeTab.value === 'qna') {
    await fetchQna()
  } else {
    await fetchAnnouncements()
  }
}

// Q&A CRUD Operations
const openQnaModal = (qna?: Qna) => {
  if (qna) {
    editingQna.value = qna
    qnaForm.value = {
      question: qna.question,
      answer: qna.answer || ''
    }
  } else {
    editingQna.value = null
    qnaForm.value = {
      question: '',
      answer: ''
    }
  }
  showQnaModal.value = true
}

const closeQnaModal = () => {
  showQnaModal.value = false
  editingQna.value = null
  qnaForm.value = { question: '', answer: '' }
}

const saveQna = async () => {
  try {
    if (!qnaForm.value.question.trim()) {
      alert('Question is required')
      return
    }

    if (editingQna.value) {
      await adminService.updateQna(editingQna.value.id, qnaForm.value)
    } else {
      await adminService.createQna(qnaForm.value)
    }
    
    await fetchQna()
    closeQnaModal()
  } catch (error: any) {
    alert(error.message || 'Failed to save Q&A')
  }
}

const deleteQna = async (id: number) => {
  if (!confirm('Are you sure you want to delete this Q&A?')) return
  
  try {
    await adminService.deleteQna(id)
    await fetchQna()
  } catch (error: any) {
    alert(error.message || 'Failed to delete Q&A')
  }
}

// Announcement CRUD Operations
const openAnnouncementModal = (announcement?: Announcement) => {
  if (announcement) {
    editingAnnouncement.value = announcement
    announcementForm.value = {
      text: announcement.text,
      postedAt: new Date(announcement.postedAt).toISOString().slice(0, 16),
      isActive: announcement.isActive
    }
  } else {
    editingAnnouncement.value = null
    announcementForm.value = {
      text: '',
      postedAt: new Date().toISOString().slice(0, 16),
      isActive: true
    }
  }
  showAnnouncementModal.value = true
}

const closeAnnouncementModal = () => {
  showAnnouncementModal.value = false
  editingAnnouncement.value = null
  announcementForm.value = { text: '', postedAt: '', isActive: true }
}

const saveAnnouncement = async () => {
  try {
    if (!announcementForm.value.text.trim()) {
      alert('Text is required')
      return
    }

    const data = {
      text: announcementForm.value.text,
      postedAt: announcementForm.value.postedAt ? new Date(announcementForm.value.postedAt).toISOString() : undefined,
      isActive: announcementForm.value.isActive
    }

    if (editingAnnouncement.value) {
      await adminService.updateAnnouncement(editingAnnouncement.value.id, data)
    } else {
      await adminService.createAnnouncement(data as any)
    }
    
    await fetchAnnouncements()
    closeAnnouncementModal()
  } catch (error: any) {
    alert(error.message || 'Failed to save announcement')
  }
}

const deleteAnnouncement = async (id: number) => {
  if (!confirm('Are you sure you want to delete this announcement?')) return
  
  try {
    await adminService.deleteAnnouncement(id)
    await fetchAnnouncements()
  } catch (error: any) {
    alert(error.message || 'Failed to delete announcement')
  }
}

// Format date
const formatDate = (date: string) => {
  return new Date(date).toLocaleString()
}

// Switch tab
const switchTab = (tab: 'qna' | 'announcements') => {
  activeTab.value = tab
  if (tab === 'qna') {
    fetchQna()
  } else {
    fetchAnnouncements()
  }
}

// Lifecycle
onMounted(async () => {
  checkAuth()
  
  if (isAuthenticated.value) {
    await fetchQna()
    
    // Auto-refresh every 10 seconds
    refreshInterval = setInterval(refreshData, 10000)
  }
})

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
})
</script>

<template>
  <!-- Login Screen -->
  <div v-if="!isAuthenticated" class="login-container">
    <div class="login-box">
      <div class="login-header">
        <h1>🔐 Admin Login</h1>
        <p>ABDUL Hackathon Management</p>
      </div>
      <form @submit.prevent="login" class="login-form">
        <div class="form-group">
          <label>Username</label>
          <input
            type="text"
            v-model="loginForm.username"
            placeholder="Enter username"
            required
            autocomplete="username"
          />
        </div>
        <div class="form-group">
          <label>Password</label>
          <input
            type="password"
            v-model="loginForm.password"
            placeholder="Enter password"
            required
            autocomplete="current-password"
          />
        </div>
        <div v-if="loginError" class="login-error">{{ loginError }}</div>
        <button type="submit" class="btn btn-primary btn-login">Login</button>
      </form>
    </div>
  </div>

  <!-- Admin Dashboard -->
  <div v-else class="admin-dashboard">
    <div class="header">
      <div class="header-content">
        <div>
          <h1>🛠️ Admin Dashboard</h1>
          <p class="subtitle">ABDUL Hackathon Management</p>
        </div>
        <button class="btn btn-logout" @click="logout">🚪 Logout</button>
      </div>
    </div>

    <div class="tabs">
      <button 
        :class="['tab', { active: activeTab === 'qna' }]"
        @click="switchTab('qna')">
        ❓ Q&A Management
      </button>
      <button 
        :class="['tab', { active: activeTab === 'announcements' }]"
        @click="switchTab('announcements')">
        📢 Announcements
      </button>
    </div>

    <!-- Q&A Tab -->
    <div v-if="activeTab === 'qna'" class="content">
      <div class="content-header">
        <h2>Q&A Records ({{ qnaList.length }})</h2>
        <button class="btn btn-primary" @click="openQnaModal()">
          ➕ Add New Q&A
        </button>
      </div>

      <div v-if="qnaLoading" class="loading">Loading...</div>
      <div v-else-if="qnaError" class="error">{{ qnaError }}</div>
      <div v-else-if="qnaList.length === 0" class="empty">No Q&A records yet</div>
      
      <div v-else class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Question</th>
              <th>Answer</th>
              <th>Created At</th>
              <th>Updated At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="qna in qnaList" :key="qna.id">
              <td>{{ qna.id }}</td>
              <td class="question-col">{{ qna.question }}</td>
              <td class="answer-col">{{ qna.answer || 'N/A' }}</td>
              <td>{{ formatDate(qna.createdAt) }}</td>
              <td>{{ formatDate(qna.updatedAt) }}</td>
              <td class="actions">
                <button class="btn btn-edit" @click="openQnaModal(qna)">✏️ Edit</button>
                <button class="btn btn-delete" @click="deleteQna(qna.id)">🗑️ Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Announcements Tab -->
    <div v-if="activeTab === 'announcements'" class="content">
      <div class="content-header">
        <h2>Announcements ({{ announcementList.length }})</h2>
        <button class="btn btn-primary" @click="openAnnouncementModal()">
          ➕ Add New Announcement
        </button>
      </div>

      <div v-if="announcementLoading" class="loading">Loading...</div>
      <div v-else-if="announcementError" class="error">{{ announcementError }}</div>
      <div v-else-if="announcementList.length === 0" class="empty">No announcements yet</div>
      
      <div v-else class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Text</th>
              <th>Posted At</th>
              <th>Status</th>
              <th>Created At</th>
              <th>Updated At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="announcement in announcementList" :key="announcement.id">
              <td>{{ announcement.id }}</td>
              <td class="text-col">{{ announcement.text }}</td>
              <td>{{ formatDate(announcement.postedAt) }}</td>
              <td>
                <span :class="['status-badge', announcement.isActive ? 'active' : 'inactive']">
                  {{ announcement.isActive ? '✅ Active' : '❌ Inactive' }}
                </span>
              </td>
              <td>{{ formatDate(announcement.createdAt) }}</td>
              <td>{{ formatDate(announcement.updatedAt) }}</td>
              <td class="actions">
                <button class="btn btn-edit" @click="openAnnouncementModal(announcement)">✏️ Edit</button>
                <button class="btn btn-delete" @click="deleteAnnouncement(announcement.id)">🗑️ Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Q&A Modal -->
    <div v-if="showQnaModal" class="modal-overlay" @click="closeQnaModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h3>{{ editingQna ? 'Edit Q&A' : 'Add New Q&A' }}</h3>
          <button class="btn-close" @click="closeQnaModal">×</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label>Question *</label>
            <textarea 
              v-model="qnaForm.question" 
              placeholder="Enter question"
              rows="3"
              required
            ></textarea>
          </div>
          <div class="form-group">
            <label>Answer</label>
            <textarea 
              v-model="qnaForm.answer" 
              placeholder="Enter answer (optional)"
              rows="4"
            ></textarea>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="closeQnaModal">Cancel</button>
          <button class="btn btn-primary" @click="saveQna">Save</button>
        </div>
      </div>
    </div>

    <!-- Announcement Modal -->
    <div v-if="showAnnouncementModal" class="modal-overlay" @click="closeAnnouncementModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h3>{{ editingAnnouncement ? 'Edit Announcement' : 'Add New Announcement' }}</h3>
          <button class="btn-close" @click="closeAnnouncementModal">×</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label>Text *</label>
            <textarea 
              v-model="announcementForm.text" 
              placeholder="Enter announcement text"
              rows="4"
              required
            ></textarea>
          </div>
          <div class="form-group">
            <label>Posted At</label>
            <input 
              type="datetime-local" 
              v-model="announcementForm.postedAt"
            />
          </div>
          <div class="form-group">
            <label class="checkbox-label">
              <input 
                type="checkbox" 
                v-model="announcementForm.isActive"
              />
              <span>Active</span>
            </label>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="closeAnnouncementModal">Cancel</button>
          <button class="btn btn-primary" @click="saveAnnouncement">Save</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

/* Login Screen Styles */
.login-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
  font-family: 'Inter', sans-serif;
}

.login-box {
  background: white;
  border-radius: 20px;
  padding: 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  width: 100%;
  max-width: 420px;
  animation: slideIn 0.5s ease;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h1 {
  font-size: 2rem;
  color: #333;
  margin-bottom: 10px;
  font-weight: 700;
}

.login-header p {
  color: #666;
  font-size: 1rem;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.login-error {
  background: #ffebee;
  color: #c62828;
  padding: 12px;
  border-radius: 8px;
  text-align: center;
  font-weight: 500;
  border: 1px solid #ef9a9a;
}

.btn-login {
  width: 100%;
  padding: 15px;
  font-size: 1.1rem;
}

/* Admin Dashboard Styles */
.admin-dashboard {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
  font-family: 'Inter', sans-serif;
}

.header {
  color: white;
  margin-bottom: 30px;
  padding: 30px;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header h1 {
  font-size: 2.5rem;
  margin-bottom: 10px;
  font-weight: 700;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
}

.subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  font-weight: 400;
}

.btn-logout {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  padding: 12px 24px;
  font-size: 1rem;
}

.btn-logout:hover {
  background: white;
  color: #667eea;
}

.tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  padding: 10px;
  border-radius: 12px;
}

.tab {
  flex: 1;
  padding: 15px 30px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 8px;
  color: white;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.tab:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.tab.active {
  background: white;
  color: #667eea;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.content {
  background: white;
  border-radius: 15px;
  padding: 30px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  padding-bottom: 20px;
  border-bottom: 2px solid #f0f0f0;
}

.content-header h2 {
  font-size: 1.8rem;
  color: #333;
  font-weight: 700;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 5px;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
}

.btn-secondary {
  background: #e0e0e0;
  color: #666;
}

.btn-secondary:hover {
  background: #d0d0d0;
}

.btn-edit {
  background: #4CAF50;
  color: white;
  padding: 8px 15px;
  font-size: 0.85rem;
}

.btn-edit:hover {
  background: #45a049;
}

.btn-delete {
  background: #f44336;
  color: white;
  padding: 8px 15px;
  font-size: 0.85rem;
}

.btn-delete:hover {
  background: #da190b;
}

.table-container {
  overflow-x: auto;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table thead {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.data-table th {
  padding: 15px;
  text-align: left;
  font-weight: 600;
  font-size: 0.95rem;
}

.data-table tbody tr {
  border-bottom: 1px solid #f0f0f0;
  transition: background 0.2s;
}

.data-table tbody tr:hover {
  background: #f8f8f8;
}

.data-table td {
  padding: 15px;
  color: #555;
  font-size: 0.9rem;
}

.question-col,
.text-col {
  max-width: 300px;
  word-wrap: break-word;
  font-weight: 500;
  color: #333;
}

.answer-col {
  max-width: 250px;
  word-wrap: break-word;
  color: #666;
}

.actions {
  display: flex;
  gap: 8px;
}

.status-badge {
  padding: 5px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  display: inline-block;
}

.status-badge.active {
  background: #e8f5e9;
  color: #2e7d32;
}

.status-badge.inactive {
  background: #ffebee;
  color: #c62828;
}

.loading,
.error,
.empty {
  text-align: center;
  padding: 60px 20px;
  font-size: 1.1rem;
  color: #999;
}

.error {
  color: #f44336;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal {
  background: white;
  border-radius: 15px;
  width: 90%;
  max-width: 600px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  animation: modalSlideIn 0.3s ease;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  padding: 25px;
  border-bottom: 2px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  font-size: 1.5rem;
  color: #333;
  font-weight: 700;
}

.btn-close {
  background: none;
  border: none;
  font-size: 2rem;
  color: #999;
  cursor: pointer;
  line-height: 1;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.2s;
}

.btn-close:hover {
  background: #f0f0f0;
  color: #333;
}

.modal-body {
  padding: 25px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #555;
  font-size: 0.95rem;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 12px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 0.95rem;
  font-family: 'Inter', sans-serif;
  transition: border-color 0.3s;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #667eea;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  width: auto;
  cursor: pointer;
}

.checkbox-label span {
  font-weight: 500;
}

.modal-footer {
  padding: 20px 25px;
  border-top: 2px solid #f0f0f0;
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

/* Responsive */
@media (max-width: 768px) {
  .header h1 {
    font-size: 1.8rem;
  }

  .tabs {
    flex-direction: column;
  }

  .content {
    padding: 20px;
  }

  .content-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
  }

  .actions {
    flex-direction: column;
  }

  .modal {
    width: 95%;
  }
}
</style>