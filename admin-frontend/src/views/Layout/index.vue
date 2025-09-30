<template>
  <div class="layout">
    <Sidebar />
    <div class="main">
      <Header />
      <TabBar />
      <el-main class="content">
        <router-view />
      </el-main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { useAppStore } from '@/stores/app'
import { useTabsStore } from '@/stores/tabs'
import Sidebar from '@/components/Layout/Sidebar.vue'
import Header from '@/components/Layout/Header.vue'
import TabBar from '@/components/Layout/TabBar.vue'

const userStore = useUserStore()
const appStore = useAppStore()
const tabsStore = useTabsStore()

onMounted(() => {
  // 初始化用户信息和主题
  userStore.initUserInfo()
  appStore.initTheme()

  // 初始化或恢复tabs
  tabsStore.restoreTabsFromStorage()
})
</script>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.content {
  background: #f0f2f5;
  padding: 24px;
  overflow: auto;
}
</style>