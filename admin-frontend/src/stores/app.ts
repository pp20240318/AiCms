import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  const collapsed = ref(false)
  const loading = ref(false)
  const theme = ref<'light' | 'dark'>('light')
  
  // 切换侧边栏折叠状态
  const toggleSidebar = () => {
    collapsed.value = !collapsed.value
  }
  
  // 设置加载状态
  const setLoading = (state: boolean) => {
    loading.value = state
  }
  
  // 切换主题
  const toggleTheme = () => {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
    localStorage.setItem('theme', theme.value)
  }
  
  // 初始化主题
  const initTheme = () => {
    const savedTheme = localStorage.getItem('theme') as 'light' | 'dark'
    if (savedTheme) {
      theme.value = savedTheme
    }
  }
  
  return {
    collapsed,
    loading,
    theme,
    toggleSidebar,
    setLoading,
    toggleTheme,
    initTheme
  }
})