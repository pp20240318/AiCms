import request from './index'

export interface ArticleCategory {
  id: number
  name: string
  description?: string
  parentId?: number
  parentName?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  articleCount: number
  children?: ArticleCategory[]
}

export interface CreateArticleCategoryData {
  name: string
  description?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
}

export interface UpdateArticleCategoryData {
  name: string
  description?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
}

// 获取文章分类列表
export const getArticleCategories = (includeInactive = false): Promise<ArticleCategory[]> => {
  return request.get(`/admin/articlecategories?includeInactive=${includeInactive}`)
}

// 获取文章分类树
export const getArticleCategoriesTree = (): Promise<ArticleCategory[]> => {
  return request.get('/admin/articlecategories/tree')
}

// 获取文章分类详情
export const getArticleCategoryById = (id: number): Promise<ArticleCategory> => {
  return request.get(`/admin/articlecategories/${id}`)
}

// 创建文章分类
export const createArticleCategory = (data: CreateArticleCategoryData): Promise<ArticleCategory> => {
  return request.post('/admin/articlecategories', data)
}

// 更新文章分类
export const updateArticleCategory = (id: number, data: UpdateArticleCategoryData): Promise<ArticleCategory> => {
  return request.put(`/admin/articlecategories/${id}`, data)
}

// 删除文章分类
export const deleteArticleCategory = (id: number): Promise<void> => {
  return request.delete(`/admin/articlecategories/${id}`)
}

// 切换文章分类状态
export const toggleArticleCategoryStatus = (id: number): Promise<ArticleCategory> => {
  return request.put(`/admin/articlecategories/${id}/toggle-status`)
}