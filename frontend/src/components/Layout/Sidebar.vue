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
      <el-menu-item index="/dashboard">
        <el-icon><HomeFilled /></el-icon>
        <template #title>仪表盘</template>
      </el-menu-item>
      
      <el-sub-menu index="content">
        <template #title>
          <el-icon><Document /></el-icon>
          <span>内容管理</span>
        </template>
        <el-menu-item index="/articles">
          <el-icon><DocumentCopy /></el-icon>
          <template #title>文章管理</template>
        </el-menu-item>
        <el-menu-item index="/products">
          <el-icon><ShoppingBag /></el-icon>
          <template #title>产品管理</template>
        </el-menu-item>
        <el-menu-item index="/categories">
          <el-icon><FolderOpened /></el-icon>
          <template #title>分类管理</template>
        </el-menu-item>
      </el-sub-menu>
      
      <el-sub-menu index="system">
        <template #title>
          <el-icon><Setting /></el-icon>
          <span>系统管理</span>
        </template>
        <el-menu-item index="/users">
          <el-icon><User /></el-icon>
          <template #title>用户管理</template>
        </el-menu-item>
        <el-menu-item index="/roles">
          <el-icon><UserFilled /></el-icon>
          <template #title>角色管理</template>
        </el-menu-item>
        <el-menu-item index="/menus">
          <el-icon><Menu /></el-icon>
          <template #title>菜单管理</template>
        </el-menu-item>
      </el-sub-menu>
      
      <el-menu-item index="/banners">
        <el-icon><PictureFilled /></el-icon>
        <template #title>轮播图管理</template>
      </el-menu-item>
    </el-menu>
  </el-aside>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAppStore } from '@/stores/app'
import {
  HomeFilled,
  Document,
  DocumentCopy,
  ShoppingBag,
  FolderOpened,
  Setting,
  User,
  UserFilled,
  Menu,
  PictureFilled
} from '@element-plus/icons-vue'

const route = useRoute()
const appStore = useAppStore()

const collapsed = computed(() => appStore.collapsed)
const currentRoute = computed(() => route.path)
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