import { useUserStore } from '@/stores/user'
import { getAllSystemMenus } from '@/api/menus'
import type { RouteLocationNormalized } from 'vue-router'

/**
 * 检查用户是否有访问路由的权限
 */
export const checkRoutePermission = (route: RouteLocationNormalized): boolean => {
  const userStore = useUserStore()
  
  // 如果路由不需要权限检查，直接通过
  if (route.meta?.requiresAuth === false || !route.meta?.permission) {
    return true
  }
  
  // 检查用户是否登录
  if (!userStore.isLoggedIn) {
    return false
  }
  
  // 临时解决方案：如果用户已登录但没有角色/权限数据，给予仪表盘访问权限
  const userRoles = userStore.userInfo?.roles || []
  if (userRoles.length === 0) {
    // 没有角色数据时，只允许访问仪表盘
    return route.path === '/admin/dashboard' || route.meta.permission === 'dashboard:view'
  }
  
  // 如果有管理员角色，允许所有访问
  if (userRoles.includes('管理员') || userRoles.includes('admin')) {
    return true
  }
  
  // 检查用户角色权限（如果有详细权限数据）
  const userPermissions = userStore.permissions || []
  if (userPermissions.length > 0) {
    const requiredPermission = route.meta.permission as string
    return userPermissions.includes(requiredPermission)
  }
  
  // 如果没有详细权限数据，但有角色，给予基本访问权限
  return true
}

/**
 * 检查用户是否有特定权限
 */
export const hasPermission = (permission: string): boolean => {
  const userStore = useUserStore()
  
  if (!userStore.isLoggedIn) {
    return false
  }
  
  // 如果有管理员角色，允许所有权限
  const userRoles = userStore.userInfo?.roles || []
  if (userRoles.includes('管理员') || userRoles.includes('admin')) {
    return true
  }
  
  const userPermissions = userStore.permissions || []
  
  // 如果没有权限数据，但有角色，给予基本权限
  if (userPermissions.length === 0 && userRoles.length > 0) {
    return true
  }
  
  return userPermissions.includes(permission)
}

/**
 * 调试用：打印当前用户权限信息
 */
export const debugUserPermissions = () => {
  const userStore = useUserStore()
  console.log('=== 用户权限调试信息 ===')
  console.log('已登录:', userStore.isLoggedIn)
  console.log('用户信息:', userStore.userInfo)
  console.log('用户角色:', userStore.userInfo?.roles)
  console.log('用户权限:', userStore.permissions)
  console.log('=======================')
}

/**
 * 检查用户是否有任意一个权限
 */
export const hasAnyPermission = (permissions: string[]): boolean => {
  return permissions.some(permission => hasPermission(permission))
}

/**
 * 检查用户是否有所有权限
 */
export const hasAllPermissions = (permissions: string[]): boolean => {
  return permissions.every(permission => hasPermission(permission))
}

/**
 * 根据权限过滤菜单
 */
export const filterMenusByPermission = (menus: any[]): any[] => {
  const userStore = useUserStore()
  
  if (!userStore.isLoggedIn) {
    return []
  }
  
  // 获取用户角色和权限
  const userRoles = userStore.userInfo?.roles || []
  const userPermissions = userStore.permissions || []
  // 默认查看权限（基于菜单生成）
  const defaultViewPermissions = userPermissions.length === 0 ? collectDefaultViewPermissions() : []
  
  const filterMenus = (menuList: any[]): any[] => {
    return menuList
      .filter(menu => {
        // 如果菜单没有权限要求，显示
        if (!menu.permission) {
          return true
        }
        
        // 如果没有角色数据，只显示仪表盘
        if (userRoles.length === 0) {
          return menu.permission === 'dashboard:view'
        }
        
        // 如果有管理员角色，显示所有菜单
        if (userRoles.includes('管理员') || userRoles.includes('admin')) {
          return true
        }
        
        // 如果没有详细权限数据，但有角色，显示默认查看权限菜单
        if (defaultViewPermissions.length > 0) {
          return defaultViewPermissions.includes(menu.permission)
        }
        
        // 检查用户是否有权限
        return userPermissions.includes(menu.permission)
      })
      .map(menu => ({
        ...menu,
        children: menu.children ? filterMenus(menu.children) : undefined
      }))
      .filter(menu => {
        // 如果是父菜单且没有可显示的子菜单，则隐藏
        if (menu.children && menu.children.length === 0) {
          return false
        }
        return true
      })
  }
  
  return filterMenus(menus)
}

/**
 * 生成权限指令 v-permission
 */
export const permissionDirective = {
  mounted(el: HTMLElement, binding: { value: string | string[] }) {
    const { value } = binding
    
    if (!value) {
      return
    }
    
    const permissions = Array.isArray(value) ? value : [value]
    const hasAuth = hasAnyPermission(permissions)
    
    if (!hasAuth) {
      el.style.display = 'none'
      // 或者移除元素: el.parentNode?.removeChild(el)
    }
  },
  
  updated(el: HTMLElement, binding: { value: string | string[] }) {
    const { value } = binding
    
    if (!value) {
      return
    }
    
    const permissions = Array.isArray(value) ? value : [value]
    const hasAuth = hasAnyPermission(permissions)
    
    if (hasAuth) {
      el.style.display = ''
    } else {
      el.style.display = 'none'
    }
  }
}

// 收集所有菜单的查看权限（用于兜底）
const collectDefaultViewPermissions = (): string[] => {
  const menus = getAllSystemMenus()
  const permissions: string[] = []

  const traverse = (items: any[]) => {
    items.forEach(item => {
      if (item.permission) {
        if (item.permission.includes(':view') && !permissions.includes(item.permission)) {
          permissions.push(item.permission)
        }
      }
      if (item.children) {
        traverse(item.children)
      }
    })
  }

  traverse(menus)
  return permissions
}
