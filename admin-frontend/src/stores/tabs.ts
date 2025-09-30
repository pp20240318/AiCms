import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { RouteLocationNormalized } from 'vue-router'

export interface TabItem {
  path: string
  name: string
  title: string
  closable: boolean
  icon?: string
}

export const useTabsStore = defineStore('tabs', () => {
  const tabs = ref<TabItem[]>([])
  const activeTab = ref<string>('')

  // 获取当前激活的tab
  const currentTab = computed(() =>
    tabs.value.find(tab => tab.path === activeTab.value)
  )

  // 初始化默认tab（仪表盘）
  const initTabs = () => {
    const dashboardTab: TabItem = {
      path: '/admin/dashboard',
      name: 'Dashboard',
      title: '仪表盘',
      closable: false,
      icon: 'Dashboard'
    }

    if (!tabs.value.some(tab => tab.path === dashboardTab.path)) {
      tabs.value = [dashboardTab]
    }

    if (!activeTab.value) {
      activeTab.value = dashboardTab.path
    }
  }

  // 添加tab
  const addTab = (route: RouteLocationNormalized) => {
    const { path, name, meta } = route

    // 检查是否已存在
    const existingTab = tabs.value.find(tab => tab.path === path)
    if (existingTab) {
      activeTab.value = path
      return
    }

    // 创建新tab
    const newTab: TabItem = {
      path,
      name: name as string,
      title: (meta?.title as string) || getDefaultTitle(path),
      closable: path !== '/admin/dashboard',
      icon: meta?.icon as string
    }

    tabs.value.push(newTab)
    activeTab.value = path
    saveTabsToStorage()
  }

  // 关闭tab
  const closeTab = (targetPath: string) => {
    const index = tabs.value.findIndex(tab => tab.path === targetPath)
    if (index === -1) return

    const tab = tabs.value[index]
    if (!tab.closable) return

    tabs.value.splice(index, 1)

    // 如果关闭的是当前激活的tab，需要切换到其他tab
    if (activeTab.value === targetPath) {
      if (tabs.value.length > 0) {
        // 优先切换到右侧tab，如果没有则切换到左侧
        const nextTab = tabs.value[index] || tabs.value[index - 1]
        activeTab.value = nextTab.path
      }
    }

    saveTabsToStorage()
  }

  // 关闭其他tab
  const closeOtherTabs = (keepPath: string) => {
    tabs.value = tabs.value.filter(tab => tab.path === keepPath || !tab.closable)
    activeTab.value = keepPath
    saveTabsToStorage()
  }

  // 关闭所有可关闭的tab
  const closeAllTabs = () => {
    tabs.value = tabs.value.filter(tab => !tab.closable)
    if (tabs.value.length > 0) {
      activeTab.value = tabs.value[0].path
    }
    saveTabsToStorage()
  }

  // 切换到指定tab
  const switchTab = (path: string) => {
    if (tabs.value.some(tab => tab.path === path)) {
      activeTab.value = path
    }
  }

  // 获取默认标题
  const getDefaultTitle = (path: string): string => {
    const titleMap: Record<string, string> = {
      '/admin/dashboard': '仪表盘',
      '/admin/users': '用户管理',
      '/admin/roles': '角色管理',
      '/admin/members': '会员管理',
      '/admin/articles': '文章管理',
      '/admin/articles/create': '新建文章',
      '/admin/products': '产品管理',
      '/admin/article-categories': '文章分类',
      '/admin/product-categories': '产品分类',
      '/admin/menus': '菜单管理',
      '/admin/banners': '轮播图管理',
      '/admin/seo': 'SEO设置',
      '/admin/contacts': '联系信息',
      '/admin/pages': '页面管理'
    }

    // 处理编辑页面
    if (path.includes('/edit')) {
      if (path.includes('/articles/')) return '编辑文章'
      if (path.includes('/products/')) return '编辑产品'
    }

    return titleMap[path] || '页面'
  }

  // 保存到本地存储
  const saveTabsToStorage = () => {
    const tabsData = {
      tabs: tabs.value,
      activeTab: activeTab.value
    }
    localStorage.setItem('admin-tabs', JSON.stringify(tabsData))
  }

  // 从本地存储恢复
  const restoreTabsFromStorage = () => {
    try {
      const savedData = localStorage.getItem('admin-tabs')
      if (savedData) {
        const { tabs: savedTabs, activeTab: savedActiveTab } = JSON.parse(savedData)
        if (Array.isArray(savedTabs) && savedTabs.length > 0) {
          tabs.value = savedTabs
          activeTab.value = savedActiveTab || savedTabs[0].path
          return
        }
      }
    } catch (error) {
      console.error('恢复tabs数据失败:', error)
    }

    // 如果恢复失败，初始化默认tabs
    initTabs()
  }

  // 清除存储的tabs数据
  const clearTabsStorage = () => {
    localStorage.removeItem('admin-tabs')
    tabs.value = []
    activeTab.value = ''
    initTabs()
  }

  return {
    tabs,
    activeTab,
    currentTab,
    initTabs,
    addTab,
    closeTab,
    closeOtherTabs,
    closeAllTabs,
    switchTab,
    saveTabsToStorage,
    restoreTabsFromStorage,
    clearTabsStorage
  }
})