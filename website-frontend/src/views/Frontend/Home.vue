<template>
  <div class="home-page">
    <!-- 轮播图区域 -->
    <section class="hero-section">
      <div class="hero-slider" v-if="banners.length > 0">
        <el-carousel height="500px" :interval="5000">
          <el-carousel-item v-for="banner in banners" :key="banner.id">
            <div class="banner-slide" :style="{ backgroundImage: `url(${banner.imageUrl})` }">
              <div class="banner-content">
                <div class="container">
                  <h2>{{ banner.title }}</h2>
                  <p v-if="banner.description">{{ banner.description }}</p>
                  <el-button
                    v-if="banner.linkUrl"
                    type="primary"
                    size="large"
                    @click="goToLink(banner.linkUrl)"
                  >
                    了解更多
                  </el-button>
                </div>
              </div>
            </div>
          </el-carousel-item>
        </el-carousel>
      </div>
      <div v-else class="hero-placeholder">
        <div class="container">
          <h2>欢迎来到{{ siteConfig.siteName || 'My CMS Website' }}</h2>
          <p>{{ siteConfig.siteDescription || 'A modern CMS website' }}</p>
          <el-button type="primary" size="large" @click="$router.push('/products')">
            查看产品
          </el-button>
        </div>
      </div>
    </section>

    <!-- 产品展示区域 -->
    <section class="products-section">
      <div class="container">
        <div class="section-header">
          <h2>我们的产品</h2>
          <p>精选优质产品，为您提供最佳解决方案</p>
        </div>
        <div class="products-grid" v-if="products.length > 0">
          <div class="product-card" v-for="product in featuredProducts" :key="product.id">
            <div class="product-image">
              <img :src="product.imageUrl || 'https://via.placeholder.com/300x200?text=Product'" :alt="product.name" />
            </div>
            <div class="product-info">
              <h3>{{ product.name }}</h3>
              <p>{{ product.description }}</p>
              <div class="product-price" v-if="product.price">
                ¥{{ product.price }}
              </div>
              <el-button @click="viewProduct(product.id)">查看详情</el-button>
            </div>
          </div>
        </div>
        <div class="section-actions">
          <el-button type="primary" @click="$router.push('/products')">
            查看全部产品
          </el-button>
        </div>
      </div>
    </section>

    <!-- 新闻资讯区域 -->
    <section class="news-section">
      <div class="container">
        <div class="section-header">
          <h2>新闻资讯</h2>
          <p>了解最新动态和行业资讯</p>
        </div>
        <div class="news-grid" v-if="articles.length > 0">
          <div class="news-card" v-for="article in featuredArticles" :key="article.id">
            <div class="news-image">
              <img :src="article.imageUrl || 'https://via.placeholder.com/350x200?text=News'" :alt="article.title" />
            </div>
            <div class="news-info">
              <div class="news-meta">
                <span class="news-date">{{ formatDate(article.publishedAt) }}</span>
                <span class="news-category">{{ article.category?.name }}</span>
              </div>
              <h3>{{ article.title }}</h3>
              <p>{{ article.summary }}</p>
              <el-button text @click="viewArticle(article.id)">阅读更多</el-button>
            </div>
          </div>
        </div>
        <div class="section-actions">
          <el-button type="primary" @click="$router.push('/articles')">
            查看全部资讯
          </el-button>
        </div>
      </div>
    </section>

    <!-- 关于我们区域 -->
    <section class="about-section">
      <div class="container">
        <div class="about-content">
          <div class="about-text">
            <h2>关于我们</h2>
            <p>
              我们致力于为客户提供优质的产品和服务，
              通过不断创新和技术进步，成为行业领先者。
              我们的团队拥有丰富的经验和专业知识，
              能够为您提供最适合的解决方案。
            </p>
            <ul class="features-list">
              <li>专业的技术团队</li>
              <li>优质的产品质量</li>
              <li>完善的售后服务</li>
              <li>持续的创新能力</li>
            </ul>
            <el-button type="primary" @click="$router.push('/about')">
              了解更多
            </el-button>
          </div>
          <div class="about-image">
            <img src="https://via.placeholder.com/500x400?text=About+Us" alt="关于我们" />
          </div>
        </div>
      </div>
    </section>

    <!-- 联系我们区域 -->
    <section class="contact-section">
      <div class="container">
        <div class="contact-content">
          <div class="contact-info">
            <h2>联系我们</h2>
            <p>有任何问题或需求，请随时与我们联系</p>
            <div class="contact-items">
              <div class="contact-item" v-if="siteConfig.contactEmail">
                <el-icon><Message /></el-icon>
                <span>{{ siteConfig.contactEmail }}</span>
              </div>
              <div class="contact-item" v-if="siteConfig.contactPhone">
                <el-icon><Phone /></el-icon>
                <span>{{ siteConfig.contactPhone }}</span>
              </div>
              <div class="contact-item" v-if="siteConfig.contactAddress">
                <el-icon><Location /></el-icon>
                <span>{{ siteConfig.contactAddress }}</span>
              </div>
            </div>
          </div>
          <div class="contact-form">
            <el-form
              ref="contactFormRef"
              :model="contactForm"
              :rules="contactFormRules"
              label-position="top"
            >
              <el-row :gutter="20">
                <el-col :span="12">
                  <el-form-item label="姓名" prop="name">
                    <el-input v-model="contactForm.name" placeholder="您的姓名" />
                  </el-form-item>
                </el-col>
                <el-col :span="12">
                  <el-form-item label="邮箱" prop="email">
                    <el-input v-model="contactForm.email" placeholder="您的邮箱" />
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row :gutter="20">
                <el-col :span="12">
                  <el-form-item label="电话" prop="phone">
                    <el-input v-model="contactForm.phone" placeholder="您的电话" />
                  </el-form-item>
                </el-col>
                <el-col :span="12">
                  <el-form-item label="公司" prop="company">
                    <el-input v-model="contactForm.company" placeholder="您的公司" />
                  </el-form-item>
                </el-col>
              </el-row>
              <el-form-item label="主题" prop="subject">
                <el-input v-model="contactForm.subject" placeholder="消息主题" />
              </el-form-item>
              <el-form-item label="消息内容" prop="message">
                <el-input
                  type="textarea"
                  v-model="contactForm.message"
                  :rows="4"
                  placeholder="请输入您的消息内容..."
                />
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="submitContact" :loading="submitting">
                  发送消息
                </el-button>
              </el-form-item>
            </el-form>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { Message, Phone, Location } from '@element-plus/icons-vue'
import { getPublicConfigs } from '@/api/website'
import { submitContact, type CreateContactDto } from '@/api/contact'
import { getArticles } from '@/api/articles'
import { getProducts } from '@/api/products'

const submitting = ref(false)
const contactFormRef = ref()

// 模拟数据 - 实际项目中应该从API获取
const banners = ref([])
const products = ref([])
const articles = ref([])

const siteConfig = reactive({
  siteName: '',
  siteDescription: '',
  contactEmail: '',
  contactPhone: '',
  contactAddress: ''
})

const contactForm = reactive<CreateContactDto>({
  name: '',
  email: '',
  phone: '',
  company: '',
  subject: '',
  message: ''
})

const contactFormRules = {
  name: [
    { required: true, message: '请输入姓名', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' }
  ],
  subject: [
    { required: true, message: '请输入主题', trigger: 'blur' }
  ],
  message: [
    { required: true, message: '请输入消息内容', trigger: 'blur' }
  ]
}

// 精选产品（前4个）
const featuredProducts = computed(() => products.value.slice(0, 4))

// 精选文章（前3个）
const featuredArticles = computed(() => articles.value.slice(0, 3))

// 加载网站配置
const loadSiteConfig = async () => {
  try {
    const response = await getPublicConfigs()
    const configs = response.data || []

    configs.forEach(config => {
      switch (config.key) {
        case 'site_name':
          siteConfig.siteName = config.value || ''
          break
        case 'site_description':
          siteConfig.siteDescription = config.value || ''
          break
        case 'contact_email':
          siteConfig.contactEmail = config.value || ''
          break
        case 'contact_phone':
          siteConfig.contactPhone = config.value || ''
          break
        case 'contact_address':
          siteConfig.contactAddress = config.value || ''
          break
      }
    })
  } catch (error) {
    console.error('加载网站配置失败:', error)
  }
}

// 加载产品数据
const loadProducts = async () => {
  try {
    // 这里应该调用产品API，暂时使用模拟数据
    products.value = []
  } catch (error) {
    console.error('加载产品失败:', error)
  }
}

// 加载文章数据
const loadArticles = async () => {
  try {
    // 这里应该调用文章API，暂时使用模拟数据
    articles.value = []
  } catch (error) {
    console.error('加载文章失败:', error)
  }
}

// 跳转到链接
const goToLink = (url: string) => {
  if (url.startsWith('http')) {
    window.open(url, '_blank')
  } else {
    window.location.href = url
  }
}

// 查看产品
const viewProduct = (id: number) => {
  // 跳转到产品详情页
  console.log('查看产品:', id)
}

// 查看文章
const viewArticle = (id: number) => {
  // 跳转到文章详情页
  console.log('查看文章:', id)
}

// 提交联系表单
const submitContact = async () => {
  if (!contactFormRef.value) return

  try {
    await contactFormRef.value.validate()
    submitting.value = true

    await submitContact(contactForm)
    ElMessage.success('消息发送成功，我们会尽快回复您！')

    // 重置表单
    Object.assign(contactForm, {
      name: '',
      email: '',
      phone: '',
      company: '',
      subject: '',
      message: ''
    })
    contactFormRef.value.resetFields()
  } catch (error) {
    ElMessage.error('消息发送失败，请稍后重试')
  } finally {
    submitting.value = false
  }
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

onMounted(() => {
  loadSiteConfig()
  loadProducts()
  loadArticles()
})
</script>

<style scoped>
.home-page {
  min-height: 100vh;
}

/* 英雄区域 */
.hero-section {
  position: relative;
}

.banner-slide {
  height: 500px;
  background-size: cover;
  background-position: center;
  position: relative;
}

.banner-content {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  color: white;
}

.banner-content h2 {
  font-size: 48px;
  margin: 0 0 20px 0;
}

.banner-content p {
  font-size: 20px;
  margin: 0 0 30px 0;
}

.hero-placeholder {
  height: 500px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  color: white;
  text-align: center;
}

.hero-placeholder h2 {
  font-size: 48px;
  margin: 0 0 20px 0;
}

.hero-placeholder p {
  font-size: 20px;
  margin: 0 0 30px 0;
}

/* 通用样式 */
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

section {
  padding: 80px 0;
}

.section-header {
  text-align: center;
  margin-bottom: 60px;
}

.section-header h2 {
  font-size: 36px;
  margin: 0 0 15px 0;
  color: #333;
}

.section-header p {
  font-size: 18px;
  color: #666;
  margin: 0;
}

.section-actions {
  text-align: center;
  margin-top: 40px;
}

/* 产品区域 */
.products-section {
  background: white;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 30px;
  margin-bottom: 40px;
}

.product-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: transform 0.3s;
}

.product-card:hover {
  transform: translateY(-5px);
}

.product-image {
  height: 200px;
  overflow: hidden;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.product-info {
  padding: 20px;
}

.product-info h3 {
  margin: 0 0 10px 0;
  color: #333;
}

.product-info p {
  color: #666;
  margin: 0 0 15px 0;
}

.product-price {
  font-size: 20px;
  font-weight: bold;
  color: #409eff;
  margin: 10px 0;
}

/* 新闻区域 */
.news-section {
  background: #f8f9fa;
}

.news-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 30px;
  margin-bottom: 40px;
}

.news-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: transform 0.3s;
}

.news-card:hover {
  transform: translateY(-5px);
}

.news-image {
  height: 200px;
  overflow: hidden;
}

.news-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.news-info {
  padding: 20px;
}

.news-meta {
  display: flex;
  gap: 15px;
  margin-bottom: 10px;
  font-size: 14px;
  color: #999;
}

.news-info h3 {
  margin: 0 0 10px 0;
  color: #333;
}

.news-info p {
  color: #666;
  margin: 0 0 15px 0;
}

/* 关于我们区域 */
.about-section {
  background: white;
}

.about-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 60px;
  align-items: center;
}

.about-text h2 {
  font-size: 36px;
  margin: 0 0 20px 0;
  color: #333;
}

.about-text p {
  font-size: 16px;
  line-height: 1.8;
  color: #666;
  margin: 0 0 25px 0;
}

.features-list {
  list-style: none;
  padding: 0;
  margin: 0 0 30px 0;
}

.features-list li {
  padding: 8px 0;
  position: relative;
  padding-left: 25px;
  color: #333;
}

.features-list li:before {
  content: '✓';
  position: absolute;
  left: 0;
  color: #409eff;
  font-weight: bold;
}

.about-image {
  text-align: center;
}

.about-image img {
  max-width: 100%;
  border-radius: 8px;
}

/* 联系我们区域 */
.contact-section {
  background: #f8f9fa;
}

.contact-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 60px;
}

.contact-info h2 {
  font-size: 36px;
  margin: 0 0 20px 0;
  color: #333;
}

.contact-info p {
  font-size: 16px;
  color: #666;
  margin: 0 0 30px 0;
}

.contact-items {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.contact-item {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #333;
}

.contact-form {
  background: white;
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .banner-content h2 {
    font-size: 32px;
  }

  .banner-content p {
    font-size: 16px;
  }

  .hero-placeholder h2 {
    font-size: 32px;
  }

  .hero-placeholder p {
    font-size: 16px;
  }

  .section-header h2 {
    font-size: 28px;
  }

  .about-content,
  .contact-content {
    grid-template-columns: 1fr;
    gap: 30px;
  }

  .products-grid,
  .news-grid {
    grid-template-columns: 1fr;
  }
}
</style>