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

export interface QuestionDto {
  id: string
  title: string
  content: string
  authorId: string
  createdAt: string
  updatedAt?: string
}

export interface GetQuestionsResponse {
  questions: QuestionDto[]
}

export interface AnswerDto {
  id: string
  content: string
  authorId: string
  createdAt: string
  updatedAt?: string
}

export interface GetAnswersResponse {
  answers: AnswerDto[]
}

export interface CreateAnswerRequest {
  content: string
  authorId: string
  questionId: string
}

export interface CreateAnswerResponse {
  answerId: string
}

export const questionService = {
  async createQuestion(data: CreateQuestionRequest): Promise<CreateQuestionResponse> {
    const response = await forumApi.post<CreateQuestionResponse>('/question', data)
    return response.data
  },

  async getQuestions(): Promise<GetQuestionsResponse> {
    const response = await forumApi.get<GetQuestionsResponse>('/question')
    return response.data
  },

  async getAnswers(questionId: string): Promise<GetAnswersResponse> {
    const response = await forumApi.get<GetAnswersResponse>(`/answer/${questionId}`)
    return response.data
  },

  async createAnswer(data: CreateAnswerRequest): Promise<CreateAnswerResponse> {
    const response = await forumApi.post<CreateAnswerResponse>('/answer', data)
    return response.data
  },
}
