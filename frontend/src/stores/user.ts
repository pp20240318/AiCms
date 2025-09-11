import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { login as apiLogin, logout as apiLogout, getUserInfo } from '@/api/auth'
import type { LoginData, LoginResponse } from '@/api/auth'
import { ElMessage } from 'element-plus'

export const useUserStore = defineStore('user', () => {
  const token = ref<string>(localStorage.getItem('token') || '')
  const userInfo = ref<any>(null)
  
  const isLoggedIn = computed(() => !!token.value)
  const hasRole = (role: string) => {
    return userInfo.value?.roles?.includes(role) || false
  }
  
  const hasPermission = (permission: string) => {
    return userInfo.value?.permissions?.includes(permission) || false
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
    hasRole,
    hasPermission,
    login,
    logout,
    fetchUserInfo,
    initUserInfo
  }
})