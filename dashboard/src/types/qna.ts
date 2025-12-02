export interface Qna {
  id: number
  question: string
  answer: string
  createdAt: string
  updatedAt: string
}

export interface QnaResponse {
  success: boolean
  data: Qna[]
}