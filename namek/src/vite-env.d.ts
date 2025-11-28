/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_ENV_CONFIG: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
