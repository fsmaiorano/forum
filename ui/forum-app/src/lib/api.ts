import axios from 'axios'

// API Base URLs - can be configured via environment variables
export const USER_API_URL = import.meta.env.VITE_USER_API_URL || 'http://localhost:5002/api'
export const FORUM_API_URL = import.meta.env.VITE_FORUM_API_URL || 'http://localhost:5000'

// User API instance
export const userApi = axios.create({
  baseURL: USER_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Forum API instance
export const forumApi = axios.create({
  baseURL: FORUM_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add token to requests
userApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

forumApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle token refresh on 401
userApi.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true
      
      try {
        const refreshToken = localStorage.getItem('refreshToken')
        const accessToken = localStorage.getItem('accessToken')
        
        if (refreshToken && accessToken) {
          const response = await axios.post(`${USER_API_URL}/auth/refresh`, {
            accessToken,
            refreshToken,
          })
          
          const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data
          localStorage.setItem('accessToken', newAccessToken)
          localStorage.setItem('refreshToken', newRefreshToken)
          
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`
          return userApi(originalRequest)
        }
      } catch (refreshError) {
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        window.location.href = '/login'
      }
    }
    
    return Promise.reject(error)
  }
)

forumApi.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Try to refresh token using user API
      try {
        const refreshToken = localStorage.getItem('refreshToken')
        const accessToken = localStorage.getItem('accessToken')
        
        if (refreshToken && accessToken) {
          const response = await userApi.post('/auth/refresh', {
            accessToken,
            refreshToken,
          })
          
          const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data
          localStorage.setItem('accessToken', newAccessToken)
          localStorage.setItem('refreshToken', newRefreshToken)
          
          error.config.headers.Authorization = `Bearer ${newAccessToken}`
          return forumApi(error.config)
        }
      } catch (refreshError) {
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        window.location.href = '/login'
      }
    }
    
    return Promise.reject(error)
  }
)

