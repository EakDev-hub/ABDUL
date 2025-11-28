import { spawn } from 'child_process'

const BUILD_STAGE = process.argv[2] || 'local'

spawn(`cross-env BUILD_STAGE=${BUILD_STAGE} vite`, {
  shell: true,
  stdio: 'inherit'
})
