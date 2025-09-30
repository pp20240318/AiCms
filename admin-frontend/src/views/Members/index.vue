<template>
  <div class="members">
    <div class="page-header">
      <h1>会员信息管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增会员
      </el-button>
    </div>

    <el-card>
      <div class="search-bar">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-input
              v-model="searchForm.keyword"
              placeholder="搜索会员编号、姓名、手机或邮箱"
              @input="handleSearch"
            >
              <template #prefix>
                <el-icon><Search /></el-icon>
              </template>
            </el-input>
          </el-col>
          <el-col :span="6">
            <el-select
              v-model="searchForm.membershipType"
              placeholder="会员类型"
              clearable
              @change="handleSearch"
            >
              <el-option label="普通会员" value="Regular" />
              <el-option label="VIP会员" value="VIP" />
              <el-option label="金牌会员" value="Gold" />
              <el-option label="钻石会员" value="Diamond" />
            </el-select>
          </el-col>
          <el-col :span="6">
            <el-select
              v-model="searchForm.status"
              placeholder="状态"
              clearable
              @change="handleSearch"
            >
              <el-option label="正常" value="Active" />
              <el-option label="暂停" value="Suspended" />
              <el-option label="过期" value="Expired" />
            </el-select>
          </el-col>
          <el-col :span="4">
            <el-button @click="generateMemberCode">生成会员编号</el-button>
          </el-col>
        </el-row>
      </div>

      <el-table
        v-loading="loading"
        :data="members"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="memberCode" label="会员编号" width="120" />
        <el-table-column prop="name" label="姓名" width="100" />
        <el-table-column prop="phone" label="手机号" width="120" />
        <el-table-column prop="email" label="邮箱" width="180" />
        <el-table-column label="会员类型" width="100">
          <template #default="{ row }">
            <el-tag :type="getMembershipTypeTag(row.membershipType)">
              {{ getMembershipTypeName(row.membershipType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="getStatusTag(row.status)">
              {{ getStatusName(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="balance" label="余额" width="100">
          <template #default="{ row }">
            ¥{{ (row.balance || 0).toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column prop="points" label="积分" width="80" />
        <el-table-column prop="joinDate" label="入会时间" width="120">
          <template #default="{ row }">
            {{ formatDate(row.joinDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="showDetailDialog(row)">详情</el-button>
            <el-button size="small" @click="showEditDialog(row)">编辑</el-button>
            <el-button size="small" type="warning" @click="showBalanceDialog(row)">充值</el-button>
            <el-button size="small" type="danger" @click="deleteMember(row)">删除</el-button>
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
          @size-change="loadMembers"
          @current-change="loadMembers"
        />
      </div>
    </el-card>

    <!-- 创建/编辑会员对话框 -->
    <el-dialog
      :title="isEdit ? '编辑会员' : '新增会员'"
      v-model="dialogVisible"
      width="800px"
      @close="resetForm"
    >
      <el-form
        ref="formRef"
        :model="memberForm"
        :rules="formRules"
        label-width="120px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="会员编号" prop="memberCode">
              <el-input v-model="memberForm.memberCode" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="姓名" prop="name">
              <el-input v-model="memberForm.name" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="性别">
              <el-radio-group v-model="memberForm.gender">
                <el-radio label="男">男</el-radio>
                <el-radio label="女">女</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="出生日期">
              <el-date-picker
                v-model="memberForm.dateOfBirth"
                type="date"
                placeholder="选择日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="手机号" prop="phone">
              <el-input v-model="memberForm.phone" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="memberForm.email" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="身份证号">
              <el-input v-model="memberForm.idNumber" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="会员类型" prop="membershipType">
              <el-select v-model="memberForm.membershipType" style="width: 100%">
                <el-option label="普通会员" value="Regular" />
                <el-option label="VIP会员" value="VIP" />
                <el-option label="金牌会员" value="Gold" />
                <el-option label="钻石会员" value="Diamond" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="地址">
          <el-input v-model="memberForm.address" type="textarea" :rows="2" />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="职业">
              <el-input v-model="memberForm.occupation" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="公司">
              <el-input v-model="memberForm.company" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="紧急联系人">
              <el-input v-model="memberForm.emergencyContact" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="紧急联系电话">
              <el-input v-model="memberForm.emergencyPhone" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="过期时间">
              <el-date-picker
                v-model="memberForm.expiryDate"
                type="date"
                placeholder="选择日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="推荐人">
              <el-input v-model="memberForm.referredBy" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="备注">
          <el-input v-model="memberForm.notes" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>

    <!-- 会员详情对话框 -->
    <el-dialog
      title="会员详情"
      v-model="detailDialogVisible"
      width="800px"
    >
      <el-descriptions :column="2" border v-if="currentMember">
        <el-descriptions-item label="会员编号">{{ currentMember.memberCode }}</el-descriptions-item>
        <el-descriptions-item label="姓名">{{ currentMember.name }}</el-descriptions-item>
        <el-descriptions-item label="性别">{{ currentMember.gender || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="出生日期">
          {{ currentMember.dateOfBirth ? formatDate(currentMember.dateOfBirth) : '未设置' }}
        </el-descriptions-item>
        <el-descriptions-item label="手机号">{{ currentMember.phone || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="邮箱">{{ currentMember.email || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="身份证号">{{ currentMember.idNumber || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="会员类型">
          <el-tag :type="getMembershipTypeTag(currentMember.membershipType)">
            {{ getMembershipTypeName(currentMember.membershipType) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="getStatusTag(currentMember.status)">
            {{ getStatusName(currentMember.status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="余额">¥{{ (currentMember.balance || 0).toFixed(2) }}</el-descriptions-item>
        <el-descriptions-item label="积分">{{ currentMember.points }}</el-descriptions-item>
        <el-descriptions-item label="入会时间">{{ formatDate(currentMember.joinDate) }}</el-descriptions-item>
        <el-descriptions-item label="过期时间">
          {{ currentMember.expiryDate ? formatDate(currentMember.expiryDate) : '无期限' }}
        </el-descriptions-item>
        <el-descriptions-item label="最后访问">
          {{ currentMember.lastVisitDate ? formatDate(currentMember.lastVisitDate) : '未访问' }}
        </el-descriptions-item>
        <el-descriptions-item label="地址" :span="2">{{ currentMember.address || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="职业">{{ currentMember.occupation || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="公司">{{ currentMember.company || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="紧急联系人">{{ currentMember.emergencyContact || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="紧急电话">{{ currentMember.emergencyPhone || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="推荐码">{{ currentMember.referralCode || '无' }}</el-descriptions-item>
        <el-descriptions-item label="推荐人">{{ currentMember.referredBy || '无' }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{ currentMember.notes || '无' }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>

    <!-- 余额充值对话框 -->
    <el-dialog
      title="余额充值"
      v-model="balanceDialogVisible"
      width="400px"
    >
      <el-form
        ref="balanceFormRef"
        :model="balanceForm"
        :rules="balanceRules"
        label-width="80px"
      >
        <el-form-item label="会员" prop="memberName">
          <el-input :value="currentMember?.name" disabled />
        </el-form-item>
        <el-form-item label="当前余额">
          <el-input :value="`¥${(currentMember?.balance || 0).toFixed(2)}`" disabled />
        </el-form-item>
        <el-form-item label="充值金额" prop="amount">
          <el-input-number
            v-model="balanceForm.amount"
            :min="0.01"
            :precision="2"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="balanceDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitBalance">确定充值</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import * as memberApi from '@/api/members'
import type { Member } from '@/api/members'

interface MemberForm {
  memberCode: string
  name: string
  gender?: string
  dateOfBirth?: Date
  idNumber?: string
  phone?: string
  email?: string
  address?: string
  membershipType: string
  expiryDate?: Date
  notes?: string
  occupation?: string
  company?: string
  emergencyContact?: string
  emergencyPhone?: string
  referredBy?: string
}

interface SearchForm {
  keyword: string
  membershipType: string
  status: string
}

// 响应式数据
const loading = ref(false)
const members = ref<Member[]>([])
const currentPage = ref(1)
const pageSize = ref(20)
const total = ref(0)
const selectedMembers = ref<Member[]>([])

const searchForm = reactive<SearchForm>({
  keyword: '',
  membershipType: '',
  status: ''
})

const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const balanceDialogVisible = ref(false)
const isEdit = ref(false)
const currentMember = ref<Member | null>(null)

const memberForm = reactive<MemberForm>({
  memberCode: '',
  name: '',
  gender: '',
  dateOfBirth: undefined,
  idNumber: '',
  phone: '',
  email: '',
  address: '',
  membershipType: 'Regular',
  expiryDate: undefined,
  notes: '',
  occupation: '',
  company: '',
  emergencyContact: '',
  emergencyPhone: '',
  referredBy: ''
})

const balanceForm = reactive({
  amount: 0
})

// 表单引用
const formRef = ref<FormInstance>()
const balanceFormRef = ref<FormInstance>()

// 表单验证规则
const formRules = reactive<FormRules>({
  memberCode: [
    { required: true, message: '请输入会员编号', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入姓名', trigger: 'blur' }
  ],
  phone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入有效的手机号', trigger: 'blur' }
  ],
  email: [
    { type: 'email', message: '请输入有效的邮箱地址', trigger: 'blur' }
  ],
  membershipType: [
    { required: true, message: '请选择会员类型', trigger: 'change' }
  ]
})

const balanceRules = reactive<FormRules>({
  amount: [
    { required: true, message: '请输入充值金额', trigger: 'blur' },
    { type: 'number', min: 0.01, message: '充值金额必须大于0', trigger: 'blur' }
  ]
})

// 方法定义
const loadMembers = async () => {
  loading.value = true
  try {
    const response = await memberApi.getMembers({
      page: currentPage.value,
      pageSize: pageSize.value,
      keyword: searchForm.keyword,
      membershipType: searchForm.membershipType,
      status: searchForm.status
    })
    members.value = response.items
    total.value = response.total
  } catch (error) {
    ElMessage.error('加载会员列表失败')
    // 临时模拟数据
    members.value = [
      {
        id: 1,
        memberCode: 'M20250901001',
        name: '张三',
        gender: '男',
        phone: '13800138001',
        email: 'zhangsan@example.com',
        membershipType: 'VIP',
        status: 'Active',
        balance: 1000.50,
        points: 2500,
        joinDate: '2025-01-01',
        createdAt: '2025-01-01'
      }
    ]
    total.value = 1
  }
  loading.value = false
}

const handleSearch = () => {
  currentPage.value = 1
  loadMembers()
}

const handleSelectionChange = (selection: Member[]) => {
  selectedMembers.value = selection
}

const showCreateDialog = async () => {
  isEdit.value = false
  dialogVisible.value = true
  // 自动生成会员编号
  await generateMemberCode()
}

const showEditDialog = (member: Member) => {
  isEdit.value = true
  currentMember.value = member

  // 填充表单
  Object.assign(memberForm, {
    ...member,
    dateOfBirth: member.dateOfBirth ? new Date(member.dateOfBirth) : undefined,
    expiryDate: member.expiryDate ? new Date(member.expiryDate) : undefined
  })

  dialogVisible.value = true
}

const showDetailDialog = (member: Member) => {
  currentMember.value = member
  detailDialogVisible.value = true
}

const showBalanceDialog = (member: Member) => {
  currentMember.value = member
  balanceForm.amount = 0
  balanceDialogVisible.value = true
}

const generateMemberCode = async () => {
  try {
    const response = await memberApi.generateMemberCode()
    memberForm.memberCode = response.memberCode
    ElMessage.success('会员编号生成成功')
  } catch (error) {
    ElMessage.error('生成会员编号失败')
    // 备用生成逻辑
    const now = new Date()
    const year = now.getFullYear()
    const month = (now.getMonth() + 1).toString().padStart(2, '0')
    const day = now.getDate().toString().padStart(2, '0')
    const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0')
    memberForm.memberCode = `M${year}${month}${day}${random}`
  }
}

const submitForm = async () => {
  if (!formRef.value) return

  await formRef.value.validate((valid) => {
    if (valid) {
      if (isEdit.value) {
        updateMember()
      } else {
        createMember()
      }
    }
  })
}

const createMember = async () => {
  try {
    const createData = {
      ...memberForm,
      dateOfBirth: memberForm.dateOfBirth ? memberForm.dateOfBirth.toISOString().split('T')[0] : undefined,
      expiryDate: memberForm.expiryDate ? memberForm.expiryDate.toISOString().split('T')[0] : undefined
    }
    await memberApi.createMember(createData)
    ElMessage.success('会员创建成功')
    dialogVisible.value = false
    loadMembers()
  } catch (error) {
    ElMessage.error('创建会员失败')
  }
}

const updateMember = async () => {
  try {
    const updateData = {
      ...memberForm,
      status: 'Active',
      dateOfBirth: memberForm.dateOfBirth ? memberForm.dateOfBirth.toISOString().split('T')[0] : undefined,
      expiryDate: memberForm.expiryDate ? memberForm.expiryDate.toISOString().split('T')[0] : undefined
    }
    await memberApi.updateMember(currentMember.value!.id, updateData)
    ElMessage.success('会员更新成功')
    dialogVisible.value = false
    loadMembers()
  } catch (error) {
    ElMessage.error('更新会员失败')
  }
}

const submitBalance = async () => {
  if (!balanceFormRef.value) return

  await balanceFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        await memberApi.updateBalance(currentMember.value!.id, balanceForm.amount)
        ElMessage.success(`充值成功，充值金额：¥${balanceForm.amount.toFixed(2)}`)
        balanceDialogVisible.value = false
        loadMembers()
      } catch (error) {
        ElMessage.error('充值失败')
      }
    }
  })
}

const deleteMember = async (member: Member) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除会员 "${member.name}" 吗？`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await memberApi.deleteMember(member.id)
    ElMessage.success('会员删除成功')
    loadMembers()
  } catch (error: any) {
    if (error.type !== 'cancel') {
      ElMessage.error('删除会员失败')
    }
  }
}

const resetForm = () => {
  Object.assign(memberForm, {
    memberCode: '',
    name: '',
    gender: '',
    dateOfBirth: undefined,
    idNumber: '',
    phone: '',
    email: '',
    address: '',
    membershipType: 'Regular',
    expiryDate: undefined,
    notes: '',
    occupation: '',
    company: '',
    emergencyContact: '',
    emergencyPhone: '',
    referredBy: ''
  })
  formRef.value?.resetFields()
}

// 辅助方法
const getMembershipTypeName = (type: string) => {
  const types: Record<string, string> = {
    'Regular': '普通会员',
    'VIP': 'VIP会员',
    'Gold': '金牌会员',
    'Diamond': '钻石会员'
  }
  return types[type] || type
}

const getMembershipTypeTag = (type: string) => {
  const tags: Record<string, string> = {
    'Regular': '',
    'VIP': 'warning',
    'Gold': 'success',
    'Diamond': 'danger'
  }
  return tags[type] || ''
}

const getStatusName = (status: string) => {
  const statuses: Record<string, string> = {
    'Active': '正常',
    'Suspended': '暂停',
    'Expired': '过期'
  }
  return statuses[status] || status
}

const getStatusTag = (status: string) => {
  const tags: Record<string, string> = {
    'Active': 'success',
    'Suspended': 'warning',
    'Expired': 'danger'
  }
  return tags[status] || ''
}

const formatDate = (date: string | Date) => {
  if (!date) return '-'
  const d = new Date(date)
  return d.toLocaleDateString('zh-CN')
}

// 生命周期
onMounted(() => {
  loadMembers()
})
</script>

<style scoped>
.members {
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
}

.search-bar {
  margin-bottom: 20px;
}

.pagination {
  margin-top: 20px;
  text-align: center;
}
</style>