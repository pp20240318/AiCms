<template>
  <div class="contact-page">
    <!-- 页面头部 -->
    <section class="page-header">
      <div class="container">
        <h1>联系我们</h1>
        <p>随时欢迎您的咨询和建议</p>
      </div>
    </section>

    <!-- 联系方式和表单 -->
    <section class="contact-content">
      <div class="container">
        <div class="contact-grid">
          <!-- 联系信息 -->
          <div class="contact-info">
            <h2>联系方式</h2>
            <p class="contact-description">
              我们重视与您的每一次沟通，无论您有任何问题、建议或合作意向，
              请通过以下方式与我们联系，我们将尽快为您回复。
            </p>

            <div class="contact-methods">
              <div class="contact-method" v-if="siteConfig.contactPhone">
                <div class="method-icon">
                  <i class="icon">📞</i>
                </div>
                <div class="method-info">
                  <h4>联系电话</h4>
                  <p>{{ siteConfig.contactPhone }}</p>
                  <span class="method-note">工作日 9:00-18:00</span>
                </div>
              </div>

              <div class="contact-method" v-if="siteConfig.contactEmail">
                <div class="method-icon">
                  <i class="icon">✉️</i>
                </div>
                <div class="method-info">
                  <h4>邮箱地址</h4>
                  <p>{{ siteConfig.contactEmail }}</p>
                  <span class="method-note">24小时内回复</span>
                </div>
              </div>

              <div class="contact-method" v-if="siteConfig.contactAddress">
                <div class="method-icon">
                  <i class="icon">📍</i>
                </div>
                <div class="method-info">
                  <h4>公司地址</h4>
                  <p>{{ siteConfig.contactAddress }}</p>
                  <span class="method-note">欢迎预约到访</span>
                </div>
              </div>

              <div class="contact-method" v-if="siteConfig.workingHours">
                <div class="method-icon">
                  <i class="icon">🕐</i>
                </div>
                <div class="method-info">
                  <h4>工作时间</h4>
                  <p>{{ siteConfig.workingHours }}</p>
                  <span class="method-note">节假日除外</span>
                </div>
              </div>
            </div>

            <!-- 社交媒体 -->
            <div class="social-media" v-if="hasSocialLinks">
              <h4>关注我们</h4>
              <div class="social-links">
                <a v-if="siteConfig.facebookUrl" :href="siteConfig.facebookUrl" target="_blank" class="social-link">
                  <i class="icon">📘</i>
                  Facebook
                </a>
                <a v-if="siteConfig.twitterUrl" :href="siteConfig.twitterUrl" target="_blank" class="social-link">
                  <i class="icon">🐦</i>
                  Twitter
                </a>
                <a v-if="siteConfig.linkedinUrl" :href="siteConfig.linkedinUrl" target="_blank" class="social-link">
                  <i class="icon">💼</i>
                  LinkedIn
                </a>
              </div>
            </div>
          </div>

          <!-- 联系表单 -->
          <div class="contact-form-container">
            <h2>发送消息</h2>
            <p class="form-description">
              请填写以下表单，我们会在24小时内回复您的消息。
            </p>

            <form class="contact-form" @submit.prevent="submitForm">
              <div class="form-group">
                <label for="name">姓名 *</label>
                <input
                  type="text"
                  id="name"
                  v-model="formData.name"
                  :class="{ error: errors.name }"
                  @blur="validateField('name')"
                  @input="clearError('name')"
                  placeholder="请输入您的姓名"
                  required
                />
                <span v-if="errors.name" class="error-message">{{ errors.name }}</span>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="email">邮箱 *</label>
                  <input
                    type="email"
                    id="email"
                    v-model="formData.email"
                    :class="{ error: errors.email }"
                    @blur="validateField('email')"
                    @input="clearError('email')"
                    placeholder="your@email.com"
                    required
                  />
                  <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
                </div>

                <div class="form-group">
                  <label for="phone">电话</label>
                  <input
                    type="tel"
                    id="phone"
                    v-model="formData.phone"
                    placeholder="请输入您的电话号码"
                  />
                </div>
              </div>

              <div class="form-group">
                <label for="company">公司名称</label>
                <input
                  type="text"
                  id="company"
                  v-model="formData.company"
                  placeholder="请输入您的公司名称（选填）"
                />
              </div>

              <div class="form-group">
                <label for="subject">主题 *</label>
                <input
                  type="text"
                  id="subject"
                  v-model="formData.subject"
                  :class="{ error: errors.subject }"
                  @blur="validateField('subject')"
                  @input="clearError('subject')"
                  placeholder="请输入消息主题"
                  required
                />
                <span v-if="errors.subject" class="error-message">{{ errors.subject }}</span>
              </div>

              <div class="form-group">
                <label for="message">消息内容 *</label>
                <textarea
                  id="message"
                  v-model="formData.message"
                  :class="{ error: errors.message }"
                  @blur="validateField('message')"
                  @input="clearError('message')"
                  placeholder="请详细描述您的需求或问题..."
                  rows="6"
                  required
                ></textarea>
                <span v-if="errors.message" class="error-message">{{ errors.message }}</span>
              </div>

              <button
                type="submit"
                class="submit-button"
                :disabled="submitting"
                :class="{ loading: submitting }"
              >
                <span v-if="submitting">发送中...</span>
                <span v-else>发送消息</span>
              </button>
            </form>

            <!-- 成功提示 -->
            <div v-if="showSuccess" class="success-message">
              <i class="success-icon">✅</i>
              <h3>消息发送成功！</h3>
              <p>感谢您的留言，我们会在24小时内回复您。</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 地图区域 -->
    <section class="map-section" v-if="siteConfig.contactAddress">
      <div class="container">
        <h2>找到我们</h2>
        <div class="map-container">
          <div class="map-placeholder">
            <p>地图区域</p>
            <span>{{ siteConfig.contactAddress }}</span>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { getPublicConfigs } from '@/api/website'
import { submitContact, type CreateContactDto } from '@/api/contact'

const submitting = ref(false)
const showSuccess = ref(false)

const siteConfig = reactive({
  contactPhone: '',
  contactEmail: '',
  contactAddress: '',
  workingHours: '',
  facebookUrl: '',
  twitterUrl: '',
  linkedinUrl: ''
})

const formData = reactive<CreateContactDto>({
  name: '',
  email: '',
  phone: '',
  company: '',
  subject: '',
  message: ''
})

const errors = reactive({
  name: '',
  email: '',
  subject: '',
  message: ''
})

// 是否有社交媒体链接
const hasSocialLinks = computed(() => {
  return siteConfig.facebookUrl || siteConfig.twitterUrl || siteConfig.linkedinUrl
})

// 验证单个字段
const validateField = (field: string) => {
  switch (field) {
    case 'name':
      if (!formData.name.trim()) {
        errors.name = '姓名不能为空'
      } else if (formData.name.length < 2) {
        errors.name = '姓名至少需要2个字符'
      } else {
        errors.name = ''
      }
      break

    case 'email':
      const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      if (!formData.email.trim()) {
        errors.email = '邮箱不能为空'
      } else if (!emailPattern.test(formData.email)) {
        errors.email = '请输入有效的邮箱地址'
      } else {
        errors.email = ''
      }
      break

    case 'subject':
      if (!formData.subject.trim()) {
        errors.subject = '主题不能为空'
      } else if (formData.subject.length < 5) {
        errors.subject = '主题至少需要5个字符'
      } else {
        errors.subject = ''
      }
      break

    case 'message':
      if (!formData.message.trim()) {
        errors.message = '消息内容不能为空'
      } else if (formData.message.length < 10) {
        errors.message = '消息内容至少需要10个字符'
      } else {
        errors.message = ''
      }
      break
  }
}

// 清除错误
const clearError = (field: string) => {
  errors[field] = ''
}

// 验证整个表单
const validateForm = () => {
  validateField('name')
  validateField('email')
  validateField('subject')
  validateField('message')

  return !errors.name && !errors.email && !errors.subject && !errors.message
}

// 提交表单
const submitForm = async () => {
  if (!validateForm()) {
    return
  }

  submitting.value = true
  try {
    await submitContact(formData)

    // 重置表单
    Object.assign(formData, {
      name: '',
      email: '',
      phone: '',
      company: '',
      subject: '',
      message: ''
    })

    // 显示成功消息
    showSuccess.value = true

    // 3秒后隐藏成功消息
    setTimeout(() => {
      showSuccess.value = false
    }, 5000)
  } catch (error) {
    console.error('提交联系表单失败:', error)
    alert('消息发送失败，请稍后重试或直接联系我们。')
  } finally {
    submitting.value = false
  }
}

// 加载网站配置
const loadSiteConfig = async () => {
  try {
    const response = await getPublicConfigs()
    const configs = response.data || []

    configs.forEach(config => {
      switch (config.key) {
        case 'contact_phone':
          siteConfig.contactPhone = config.value || ''
          break
        case 'contact_email':
          siteConfig.contactEmail = config.value || ''
          break
        case 'contact_address':
          siteConfig.contactAddress = config.value || ''
          break
        case 'working_hours':
          siteConfig.workingHours = config.value || '周一至周五 9:00-18:00'
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

    // 设置默认值
    if (!siteConfig.contactPhone) siteConfig.contactPhone = '400-123-4567'
    if (!siteConfig.contactEmail) siteConfig.contactEmail = 'contact@example.com'
    if (!siteConfig.contactAddress) siteConfig.contactAddress = '北京市朝阳区xxx路xxx号'
  } catch (error) {
    console.error('加载网站配置失败:', error)
  }
}

onMounted(() => {
  loadSiteConfig()
})
</script>

<style scoped>
.contact-page {
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

/* 联系内容 */
.contact-content {
  padding: 80px 0;
  background: white;
}

.contact-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 60px;
  align-items: start;
}

/* 联系信息 */
.contact-info h2 {
  font-size: 32px;
  margin: 0 0 20px 0;
  color: #333;
}

.contact-description {
  color: #666;
  line-height: 1.6;
  margin: 0 0 40px 0;
}

.contact-methods {
  margin-bottom: 40px;
}

.contact-method {
  display: flex;
  gap: 20px;
  margin-bottom: 30px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 12px;
  transition: transform 0.3s ease;
}

.contact-method:hover {
  transform: translateX(5px);
}

.method-icon {
  flex-shrink: 0;
  width: 60px;
  height: 60px;
  background: #409eff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
}

.method-info h4 {
  margin: 0 0 8px 0;
  color: #333;
  font-size: 18px;
}

.method-info p {
  margin: 0 0 5px 0;
  color: #409eff;
  font-weight: 500;
  font-size: 16px;
}

.method-note {
  color: #999;
  font-size: 14px;
}

/* 社交媒体 */
.social-media {
  border-top: 1px solid #eee;
  padding-top: 30px;
}

.social-media h4 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 18px;
}

.social-links {
  display: flex;
  gap: 15px;
}

.social-link {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 15px;
  background: #f0f2f5;
  border-radius: 20px;
  text-decoration: none;
  color: #666;
  font-size: 14px;
  transition: all 0.3s ease;
}

.social-link:hover {
  background: #409eff;
  color: white;
}

/* 联系表单 */
.contact-form-container {
  background: #f8f9fa;
  padding: 40px;
  border-radius: 12px;
  position: relative;
}

.contact-form-container h2 {
  font-size: 32px;
  margin: 0 0 20px 0;
  color: #333;
}

.form-description {
  color: #666;
  line-height: 1.6;
  margin: 0 0 30px 0;
}

.contact-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  color: #333;
  font-weight: 500;
  margin-bottom: 8px;
  font-size: 14px;
}

.form-group input,
.form-group textarea {
  padding: 12px 16px;
  border: 2px solid #e1e5e9;
  border-radius: 8px;
  font-size: 16px;
  transition: all 0.3s ease;
  background: white;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #409eff;
  box-shadow: 0 0 0 3px rgba(64, 158, 255, 0.1);
}

.form-group input.error,
.form-group textarea.error {
  border-color: #f56c6c;
  box-shadow: 0 0 0 3px rgba(245, 108, 108, 0.1);
}

.error-message {
  color: #f56c6c;
  font-size: 14px;
  margin-top: 5px;
}

.submit-button {
  background: #409eff;
  color: white;
  border: none;
  padding: 15px 30px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 10px;
}

.submit-button:hover:not(:disabled) {
  background: #337ecc;
  transform: translateY(-2px);
}

.submit-button:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
}

.submit-button.loading {
  position: relative;
}

/* 成功消息 */
.success-message {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.95);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  border-radius: 12px;
}

.success-icon {
  font-size: 48px;
  margin-bottom: 20px;
}

.success-message h3 {
  color: #67c23a;
  margin: 0 0 10px 0;
  font-size: 24px;
}

.success-message p {
  color: #666;
  margin: 0;
  line-height: 1.6;
}

/* 地图区域 */
.map-section {
  padding: 80px 0;
  background: white;
}

.map-section h2 {
  text-align: center;
  font-size: 32px;
  margin: 0 0 50px 0;
  color: #333;
}

.map-container {
  height: 400px;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.map-placeholder {
  height: 100%;
  background: #f0f2f5;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #666;
}

.map-placeholder p {
  font-size: 20px;
  margin: 0 0 10px 0;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-header h1 {
    font-size: 32px;
  }

  .page-header p {
    font-size: 16px;
  }

  .contact-grid {
    grid-template-columns: 1fr;
    gap: 40px;
  }

  .contact-form-container {
    padding: 30px 20px;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .contact-method {
    flex-direction: column;
    text-align: center;
  }

  .method-icon {
    align-self: center;
  }

  .social-links {
    flex-direction: column;
  }
}
</style>