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
      // 临时模拟登录成功（因为后端API可能还没有完全对接）
      if (loginData.username === 'admin' && loginData.password === 'admin123') {
        const mockResponse: LoginResponse = {
          token: 'mock-jwt-token-' + Date.now(),
          user: {
            id: 1,
            username: 'admin',
            email: 'admin@example.com',
            roles: ['admin', 'user']
          }
        }
        
        token.value = mockResponse.token
        userInfo.value = mockResponse.user
        localStorage.setItem('token', mockResponse.token)
        localStorage.setItem('userInfo', JSON.stringify(mockResponse.user))
        ElMessage.success('登录成功')
        return mockResponse
      } else {
        throw new Error('用户名或密码错误')
      }
      
      // 实际API调用（暂时注释）
      // const response: LoginResponse = await apiLogin(loginData)
      // token.value = response.token
      // userInfo.value = response.user
      // localStorage.setItem('token', response.token)
      // localStorage.setItem('userInfo', JSON.stringify(response.user))
      // ElMessage.success('登录成功')
      // return response
    } catch (error) {
      ElMessage.error('登录失败')
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