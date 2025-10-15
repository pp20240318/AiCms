import request from './index'

export interface SeoSetting {
  id: number
  pagePath: string
  title: string
  description?: string
  keywords?: string
  ogTitle?: string
  ogDescription?: string
  ogImage?: string
  structuredData?: string
  isEnabled: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateSeoSettingDto {
  pagePath: string
  title: string
  description?: string
  keywords?: string
  ogTitle?: string
  ogDescription?: string
  ogImage?: string
  structuredData?: string
  isEnabled?: boolean
}

export interface UpdateSeoSettingDto {
  title: string
  description?: string
  keywords?: string
  ogTitle?: string
  ogDescription?: string
  ogImage?: string
  structuredData?: string
  isEnabled?: boolean
}

// 获取所有SEO设置
export const getSeoSettings = () => {
  return request.get<SeoSetting[]>('/admin/seosettings')
}

// 根据路径获取SEO设置（公开接口）
export const getSeoSettingByPath = (pagePath: string) => {
  return request.get<SeoSetting>(`/public/seosettings/by-path/${encodeURIComponent(pagePath)}`)
}

// 获取指定SEO设置
export const getSeoSetting = (id: number) => {
  return request.get<SeoSetting>(`/admin/seosettings/${id}`)
}

// 创建SEO设置
export const createSeoSetting = (data: CreateSeoSettingDto) => {
  return request.post<SeoSetting>('/admin/seosettings', data)
}

// 更新SEO设置
export const updateSeoSetting = (id: number, data: UpdateSeoSettingDto) => {
  return request.put(`/admin/seosettings/${id}`, data)
}

// 删除SEO设置
export const deleteSeoSetting = (id: number) => {
  return request.delete(`/admin/seosettings/${id}`)
}