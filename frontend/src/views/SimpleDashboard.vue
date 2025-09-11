<template>
  <div style="padding: 20px;">
    <h1>🎉 登录成功！</h1>
    <h2>欢迎来到CMS管理系统</h2>
    <p>恭喜您成功登录！登录跳转功能正常工作。</p>
    
    <el-card style="margin-top: 20px;">
      <h3>用户信息</h3>
      <p><strong>用户名:</strong> {{ userInfo?.username || '未知' }}</p>
      <p><strong>邮箱:</strong> {{ userInfo?.email || '未设置' }}</p>
      <p><strong>角色:</strong> {{ userInfo?.roles?.join(', ') || '无角色' }}</p>
    </el-card>
    
    <div style="margin-top: 20px;">
      <el-button type="primary" @click="goToFullDashboard">前往完整仪表盘</el-button>
      <el-button @click="logout">退出登录</el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'

const router = useRouter()
const userStore = useUserStore()

const userInfo = computed(() => userStore.userInfo)

const goToFullDashboard = () => {
  router.push('/dashboard')
}

const logout = async () => {
  await userStore.logout()
  router.push('/login')
}
</script>