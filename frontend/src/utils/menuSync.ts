import type { RouteRecordNormalized } from 'vue-router'
import type { Menu } from '@/api/menus'

// 从路由生成菜单项
export const generateMenuFromRoute = (route: RouteRecordNormalized, parentId?: number): Menu | null => {
  // 跳过不需要在菜单中显示的路由
  if (route.meta?.hideInMenu || route.path.includes(':') || route.redirect) {
    return null
  }

  // 获取路由名称
  const name = route.meta?.title as string || route.name as string || route.path.split('/').pop() || ''
  
  // 如果没有名称，跳过
  if (!name) return null

  return {
    id: Math.random(), // 临时ID，实际应该由后端生成
    name,
    path: route.path,
    icon: route.meta?.icon as string,
    component: route.name as string,
    parentId,
    sortOrder: route.meta?.order as number || 0,
    isActive: true,
    isVisible: route.meta?.visible !== false,
    permission: route.meta?.permission as string,
    description: route.meta?.description as string,
    createdAt: new Date().toISOString()
  }
}

// 比较菜单和路由的差异
export const compareMenuWithRoutes = (menus: Menu[], routes: RouteRecordNormalized[]) => {
  const menuPaths = new Set<string>()
  const routePaths = new Set<string>()

  // 收集所有菜单路径
  const collectMenuPaths = (items: Menu[]) => {
    items.forEach(item => {
      menuPaths.add(item.path)
      if (item.children) {
        collectMenuPaths(item.children)
      }
    })
  }
  collectMenuPaths(menus)

  // 收集所有路由路径（排除特殊路由）
  routes.forEach(route => {
    if (!route.meta?.hideInMenu && !route.path.includes(':') && !route.redirect) {
      routePaths.add(route.path)
    }
  })

  return {
    menuOnly: Array.from(menuPaths).filter(path => !routePaths.has(path)),
    routeOnly: Array.from(routePaths).filter(path => !menuPaths.has(path)),
    common: Array.from(menuPaths).filter(path => routePaths.has(path))
  }
}

// 建议的菜单结构更新
export const suggestMenuUpdates = (comparison: ReturnType<typeof compareMenuWithRoutes>) => {
  const suggestions: string[] = []

  if (comparison.menuOnly.length > 0) {
    suggestions.push(`发现 ${comparison.menuOnly.length} 个菜单项没有对应的路由：${comparison.menuOnly.join(', ')}`)
  }

  if (comparison.routeOnly.length > 0) {
    suggestions.push(`发现 ${comparison.routeOnly.length} 个路由没有对应的菜单项：${comparison.routeOnly.join(', ')}`)
  }

  return suggestions
}

// 自动修复菜单结构
export const autoFixMenuStructure = (menus: Menu[], routes: RouteRecordNormalized[]): Menu[] => {
  const comparison = compareMenuWithRoutes(menus, routes)
  
  // 创建新的菜单副本
  const updatedMenus = [...menus]

  // 为没有菜单项的路由创建菜单项
  comparison.routeOnly.forEach(routePath => {
    const route = routes.find(r => r.path === routePath)
    if (route) {
      const menuItem = generateMenuFromRoute(route)
      if (menuItem) {
        // 根据路径判断应该放在哪个分类下
        if (routePath.includes('/admin/articles') || routePath.includes('/admin/products') || routePath.includes('/admin/pages')) {
          // 内容管理
          const contentMenu = updatedMenus.find(m => m.path === '/admin/content')
          if (contentMenu) {
            contentMenu.children = contentMenu.children || []
            contentMenu.children.push(menuItem)
          }
        } else if (routePath.includes('/admin/users') || routePath.includes('/admin/roles') || routePath.includes('/admin/members')) {
          // 用户管理
          const userMenu = updatedMenus.find(m => m.path === '/admin/users-management')
          if (userMenu) {
            userMenu.children = userMenu.children || []
            userMenu.children.push(menuItem)
          }
        } else if (routePath.includes('/admin/menus') || routePath.includes('/admin/seo') || routePath.includes('/admin/contacts')) {
          // 系统设置
          const systemMenu = updatedMenus.find(m => m.path === '/admin/system')
          if (systemMenu) {
            systemMenu.children = systemMenu.children || []
            systemMenu.children.push(menuItem)
          }
        } else {
          // 顶级菜单
          updatedMenus.push(menuItem)
        }
      }
    }
  })

  return updatedMenus
}
