<template>
  <div class="article-edit">
    <div class="page-header">
      <h1>编辑文章</h1>
      <div class="header-actions">
        <el-button @click="$router.back()">返回</el-button>
        <el-button type="primary" @click="updateArticle" :loading="saving">
          更新文章
        </el-button>
      </div>
    </div>

    <div v-if="loading" class="loading-container">
      <el-skeleton :rows="10" animated />
    </div>

    <el-form
      v-else-if="!loading && articleForm.title"
      ref="formRef"
      :model="articleForm"
      :rules="formRules"
      label-width="100px"
      class="article-form"
    >
      <el-card>
        <el-row :gutter="20">
          <el-col :span="16">
            <el-form-item label="文章标题" prop="title">
              <el-input
                v-model="articleForm.title"
                placeholder="请输入文章标题"
                maxlength="100"
                show-word-limit
              />
            </el-form-item>

            <el-form-item label="文章摘要" prop="excerpt">
              <el-input
                v-model="articleForm.excerpt"
                type="textarea"
                :rows="3"
                placeholder="请输入文章摘要（可选）"
                maxlength="200"
                show-word-limit
              />
            </el-form-item>

            <el-form-item label="文章内容" prop="content">
              <el-input
                v-model="articleForm.content"
                type="textarea"
                :rows="15"
                placeholder="请输入文章内容"
              />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="文章分类" prop="categoryId">
              <el-select
                v-model="articleForm.categoryId"
                placeholder="请选择文章分类"
                style="width: 100%"
              >
                <el-option
                  v-for="category in categories"
                  :key="category.id"
                  :label="category.name"
                  :value="category.id"
                />
              </el-select>
            </el-form-item>

            <el-form-item label="发布状态" prop="status">
              <el-radio-group v-model="articleForm.status">
                <el-radio value="draft">草稿</el-radio>
                <el-radio value="published">发布</el-radio>
                <el-radio value="archived">归档</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item label="特色图片" prop="featuredImage">
              <el-input
                v-model="articleForm.featuredImage"
                placeholder="图片URL（可选）"
              />
            </el-form-item>

            <el-form-item label="标签" prop="tags">
              <el-input
                v-model="articleForm.tags"
                placeholder="用逗号分隔多个标签"
              />
            </el-form-item>

            <el-form-item label="SEO标题" prop="metaTitle">
              <el-input
                v-model="articleForm.metaTitle"
                placeholder="SEO标题（可选）"
                maxlength="60"
                show-word-limit
              />
            </el-form-item>

            <el-form-item label="SEO描述" prop="metaDescription">
              <el-input
                v-model="articleForm.metaDescription"
                type="textarea"
                :rows="3"
                placeholder="SEO描述（可选）"
                maxlength="160"
                show-word-limit
              />
            </el-form-item>

            <el-divider />

            <el-form-item label="创建时间">
              <el-text>{{ formatDate(articleForm.createdAt) }}</el-text>
            </el-form-item>

            <el-form-item label="更新时间">
              <el-text>{{ formatDate(articleForm.updatedAt) }}</el-text>
            </el-form-item>

            <el-form-item label="浏览量">
              <el-text>{{ articleForm.viewCount || 0 }}</el-text>
            </el-form-item>
          </el-col>
        </el-row>
      </el-card>
    </el-form>

    <el-result
      v-else-if="!loading && !articleForm.title"
      icon="warning"
      title="文章不存在"
      sub-title="请检查文章ID是否正确"
    >
      <template #extra>
        <el-button type="primary" @click="$router.push('/admin/articles')">返回文章列表</el-button>
      </template>
    </el-result>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { getArticle, updateArticle as updateArticleApi, getCategories } from '@/api/articles'
import type { UpdateArticleDto, ArticleCategory } from '@/api/articles'
import dayjs from 'dayjs'

const router = useRouter()
const route = useRoute()
const formRef = ref<FormInstance>()
const loading = ref(true)
const saving = ref(false)
const categories = ref<ArticleCategory[]>([])

// 表单数据
const articleForm = reactive<UpdateArticleDto & {
  createdAt?: string
  updatedAt?: string
  viewCount?: number
}>({
  title: '',
  content: '',
  excerpt: '',
  categoryId: 0,
  status: 'draft',
  featuredImage: '',
  tags: '',
  metaTitle: '',
  metaDescription: ''
})

// 表单验证规则
const formRules: FormRules = {
  title: [
    { required: true, message: '请输入文章标题', trigger: 'blur' },
    { min: 1, max: 100, message: '标题长度在 1 到 100 个字符', trigger: 'blur' }
  ],
  content: [
    { required: true, message: '请输入文章内容', trigger: 'blur' }
  ],
  categoryId: [
    { required: true, message: '请选择文章分类', trigger: 'change' }
  ]
}

// 格式化日期
const formatDate = (dateStr?: string) => {
  if (!dateStr) return '-'
  return dayjs(dateStr).format('YYYY-MM-DD HH:mm:ss')
}

// 加载分类列表
const loadCategories = async () => {
  try {
    categories.value = await getCategories()
  } catch (error) {
    console.error('加载分类失败:', error)
    ElMessage.error('加载分类列表失败')
  }
}

// 加载文章数据
const loadArticle = async () => {
  const articleId = Number(route.params.id)
  if (!articleId) {
    ElMessage.error('无效的文章ID')
    router.push('/admin/articles')
    return
  }

  try {
    loading.value = true
    const article = await getArticle(articleId)

    // 填充表单数据
    Object.assign(articleForm, {
      title: article.title,
      content: article.content,
      excerpt: article.excerpt || '',
      categoryId: article.categoryId,
      status: article.status,
      featuredImage: article.featuredImage || '',
      tags: article.tags || '',
      metaTitle: article.metaTitle || '',
      metaDescription: article.metaDescription || '',
      createdAt: article.createdAt,
      updatedAt: article.updatedAt,
      viewCount: article.viewCount
    })
  } catch (error) {
    console.error('加载文章失败:', error)
    ElMessage.error('加载文章失败')
  } finally {
    loading.value = false
  }
}

// 更新文章
const updateArticle = async () => {
  if (!formRef.value) return

  const articleId = Number(route.params.id)
  if (!articleId) return

  try {
    await formRef.value.validate()
    saving.value = true

    const updateData: UpdateArticleDto = {
      title: articleForm.title,
      content: articleForm.content,
      excerpt: articleForm.excerpt,
      categoryId: articleForm.categoryId,
      status: articleForm.status,
      featuredImage: articleForm.featuredImage,
      tags: articleForm.tags,
      metaTitle: articleForm.metaTitle,
      metaDescription: articleForm.metaDescription
    }

    await updateArticleApi(articleId, updateData)
    ElMessage.success('文章更新成功')
    router.push('/admin/articles')
  } catch (error) {
    console.error('更新文章失败:', error)
    ElMessage.error('更新文章失败')
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  await Promise.all([
    loadCategories(),
    loadArticle()
  ])
})
</script>

<style scoped>
.article-edit {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  color: #303133;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.article-form {
  max-width: 1200px;
}

.loading-container {
  padding: 20px;
}

:deep(.el-textarea__inner) {
  font-family: 'Monaco', 'Consolas', monospace;
}

.el-divider {
  margin: 20px 0;
}
</style>