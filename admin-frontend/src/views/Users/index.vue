<template>
  <div class="users">
    <div class="page-header">
      <h1>用户管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增用户
      </el-button>
    </div>
    
    <el-card>
      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="搜索用户名或邮箱"
          @input="handleSearch"
          style="width: 300px"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>
      </div>
      
      <el-table
        v-loading="loading"
        :data="users"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="username" label="用户名" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column label="角色">
          <template #default="{ row }">
            <el-tag v-for="role in row.roles" :key="role" type="info" size="small">
              {{ role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'">
              {{ row.isActive ? '正常' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">编辑</el-button>
            <el-button
              size="small"
              :type="row.isActive ? 'warning' : 'success'"
              @click="toggleUserStatus(row)"
            >
              {{ row.isActive ? '禁用' : '启用' }}
            </el-button>
            <el-button size="small" type="danger" @click="deleteUser(row)">删除</el-button>
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
          @size-change="loadUsers"
          @current-change="loadUsers"
        />
      </div>
    </el-card>
    
    <!-- 创建/编辑用户对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingUser ? '编辑用户' : '新增用户'"
      width="500px"
    >
      <el-form
        ref="formRef"
        :model="userForm"
        :rules="formRules"
        label-width="80px"
      >
        <el-form-item label="用户名" prop="username">
          <el-input v-model="userForm.username" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="userForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!editingUser">
          <el-input
            v-model="userForm.password"
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="角色" prop="roleIds">
          <el-select
            v-model="userForm.roleIds"
            multiple
            placeholder="请选择角色"
            style="width: 100%"
          >
            <el-option
              v-for="role in roles"
              :key="role.id"
              :label="role.name"
              :value="role.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="状态" v-if="editingUser">
          <el-switch
            v-model="userForm.isActive"
            active-text="正常"
            inactive-text="禁用"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveUser" :loading="saving">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { getUsers, createUser, updateUser, deleteUser as deleteUserApi } from '@/api/users'
import { getRoles } from '@/api/roles'
import type { User, CreateUserData, UpdateUserData } from '@/api/users'
import type { Role } from '@/api/roles'
import dayjs from 'dayjs'

const loading = ref(false)
const saving = ref(false)
const users = ref<User[]>([])
const roles = ref<Role[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)
const searchText = ref('')

const dialogVisible = ref(false)
const editingUser = ref<User | null>(null)
const formRef = ref()
const userForm = reactive({
  username: '',
  email: '',
  password: '',
  roleIds: [] as number[],
  isActive: true
})

const formRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 2, max: 20, message: '用户名长度为2-20个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入有效的邮箱地址', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度至少6个字符', trigger: 'blur' }
  ],
  roleIds: [
    { required: true, message: '请选择角色', trigger: 'change' }
  ]
}

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

const loadUsers = async () => {
  loading.value = true
  try {
    const response = await getUsers({
      page: currentPage.value,
      pageSize: pageSize.value,
      search: searchText.value
    })
    users.value = response.items
    total.value = response.total
  } catch (error) {
    ElMessage.error('加载用户列表失败')
  } finally {
    loading.value = false
  }
}

const loadRoles = async () => {
  try {
    roles.value = await getRoles()
  } catch (error) {
    ElMessage.error('加载角色列表失败')
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadUsers()
}

const showCreateDialog = () => {
  editingUser.value = null
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (user: User) => {
  editingUser.value = user
  userForm.username = user.username
  userForm.email = user.email
  userForm.password = ''
  userForm.roleIds = [] // 这里需要根据实际API返回的数据结构调整
  userForm.isActive = user.isActive
  dialogVisible.value = true
}

const resetForm = () => {
  userForm.username = ''
  userForm.email = ''
  userForm.password = ''
  userForm.roleIds = []
  userForm.isActive = true
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const saveUser = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    
    if (editingUser.value) {
      const updateData: UpdateUserData = {
        username: userForm.username,
        email: userForm.email,
        roleIds: userForm.roleIds,
        isActive: userForm.isActive
      }
      if (userForm.password) {
        updateData.password = userForm.password
      }
      await updateUser(editingUser.value.id, updateData)
      ElMessage.success('更新用户成功')
    } else {
      const createData: CreateUserData = {
        username: userForm.username,
        email: userForm.email,
        password: userForm.password,
        roleIds: userForm.roleIds
      }
      await createUser(createData)
      ElMessage.success('创建用户成功')
    }
    
    dialogVisible.value = false
    loadUsers()
  } catch (error) {
    ElMessage.error('保存用户失败')
  } finally {
    saving.value = false
  }
}

const toggleUserStatus = async (user: User) => {
  try {
    await updateUser(user.id, { isActive: !user.isActive })
    ElMessage.success(`${user.isActive ? '禁用' : '启用'}用户成功`)
    loadUsers()
  } catch (error) {
    ElMessage.error(`${user.isActive ? '禁用' : '启用'}用户失败`)
  }
}

const deleteUser = async (user: User) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除用户"${user.username}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteUserApi(user.id)
    ElMessage.success('删除用户成功')
    loadUsers()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除用户失败')
    }
  }
}

onMounted(() => {
  loadUsers()
  loadRoles()
})
</script>

<style scoped>
.users {
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
}

.pagination {
  margin-top: 16px;
  text-align: right;
}
</style>