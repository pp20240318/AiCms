<template>
  <div class="contacts-management">
    <div class="page-header">
      <div class="header-left">
        <h1>联系我们管理</h1>
        <p>管理用户提交的联系我们消息</p>
      </div>
      <div class="header-right">
        <el-button @click="loadStatistics">
          <el-icon><DataAnalysis /></el-icon>
          查看统计
        </el-button>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-cards" v-if="statistics">
      <div class="stat-card">
        <div class="stat-value">{{ statistics.totalCount }}</div>
        <div class="stat-label">总消息数</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ statistics.todayCount }}</div>
        <div class="stat-label">今日新增</div>
      </div>
      <div class="stat-card" v-for="stat in statistics.statusStats" :key="stat.status">
        <div class="stat-value">{{ stat.count }}</div>
        <div class="stat-label">{{ getStatusLabel(stat.status) }}</div>
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

        <el-input
          v-model="queryParams.searchTerm"
          placeholder="搜索姓名、邮箱或主题..."
          style="width: 300px"
          @input="handleSearch"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>

        <el-date-picker
          v-model="dateRange"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          @change="handleDateChange"
          style="width: 240px"
        />

        <el-button @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新
        </el-button>
      </div>

      <el-table
        v-loading="loading"
        :data="contactList"
        stripe
        style="width: 100%"
      >
        <el-table-column prop="name" label="姓名" width="100" />
        <el-table-column prop="email" label="邮箱" width="200" />
        <el-table-column prop="phone" label="电话" width="120" />
        <el-table-column prop="company" label="公司" width="150" />
        <el-table-column prop="subject" label="主题" min-width="200" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusLabel(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="提交时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="showDetailDialog(row)">
              查看
            </el-button>
            <el-button
              size="small"
              type="warning"
              @click="showReplyDialog(row)"
              v-if="row.status !== ContactStatus.Replied"
            >
              回复
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

    <!-- 详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="消息详情"
      width="600px"
    >
      <div v-if="selectedContact" class="contact-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="姓名">{{ selectedContact.name }}</el-descriptions-item>
          <el-descriptions-item label="邮箱">{{ selectedContact.email }}</el-descriptions-item>
          <el-descriptions-item label="电话">{{ selectedContact.phone || '未填写' }}</el-descriptions-item>
          <el-descriptions-item label="公司">{{ selectedContact.company || '未填写' }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="getStatusType(selectedContact.status)">
              {{ getStatusLabel(selectedContact.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="IP地址">{{ selectedContact.ipAddress || '未知' }}</el-descriptions-item>
          <el-descriptions-item label="提交时间" :span="2">
            {{ formatDate(selectedContact.createdAt) }}
          </el-descriptions-item>
          <el-descriptions-item label="主题" :span="2">{{ selectedContact.subject }}</el-descriptions-item>
        </el-descriptions>

        <div class="message-content">
          <h4>消息内容：</h4>
          <div class="message-text">{{ selectedContact.message }}</div>
        </div>

        <div v-if="selectedContact.reply" class="reply-content">
          <h4>回复内容：</h4>
          <div class="reply-text">{{ selectedContact.reply }}</div>
          <div class="reply-time">回复时间: {{ formatDate(selectedContact.repliedAt!) }}</div>
        </div>
      </div>
    </el-dialog>

    <!-- 回复对话框 -->
    <el-dialog
      v-model="replyDialogVisible"
      title="回复消息"
      width="600px"
      @close="resetReplyForm"
    >
      <el-form
        ref="replyFormRef"
        :model="replyFormData"
        :rules="replyFormRules"
        label-width="80px"
      >
        <el-form-item label="状态" prop="status">
          <el-select v-model="replyFormData.status" style="width: 100%">
            <el-option
              v-for="(label, value) in statusOptions"
              :key="value"
              :label="label"
              :value="parseInt(value)"
            />
          </el-select>
        </el-form-item>

        <el-form-item label="回复内容" prop="reply" v-if="replyFormData.status === ContactStatus.Replied">
          <el-input
            type="textarea"
            v-model="replyFormData.reply"
            :rows="6"
            placeholder="请输入回复内容..."
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="replyDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleReply" :loading="replying">
          确认
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, DataAnalysis } from '@element-plus/icons-vue'
import {
  getContacts,
  getContactStatistics,
  updateContactStatus,
  deleteContact,
  type Contact,
  type ContactQueryDto,
  type ContactStatistics,
  type UpdateContactStatusDto,
  ContactStatus,
  ContactStatusLabels
} from '@/api/contact'

const loading = ref(false)
const replying = ref(false)
const detailDialogVisible = ref(false)
const replyDialogVisible = ref(false)
const dateRange = ref<[Date, Date] | null>(null)
const selectedContact = ref<Contact | null>(null)
const replyFormRef = ref()

const contactList = ref<Contact[]>([])
const totalCount = ref(0)
const statistics = ref<ContactStatistics | null>(null)

const queryParams = reactive<ContactQueryDto>({
  page: 1,
  pageSize: 20
})

const replyFormData = reactive<UpdateContactStatusDto>({
  status: ContactStatus.Processing,
  reply: ''
})

const replyFormRules = {
  status: [
    { required: true, message: '请选择状态', trigger: 'change' }
  ],
  reply: [
    { required: true, message: '请输入回复内容', trigger: 'blur' }
  ]
}

// 状态选项
const statusOptions = computed(() => ContactStatusLabels)

// 获取状态标签
const getStatusLabel = (status: ContactStatus) => {
  return ContactStatusLabels[status] || '未知'
}

// 获取状态标签类型
const getStatusType = (status: ContactStatus) => {
  switch (status) {
    case ContactStatus.New:
      return 'danger'
    case ContactStatus.Processing:
      return 'warning'
    case ContactStatus.Replied:
      return 'success'
    case ContactStatus.Closed:
      return 'info'
    default:
      return ''
  }
}

// 加载数据
const loadData = async () => {
  loading.value = true
  try {
    const response = await getContacts(queryParams)
    contactList.value = response.data?.data || []
    totalCount.value = response.data?.totalCount || 0
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

// 加载统计数据
const loadStatistics = async () => {
  try {
    const response = await getContactStatistics()
    statistics.value = response.data || null
  } catch (error) {
    ElMessage.error('加载统计数据失败')
  }
}

// 搜索
const handleSearch = () => {
  queryParams.page = 1
  loadData()
}

// 日期范围变化
const handleDateChange = (dates: [Date, Date] | null) => {
  if (dates) {
    queryParams.startDate = dates[0].toISOString().split('T')[0]
    queryParams.endDate = dates[1].toISOString().split('T')[0]
  } else {
    queryParams.startDate = undefined
    queryParams.endDate = undefined
  }
  handleSearch()
}

// 刷新数据
const refreshData = () => {
  loadData()
  loadStatistics()
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

// 显示详情对话框
const showDetailDialog = (contact: Contact) => {
  selectedContact.value = contact
  detailDialogVisible.value = true
}

// 显示回复对话框
const showReplyDialog = (contact: Contact) => {
  selectedContact.value = contact
  replyFormData.status = ContactStatus.Processing
  replyFormData.reply = ''
  replyDialogVisible.value = true
}

// 重置回复表单
const resetReplyForm = () => {
  replyFormData.status = ContactStatus.Processing
  replyFormData.reply = ''
  if (replyFormRef.value) {
    replyFormRef.value.clearValidate()
  }
}

// 处理回复
const handleReply = async () => {
  if (!selectedContact.value || !replyFormRef.value) return

  try {
    if (replyFormData.status === ContactStatus.Replied) {
      await replyFormRef.value.validate()
    }

    replying.value = true

    await updateContactStatus(selectedContact.value.id, replyFormData)
    ElMessage.success('操作成功')
    replyDialogVisible.value = false
    loadData()
    loadStatistics()
  } catch (error) {
    ElMessage.error('操作失败')
  } finally {
    replying.value = false
  }
}

// 删除
const handleDelete = async (contact: Contact) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除来自"${contact.name}"的消息吗？`,
      '确认删除',
      {
        type: 'warning'
      }
    )

    await deleteContact(contact.id)
    ElMessage.success('删除成功')
    loadData()
    loadStatistics()
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
  loadStatistics()
})
</script>

<style scoped>
.contacts-management {
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

.stats-cards {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

.stat-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  text-align: center;
  min-width: 120px;
}

.stat-value {
  font-size: 28px;
  font-weight: bold;
  color: #409eff;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  color: #666;
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

.contact-detail {
  padding: 10px 0;
}

.message-content,
.reply-content {
  margin-top: 20px;
}

.message-content h4,
.reply-content h4 {
  margin: 0 0 10px 0;
  color: #333;
}

.message-text,
.reply-text {
  background: #f5f7fa;
  padding: 15px;
  border-radius: 6px;
  border-left: 4px solid #409eff;
  line-height: 1.6;
  white-space: pre-wrap;
}

.reply-time {
  margin-top: 10px;
  font-size: 12px;
  color: #999;
}
</style>