<template>
  <el-header class="layout-header">
    <div class="left">
      <el-button 
        :icon="Fold" 
        @click="toggleSidebar"
        circle
        size="small"
      />
    </div>
    
    <div class="right">
      <el-dropdown @command="handleCommand">
        <div class="user-info">
          <el-avatar :size="32" :src="userInfo?.avatar || ''">
            <el-icon><User /></el-icon>
          </el-avatar>
          <span class="username">{{ userInfo?.username || '管理员' }}</span>
          <el-icon class="arrow"><ArrowDown /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="profile">
              <el-icon><User /></el-icon>
              个人信息
            </el-dropdown-item>
            <el-dropdown-item command="settings">
              <el-icon><Setting /></el-icon>
              系统设置
            </el-dropdown-item>
            <el-dropdown-item divided command="logout">
              <el-icon><SwitchButton /></el-icon>
              退出登录
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </el-header>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessageBox } from 'element-plus'
import { useUserStore } from '@/stores/user'
import { useAppStore } from '@/stores/app'
import {
  Fold,
  User,
  ArrowDown,
  Setting,
  SwitchButton
} from '@element-plus/icons-vue'

const router = useRouter()
const userStore = useUserStore()
const appStore = useAppStore()

const userInfo = computed(() => userStore.userInfo)

const toggleSidebar = () => {
  appStore.toggleSidebar()
}

const handleCommand = async (command: string) => {
  switch (command) {
    case 'profile':
      // 跳转到个人信息页面
      router.push('/profile')
      break
    case 'settings':
      // 跳转到系统设置页面
      router.push('/settings')
      break
    case 'logout':
      try {
        await ElMessageBox.confirm('确定要退出登录吗？', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        })
        await userStore.logout()
        router.push('/login')
      } catch (error) {
        // 用户取消操作
      }
      break
  }
}
</script>

<style scoped>
.layout-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #fff;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  padding: 0 16px;
}

.left {
  display: flex;
  align-items: center;
}

.right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 8px;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.user-info:hover {
  background-color: #f5f5f5;
}

.username {
  margin: 0 8px 0 12px;
  font-size: 14px;
  color: #333;
}

.arrow {
  font-size: 12px;
  color: #999;
}
</style>