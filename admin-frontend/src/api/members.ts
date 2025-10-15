import request from './index'

export interface Member {
  id: number
  memberCode: string
  name: string
  gender?: string
  dateOfBirth?: string
  idNumber?: string
  phone?: string
  email?: string
  address?: string
  membershipType: string
  status: string
  joinDate: string
  expiryDate?: string
  notes?: string
  avatar?: string
  occupation?: string
  company?: string
  emergencyContact?: string
  emergencyPhone?: string
  balance?: number
  points: number
  lastVisitDate?: string
  referralCode?: string
  referredBy?: string
  createdAt: string
  updatedAt?: string
}

export interface CreateMemberData {
  memberCode: string
  name: string
  gender?: string
  dateOfBirth?: string
  idNumber?: string
  phone?: string
  email?: string
  address?: string
  membershipType: string
  expiryDate?: string
  notes?: string
  avatar?: string
  occupation?: string
  company?: string
  emergencyContact?: string
  emergencyPhone?: string
  balance?: number
  points?: number
  referralCode?: string
  referredBy?: string
}

export interface UpdateMemberData {
  name: string
  gender?: string
  dateOfBirth?: string
  idNumber?: string
  phone?: string
  email?: string
  address?: string
  membershipType: string
  status: string
  expiryDate?: string
  notes?: string
  avatar?: string
  occupation?: string
  company?: string
  emergencyContact?: string
  emergencyPhone?: string
  balance?: number
  points: number
  referralCode?: string
  referredBy?: string
}

export interface MemberQuery {
  page?: number
  pageSize?: number
  keyword?: string
  membershipType?: string
  status?: string
}

// 获取会员列表
export const getMembers = (params?: MemberQuery): Promise<{
  items: Member[]
  total: number
  page: number
  pageSize: number
}> => {
  return request.get('/admin/members', { params })
}

// 搜索会员
export const searchMembers = (params: {
  keyword?: string
  membershipType?: string
  status?: string
}): Promise<Member[]> => {
  return request.get('/admin/members/search', { params })
}

// 获取会员详情
export const getMemberById = (id: number): Promise<Member> => {
  return request.get(`/admin/members/${id}`)
}

// 根据会员编号获取会员
export const getMemberByCode = (memberCode: string): Promise<Member> => {
  return request.get(`/admin/members/code/${memberCode}`)
}

// 创建会员
export const createMember = (data: CreateMemberData): Promise<Member> => {
  return request.post('/admin/members', data)
}

// 更新会员
export const updateMember = (id: number, data: UpdateMemberData): Promise<Member> => {
  return request.put(`/admin/members/${id}`, data)
}

// 删除会员
export const deleteMember = (id: number): Promise<void> => {
  return request.delete(`/members/${id}`)
}

// 生成会员编号
export const generateMemberCode = (): Promise<{ memberCode: string }> => {
  return request.get('/admin/members/generate-code')
}

// 更新余额
export const updateBalance = (id: number, amount: number): Promise<void> => {
  return request.post(`/admin/members/${id}/balance`, amount, {
    headers: {
      'Content-Type': 'application/json'
    }
  })
}

// 更新积分
export const updatePoints = (id: number, points: number): Promise<void> => {
  return request.post(`/admin/members/${id}/points`, points, {
    headers: {
      'Content-Type': 'application/json'
    }
  })
}

// 更新访问记录
export const updateLastVisit = (id: number): Promise<void> => {
  return request.post(`/admin/members/${id}/visit`)
}