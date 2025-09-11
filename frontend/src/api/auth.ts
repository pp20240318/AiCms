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
    roles: string[]
  }
}

// 登录
export const login = (data: LoginData): Promise<LoginResponse> => {
  return request.post('/auth/login', data)
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