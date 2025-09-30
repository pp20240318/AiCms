import request from './index'

export interface ProductCategory {
  id: number
  name: string
  description?: string
  image?: string
  parentId?: number
  parentName?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  productCount: number
  children?: ProductCategory[]
}

export interface CreateProductCategoryData {
  name: string
  description?: string
  image?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
}

export interface UpdateProductCategoryData {
  name: string
  description?: string
  image?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
}

// 获取产品分类列表
export const getProductCategories = (includeInactive = false): Promise<ProductCategory[]> => {
  return request.get(`/productcategories?includeInactive=${includeInactive}`)
}

// 获取产品分类树
export const getProductCategoriesTree = (): Promise<ProductCategory[]> => {
  return request.get('/productcategories/tree')
}

// 获取产品分类详情
export const getProductCategoryById = (id: number): Promise<ProductCategory> => {
  return request.get(`/productcategories/${id}`)
}

// 创建产品分类
export const createProductCategory = (data: CreateProductCategoryData): Promise<ProductCategory> => {
  return request.post('/productcategories', data)
}

// 更新产品分类
export const updateProductCategory = (id: number, data: UpdateProductCategoryData): Promise<ProductCategory> => {
  return request.put(`/productcategories/${id}`, data)
}

// 删除产品分类
export const deleteProductCategory = (id: number): Promise<void> => {
  return request.delete(`/productcategories/${id}`)
}