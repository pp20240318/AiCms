import { request } from './index'

export interface WebsiteConfig {
  id: number
  key: string
  value?: string
  description?: string
  group: string
  dataType: string
  isPublic: boolean
  sortOrder: number
  createdAt: string
  updatedAt?: string
}

export interface PublicConfig {
  key: string
  value?: string
  dataType: string
}

export interface CreateWebsiteConfigDto {
  key: string
  value?: string
  description?: string
  group?: string
  dataType?: string
  isPublic?: boolean
  sortOrder?: number
}

export interface UpdateWebsiteConfigDto {
  value?: string
  description?: string
  group?: string
  dataType?: string
  isPublic?: boolean
  sortOrder?: number
}

export interface WebsiteConfigQueryDto {
  group?: string
  isPublic?: boolean
  searchTerm?: string
  page?: number
  pageSize?: number
}

export interface WebsiteConfigListResponse {
  data: WebsiteConfig[]
  totalCount: number
  pageCount: number
  currentPage: number
  pageSize: number
}

// 获取公开配置（前台使用）
export const getPublicConfigs = () => {
  return request.get<PublicConfig[]>('/api/websiteconfig/public')
}

// 根据Key获取公开配置
export const getPublicConfigByKey = (key: string) => {
  return request.get<PublicConfig>(`/api/websiteconfig/public/${key}`)
}

// 获取所有配置（管理员）
export const getWebsiteConfigs = (params?: WebsiteConfigQueryDto) => {
  return request.get<WebsiteConfigListResponse>('/api/websiteconfig', { params })
}

// 获取配置分组列表
export const getConfigGroups = () => {
  return request.get<string[]>('/api/websiteconfig/groups')
}

// 获取指定配置
export const getWebsiteConfig = (id: number) => {
  return request.get<WebsiteConfig>(`/api/websiteconfig/${id}`)
}

// 创建配置
export const createWebsiteConfig = (data: CreateWebsiteConfigDto) => {
  return request.post<WebsiteConfig>('/api/websiteconfig', data)
}

// 更新配置
export const updateWebsiteConfig = (id: number, data: UpdateWebsiteConfigDto) => {
  return request.put(`/api/websiteconfig/${id}`, data)
}

// 批量更新配置
export const updateConfigsBatch = (configs: Record<string, string>) => {
  return request.put('/api/websiteconfig/batch', configs)
}

// 删除配置
export const deleteWebsiteConfig = (id: number) => {
  return request.delete(`/api/websiteconfig/${id}`)
}

// 初始化默认配置
export const initializeDefaultConfigs = () => {
  return request.post('/api/websiteconfig/initialize')
}

// 网站配置键值常量
export const WebsiteConfigKeys = {
  SiteName: 'site_name',
  SiteDescription: 'site_description',
  SiteLogo: 'site_logo',
  SiteIcon: 'site_icon',
  SiteKeywords: 'site_keywords',
  ContactEmail: 'contact_email',
  ContactPhone: 'contact_phone',
  ContactAddress: 'contact_address',
  WorkingHours: 'working_hours',
  FacebookUrl: 'facebook_url',
  TwitterUrl: 'twitter_url',
  LinkedinUrl: 'linkedin_url',
  WechatQr: 'wechat_qr',
  DefaultSeoTitle: 'default_seo_title',
  DefaultSeoDescription: 'default_seo_description',
  GoogleAnalytics: 'google_analytics',
  BaiduAnalytics: 'baidu_analytics',
  EnableComments: 'enable_comments',
  EnableContact: 'enable_contact',
  EnableNewsletter: 'enable_newsletter'
}