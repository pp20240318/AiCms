<template>
  <div class="articles-page">
    <!-- 页面头部 -->
    <section class="page-header">
      <div class="container">
        <h1>新闻资讯</h1>
        <p>了解最新动态和行业资讯</p>
      </div>
    </section>

    <!-- 文章筛选 -->
    <section class="articles-filters">
      <div class="container">
        <div class="filter-bar">
          <div class="filter-item">
            <label>文章分类:</label>
            <select v-model="selectedCategory" @change="filterArticles">
              <option value="">全部分类</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>
          <div class="filter-item">
            <label>发布时间:</label>
            <select v-model="sortBy" @change="sortArticles">
              <option value="">默认排序</option>
              <option value="latest">最新发布</option>
              <option value="oldest">最早发布</option>
              <option value="title">按标题排序</option>
            </select>
          </div>
          <div class="filter-item">
            <input
              type="text"
              v-model="searchKeyword"
              placeholder="搜索文章..."
              @input="searchArticles"
            >
          </div>
        </div>
      </div>
    </section>

    <!-- 文章列表 -->
    <section class="articles-list">
      <div class="container">
        <div v-if="loading" class="loading">
          <p>加载中...</p>
        </div>
        <div v-else-if="filteredArticles.length === 0" class="empty-state">
          <p>暂无文章数据</p>
        </div>
        <div v-else class="articles-container">
          <!-- 文章网格 -->
          <div class="articles-grid">
            <article
              class="article-card"
              v-for="article in paginatedArticles"
              :key="article.id"
              @click="viewArticleDetail(article)"
            >
              <div class="article-image">
                <img
                  :src="article.imageUrl || 'https://via.placeholder.com/400x250?text=Article'"
                  :alt="article.title"
                />
                <div class="article-overlay">
                  <button class="read-btn">阅读全文</button>
                </div>
              </div>
              <div class="article-content">
                <div class="article-meta">
                  <span class="article-date">{{ formatDate(article.publishedAt) }}</span>
                  <span class="article-category">{{ getCategoryName(article.categoryId) }}</span>
                </div>
                <h3 class="article-title">{{ article.title }}</h3>
                <p class="article-summary">{{ article.summary }}</p>
                <div class="article-footer">
                  <span class="article-author">作者: {{ article.author }}</span>
                  <span class="article-views">阅读 {{ article.viewCount || 0 }}</span>
                </div>
              </div>
            </article>
          </div>

          <!-- 侧边栏 -->
          <aside class="sidebar">
            <!-- 热门文章 -->
            <div class="widget">
              <h3>热门文章</h3>
              <ul class="popular-articles">
                <li v-for="article in popularArticles" :key="article.id" @click="viewArticleDetail(article)">
                  <div class="popular-article-image">
                    <img
                      :src="article.imageUrl || 'https://via.placeholder.com/80x60?text=News'"
                      :alt="article.title"
                    />
                  </div>
                  <div class="popular-article-info">
                    <h4>{{ article.title }}</h4>
                    <span class="popular-article-date">{{ formatDate(article.publishedAt) }}</span>
                  </div>
                </li>
              </ul>
            </div>

            <!-- 文章分类 -->
            <div class="widget">
              <h3>文章分类</h3>
              <ul class="category-list">
                <li
                  v-for="category in categories"
                  :key="category.id"
                  @click="filterByCategory(category.id)"
                  :class="{ active: selectedCategory == category.id }"
                >
                  <span>{{ category.name }}</span>
                  <span class="category-count">({{ getCategoryArticleCount(category.id) }})</span>
                </li>
              </ul>
            </div>

            <!-- 标签云 -->
            <div class="widget">
              <h3>热门标签</h3>
              <div class="tags">
                <span
                  v-for="tag in popularTags"
                  :key="tag"
                  class="tag"
                  @click="searchByTag(tag)"
                >
                  {{ tag }}
                </span>
              </div>
            </div>
          </aside>
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
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)
const articles = ref([])
const categories = ref([])
const selectedCategory = ref('')
const sortBy = ref('')
const searchKeyword = ref('')
const currentPage = ref(1)
const pageSize = 9

// 模拟文章数据
const mockArticles = [
  {
    id: 1,
    title: '人工智能技术在现代企业中的应用',
    summary: '探讨人工智能技术如何帮助企业提高效率，降低成本，并创造新的商业机会。',
    content: '详细内容...',
    categoryId: 1,
    author: '张三',
    publishedAt: '2024-03-15T10:00:00Z',
    viewCount: 1250,
    imageUrl: 'https://via.placeholder.com/400x250?text=AI+Technology'
  },
  {
    id: 2,
    title: '区块链技术的发展趋势与前景',
    summary: '分析区块链技术的最新发展动态，以及在金融、供应链等领域的应用前景。',
    content: '详细内容...',
    categoryId: 1,
    author: '李四',
    publishedAt: '2024-03-10T14:30:00Z',
    viewCount: 890,
    imageUrl: 'https://via.placeholder.com/400x250?text=Blockchain'
  },
  {
    id: 3,
    title: '公司年度发展战略发布',
    summary: '公司正式发布2024年度发展战略，将重点投资新兴技术领域。',
    content: '详细内容...',
    categoryId: 2,
    author: '王五',
    publishedAt: '2024-03-08T09:00:00Z',
    viewCount: 2100,
    imageUrl: 'https://via.placeholder.com/400x250?text=Strategy'
  },
  {
    id: 4,
    title: '云计算服务的安全性考量',
    summary: '深入分析云计算服务中的安全挑战和解决方案，保护企业数据安全。',
    content: '详细内容...',
    categoryId: 1,
    author: '赵六',
    publishedAt: '2024-03-05T16:20:00Z',
    viewCount: 750,
    imageUrl: 'https://via.placeholder.com/400x250?text=Cloud+Security'
  },
  {
    id: 5,
    title: '新产品发布会圆满举行',
    summary: '公司新产品发布会成功举办，吸引了众多行业专家和媒体关注。',
    content: '详细内容...',
    categoryId: 2,
    author: '孙七',
    publishedAt: '2024-03-01T11:00:00Z',
    viewCount: 1680,
    imageUrl: 'https://via.placeholder.com/400x250?text=Product+Launch'
  },
  {
    id: 6,
    title: '5G技术对物联网发展的推动作用',
    summary: '分析5G技术如何加速物联网应用的发展，以及带来的新机遇。',
    content: '详细内容...',
    categoryId: 1,
    author: '周八',
    publishedAt: '2024-02-28T13:45:00Z',
    viewCount: 920,
    imageUrl: 'https://via.placeholder.com/400x250?text=5G+IoT'
  }
]

// 模拟分类数据
const mockCategories = [
  { id: 1, name: '技术资讯' },
  { id: 2, name: '公司新闻' },
  { id: 3, name: '行业动态' }
]

// 热门标签
const popularTags = ['人工智能', '区块链', '云计算', '5G', '物联网', '大数据', '移动开发']

// 筛选后的文章
const filteredArticles = computed(() => {
  let result = [...articles.value]

  // 按分类筛选
  if (selectedCategory.value) {
    result = result.filter(article => article.categoryId === parseInt(selectedCategory.value))
  }

  // 按关键词搜索
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase()
    result = result.filter(article =>
      article.title.toLowerCase().includes(keyword) ||
      article.summary.toLowerCase().includes(keyword)
    )
  }

  // 排序
  if (sortBy.value) {
    switch (sortBy.value) {
      case 'latest':
        result.sort((a, b) => new Date(b.publishedAt).getTime() - new Date(a.publishedAt).getTime())
        break
      case 'oldest':
        result.sort((a, b) => new Date(a.publishedAt).getTime() - new Date(b.publishedAt).getTime())
        break
      case 'title':
        result.sort((a, b) => a.title.localeCompare(b.title))
        break
    }
  }

  return result
})

// 分页后的文章
const paginatedArticles = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  const end = start + pageSize
  return filteredArticles.value.slice(start, end)
})

// 总页数
const totalPages = computed(() => {
  return Math.ceil(filteredArticles.value.length / pageSize)
})

// 热门文章（按浏览量排序的前5篇）
const popularArticles = computed(() => {
  return [...articles.value]
    .sort((a, b) => (b.viewCount || 0) - (a.viewCount || 0))
    .slice(0, 5)
})

// 获取分类名称
const getCategoryName = (categoryId: number) => {
  const category = categories.value.find(cat => cat.id === categoryId)
  return category ? category.name : '未分类'
}

// 获取分类文章数量
const getCategoryArticleCount = (categoryId: number) => {
  return articles.value.filter(article => article.categoryId === categoryId).length
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 筛选文章
const filterArticles = () => {
  currentPage.value = 1
}

// 排序文章
const sortArticles = () => {
  currentPage.value = 1
}

// 搜索文章
const searchArticles = () => {
  currentPage.value = 1
}

// 按分类筛选
const filterByCategory = (categoryId: number) => {
  selectedCategory.value = categoryId.toString()
  currentPage.value = 1
}

// 按标签搜索
const searchByTag = (tag: string) => {
  searchKeyword.value = tag
  currentPage.value = 1
}

// 查看文章详情
const viewArticleDetail = (article) => {
  console.log('查看文章详情:', article.title)
  alert(`文章详情：${article.title}\n作者：${article.author}\n摘要：${article.summary}`)
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
    // const response = await getArticles()
    // articles.value = response.data || []

    // 暂时使用模拟数据
    await new Promise(resolve => setTimeout(resolve, 500))
    articles.value = mockArticles
    categories.value = mockCategories
  } catch (error) {
    console.error('加载文章数据失败:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.articles-page {
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
.articles-filters {
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

/* 文章列表 */
.articles-list {
  padding: 50px 0;
}

.loading,
.empty-state {
  text-align: center;
  padding: 100px 0;
  color: #666;
  font-size: 18px;
}

.articles-container {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 40px;
  margin-bottom: 50px;
}

.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 30px;
}

.article-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  cursor: pointer;
}

.article-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15);
}

.article-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.article-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.article-card:hover .article-image img {
  transform: scale(1.05);
}

.article-overlay {
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

.article-card:hover .article-overlay {
  opacity: 1;
}

.read-btn {
  background: #409eff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 20px;
  font-size: 14px;
  cursor: pointer;
  transition: background 0.3s ease;
}

.read-btn:hover {
  background: #337ecc;
}

.article-content {
  padding: 20px;
}

.article-meta {
  display: flex;
  gap: 15px;
  margin-bottom: 10px;
  font-size: 12px;
  color: #999;
}

.article-category {
  background: #f0f2f5;
  padding: 2px 8px;
  border-radius: 4px;
}

.article-title {
  margin: 0 0 10px 0;
  font-size: 18px;
  color: #333;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-summary {
  color: #666;
  font-size: 14px;
  line-height: 1.5;
  margin: 0 0 15px 0;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 12px;
  color: #999;
}

/* 侧边栏 */
.sidebar {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.widget {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.widget h3 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 18px;
  border-bottom: 2px solid #409eff;
  padding-bottom: 10px;
}

/* 热门文章 */
.popular-articles {
  list-style: none;
  padding: 0;
  margin: 0;
}

.popular-articles li {
  display: flex;
  gap: 10px;
  padding: 10px 0;
  border-bottom: 1px solid #f0f2f5;
  cursor: pointer;
  transition: background 0.3s ease;
}

.popular-articles li:hover {
  background: #f8f9fa;
}

.popular-articles li:last-child {
  border-bottom: none;
}

.popular-article-image {
  flex-shrink: 0;
  width: 60px;
  height: 45px;
  border-radius: 4px;
  overflow: hidden;
}

.popular-article-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.popular-article-info h4 {
  margin: 0 0 5px 0;
  font-size: 14px;
  color: #333;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.popular-article-date {
  font-size: 12px;
  color: #999;
}

/* 分类列表 */
.category-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.category-list li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #f0f2f5;
  cursor: pointer;
  transition: color 0.3s ease;
}

.category-list li:hover,
.category-list li.active {
  color: #409eff;
}

.category-list li:last-child {
  border-bottom: none;
}

.category-count {
  font-size: 12px;
  color: #999;
}

/* 标签云 */
.tags {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.tag {
  background: #f0f2f5;
  color: #666;
  padding: 5px 12px;
  border-radius: 15px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.tag:hover {
  background: #409eff;
  color: white;
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

  .articles-container {
    grid-template-columns: 1fr;
    gap: 30px;
  }

  .articles-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }

  .pagination {
    flex-direction: column;
    gap: 15px;
  }
}
</style>