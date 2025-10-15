import request from './index'

export interface User {
  id: number
  username: string
  email: string
  createdAt: string
  isActive: boolean
  roles: string[]
}

export interface CreateUserData {
  username: string
  email: string
  password: string
  roleIds: number[]
}

export interface UpdateUserData {
  username?: string
  email?: string
  password?: string
  roleIds?: number[]
  isActive?: boolean
}

export interface UsersQuery {
  page?: number
  pageSize?: number
  search?: string
}

// 获取用户列表
export const getUsers = (params?: UsersQuery): Promise<{
  items: User[]
  total: number
  page: number
  pageSize: number
}> => {
  return request.get('/admin/users', { params })
}

// 获取用户详情
export const getUserById = (id: number): Promise<User> => {
  return request.get(`/admin/users/${id}`)
}

// 创建用户
export const createUser = (data: CreateUserData): Promise<User> => {
  return request.post('/admin/users', data)
}

// 更新用户
export const updateUser = (id: number, data: UpdateUserData): Promise<User> => {
  return request.put(`/admin/users/${id}`, data)
}

// 删除用户
export const deleteUser = (id: number): Promise<void> => {
  return request.delete(`/admin/users/${id}`)
}