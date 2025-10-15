import request from './index'

export interface Category {
  id: number
  name: string
  description: string
  parentId?: number
  parentName?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
  children?: Category[]
}

export interface CreateCategoryData {
  name: string
  description: string
  parentId?: number
  sortOrder: number
}

export interface UpdateCategoryData {
  name?: string
  description?: string
  parentId?: number
  sortOrder?: number
  isActive?: boolean
}

// 获取分类列表
export const getCategories = (): Promise<Category[]> => {
  return request.get('/admin/articlecategories')
}

// 获取分类树
export const getCategoriesTree = (): Promise<Category[]> => {
  return request.get('/admin/articlecategories/tree')
}

// 获取分类详情
export const getCategoryById = (id: number): Promise<Category> => {
  return request.get(`/admin/articlecategories/${id}`)
}

// 创建分类
export const createCategory = (data: CreateCategoryData): Promise<Category> => {
  return request.post('/admin/articlecategories', data)
}

// 更新分类
export const updateCategory = (id: number, data: UpdateCategoryData): Promise<Category> => {
  return request.put(`/admin/articlecategories/${id}`, data)
}

// 删除分类
export const deleteCategory = (id: number): Promise<void> => {
  return request.delete(`/admin/articlecategories/${id}`)
}