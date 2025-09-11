import request from './index'

export interface Article {
  id: number
  title: string
  content: string
  summary: string
  author: string
  categoryId: number
  categoryName: string
  status: 'draft' | 'published' | 'archived'
  featuredImage: string
  tags: string[]
  viewCount: number
  createdAt: string
  updatedAt: string
}

export interface CreateArticleData {
  title: string
  content: string
  summary: string
  categoryId: number
  status: 'draft' | 'published'
  featuredImage?: string
  tags: string[]
}

export interface UpdateArticleData {
  title?: string
  content?: string
  summary?: string
  categoryId?: number
  status?: 'draft' | 'published' | 'archived'
  featuredImage?: string
  tags?: string[]
}

export interface ArticlesQuery {
  page?: number
  pageSize?: number
  search?: string
  categoryId?: number
  status?: string
  author?: string
}

// 获取文章列表
export const getArticles = (params?: ArticlesQuery): Promise<{
  items: Article[]
  total: number
  page: number
  pageSize: number
}> => {
  return request.get('/articles', { params })
}

// 获取文章详情
export const getArticleById = (id: number): Promise<Article> => {
  return request.get(`/articles/${id}`)
}

// 创建文章
export const createArticle = (data: CreateArticleData): Promise<Article> => {
  return request.post('/articles', data)
}

// 更新文章
export const updateArticle = (id: number, data: UpdateArticleData): Promise<Article> => {
  return request.put(`/articles/${id}`, data)
}

// 删除文章
export const deleteArticle = (id: number): Promise<void> => {
  return request.delete(`/articles/${id}`)
}

// 发布文章
export const publishArticle = (id: number): Promise<Article> => {
  return request.post(`/articles/${id}/publish`)
}

// 归档文章
export const archiveArticle = (id: number): Promise<Article> => {
  return request.post(`/articles/${id}/archive`)
}