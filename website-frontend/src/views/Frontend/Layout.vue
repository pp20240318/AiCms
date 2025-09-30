<template>
  <div class="frontend-layout">
    <!-- 顶部导航 -->
    <header class="main-header">
      <div class="container">
        <div class="header-content">
          <div class="logo">
            <h1>{{ siteConfig.siteName || 'My CMS Website' }}</h1>
          </div>
          <nav class="main-nav">
            <ul>
              <li><router-link to="/home">首页</router-link></li>
              <li><router-link to="/products">产品</router-link></li>
              <li><router-link to="/articles">新闻</router-link></li>
              <li><router-link to="/about">关于我们</router-link></li>
              <li><router-link to="/contact">联系我们</router-link></li>
            </ul>
          </nav>
          <div class="header-actions">
            <router-link to="/admin" class="login-btn">管理后台</router-link>
          </div>
        </div>
      </div>
    </header>

    <!-- 主内容区 -->
    <main class="main-content">
      <router-view />
    </main>

    <!-- 底部 -->
    <footer class="main-footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-section">
            <h4>{{ siteConfig.siteName || 'My CMS Website' }}</h4>
            <p>{{ siteConfig.siteDescription || 'A modern CMS website' }}</p>
          </div>
          <div class="footer-section">
            <h4>联系信息</h4>
            <p v-if="siteConfig.contactEmail">邮箱: {{ siteConfig.contactEmail }}</p>
            <p v-if="siteConfig.contactPhone">电话: {{ siteConfig.contactPhone }}</p>
            <p v-if="siteConfig.contactAddress">地址: {{ siteConfig.contactAddress }}</p>
          </div>
          <div class="footer-section">
            <h4>快速链接</h4>
            <ul>
              <li><router-link to="/products">产品中心</router-link></li>
              <li><router-link to="/articles">新闻资讯</router-link></li>
              <li><router-link to="/about">关于我们</router-link></li>
              <li><router-link to="/contact">联系我们</router-link></li>
            </ul>
          </div>
          <div class="footer-section">
            <h4>关注我们</h4>
            <div class="social-links">
              <a v-if="siteConfig.facebookUrl" :href="siteConfig.facebookUrl" target="_blank">Facebook</a>
              <a v-if="siteConfig.twitterUrl" :href="siteConfig.twitterUrl" target="_blank">Twitter</a>
              <a v-if="siteConfig.linkedinUrl" :href="siteConfig.linkedinUrl" target="_blank">LinkedIn</a>
            </div>
          </div>
        </div>
        <div class="footer-bottom">
          <p>&copy; {{ currentYear }} {{ siteConfig.siteName || 'My CMS Website' }}. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { getPublicConfigs } from '@/api/website'

const siteConfig = reactive({
  siteName: '',
  siteDescription: '',
  contactEmail: '',
  contactPhone: '',
  contactAddress: '',
  facebookUrl: '',
  twitterUrl: '',
  linkedinUrl: ''
})

const currentYear = computed(() => new Date().getFullYear())

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
        case 'facebook_url':
          siteConfig.facebookUrl = config.value || ''
          break
        case 'twitter_url':
          siteConfig.twitterUrl = config.value || ''
          break
        case 'linkedin_url':
          siteConfig.linkedinUrl = config.value || ''
          break
      }
    })
  } catch (error) {
    console.error('加载网站配置失败:', error)
  }
}

onMounted(() => {
  loadSiteConfig()
})
</script>

<style scoped>
.frontend-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* 头部样式 */
.main-header {
  background: #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 100;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 70px;
}

.logo h1 {
  margin: 0;
  color: #333;
  font-size: 28px;
}

.main-nav ul {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 30px;
}

.main-nav a {
  text-decoration: none;
  color: #333;
  font-weight: 500;
  transition: color 0.3s;
}

.main-nav a:hover,
.main-nav a.router-link-active {
  color: #409eff;
}

.login-btn {
  background: #409eff;
  color: white;
  padding: 8px 16px;
  border-radius: 4px;
  text-decoration: none;
  font-size: 14px;
  transition: background 0.3s;
}

.login-btn:hover {
  background: #337ecc;
}

/* 主内容区 */
.main-content {
  flex: 1;
  background: #f5f7fa;
}

/* 底部样式 */
.main-footer {
  background: #333;
  color: white;
  padding: 40px 0 20px;
  margin-top: auto;
}

.footer-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 30px;
  margin-bottom: 30px;
}

.footer-section h4 {
  margin: 0 0 15px 0;
  color: #409eff;
}

.footer-section p {
  margin: 5px 0;
  color: #ccc;
  line-height: 1.6;
}

.footer-section ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.footer-section ul li {
  margin: 8px 0;
}

.footer-section ul li a {
  color: #ccc;
  text-decoration: none;
  transition: color 0.3s;
}

.footer-section ul li a:hover {
  color: #409eff;
}

.social-links {
  display: flex;
  gap: 15px;
}

.social-links a {
  color: #ccc;
  text-decoration: none;
  transition: color 0.3s;
}

.social-links a:hover {
  color: #409eff;
}

.footer-bottom {
  border-top: 1px solid #555;
  padding-top: 20px;
  text-align: center;
  color: #999;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    height: auto;
    padding: 15px 0;
  }

  .main-nav ul {
    gap: 15px;
    margin: 15px 0;
  }

  .footer-content {
    grid-template-columns: 1fr;
    text-align: center;
  }
}
</style>