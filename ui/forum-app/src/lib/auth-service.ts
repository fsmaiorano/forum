import { userApi } from './api'

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
}

export interface AuthResponse {
  accessToken: string
  accessTokenExpiration: string
  refreshToken: string
  refreshTokenExpiration: string
}

export interface UserInfo {
  userId: string
  email: string
}

export const authService = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    const response = await userApi.post<AuthResponse>('/auth/login', data)
    return response.data
  },

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await userApi.post<AuthResponse>('/auth/register', data)
    return response.data
  },

  async getMe(): Promise<UserInfo> {
    const response = await userApi.get<UserInfo>('/auth/me')
    return response.data
  },

  async logout(): Promise<void> {
    const accessToken = localStorage.getItem('accessToken')
    const refreshToken = localStorage.getItem('refreshToken')
    
    if (accessToken && refreshToken) {
      try {
        await userApi.post('/auth/revoke', { accessToken, refreshToken })
      } catch (error) {
        console.error('Error revoking token:', error)
      }
    }
    
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
  },
}

