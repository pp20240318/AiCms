import request from './index'

export interface Contact {
  id: number
  name: string
  email: string
  phone?: string
  company?: string
  subject: string
  message: string
  status: ContactStatus
  reply?: string
  repliedAt?: string
  repliedById?: number
  ipAddress?: string
  createdAt: string
}

export enum ContactStatus {
  New = 0,
  Processing = 1,
  Replied = 2,
  Closed = 3
}

export const ContactStatusLabels = {
  [ContactStatus.New]: '新消息',
  [ContactStatus.Processing]: '处理中',
  [ContactStatus.Replied]: '已回复',
  [ContactStatus.Closed]: '已关闭'
}

export interface CreateContactDto {
  name: string
  email: string
  phone?: string
  company?: string
  subject: string
  message: string
}

export interface UpdateContactStatusDto {
  status: ContactStatus
  reply?: string
}

export interface ContactQueryDto {
  status?: ContactStatus
  searchTerm?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

export interface ContactListResponse {
  data: Contact[]
  totalCount: number
  pageCount: number
  currentPage: number
  pageSize: number
}

export interface ContactStatistics {
  totalCount: number
  todayCount: number
  statusStats: Array<{ status: ContactStatus; count: number }>
}

// 提交联系我们表单（公开接口）
export const submitContact = (data: CreateContactDto) => {
  return request.post<Contact>('/public/contact', data)
}

// 获取联系消息列表（管理员）
export const getContacts = (params?: ContactQueryDto) => {
  return request.get<ContactListResponse>('/admin/contacts', { params })
}

// 获取指定联系消息
export const getContact = (id: number) => {
  return request.get<Contact>(`/admin/contacts/${id}`)
}

// 更新联系消息状态和回复
export const updateContactStatus = (id: number, data: UpdateContactStatusDto) => {
  return request.put(`/admin/contacts/${id}/status`, data)
}

// 删除联系消息
export const deleteContact = (id: number) => {
  return request.delete(`/admin/contacts/${id}`)
}

// 获取联系消息统计
export const getContactStatistics = () => {
  return request.get<ContactStatistics>('/admin/contacts/statistics')
}