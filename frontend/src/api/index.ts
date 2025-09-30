import axios from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import router from '@/router'

// 创建axios实例
const request = axios.create({
  baseURL: '/api',
  timeout: 10000
})

// 请求拦截器
request.interceptors.request.use(
  config => {
    const userStore = useUserStore()
    const token = userStore.token
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => {
    return Promise.reject(error)
  }
)

// 响应拦截器
request.interceptors.response.use(
  response => {
    // 检查是否是包装的API响应格式
    if (response.data && typeof response.data === 'object' && 'success' in response.data) {
      if (response.data.success) {
        return response.data.data // 返回实际数据
      } else {
        // API返回失败，抛出错误
        throw new Error(response.data.message || '请求失败')
      }
    }
    // 如果不是包装格式，直接返回数据
    return response.data
  },
  error => {
    const { response } = error
    if (response) {
      switch (response.status) {
        case 401:
          ElMessage.error('登录已过期，请重新登录')
          const userStore = useUserStore()
          userStore.logout()
          router.push('/login')
          break
        case 403:
          ElMessage.error('没有权限访问该资源')
          break
        case 404:
          ElMessage.error('请求的资源不存在')
          break
        case 500:
          ElMessage.error('服务器内部错误')
          break
        default:
          // 处理包装的错误响应
          let errorMessage = '请求失败'
          if (response.data && typeof response.data === 'object') {
            if ('message' in response.data) {
              errorMessage = response.data.message
            } else if ('data' in response.data && response.data.data?.message) {
              errorMessage = response.data.data.message
            }
          }
          ElMessage.error(errorMessage)
      }
    } else {
      ElMessage.error('网络连接失败')
    }
    return Promise.reject(error)
  }
)

export default request