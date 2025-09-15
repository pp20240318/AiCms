import { request } from './index'

export interface Page {
  id: number
  title: string
  slug: string
  content: string
  excerpt?: string
  featuredImage?: string
  template: string
  status: PageStatus
  sortOrder: number
  showInMenu: boolean
  parentId?: number
  seoTitle?: string
  seoDescription?: string
  seoKeywords?: string
  publishedAt?: string
  createdById: number
  updatedById?: number
  viewCount: number
  createdAt: string
  updatedAt?: string
}

export enum PageStatus {
  Draft = 0,
  Published = 1,
  Archived = 2
}

export const PageStatusLabels = {
  [PageStatus.Draft]: '草稿',
  [PageStatus.Published]: '已发布',
  [PageStatus.Archived]: '已归档'
}

export interface PublicPage {
  id: number
  title: string
  slug: string
  content: string
  excerpt?: string
  featuredImage?: string
  seoTitle?: string
  seoDescription?: string
  seoKeywords?: string
  publishedAt?: string
  viewCount: number
}

export interface CreatePageDto {
  title: string
  slug: string
  content: string
  excerpt?: string
  featuredImage?: string
  template?: string
  status?: PageStatus
  sortOrder?: number
  showInMenu?: boolean
  parentId?: number
  seoTitle?: string
  seoDescription?: string
  seoKeywords?: string
}

export interface UpdatePageDto {
  title: string
  slug: string
  content: string
  excerpt?: string
  featuredImage?: string
  template?: string
  status: PageStatus
  sortOrder?: number
  showInMenu?: boolean
  parentId?: number
  seoTitle?: string
  seoDescription?: string
  seoKeywords?: string
}

export interface PageQueryDto {
  status?: PageStatus
  template?: string
  showInMenu?: boolean
  parentId?: number
  searchTerm?: string
  page?: number
  pageSize?: number
}

export interface PageListResponse {
  data: Page[]
  totalCount: number
  pageCount: number
  currentPage: number
  pageSize: number
}

// 获取已发布的页面（公开接口）
export const getPublicPages = () => {
  return request.get<PublicPage[]>('/api/pages/public')
}

// 根据Slug获取已发布的页面（公开接口）
export const getPublicPageBySlug = (slug: string) => {
  return request.get<PublicPage>(`/api/pages/public/${slug}`)
}

// 获取所有页面（管理员）
export const getPages = (params?: PageQueryDto) => {
  return request.get<PageListResponse>('/api/pages', { params })
}

// 获取指定页面
export const getPage = (id: number) => {
  return request.get<Page>(`/api/pages/${id}`)
}

// 创建页面
export const createPage = (data: CreatePageDto) => {
  return request.post<Page>('/api/pages', data)
}

// 更新页面
export const updatePage = (id: number, data: UpdatePageDto) => {
  return request.put(`/api/pages/${id}`, data)
}

// 删除页面
export const deletePage = (id: number) => {
  return request.delete(`/api/pages/${id}`)
}