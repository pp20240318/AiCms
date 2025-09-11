import request from './index'

export interface Product {
  id: number
  name: string
  description: string
  price: number
  originalPrice: number
  categoryId: number
  categoryName: string
  images: string[]
  specifications: Record<string, any>
  stock: number
  status: 'active' | 'inactive' | 'outOfStock'
  featured: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateProductData {
  name: string
  description: string
  price: number
  originalPrice: number
  categoryId: number
  images: string[]
  specifications: Record<string, any>
  stock: number
  status: 'active' | 'inactive'
  featured: boolean
}

export interface UpdateProductData {
  name?: string
  description?: string
  price?: number
  originalPrice?: number
  categoryId?: number
  images?: string[]
  specifications?: Record<string, any>
  stock?: number
  status?: 'active' | 'inactive' | 'outOfStock'
  featured?: boolean
}

export interface ProductsQuery {
  page?: number
  pageSize?: number
  search?: string
  categoryId?: number
  status?: string
  featured?: boolean
}

// 获取产品列表
export const getProducts = (params?: ProductsQuery): Promise<{
  items: Product[]
  total: number
  page: number
  pageSize: number
}> => {
  return request.get('/products', { params })
}

// 获取产品详情
export const getProductById = (id: number): Promise<Product> => {
  return request.get(`/products/${id}`)
}

// 创建产品
export const createProduct = (data: CreateProductData): Promise<Product> => {
  return request.post('/products', data)
}

// 更新产品
export const updateProduct = (id: number, data: UpdateProductData): Promise<Product> => {
  return request.put(`/products/${id}`, data)
}

// 删除产品
export const deleteProduct = (id: number): Promise<void> => {
  return request.delete(`/products/${id}`)
}