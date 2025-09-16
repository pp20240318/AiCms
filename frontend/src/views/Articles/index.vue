<template>
  <div class="articles">
    <div class="page-header">
      <h1>文章管理</h1>
      <el-button type="primary" @click="$router.push('/admin/articles/create')">
        <el-icon><Plus /></el-icon>
        新增文章
      </el-button>
    </div>
    
    <el-card>
      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="搜索文章标题或作者"
          @input="handleSearch"
          style="width: 300px"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>
        
        <el-select
          v-model="statusFilter"
          placeholder="状态"
          @change="handleSearch"
          style="width: 120px; margin-left: 12px"
        >
          <el-option label="全部" value="" />
          <el-option label="草稿" value="draft" />
          <el-option label="已发布" value="published" />
          <el-option label="已归档" value="archived" />
        </el-select>
      </div>
      
      <el-table
        v-loading="loading"
        :data="articles"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="title" label="标题" min-width="200" />
        <el-table-column prop="author" label="作者" width="120" />
        <el-table-column prop="categoryName" label="分类" width="120" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag
              :type="getStatusType(row.status)"
              size="small"
            >
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="viewCount" label="阅读量" width="100" />
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="$router.push(`/admin/articles/${row.id}/edit`)">编辑</el-button>
            <el-button 
              size="small" 
              :type="row.status === 'published' ? 'warning' : 'success'"
              @click="togglePublish(row)"
              v-if="row.status !== 'archived'"
            >
              {{ row.status === 'published' ? '下线' : '发布' }}
            </el-button>
            <el-button size="small" type="danger" @click="deleteArticle(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :total="total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="loadArticles"
          @current-change="loadArticles"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { getArticles, deleteArticle as deleteArticleApi, publishArticle, archiveArticle } from '@/api/articles'
import type { Article } from '@/api/articles'
import dayjs from 'dayjs'

const loading = ref(false)
const articles = ref<Article[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)
const searchText = ref('')
const statusFilter = ref('')

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

const getStatusType = (status: string) => {
  switch (status) {
    case 'published':
      return 'success'
    case 'draft':
      return 'warning'
    case 'archived':
      return 'info'
    default:
      return ''
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'published':
      return '已发布'
    case 'draft':
      return '草稿'
    case 'archived':
      return '已归档'
    default:
      return status
  }
}

const loadArticles = async () => {
  loading.value = true
  try {
    const response = await getArticles({
      page: currentPage.value,
      pageSize: pageSize.value,
      search: searchText.value,
      status: statusFilter.value
    })
    articles.value = response.items
    total.value = response.total
  } catch (error) {
    ElMessage.error('加载文章列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadArticles()
}

const togglePublish = async (article: Article) => {
  try {
    if (article.status === 'published') {
      await archiveArticle(article.id)
      ElMessage.success('文章已下线')
    } else {
      await publishArticle(article.id)
      ElMessage.success('文章已发布')
    }
    loadArticles()
  } catch (error) {
    ElMessage.error('操作失败')
  }
}

const deleteArticle = async (article: Article) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除文章"${article.title}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteArticleApi(article.id)
    ElMessage.success('删除文章成功')
    loadArticles()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除文章失败')
    }
  }
}

onMounted(() => {
  loadArticles()
})
</script>

<style scoped>
.articles {
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

.search-bar {
  margin-bottom: 16px;
  display: flex;
  align-items: center;
}

.pagination {
  margin-top: 16px;
  text-align: right;
}
</style>