<template>
  <div class="pages-management">
    <div class="page-header">
      <div class="header-left">
        <h1>页面管理</h1>
        <p>管理网站自定义页面</p>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="showCreateDialog">
          <el-icon><Plus /></el-icon>
          新增页面
        </el-button>
      </div>
    </div>

    <div class="content-card">
      <div class="filter-bar">
        <el-select
          v-model="queryParams.status"
          placeholder="状态筛选"
          clearable
          @change="handleSearch"
          style="width: 120px"
        >
          <el-option
            v-for="(label, value) in statusOptions"
            :key="value"
            :label="label"
            :value="parseInt(value)"
          />
        </el-select>

        <el-select
          v-model="queryParams.template"
          placeholder="模板筛选"
          clearable
          @change="handleSearch"
          style="width: 150px"
        >
          <el-option label="默认模板" value="default" />
          <el-option label="关于我们" value="about" />
          <el-option label="联系我们" value="contact" />
        </el-select>

        <el-input
          v-model="queryParams.searchTerm"
          placeholder="搜索标题或内容..."
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
        :data="pageList"
        stripe
        style="width: 100%"
      >
        <el-table-column prop="title" label="页面标题" min-width="200" />
        <el-table-column prop="slug" label="URL路径" width="150">
          <template #default="{ row }">
            <el-tag>{{ row.slug }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="template" label="模板" width="120" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusLabel(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="showInMenu" label="显示在菜单" width="120">
          <template #default="{ row }">
            <el-switch
              v-model="row.showInMenu"
              @change="toggleMenuDisplay(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="viewCount" label="访问量" width="100" />
        <el-table-column prop="publishedAt" label="发布时间" width="180">
          <template #default="{ row }">
            {{ row.publishedAt ? formatDate(row.publishedAt) : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">
              编辑
            </el-button>
            <el-button
              size="small"
              type="success"
              @click="previewPage(row)"
              v-if="row.status === PageStatus.Published"
            >
              预览
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

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryParams.page"
          v-model:page-size="queryParams.pageSize"
          :total="totalCount"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </div>

    <!-- 创建/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingItem ? '编辑页面' : '新增页面'"
      width="900px"
      @close="resetForm"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="页面标题" prop="title">
              <el-input
                v-model="formData.title"
                placeholder="页面标题"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="URL路径" prop="slug">
              <el-input
                v-model="formData.slug"
                placeholder="如: about-us"
              />
              <div class="form-tip">页面访问路径，只能包含字母、数字、连字符</div>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="页面模板" prop="template">
              <el-select v-model="formData.template" style="width: 100%">
                <el-option label="默认模板" value="default" />
                <el-option label="关于我们" value="about" />
                <el-option label="联系我们" value="contact" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="页面状态" prop="status">
              <el-select v-model="formData.status" style="width: 100%">
                <el-option
                  v-for="(label, value) in statusOptions"
                  :key="value"
                  :label="label"
                  :value="parseInt(value)"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="页面摘要" prop="excerpt">
          <el-input
            type="textarea"
            v-model="formData.excerpt"
            :rows="3"
            placeholder="页面简短描述"
          />
        </el-form-item>

        <el-form-item label="页面内容" prop="content">
          <el-input
            type="textarea"
            v-model="formData.content"
            :rows="10"
            placeholder="页面内容，支持HTML"
          />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="显示顺序">
              <el-input-number
                v-model="formData.sortOrder"
                :min="0"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="显示在菜单">
              <el-switch v-model="formData.showInMenu" />
            </el-form-item>
          </el-col>
        </el-row>

        <!-- SEO设置 -->
        <el-divider content-position="left">SEO设置</el-divider>

        <el-form-item label="SEO标题" prop="seoTitle">
          <el-input
            v-model="formData.seoTitle"
            placeholder="搜索引擎显示的标题"
          />
        </el-form-item>

        <el-form-item label="SEO描述" prop="seoDescription">
          <el-input
            type="textarea"
            v-model="formData.seoDescription"
            :rows="3"
            placeholder="搜索引擎显示的描述"
          />
        </el-form-item>

        <el-form-item label="SEO关键词" prop="seoKeywords">
          <el-input
            v-model="formData.seoKeywords"
            placeholder="关键词，用英文逗号分隔"
          />
        </el-form-item>

        <el-form-item label="特色图片" prop="featuredImage">
          <el-input
            v-model="formData.featuredImage"
            placeholder="特色图片URL"
          />
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
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, Refresh } from '@element-plus/icons-vue'
import {
  getPages,
  createPage,
  updatePage,
  deletePage,
  type Page,
  type CreatePageDto,
  type UpdatePageDto,
  type PageQueryDto,
  PageStatus,
  PageStatusLabels
} from '@/api/pages'

const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const editingItem = ref<Page | null>(null)
const formRef = ref()

const pageList = ref<Page[]>([])
const totalCount = ref(0)

const queryParams = reactive<PageQueryDto>({
  page: 1,
  pageSize: 20
})

const formData = reactive<CreatePageDto>({
  title: '',
  slug: '',
  content: '',
  excerpt: '',
  featuredImage: '',
  template: 'default',
  status: PageStatus.Draft,
  sortOrder: 0,
  showInMenu: false,
  seoTitle: '',
  seoDescription: '',
  seoKeywords: ''
})

const formRules = {
  title: [
    { required: true, message: '请输入页面标题', trigger: 'blur' }
  ],
  slug: [
    { required: true, message: '请输入URL路径', trigger: 'blur' },
    { pattern: /^[a-z0-9-]+$/, message: '只能包含小写字母、数字、连字符', trigger: 'blur' }
  ],
  content: [
    { required: true, message: '请输入页面内容', trigger: 'blur' }
  ]
}

// 状态选项
const statusOptions = computed(() => PageStatusLabels)

// 获取状态标签
const getStatusLabel = (status: PageStatus) => {
  return PageStatusLabels[status] || '未知'
}

// 获取状态标签类型
const getStatusType = (status: PageStatus) => {
  switch (status) {
    case PageStatus.Draft:
      return 'info'
    case PageStatus.Published:
      return 'success'
    case PageStatus.Archived:
      return 'warning'
    default:
      return ''
  }
}

// 加载数据
const loadData = async () => {
  loading.value = true
  try {
    const response = await getPages(queryParams)
    pageList.value = response.data?.data || []
    totalCount.value = response.data?.totalCount || 0
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  queryParams.page = 1
  loadData()
}

// 刷新数据
const refreshData = () => {
  loadData()
}

// 分页大小变化
const handleSizeChange = (val: number) => {
  queryParams.pageSize = val
  queryParams.page = 1
  loadData()
}

// 当前页变化
const handleCurrentChange = (val: number) => {
  queryParams.page = val
  loadData()
}

// 显示创建对话框
const showCreateDialog = () => {
  editingItem.value = null
  resetForm()
  dialogVisible.value = true
}

// 显示编辑对话框
const showEditDialog = (item: Page) => {
  editingItem.value = item
  Object.assign(formData, {
    title: item.title,
    slug: item.slug,
    content: item.content,
    excerpt: item.excerpt || '',
    featuredImage: item.featuredImage || '',
    template: item.template,
    status: item.status,
    sortOrder: item.sortOrder,
    showInMenu: item.showInMenu,
    seoTitle: item.seoTitle || '',
    seoDescription: item.seoDescription || '',
    seoKeywords: item.seoKeywords || ''
  })
  dialogVisible.value = true
}

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    title: '',
    slug: '',
    content: '',
    excerpt: '',
    featuredImage: '',
    template: 'default',
    status: PageStatus.Draft,
    sortOrder: 0,
    showInMenu: false,
    seoTitle: '',
    seoDescription: '',
    seoKeywords: ''
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
      const updateData: UpdatePageDto = {
        title: formData.title,
        slug: formData.slug,
        content: formData.content,
        excerpt: formData.excerpt,
        featuredImage: formData.featuredImage,
        template: formData.template,
        status: formData.status!,
        sortOrder: formData.sortOrder,
        showInMenu: formData.showInMenu,
        seoTitle: formData.seoTitle,
        seoDescription: formData.seoDescription,
        seoKeywords: formData.seoKeywords
      }
      await updatePage(editingItem.value.id, updateData)
      ElMessage.success('更新成功')
    } else {
      // 创建
      await createPage(formData)
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

// 切换菜单显示
const toggleMenuDisplay = async (item: Page) => {
  try {
    const updateData: UpdatePageDto = {
      title: item.title,
      slug: item.slug,
      content: item.content,
      excerpt: item.excerpt,
      featuredImage: item.featuredImage,
      template: item.template,
      status: item.status,
      sortOrder: item.sortOrder,
      showInMenu: item.showInMenu,
      seoTitle: item.seoTitle,
      seoDescription: item.seoDescription,
      seoKeywords: item.seoKeywords
    }
    await updatePage(item.id, updateData)
    ElMessage.success('设置更新成功')
  } catch (error) {
    ElMessage.error('设置更新失败')
    // 恢复原状态
    item.showInMenu = !item.showInMenu
  }
}

// 预览页面
const previewPage = (item: Page) => {
  const url = `/pages/${item.slug}`
  window.open(url, '_blank')
}

// 删除
const handleDelete = async (item: Page) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除页面"${item.title}"吗？`,
      '确认删除',
      {
        type: 'warning'
      }
    )

    await deletePage(item.id)
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
.pages-management {
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
  flex-wrap: wrap;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}
</style>