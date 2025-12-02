export interface SubmitRequest {
  team: string
  passKey: string
  apiUrl: string
}

export interface QuestionResult {
  no: number
  question: string
  expectedAnswer: string
  actualAnswer: string
  score: number
}

export interface SubmitResponse {
  uuid: string
  passKeyType: 'develop' | 'present' | 'finalist'
  maxDurationInSecs: number
  timeUsedInSeconds: number
  totalQuestion: number
  maximumScore: number
  answeredQuestion: number
  score: number
  results: QuestionResult[]
}

export interface ApiError {
  status?: string
  error?: string
  message?: string
  detail?: string
  details?: string
}

export interface ErrorState {
  hasError: boolean
  errorCode?: number
  errorMessage?: string
  errorDetails?: string
}