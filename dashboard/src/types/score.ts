export interface Score {
  team: string
  totalScore: number
  timeUsedInSeconds: number
}

export interface ScoreResponse {
  success: boolean
  data: Score[]
}