<template>
  <!-- 有子菜单的情况 -->
  <el-sub-menu 
    v-if="menu.children && menu.children.length > 0" 
    :index="menu.path"
  >
    <template #title>
      <el-icon v-if="menu.icon">
        <component :is="getIconComponent(menu.icon)" />
      </el-icon>
      <span>{{ menu.name }}</span>
    </template>
    
    <!-- 递归渲染子菜单 -->
    <template v-for="child in menu.children" :key="child.id">
      <sidebar-menu-item :menu="child" />
    </template>
  </el-sub-menu>
  
  <!-- 没有子菜单的普通菜单项 -->
  <el-menu-item v-else :index="menu.path">
    <el-icon v-if="menu.icon">
      <component :is="getIconComponent(menu.icon)" />
    </el-icon>
    <template #title>{{ menu.name }}</template>
  </el-menu-item>
</template>

<script setup lang="ts">
import type { Menu } from '@/api/menus'
import {
  HomeFilled,
  Document,
  DocumentCopy,
  EditPen,
  Plus,
  FolderOpened,
  ShoppingBag,
  CollectionTag,
  Notebook,
  Picture,
  User,
  Avatar,
  UserFilled,
  Postcard,
  Setting,
  Menu as MenuIcon,
  Search,
  Message,
  PictureFilled,
  Monitor,
  Wallet
} from '@element-plus/icons-vue'

interface Props {
  menu: Menu
}

const props = defineProps<Props>()

// 图标映射
const iconMap: Record<string, any> = {
  HomeFilled,
  Document,
  DocumentCopy,
  EditPen,
  Plus,
  FolderOpened,
  ShoppingBag,
  CollectionTag,
  Notebook,
  Picture,
  User,
  Avatar,
  UserFilled,
  Postcard,
  Setting,
  Menu: MenuIcon,
  Search,
  Message,
  PictureFilled,
  Monitor,
  Wallet
}

// 获取图标组件
const getIconComponent = (iconName: string) => {
  return iconMap[iconName] || Document
}
</script>

<style scoped>
/* 继承父组件的样式 */
</style>

