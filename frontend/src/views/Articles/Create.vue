<template>
  <div class="article-create">
    <div class="page-header">
      <h1>新增文章</h1>
      <div class="header-actions">
        <el-button @click="$router.back()">返回</el-button>
        <el-button type="primary" @click="saveArticle" :loading="saving">
          保存文章
        </el-button>
      </div>
    </div>

    <el-form
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
              <RichEditor
                v-model="articleForm.content"
                placeholder="请输入文章内容"
                height="500px"
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
          </el-col>
        </el-row>
      </el-card>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { createArticle, getCategories } from '@/api/articles'
import type { CreateArticleDto, ArticleCategory } from '@/api/articles'
import RichEditor from '@/components/RichEditor.vue'

const router = useRouter()
const formRef = ref<FormInstance>()
const saving = ref(false)
const categories = ref<ArticleCategory[]>([])

// 表单数据
const articleForm = reactive<CreateArticleDto>({
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

// 加载分类列表
const loadCategories = async () => {
  try {
    categories.value = await getCategories()
  } catch (error) {
    console.error('加载分类失败:', error)
    ElMessage.error('加载分类列表失败')
  }
}

// 保存文章
const saveArticle = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
    saving.value = true

    await createArticle(articleForm)
    ElMessage.success('文章创建成功')
    router.push('/admin/articles')
  } catch (error) {
    console.error('保存文章失败:', error)
    ElMessage.error('保存文章失败')
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadCategories()
})
</script>

<style scoped>
.article-create {
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

:deep(.el-textarea__inner) {
  font-family: 'Monaco', 'Consolas', monospace;
}
</style>