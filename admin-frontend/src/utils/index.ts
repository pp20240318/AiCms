// 通用工具函数

/**
 * 格式化日期
 */
export const formatDate = (date: string | Date, format: string = 'YYYY-MM-DD HH:mm:ss'): string => {
  const d = new Date(date)
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const hour = String(d.getHours()).padStart(2, '0')
  const minute = String(d.getMinutes()).padStart(2, '0')
  const second = String(d.getSeconds()).padStart(2, '0')

  return format
    .replace('YYYY', String(year))
    .replace('MM', month)
    .replace('DD', day)
    .replace('HH', hour)
    .replace('mm', minute)
    .replace('ss', second)
}

/**
 * 深度克隆对象
 */
export const deepClone = <T>(obj: T): T => {
  if (obj === null || typeof obj !== 'object') {
    return obj
  }
  
  if (obj instanceof Date) {
    return new Date(obj.getTime()) as unknown as T
  }
  
  if (obj instanceof Array) {
    return obj.map(item => deepClone(item)) as unknown as T
  }
  
  if (typeof obj === 'object') {
    const clonedObj = {} as T
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        clonedObj[key] = deepClone(obj[key])
      }
    }
    return clonedObj
  }
  
  return obj
}

/**
 * 防抖函数
 */
export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout | null = null
  
  return (...args: Parameters<T>) => {
    if (timeout) {
      clearTimeout(timeout)
    }
    
    timeout = setTimeout(() => {
      func(...args)
    }, wait)
  }
}

/**
 * 节流函数
 */
export const throttle = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let lastTime = 0
  
  return (...args: Parameters<T>) => {
    const now = Date.now()
    
    if (now - lastTime >= wait) {
      lastTime = now
      func(...args)
    }
  }
}

/**
 * 生成随机字符串
 */
export const generateRandomString = (length: number = 8): string => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
  let result = ''
  
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length))
  }
  
  return result
}

/**
 * 文件大小格式化
 */
export const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

/**
 * 树形数据扁平化
 */
export const flattenTree = <T extends { children?: T[] }>(
  tree: T[],
  childrenKey: keyof T = 'children' as keyof T
): T[] => {
  const result: T[] = []
  
  const flatten = (nodes: T[]) => {
    nodes.forEach(node => {
      result.push(node)
      if (node[childrenKey] && Array.isArray(node[childrenKey])) {
        flatten(node[childrenKey] as T[])
      }
    })
  }
  
  flatten(tree)
  return result
}

/**
 * 数组转树形结构
 */
export const arrayToTree = <T extends Record<string, any>>(
  array: T[],
  options: {
    idKey?: string
    parentIdKey?: string
    childrenKey?: string
  } = {}
): T[] => {
  const { idKey = 'id', parentIdKey = 'parentId', childrenKey = 'children' } = options
  
  const tree: T[] = []
  const childrenMap: Record<string, T[]> = {}
  
  // 构建子节点映射
  array.forEach(item => {
    const parentId = item[parentIdKey]
    if (parentId) {
      if (!childrenMap[parentId]) {
        childrenMap[parentId] = []
      }
      childrenMap[parentId].push(item)
    }
  })
  
  // 构建树形结构
  array.forEach(item => {
    const id = item[idKey]
    const parentId = item[parentIdKey]
    
    if (childrenMap[id]) {
      item[childrenKey] = childrenMap[id]
    }
    
    if (!parentId) {
      tree.push(item)
    }
  })
  
  return tree
}