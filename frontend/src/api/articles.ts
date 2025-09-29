import request from './index'

// 后端返回的文章数据结构
export interface BackendArticle {
  id: number
  title: string
  content: string
  summary?: string
  authorName: string
  categoryId: number
  categoryName: string
  isPublished: boolean
  coverImage?: string
  viewCount: number
  createdAt: string
  updatedAt: string
  publishedAt?: string
  seoTitle?: string
  seoDescription?: string
  seoKeywords?: string
  sortOrder: number
}

// 前端使用的文章数据结构（兼容原有代码）
export interface Article {
  id: number
  title: string
  content: string
  excerpt: string
  author: string
  categoryId: number
  categoryName: string
  status: 'draft' | 'published' | 'archived'
  featuredImage: string
  tags: string
  viewCount: number
  createdAt: string
  updatedAt: string
  metaTitle?: string
  metaDescription?: string
}

// 数据转换函数
export const transformBackendArticle = (backendArticle: BackendArticle): Article => {
  return {
    id: backendArticle.id,
    title: backendArticle.title,
    content: backendArticle.content,
    excerpt: backendArticle.summary || '',
    author: backendArticle.authorName,
    categoryId: backendArticle.categoryId,
    categoryName: backendArticle.categoryName,
    status: backendArticle.isPublished ? 'published' : 'draft',
    featuredImage: backendArticle.coverImage || '',
    tags: '', // 后端暂未实现tags字段
    viewCount: backendArticle.viewCount,
    createdAt: backendArticle.createdAt,
    updatedAt: backendArticle.updatedAt,
    metaTitle: backendArticle.seoTitle,
    metaDescription: backendArticle.seoDescription
  }
}

export interface CreateArticleDto {
  title: string
  content: string
  excerpt: string
  categoryId: number
  status: 'draft' | 'published'
  featuredImage?: string
  tags: string
  metaTitle?: string
  metaDescription?: string
}

export interface UpdateArticleDto {
  title: string
  content: string
  excerpt: string
  categoryId: number
  status: 'draft' | 'published' | 'archived'
  featuredImage?: string
  tags: string
  metaTitle?: string
  metaDescription?: string
}

// 转换前端数据到后端格式
export const transformToBackendRequest = (frontendData: CreateArticleDto) => {
  return {
    title: frontendData.title,
    content: frontendData.content,
    summary: frontendData.excerpt,
    categoryId: frontendData.categoryId,
    isPublished: frontendData.status === 'published',
    coverImage: frontendData.featuredImage,
    seoTitle: frontendData.metaTitle,
    seoDescription: frontendData.metaDescription,
    seoKeywords: '',
    sortOrder: 0
  }
}

export interface ArticleCategory {
  id: number
  name: string
  description?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
  createdAt: string
  updatedAt?: string
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
export const getArticles = async (params?: ArticlesQuery): Promise<{
  items: Article[]
  total: number
  page: number
  pageSize: number
}> => {
  const response = await request.get('/articles', { params })
  return {
    items: response.items.map(transformBackendArticle),
    total: response.totalCount,
    page: response.page,
    pageSize: response.pageSize
  }
}

// 获取文章详情
export const getArticleById = async (id: number): Promise<Article> => {
  const backendArticle: BackendArticle = await request.get(`/articles/${id}`)
  return transformBackendArticle(backendArticle)
}

// 创建文章
export const createArticle = async (data: CreateArticleDto): Promise<Article> => {
  const backendRequest = transformToBackendRequest(data)
  const backendArticle: BackendArticle = await request.post('/articles', backendRequest)
  return transformBackendArticle(backendArticle)
}

// 更新文章
export const updateArticle = async (id: number, data: UpdateArticleDto): Promise<Article> => {
  const backendRequest = transformToBackendRequest(data)
  const backendArticle: BackendArticle = await request.put(`/articles/${id}`, backendRequest)
  return transformBackendArticle(backendArticle)
}

// 获取单个文章 (别名函数，与组件中的调用保持一致)
export const getArticle = (id: number): Promise<Article> => {
  return getArticleById(id)
}

// 获取文章分类列表
export const getCategories = (): Promise<ArticleCategory[]> => {
  return request.get('/categories')
}

// 删除文章
export const deleteArticle = (id: number): Promise<void> => {
  return request.delete(`/articles/${id}`)
}

// 发布文章
export const publishArticle = async (id: number): Promise<void> => {
  await request.patch(`/articles/${id}/publish`)
}

// 取消发布文章（归档）
export const archiveArticle = async (id: number): Promise<void> => {
  await request.patch(`/articles/${id}/unpublish`)
}