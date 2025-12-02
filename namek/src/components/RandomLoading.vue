<template>
  <component
    :is="currentLoadingComponent"
    :isActive="isActive"
    :stopRequested="stopRequested"
  />
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import TerminalLoading from './TerminalLoading.vue'
import SciFiLoading from './SciFiLoading.vue'
import LogoLoading from './LogoLoading.vue'

const props = defineProps<{
  isActive: boolean
  stopRequested: boolean
  mode?: 'random' | 'sequence' | 'terminal' | 'scifi' | 'logo'
}>()

// Available loading components
const loadingComponents = [
  TerminalLoading,
  SciFiLoading,
  LogoLoading
]

const loadingNames = ['terminal', 'scifi', 'logo']

const currentLoadingComponent = ref(TerminalLoading)

// Storage key for sequence index
const SEQUENCE_INDEX_KEY = 'loading_sequence_index'

// Get sequence index from localStorage
function getSequenceIndex(): number {
  try {
    const stored = localStorage.getItem(SEQUENCE_INDEX_KEY)
    return stored ? parseInt(stored, 10) : 0
  } catch (error) {
    console.error('Failed to load sequence index:', error)
    return 0
  }
}

// Save sequence index to localStorage
function saveSequenceIndex(index: number) {
  try {
    localStorage.setItem(SEQUENCE_INDEX_KEY, index.toString())
  } catch (error) {
    console.error('Failed to save sequence index:', error)
  }
}

function selectLoadingComponent() {
  const mode = props.mode || 'sequence'

  switch (mode) {
    case 'random': {
      // Random selection
      const randomIndex = Math.floor(Math.random() * loadingComponents.length)
      currentLoadingComponent.value = loadingComponents[randomIndex]
      console.log(`🎲 Random Loading: ${loadingNames[randomIndex]}`)
      break
    }

    case 'sequence': {
      // Sequential selection - load from localStorage
      const sequenceIndex = getSequenceIndex()
      currentLoadingComponent.value = loadingComponents[sequenceIndex]
      console.log(`🔄 Sequential Loading: ${loadingNames[sequenceIndex]} (${sequenceIndex + 1}/${loadingComponents.length})`)

      // Save next index for next time
      const nextIndex = (sequenceIndex + 1) % loadingComponents.length
      saveSequenceIndex(nextIndex)
      break
    }

    case 'terminal':
      currentLoadingComponent.value = TerminalLoading
      console.log('💻 Loading: Terminal')
      break

    case 'scifi':
      currentLoadingComponent.value = SciFiLoading
      console.log('🎯 Loading: Sci-Fi HUD')
      break

    case 'logo':
      currentLoadingComponent.value = LogoLoading
      console.log('🚀 Loading: Logo Animation')
      break

    default:
      currentLoadingComponent.value = TerminalLoading
  }
}

// Select component on mount
onMounted(() => {
  selectLoadingComponent()
})

// Watch for activation to potentially change component
watch(() => props.isActive, (newVal) => {
  if (newVal) {
    // Only change component when loading starts (not when it's already active)
    selectLoadingComponent()
  }
})
</script>