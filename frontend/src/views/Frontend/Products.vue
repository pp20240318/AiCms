<template>
  <div class="products-page">
    <!-- 页面头部 -->
    <section class="page-header">
      <div class="container">
        <h1>产品中心</h1>
        <p>为您提供优质的产品和解决方案</p>
      </div>
    </section>

    <!-- 产品筛选 -->
    <section class="products-filters">
      <div class="container">
        <div class="filter-bar">
          <div class="filter-item">
            <label>产品分类:</label>
            <select v-model="selectedCategory" @change="filterProducts">
              <option value="">全部分类</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>
          <div class="filter-item">
            <label>价格排序:</label>
            <select v-model="sortBy" @change="sortProducts">
              <option value="">默认排序</option>
              <option value="price_asc">价格从低到高</option>
              <option value="price_desc">价格从高到低</option>
              <option value="name">按名称排序</option>
            </select>
          </div>
          <div class="filter-item">
            <input
              type="text"
              v-model="searchKeyword"
              placeholder="搜索产品..."
              @input="searchProducts"
            >
          </div>
        </div>
      </div>
    </section>

    <!-- 产品列表 -->
    <section class="products-list">
      <div class="container">
        <div v-if="loading" class="loading">
          <p>加载中...</p>
        </div>
        <div v-else-if="filteredProducts.length === 0" class="empty-state">
          <p>暂无产品数据</p>
        </div>
        <div v-else class="products-grid">
          <div
            class="product-card"
            v-for="product in paginatedProducts"
            :key="product.id"
            @click="viewProductDetail(product)"
          >
            <div class="product-image">
              <img
                :src="product.imageUrl || 'https://via.placeholder.com/300x200?text=Product'"
                :alt="product.name"
              />
              <div class="product-overlay">
                <button class="view-btn">查看详情</button>
              </div>
            </div>
            <div class="product-info">
              <h3>{{ product.name }}</h3>
              <p class="product-description">{{ product.description }}</p>
              <div class="product-meta">
                <span class="product-category">{{ getCategoryName(product.categoryId) }}</span>
                <span class="product-price" v-if="product.price">¥{{ product.price }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- 分页 -->
        <div v-if="totalPages > 1" class="pagination">
          <button
            @click="prevPage"
            :disabled="currentPage === 1"
            class="pagination-btn"
          >
            上一页
          </button>
          <span class="pagination-info">
            第 {{ currentPage }} 页，共 {{ totalPages }} 页
          </span>
          <button
            @click="nextPage"
            :disabled="currentPage === totalPages"
            class="pagination-btn"
          >
            下一页
          </button>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)
const products = ref([])
const categories = ref([])
const selectedCategory = ref('')
const sortBy = ref('')
const searchKeyword = ref('')
const currentPage = ref(1)
const pageSize = 12

// 模拟产品数据
const mockProducts = [
  {
    id: 1,
    name: '智能手机 Pro Max',
    description: '最新旗舰智能手机，配备先进的处理器和摄像系统',
    price: 6999,
    categoryId: 1,
    imageUrl: 'https://via.placeholder.com/300x200?text=Phone'
  },
  {
    id: 2,
    name: '无线耳机',
    description: '高品质无线蓝牙耳机，降噪效果出色',
    price: 1299,
    categoryId: 1,
    imageUrl: 'https://via.placeholder.com/300x200?text=Earphones'
  },
  {
    id: 3,
    name: '笔记本电脑',
    description: '轻薄便携笔记本电脑，适合办公和学习',
    price: 8999,
    categoryId: 2,
    imageUrl: 'https://via.placeholder.com/300x200?text=Laptop'
  },
  {
    id: 4,
    name: '平板电脑',
    description: '大屏高清平板电脑，娱乐办公两不误',
    price: 3999,
    categoryId: 2,
    imageUrl: 'https://via.placeholder.com/300x200?text=Tablet'
  },
  {
    id: 5,
    name: '智能手表',
    description: '多功能智能手表，健康监测运动追踪',
    price: 2199,
    categoryId: 1,
    imageUrl: 'https://via.placeholder.com/300x200?text=Watch'
  },
  {
    id: 6,
    name: '相机镜头',
    description: '专业相机镜头，拍摄效果出众',
    price: 4599,
    categoryId: 3,
    imageUrl: 'https://via.placeholder.com/300x200?text=Lens'
  }
]

// 模拟分类数据
const mockCategories = [
  { id: 1, name: '电子设备' },
  { id: 2, name: '电脑数码' },
  { id: 3, name: '摄影器材' }
]

// 筛选后的产品
const filteredProducts = computed(() => {
  let result = [...products.value]

  // 按分类筛选
  if (selectedCategory.value) {
    result = result.filter(product => product.categoryId === parseInt(selectedCategory.value))
  }

  // 按关键词搜索
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase()
    result = result.filter(product =>
      product.name.toLowerCase().includes(keyword) ||
      product.description.toLowerCase().includes(keyword)
    )
  }

  // 排序
  if (sortBy.value) {
    switch (sortBy.value) {
      case 'price_asc':
        result.sort((a, b) => (a.price || 0) - (b.price || 0))
        break
      case 'price_desc':
        result.sort((a, b) => (b.price || 0) - (a.price || 0))
        break
      case 'name':
        result.sort((a, b) => a.name.localeCompare(b.name))
        break
    }
  }

  return result
})

// 分页后的产品
const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  const end = start + pageSize
  return filteredProducts.value.slice(start, end)
})

// 总页数
const totalPages = computed(() => {
  return Math.ceil(filteredProducts.value.length / pageSize)
})

// 获取分类名称
const getCategoryName = (categoryId: number) => {
  const category = categories.value.find(cat => cat.id === categoryId)
  return category ? category.name : '未分类'
}

// 筛选产品
const filterProducts = () => {
  currentPage.value = 1
}

// 排序产品
const sortProducts = () => {
  currentPage.value = 1
}

// 搜索产品
const searchProducts = () => {
  currentPage.value = 1
}

// 查看产品详情
const viewProductDetail = (product) => {
  // 这里可以跳转到产品详情页，暂时只是控制台输出
  console.log('查看产品详情:', product.name)
  alert(`产品详情：${product.name}\n价格：¥${product.price}\n描述：${product.description}`)
}

// 上一页
const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
    scrollToTop()
  }
}

// 下一页
const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
    scrollToTop()
  }
}

// 滚动到顶部
const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// 加载数据
const loadData = async () => {
  loading.value = true
  try {
    // 这里应该调用实际的API
    // const response = await getProducts()
    // products.value = response.data || []

    // 暂时使用模拟数据
    await new Promise(resolve => setTimeout(resolve, 500)) // 模拟加载延迟
    products.value = mockProducts
    categories.value = mockCategories
  } catch (error) {
    console.error('加载产品数据失败:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.products-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* 页面头部 */
.page-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 80px 0;
  text-align: center;
}

.page-header h1 {
  font-size: 48px;
  margin: 0 0 15px 0;
}

.page-header p {
  font-size: 20px;
  margin: 0;
  opacity: 0.9;
}

/* 筛选栏 */
.products-filters {
  background: white;
  padding: 30px 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.filter-bar {
  display: flex;
  gap: 30px;
  align-items: center;
  flex-wrap: wrap;
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 10px;
}

.filter-item label {
  font-weight: 500;
  color: #333;
}

.filter-item select,
.filter-item input {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.filter-item input {
  width: 200px;
}

/* 产品列表 */
.products-list {
  padding: 50px 0;
}

.loading,
.empty-state {
  text-align: center;
  padding: 100px 0;
  color: #666;
  font-size: 18px;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 30px;
  margin-bottom: 50px;
}

.product-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  cursor: pointer;
}

.product-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15);
}

.product-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.product-card:hover .product-image img {
  transform: scale(1.05);
}

.product-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.product-card:hover .product-overlay {
  opacity: 1;
}

.view-btn {
  background: #409eff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: background 0.3s ease;
}

.view-btn:hover {
  background: #337ecc;
}

.product-info {
  padding: 20px;
}

.product-info h3 {
  margin: 0 0 10px 0;
  font-size: 18px;
  color: #333;
  line-height: 1.4;
}

.product-description {
  color: #666;
  font-size: 14px;
  line-height: 1.5;
  margin: 0 0 15px 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.product-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.product-category {
  background: #f0f2f5;
  color: #666;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.product-price {
  font-size: 20px;
  font-weight: bold;
  color: #409eff;
}

/* 分页 */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
}

.pagination-btn {
  background: #409eff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.3s ease;
}

.pagination-btn:hover:not(:disabled) {
  background: #337ecc;
}

.pagination-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.pagination-info {
  color: #666;
  font-size: 14px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-header h1 {
    font-size: 32px;
  }

  .page-header p {
    font-size: 16px;
  }

  .filter-bar {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
  }

  .filter-item input {
    width: 150px;
  }

  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 20px;
  }

  .pagination {
    flex-direction: column;
    gap: 15px;
  }
}
</style>