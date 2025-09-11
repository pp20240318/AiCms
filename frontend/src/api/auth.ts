import request from './index'

export interface LoginData {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  user: {
    id: number
    username: string
    email: string
    realName?: string
    phone?: string
    isActive: boolean
    lastLoginAt?: string
    createdAt: string
  }
  roles: string[]
}

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
}

// 登录
export const login = async (data: LoginData): Promise<LoginResponse> => {
  const response: ApiResponse<LoginResponse> = await request.post('/auth/login', data)
  if (!response.success) {
    throw new Error(response.message)
  }
  return response.data
}

// 退出登录
export const logout = (): Promise<void> => {
  return request.post('/auth/logout')
}

// 获取用户信息
export const getUserInfo = (): Promise<any> => {
  return request.get('/auth/userinfo')
}

// 刷新token
export const refreshToken = (): Promise<{ token: string }> => {
  return request.post('/auth/refresh')
}