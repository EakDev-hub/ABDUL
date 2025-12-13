export interface Score {
  team: string
  totalScore: number
  timeUsedInSeconds: number
}

export interface ScoreFilter {
  passKeyType: string
}

export interface ScoreResponse {
  success: boolean
  data: Score[]
  filter?: ScoreFilter
}