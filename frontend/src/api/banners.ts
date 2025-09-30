import request from './index'

export interface Banner {
  id: number
  title: string
  description?: string
  imageUrl: string
  linkUrl?: string
  linkTarget?: string
  sortOrder: number
  isActive: boolean
  startTime?: string
  endTime?: string
  createdAt: string
  updatedAt: string
}

export interface CreateBannerRequest {
  title: string
  description?: string
  imageUrl: string
  linkUrl?: string
  linkTarget?: string
  sortOrder: number
  isActive: boolean
  startTime?: string
  endTime?: string
}

export interface UpdateBannerRequest extends CreateBannerRequest {
  id: number
}

export interface BannerListRequest {
  page?: number
  pageSize?: number
  search?: string
  isActive?: boolean
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

// 获取轮播图列表
export const getBanners = (params?: BannerListRequest): Promise<PagedResult<Banner>> => {
  return request.get('/banners', { params })
}

// 获取活跃的轮播图
export const getActiveBanners = (): Promise<Banner[]> => {
  return request.get('/banners/active')
}

// 获取单个轮播图
export const getBanner = (id: number): Promise<Banner> => {
  return request.get(`/banners/${id}`)
}

// 创建轮播图
export const createBanner = (data: CreateBannerRequest): Promise<Banner> => {
  return request.post('/banners', data)
}

// 更新轮播图
export const updateBanner = (id: number, data: CreateBannerRequest): Promise<Banner> => {
  return request.put(`/banners/${id}`, data)
}

// 删除轮播图
export const deleteBanner = (id: number): Promise<void> => {
  return request.delete(`/banners/${id}`)
}

// 切换轮播图状态
export const toggleBannerStatus = (id: number): Promise<void> => {
  return request.patch(`/banners/${id}/toggle-active`)
}

