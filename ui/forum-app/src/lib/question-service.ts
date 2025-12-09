import { forumApi } from './api'

export interface Attachment {
  ownerId: string
  ownerType: number
  title: string
  link: string
}

export interface CreateQuestionRequest {
  title: string
  content: string
  authorId: string
  attachments?: Attachment[]
}

export interface CreateQuestionResponse {
  questionId: string
}

export const questionService = {
  async createQuestion(data: CreateQuestionRequest): Promise<CreateQuestionResponse> {
    const response = await forumApi.post<CreateQuestionResponse>('/question', data)
    return response.data
  },
}

