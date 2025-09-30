<template>
  <el-aside :width="collapsed ? '64px' : '250px'" class="sidebar">
    <div class="logo">
      <h2 v-if="!collapsed">CMS管理系统</h2>
      <h2 v-else>CMS</h2>
    </div>
    
    <el-menu
      :default-active="currentRoute"
      class="sidebar-menu"
      :collapse="collapsed"
      :router="true"
    >
      <!-- 递归渲染菜单 -->
      <template v-for="menu in visibleMenus" :key="menu.id">
        <sidebar-menu-item :menu="menu" />
      </template>
    </el-menu>
  </el-aside>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAppStore } from '@/stores/app'
import { getAllSystemMenus, type Menu } from '@/api/menus'
import { filterMenusByPermission } from '@/utils/permission'
import SidebarMenuItem from './SidebarMenuItem.vue'

const route = useRoute()
const appStore = useAppStore()

const collapsed = computed(() => appStore.collapsed)
const currentRoute = computed(() => route.path)
const menus = ref<Menu[]>([])

// 获取可见的菜单（基于权限过滤）
const visibleMenus = computed(() => filterMenusByPermission(menus.value))

// 加载菜单数据
const loadMenus = async () => {
  try {
    menus.value = getAllSystemMenus()
  } catch (error) {
    console.error('加载菜单失败:', error)
  }
}

onMounted(() => {
  loadMenus()
})
</script>

<style scoped>
.sidebar {
  background: #304156;
  color: #bfcbd9;
  transition: width 0.28s;
  min-height: 100vh;
}

.logo {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 60px;
  background: #2b3a4b;
  border-bottom: 1px solid #1f2d3d;
}

.logo h2 {
  color: #fff;
  margin: 0;
  font-size: 16px;
  white-space: nowrap;
}

.sidebar-menu {
  border: none;
  background: transparent;
}

:deep(.el-menu-item) {
  color: #bfcbd9;
  background: transparent !important;
}

:deep(.el-menu-item:hover) {
  color: #fff;
  background: #263445 !important;
}

:deep(.el-menu-item.is-active) {
  color: #409eff;
  background: rgba(64, 158, 255, 0.1) !important;
}

:deep(.el-sub-menu__title) {
  color: #bfcbd9;
  background: transparent !important;
}

:deep(.el-sub-menu__title:hover) {
  color: #fff;
  background: #263445 !important;
}

:deep(.el-sub-menu .el-menu) {
  background: #1f2d3d;
}
</style>