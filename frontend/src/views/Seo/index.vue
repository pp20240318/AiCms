<template>
  <div class="seo-management">
    <div class="page-header">
      <div class="header-left">
        <h1>SEO管理</h1>
        <p>管理网站各页面的SEO设置</p>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="showCreateDialog">
          <el-icon><Plus /></el-icon>
          新增SEO设置
        </el-button>
      </div>
    </div>

    <div class="content-card">
      <div class="filter-bar">
        <el-input
          v-model="searchTerm"
          placeholder="搜索页面路径或标题..."
          style="width: 300px"
          @input="handleSearch"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>
        <el-button @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新
        </el-button>
      </div>

      <el-table
        v-loading="loading"
        :data="seoList"
        stripe
        style="width: 100%"
      >
        <el-table-column prop="pagePath" label="页面路径" min-width="200">
          <template #default="{ row }">
            <el-tag>{{ row.pagePath }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="title" label="页面标题" min-width="200" />
        <el-table-column prop="description" label="页面描述" min-width="300" show-overflow-tooltip />
        <el-table-column prop="keywords" label="关键词" min-width="200" show-overflow-tooltip />
        <el-table-column prop="isEnabled" label="状态" width="100">
          <template #default="{ row }">
            <el-switch
              v-model="row.isEnabled"
              @change="toggleStatus(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">
              编辑
            </el-button>
            <el-button
              size="small"
              type="danger"
              @click="handleDelete(row)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 创建/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingItem ? '编辑SEO设置' : '新增SEO设置'"
      width="800px"
      @close="resetForm"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-form-item label="页面路径" prop="pagePath" v-if="!editingItem">
          <el-input
            v-model="formData.pagePath"
            placeholder="如: /about, /products"
          />
          <div class="form-tip">页面路径用于匹配前台页面URL</div>
        </el-form-item>

        <el-form-item label="页面标题" prop="title">
          <el-input
            v-model="formData.title"
            placeholder="页面标题，会显示在浏览器标签栏"
          />
        </el-form-item>

        <el-form-item label="页面描述" prop="description">
          <el-input
            type="textarea"
            v-model="formData.description"
            :rows="3"
            placeholder="页面描述，用于搜索引擎展示"
          />
        </el-form-item>

        <el-form-item label="关键词" prop="keywords">
          <el-input
            v-model="formData.keywords"
            placeholder="关键词，用英文逗号分隔"
          />
        </el-form-item>

        <el-form-item label="OG标题" prop="ogTitle">
          <el-input
            v-model="formData.ogTitle"
            placeholder="社交媒体分享时的标题"
          />
        </el-form-item>

        <el-form-item label="OG描述" prop="ogDescription">
          <el-input
            type="textarea"
            v-model="formData.ogDescription"
            :rows="2"
            placeholder="社交媒体分享时的描述"
          />
        </el-form-item>

        <el-form-item label="OG图片" prop="ogImage">
          <el-input
            v-model="formData.ogImage"
            placeholder="社交媒体分享时的图片URL"
          />
        </el-form-item>

        <el-form-item label="结构化数据" prop="structuredData">
          <el-input
            type="textarea"
            v-model="formData.structuredData"
            :rows="4"
            placeholder="JSON-LD格式的结构化数据"
          />
        </el-form-item>

        <el-form-item label="启用状态">
          <el-switch v-model="formData.isEnabled" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitting">
          确认
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, Refresh } from '@element-plus/icons-vue'
import {
  getSeoSettings,
  createSeoSetting,
  updateSeoSetting,
  deleteSeoSetting,
  type SeoSetting,
  type CreateSeoSettingDto,
  type UpdateSeoSettingDto
} from '@/api/seo'

const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const searchTerm = ref('')
const editingItem = ref<SeoSetting | null>(null)
const formRef = ref()

const seoList = ref<SeoSetting[]>([])

const formData = reactive<CreateSeoSettingDto>({
  pagePath: '',
  title: '',
  description: '',
  keywords: '',
  ogTitle: '',
  ogDescription: '',
  ogImage: '',
  structuredData: '',
  isEnabled: true
})

const formRules = {
  pagePath: [
    { required: true, message: '请输入页面路径', trigger: 'blur' }
  ],
  title: [
    { required: true, message: '请输入页面标题', trigger: 'blur' }
  ]
}

// 加载数据
const loadData = async () => {
  loading.value = true
  try {
    const response = await getSeoSettings()
    seoList.value = response.data || []
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  const term = searchTerm.value.toLowerCase()
  // 这里可以根据实际需求实现前端搜索或调用后端搜索接口
  // 暂时使用前端搜索
}

// 刷新数据
const refreshData = () => {
  loadData()
}

// 显示创建对话框
const showCreateDialog = () => {
  editingItem.value = null
  resetForm()
  dialogVisible.value = true
}

// 显示编辑对话框
const showEditDialog = (item: SeoSetting) => {
  editingItem.value = item
  Object.assign(formData, {
    title: item.title,
    description: item.description || '',
    keywords: item.keywords || '',
    ogTitle: item.ogTitle || '',
    ogDescription: item.ogDescription || '',
    ogImage: item.ogImage || '',
    structuredData: item.structuredData || '',
    isEnabled: item.isEnabled
  })
  dialogVisible.value = true
}

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    pagePath: '',
    title: '',
    description: '',
    keywords: '',
    ogTitle: '',
    ogDescription: '',
    ogImage: '',
    structuredData: '',
    isEnabled: true
  })
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
    submitting.value = true

    if (editingItem.value) {
      // 编辑
      const updateData: UpdateSeoSettingDto = {
        title: formData.title,
        description: formData.description,
        keywords: formData.keywords,
        ogTitle: formData.ogTitle,
        ogDescription: formData.ogDescription,
        ogImage: formData.ogImage,
        structuredData: formData.structuredData,
        isEnabled: formData.isEnabled
      }
      await updateSeoSetting(editingItem.value.id, updateData)
      ElMessage.success('更新成功')
    } else {
      // 创建
      await createSeoSetting(formData)
      ElMessage.success('创建成功')
    }

    dialogVisible.value = false
    loadData()
  } catch (error) {
    ElMessage.error('操作失败')
  } finally {
    submitting.value = false
  }
}

// 切换状态
const toggleStatus = async (item: SeoSetting) => {
  try {
    const updateData: UpdateSeoSettingDto = {
      title: item.title,
      description: item.description,
      keywords: item.keywords,
      ogTitle: item.ogTitle,
      ogDescription: item.ogDescription,
      ogImage: item.ogImage,
      structuredData: item.structuredData,
      isEnabled: item.isEnabled
    }
    await updateSeoSetting(item.id, updateData)
    ElMessage.success('状态更新成功')
  } catch (error) {
    ElMessage.error('状态更新失败')
    // 恢复原状态
    item.isEnabled = !item.isEnabled
  }
}

// 删除
const handleDelete = async (item: SeoSetting) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除页面"${item.pagePath}"的SEO设置吗？`,
      '确认删除',
      {
        type: 'warning'
      }
    )

    await deleteSeoSetting(item.id)
    ElMessage.success('删除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN')
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.seo-management {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header-left h1 {
  margin: 0 0 5px 0;
  font-size: 24px;
  color: #333;
}

.header-left p {
  margin: 0;
  color: #666;
  font-size: 14px;
}

.content-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.filter-bar {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}
</style>