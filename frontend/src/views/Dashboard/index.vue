<template>
  <div class="dashboard">
    <div class="dashboard-header">
      <h1>仪表盘</h1>
      <p>欢迎回来，{{ userInfo?.username }}！</p>
      <el-button type="info" size="small" @click="debugPermissions">调试权限信息</el-button>
    </div>
    
    <div class="stats-grid">
      <el-card class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.users }}</div>
          <div class="stat-label">用户总数</div>
        </div>
        <el-icon class="stat-icon user"><User /></el-icon>
      </el-card>
      
      <el-card class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.articles }}</div>
          <div class="stat-label">文章总数</div>
        </div>
        <el-icon class="stat-icon article"><Document /></el-icon>
      </el-card>
      
      <el-card class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.products }}</div>
          <div class="stat-label">产品总数</div>
        </div>
        <el-icon class="stat-icon product"><ShoppingBag /></el-icon>
      </el-card>
      
      <el-card class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.categories }}</div>
          <div class="stat-label">分类总数</div>
        </div>
        <el-icon class="stat-icon category"><FolderOpened /></el-icon>
      </el-card>
    </div>
    
    <div class="dashboard-content">
      <el-row :gutter="24">
        <el-col :span="12">
          <el-card title="最近文章">
            <template #header>
              <div class="card-header">
                <span>最近文章</span>
                <el-button text @click="$router.push('/articles')">查看更多</el-button>
              </div>
            </template>
            <div class="recent-list">
              <div v-for="article in recentArticles" :key="article.id" class="recent-item">
                <div class="item-title">{{ article.title }}</div>
                <div class="item-meta">{{ article.author }} · {{ formatDate(article.createdAt) }}</div>
              </div>
              <el-empty v-if="recentArticles.length === 0" description="暂无数据" />
            </div>
          </el-card>
        </el-col>
        
        <el-col :span="12">
          <el-card>
            <template #header>
              <div class="card-header">
                <span>最近产品</span>
                <el-button text @click="$router.push('/products')">查看更多</el-button>
              </div>
            </template>
            <div class="recent-list">
              <div v-for="product in recentProducts" :key="product.id" class="recent-item">
                <div class="item-title">{{ product.name }}</div>
                <div class="item-meta">¥{{ product.price }} · {{ formatDate(product.createdAt) }}</div>
              </div>
              <el-empty v-if="recentProducts.length === 0" description="暂无数据" />
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { debugUserPermissions } from '@/utils/permission'
import { User, Document, ShoppingBag, FolderOpened } from '@element-plus/icons-vue'
import dayjs from 'dayjs'

const userStore = useUserStore()
const userInfo = computed(() => userStore.userInfo)

const stats = ref({
  users: 0,
  articles: 0,
  products: 0,
  categories: 0
})

const recentArticles = ref<any[]>([])
const recentProducts = ref<any[]>([])

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD')
}

const loadDashboardData = async () => {
  try {
    // 这里应该调用API获取数据，暂时使用模拟数据
    stats.value = {
      users: 128,
      articles: 45,
      products: 32,
      categories: 12
    }
    
    recentArticles.value = [
      { id: 1, title: 'Vue 3 新特性详解', author: '张三', createdAt: '2024-01-15' },
      { id: 2, title: 'TypeScript 实践指南', author: '李四', createdAt: '2024-01-14' },
      { id: 3, title: 'Element Plus 组件使用', author: '王五', createdAt: '2024-01-13' }
    ]
    
    recentProducts.value = [
      { id: 1, name: '高端笔记本电脑', price: 8999, createdAt: '2024-01-15' },
      { id: 2, name: '无线蓝牙耳机', price: 299, createdAt: '2024-01-14' },
      { id: 3, name: '智能手机', price: 3999, createdAt: '2024-01-13' }
    ]
  } catch (error) {
    console.error('加载仪表盘数据失败:', error)
  }
}

// 调试权限信息
const debugPermissions = () => {
  debugUserPermissions()
}

onMounted(() => {
  loadDashboardData()
})
</script>

<style scoped>
.dashboard {
  height: 100%;
}

.dashboard-header {
  margin-bottom: 24px;
}

.dashboard-header h1 {
  margin: 0 0 8px 0;
  font-size: 24px;
  color: #303133;
}

.dashboard-header p {
  margin: 0;
  color: #909399;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 24px;
  margin-bottom: 24px;
}

.stat-card {
  position: relative;
  overflow: hidden;
}

.stat-content {
  z-index: 2;
  position: relative;
}

.stat-number {
  font-size: 32px;
  font-weight: bold;
  color: #303133;
  line-height: 1;
}

.stat-label {
  font-size: 14px;
  color: #909399;
  margin-top: 8px;
}

.stat-icon {
  position: absolute;
  top: 16px;
  right: 16px;
  font-size: 48px;
  opacity: 0.2;
}

.stat-icon.user {
  color: #409eff;
}

.stat-icon.article {
  color: #67c23a;
}

.stat-icon.product {
  color: #e6a23c;
}

.stat-icon.category {
  color: #f56c6c;
}

.dashboard-content {
  margin-top: 24px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.recent-list {
  max-height: 300px;
  overflow-y: auto;
}

.recent-item {
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}

.recent-item:last-child {
  border-bottom: none;
}

.item-title {
  font-size: 14px;
  color: #303133;
  margin-bottom: 4px;
}

.item-meta {
  font-size: 12px;
  color: #909399;
}
</style>