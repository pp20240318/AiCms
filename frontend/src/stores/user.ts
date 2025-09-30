import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { login as apiLogin, logout as apiLogout, getUserInfo } from '@/api/auth'
import { getAllSystemMenus } from '@/api/menus'
import type { LoginData, LoginResponse } from '@/api/auth'
import { ElMessage } from 'element-plus'

export const useUserStore = defineStore('user', () => {
  const token = ref<string>(localStorage.getItem('token') || '')
  const userInfo = ref<any>(null)
  
  const isLoggedIn = computed(() => !!token.value)
  
  // 收集所有菜单的查看权限
  const collectViewPermissions = () => {
    const menus = getAllSystemMenus()
    const permissions: string[] = []

    const traverse = (items: any[]) => {
      items.forEach(item => {
        if (item.permission) {
          // 只收集查看权限，或默认添加主菜单权限
          if (item.permission.includes(':view') || !permissions.includes(item.permission)) {
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

  // 获取用户权限列表
  const defaultViewPermissions = computed(() => collectViewPermissions())

  const permissions = computed(() => {
    if (!userInfo.value?.roles) return []
    
    // 如果角色是字符串数组（后端当前返回格式）
    if (Array.isArray(userInfo.value.roles) && userInfo.value.roles.length > 0 && typeof userInfo.value.roles[0] === 'string') {
      // 临时方案：为已登录用户生成基本权限
      const roleNames = userInfo.value.roles as string[]
      if (roleNames.includes('管理员') || roleNames.includes('admin')) {
        // 管理员拥有所有权限
        return [
          'dashboard:view',
          'users:view', 'users:create', 'users:edit', 'users:delete',
          'roles:view', 'roles:create', 'roles:edit', 'roles:delete',
          'articles:view', 'articles:create', 'articles:edit', 'articles:delete',
          'products:view', 'products:create', 'products:edit', 'products:delete',
          'categories:view', 'categories:create', 'categories:edit', 'categories:delete',
          'banners:view', 'banners:create', 'banners:edit', 'banners:delete',
          'pages:view', 'pages:create', 'pages:edit', 'pages:delete',
          'menus:view', 'menus:create', 'menus:edit', 'menus:delete',
          'members:view', 'members:create', 'members:edit', 'members:delete',
          'seo:view', 'seo:edit',
          'contacts:view', 'contacts:delete'
        ]
      }
      // 其他角色给予所有查看权限，确保菜单可见
      return defaultViewPermissions.value
    }
    
    // 从角色对象中提取权限（未来的格式）
    const allPermissions: string[] = []
    userInfo.value.roles.forEach((role: any) => {
      if (role.permissions) {
        role.permissions.forEach((permission: any) => {
          if (permission.name && !allPermissions.includes(permission.name)) {
            allPermissions.push(permission.name)
          }
        })
      }
    })
    return allPermissions
  })

  const hasRole = (role: string) => {
    if (!userInfo.value?.roles) return false
    
    // 处理字符串数组格式的角色
    if (Array.isArray(userInfo.value.roles) && typeof userInfo.value.roles[0] === 'string') {
      return userInfo.value.roles.includes(role)
    }
    
    // 处理对象数组格式的角色
    return userInfo.value.roles.some((r: any) => r.name === role)
  }
  
  const hasPermission = (permission: string) => {
    return permissions.value.includes(permission)
  }

  // 登录
  const login = async (loginData: LoginData) => {
    try {
      const response: LoginResponse = await apiLogin(loginData)
      token.value = response.token
      // 将roles合并到用户信息中以保持兼容性
      const userWithRoles = {
        ...response.user,
        roles: response.roles
      }
      userInfo.value = userWithRoles
      
      // 调试日志
      console.log('=== 用户登录调试 ===')
      console.log('登录响应:', response)
      console.log('用户角色:', response.roles)
      console.log('用户信息:', userWithRoles)
      console.log('===================')
      localStorage.setItem('token', response.token)
      localStorage.setItem('userInfo', JSON.stringify(userWithRoles))
      ElMessage.success('登录成功')
      return response
    } catch (error: any) {
      const errorMessage = error?.response?.data?.message || error?.message || '登录失败'
      ElMessage.error(errorMessage)
      throw error
    }
  }

  // 退出登录
  const logout = async () => {
    try {
      await apiLogout()
    } catch (error) {
      // 忽略退出登录的错误
    } finally {
      token.value = ''
      userInfo.value = null
      localStorage.removeItem('token')
      localStorage.removeItem('userInfo')
      ElMessage.success('已退出登录')
    }
  }

  // 获取用户信息
  const fetchUserInfo = async () => {
    try {
      const response = await getUserInfo()
      userInfo.value = response
      localStorage.setItem('userInfo', JSON.stringify(response))
      return response
    } catch (error) {
      // 如果获取用户信息失败，清除登录状态
      await logout()
      throw error
    }
  }

  // 初始化用户信息
  const initUserInfo = () => {
    const savedUserInfo = localStorage.getItem('userInfo')
    if (savedUserInfo) {
      try {
        userInfo.value = JSON.parse(savedUserInfo)
      } catch (error) {
        localStorage.removeItem('userInfo')
      }
    }
  }

  return {
    token,
    userInfo,
    isLoggedIn,
    permissions,
    defaultViewPermissions,
    hasRole,
    hasPermission,
    login,
    logout,
    fetchUserInfo,
    initUserInfo
  }
})