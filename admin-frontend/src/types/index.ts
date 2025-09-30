// 通用类型定义

export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
}

export interface PaginationData<T = any> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

export interface BaseEntity {
  id: number
  createdAt: string
  updatedAt: string
}

// 表格列配置
export interface TableColumn {
  prop?: string
  label: string
  width?: string | number
  minWidth?: string | number
  align?: 'left' | 'center' | 'right'
  sortable?: boolean
  fixed?: boolean | 'left' | 'right'
  type?: 'selection' | 'index' | 'expand'
  formatter?: (row: any, column: any, cellValue: any, index: number) => string
}

// 表单规则
export interface FormRule {
  required?: boolean
  message?: string
  trigger?: 'blur' | 'change'
  min?: number
  max?: number
  type?: 'string' | 'number' | 'boolean' | 'method' | 'regexp' | 'integer' | 'float' | 'array' | 'object' | 'enum' | 'date' | 'url' | 'hex' | 'email'
  pattern?: RegExp
  validator?: (rule: any, value: any, callback: (error?: string | Error) => void) => void
}

// 菜单项
export interface MenuItem {
  id: number
  name: string
  path: string
  icon?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
  children?: MenuItem[]
}

// 文件上传
export interface UploadFile {
  id?: string
  name: string
  url: string
  size: number
  type: string
  status: 'uploading' | 'success' | 'error'
  progress?: number
}

// 通用查询参数
export interface BaseQuery {
  page?: number
  pageSize?: number
  search?: string
  sortField?: string
  sortOrder?: 'asc' | 'desc'
}

// 状态选项
export interface StatusOption {
  label: string
  value: string | number
  type?: 'primary' | 'success' | 'warning' | 'danger' | 'info'
}

// 操作按钮配置
export interface ActionButton {
  text: string
  type?: 'primary' | 'success' | 'warning' | 'danger' | 'info'
  size?: 'large' | 'default' | 'small'
  icon?: string
  loading?: boolean
  disabled?: boolean
  onClick: (row?: any) => void
}

// 确认对话框配置
export interface ConfirmConfig {
  title?: string
  message: string
  type?: 'info' | 'success' | 'warning' | 'error'
  confirmButtonText?: string
  cancelButtonText?: string
  showCancelButton?: boolean
}

// 通知配置
export interface NotificationConfig {
  title?: string
  message: string
  type?: 'success' | 'warning' | 'info' | 'error'
  duration?: number
  showClose?: boolean
  position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left'
}