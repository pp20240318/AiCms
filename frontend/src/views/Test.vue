<template>
  <div style="padding: 20px;">
    <h1>测试页面</h1>
    <p>如果您能看到这个页面，说明路由跳转正常工作！</p>
    <el-button type="primary" @click="goBack">返回登录页</el-button>
    <el-button @click="goToDashboard">去仪表盘</el-button>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goBack = () => {
  router.push('/login')
}

const goToDashboard = () => {
  router.push('/dashboard')
}
</script>