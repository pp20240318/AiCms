<template>
  <div class="article-categories">
    <div class="page-header">
      <h1>文章分类管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增分类
      </el-button>
    </div>

    <el-card>
      <el-table
        v-loading="loading"
        :data="categories"
        style="width: 100%"
        row-key="id"
        :tree-props="{ children: 'children' }"
      >
        <el-table-column prop="name" label="分类名称" min-width="200" />
        <el-table-column prop="description" label="描述" min-width="200" />
        <el-table-column prop="sortOrder" label="排序" width="100" />
        <el-table-column prop="articleCount" label="文章数量" width="120" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="260">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">编辑</el-button>
            <el-button size="small" @click="showCreateDialog(row)">添加子分类</el-button>
            <el-button size="small" type="warning" @click="toggleStatus(row)">
              {{ row.isActive ? '禁用' : '启用' }}
            </el-button>
            <el-button size="small" type="danger" @click="deleteCategory(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 创建/编辑分类对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingCategory ? '编辑文章分类' : '新增文章分类'"
      width="500px"
    >
      <el-form
        ref="formRef"
        :model="categoryForm"
        :rules="formRules"
        label-width="80px"
      >
        <el-form-item label="父分类">
          <el-tree-select
            v-model="categoryForm.parentId"
            :data="categoryOptions"
            :props="{ label: 'name', value: 'id', children: 'children' }"
            placeholder="请选择父分类（可选）"
            check-strictly
            clearable
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="分类名称" prop="name">
          <el-input v-model="categoryForm.name" placeholder="请输入分类名称" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input
            v-model="categoryForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入分类描述"
          />
        </el-form-item>
        <el-form-item label="排序" prop="sortOrder">
          <el-input-number
            v-model="categoryForm.sortOrder"
            :min="0"
            :max="9999"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="状态">
          <el-switch
            v-model="categoryForm.isActive"
            active-text="启用"
            inactive-text="禁用"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveCategory" :loading="saving">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import {
  getArticleCategoriesTree,
  createArticleCategory,
  updateArticleCategory,
  deleteArticleCategory,
  toggleArticleCategoryStatus
} from '@/api/articleCategories'
import type { ArticleCategory, CreateArticleCategoryData, UpdateArticleCategoryData } from '@/api/articleCategories'
import dayjs from 'dayjs'

const loading = ref(false)
const saving = ref(false)
const categories = ref<ArticleCategory[]>([])

const dialogVisible = ref(false)
const editingCategory = ref<ArticleCategory | null>(null)
const parentCategory = ref<ArticleCategory | null>(null)
const formRef = ref()
const categoryForm = reactive({
  name: '',
  description: '',
  parentId: null as number | null,
  sortOrder: 0,
  isActive: true
})

const formRules = {
  name: [
    { required: true, message: '请输入分类名称', trigger: 'blur' },
    { min: 2, max: 50, message: '分类名称长度为2-50个字符', trigger: 'blur' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序值', trigger: 'blur' }
  ]
}

const categoryOptions = computed(() => {
  const options = [...categories.value]
  if (editingCategory.value) {
    // 编辑时排除当前分类及其子分类
    return filterCategories(options, editingCategory.value.id)
  }
  return options
})

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

const filterCategories = (categories: ArticleCategory[], excludeId: number): ArticleCategory[] => {
  return categories.filter(cat => {
    if (cat.id === excludeId) return false
    if (cat.children) {
      cat.children = filterCategories(cat.children, excludeId)
    }
    return true
  })
}

const loadCategories = async () => {
  loading.value = true
  try {
    categories.value = await getArticleCategoriesTree()
  } catch (error) {
    ElMessage.error('加载文章分类列表失败')
  } finally {
    loading.value = false
  }
}

const showCreateDialog = (parent?: ArticleCategory) => {
  editingCategory.value = null
  parentCategory.value = parent || null
  resetForm()
  categoryForm.parentId = parent?.id || null
  dialogVisible.value = true
}

const showEditDialog = (category: ArticleCategory) => {
  editingCategory.value = category
  parentCategory.value = null
  categoryForm.name = category.name
  categoryForm.description = category.description || ''
  categoryForm.parentId = category.parentId || null
  categoryForm.sortOrder = category.sortOrder
  categoryForm.isActive = category.isActive
  dialogVisible.value = true
}

const resetForm = () => {
  categoryForm.name = ''
  categoryForm.description = ''
  categoryForm.parentId = null
  categoryForm.sortOrder = 0
  categoryForm.isActive = true
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const saveCategory = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
    saving.value = true

    if (editingCategory.value) {
      const updateData: UpdateArticleCategoryData = {
        name: categoryForm.name,
        description: categoryForm.description,
        parentId: categoryForm.parentId,
        sortOrder: categoryForm.sortOrder,
        isActive: categoryForm.isActive
      }
      await updateArticleCategory(editingCategory.value.id, updateData)
      ElMessage.success('更新文章分类成功')
    } else {
      const createData: CreateArticleCategoryData = {
        name: categoryForm.name,
        description: categoryForm.description,
        parentId: categoryForm.parentId,
        sortOrder: categoryForm.sortOrder,
        isActive: categoryForm.isActive
      }
      await createArticleCategory(createData)
      ElMessage.success('创建文章分类成功')
    }

    dialogVisible.value = false
    loadCategories()
  } catch (error) {
    ElMessage.error('保存文章分类失败')
  } finally {
    saving.value = false
  }
}

const toggleStatus = async (category: ArticleCategory) => {
  try {
    await toggleArticleCategoryStatus(category.id)
    ElMessage.success(`${category.isActive ? '禁用' : '启用'}分类成功`)
    loadCategories()
  } catch (error) {
    ElMessage.error('更新分类状态失败')
  }
}

const deleteCategory = async (category: ArticleCategory) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除分类"${category.name}"吗？此操作会同时删除所有子分类，且不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await deleteArticleCategory(category.id)
    ElMessage.success('删除文章分类成功')
    loadCategories()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除文章分类失败')
    }
  }
}

onMounted(() => {
  loadCategories()
})
</script>

<style scoped>
.article-categories {
  height: 100%;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  color: #303133;
}
</style>