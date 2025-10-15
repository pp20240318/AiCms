import request from './index'

export interface Role {
  id: number
  name: string
  description: string
  createdAt: string
  permissions: Permission[]
}

export interface Permission {
  id: number
  name: string
  description: string
}

export interface CreateRoleData {
  name: string
  description: string
  permissionIds: number[]
}

export interface UpdateRoleData {
  name?: string
  description?: string
  permissionIds?: number[]
}

// 获取角色列表
export const getRoles = (): Promise<Role[]> => {
  return request.get('/admin/roles')
}

// 获取角色详情
export const getRoleById = (id: number): Promise<Role> => {
  return request.get(`/admin/roles/${id}`)
}

// 创建角色
export const createRole = (data: CreateRoleData): Promise<Role> => {
  return request.post('/admin/roles', data)
}

// 更新角色
export const updateRole = (id: number, data: UpdateRoleData): Promise<Role> => {
  return request.put(`/admin/roles/${id}`, data)
}

// 删除角色
export const deleteRole = (id: number): Promise<void> => {
  return request.delete(`/admin/roles/${id}`)
}

// 获取权限列表
export const getPermissions = (): Promise<Permission[]> => {
  return request.get('/admin/permissions')
}