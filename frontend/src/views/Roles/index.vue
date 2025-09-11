<template>
  <div class="roles">
    <div class="page-header">
      <h1>角色管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增角色
      </el-button>
    </div>
    
    <el-card>
      <el-table
        v-loading="loading"
        :data="roles"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="角色名称" />
        <el-table-column prop="description" label="描述" />
        <el-table-column label="权限">
          <template #default="{ row }">
            <el-tag
              v-for="permission in row.permissions"
              :key="permission.id"
              type="info"
              size="small"
              style="margin-right: 4px; margin-bottom: 4px;"
            >
              {{ permission.name }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">编辑</el-button>
            <el-button size="small" type="danger" @click="deleteRole(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 创建/编辑角色对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingRole ? '编辑角色' : '新增角色'"
      width="600px"
    >
      <el-form
        ref="formRef"
        :model="roleForm"
        :rules="formRules"
        label-width="80px"
      >
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input
            v-model="roleForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入角色描述"
          />
        </el-form-item>
        <el-form-item label="权限" prop="permissionIds">
          <div class="permissions-grid">
            <el-checkbox
              v-for="permission in permissions"
              :key="permission.id"
              :label="permission.id"
              v-model="roleForm.permissionIds"
            >
              {{ permission.name }}
              <span class="permission-desc">{{ permission.description }}</span>
            </el-checkbox>
          </div>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveRole" :loading="saving">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getRoles, createRole, updateRole, deleteRole as deleteRoleApi, getPermissions } from '@/api/roles'
import type { Role, Permission, CreateRoleData, UpdateRoleData } from '@/api/roles'
import dayjs from 'dayjs'

const loading = ref(false)
const saving = ref(false)
const roles = ref<Role[]>([])
const permissions = ref<Permission[]>([])

const dialogVisible = ref(false)
const editingRole = ref<Role | null>(null)
const formRef = ref()
const roleForm = reactive({
  name: '',
  description: '',
  permissionIds: [] as number[]
})

const formRules = {
  name: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 20, message: '角色名称长度为2-20个字符', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入角色描述', trigger: 'blur' }
  ],
  permissionIds: [
    { required: true, message: '请选择权限', trigger: 'change' }
  ]
}

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

const loadRoles = async () => {
  loading.value = true
  try {
    roles.value = await getRoles()
  } catch (error) {
    ElMessage.error('加载角色列表失败')
  } finally {
    loading.value = false
  }
}

const loadPermissions = async () => {
  try {
    permissions.value = await getPermissions()
  } catch (error) {
    ElMessage.error('加载权限列表失败')
  }
}

const showCreateDialog = () => {
  editingRole.value = null
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (role: Role) => {
  editingRole.value = role
  roleForm.name = role.name
  roleForm.description = role.description
  roleForm.permissionIds = role.permissions.map(p => p.id)
  dialogVisible.value = true
}

const resetForm = () => {
  roleForm.name = ''
  roleForm.description = ''
  roleForm.permissionIds = []
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const saveRole = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    
    if (editingRole.value) {
      const updateData: UpdateRoleData = {
        name: roleForm.name,
        description: roleForm.description,
        permissionIds: roleForm.permissionIds
      }
      await updateRole(editingRole.value.id, updateData)
      ElMessage.success('更新角色成功')
    } else {
      const createData: CreateRoleData = {
        name: roleForm.name,
        description: roleForm.description,
        permissionIds: roleForm.permissionIds
      }
      await createRole(createData)
      ElMessage.success('创建角色成功')
    }
    
    dialogVisible.value = false
    loadRoles()
  } catch (error) {
    ElMessage.error('保存角色失败')
  } finally {
    saving.value = false
  }
}

const deleteRole = async (role: Role) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除角色"${role.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteRoleApi(role.id)
    ElMessage.success('删除角色成功')
    loadRoles()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除角色失败')
    }
  }
}

onMounted(() => {
  loadRoles()
  loadPermissions()
})
</script>

<style scoped>
.roles {
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

.permissions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 12px;
  max-height: 300px;
  overflow-y: auto;
  padding: 8px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
}

.permission-desc {
  display: block;
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

:deep(.el-checkbox) {
  display: block;
  margin: 0;
}

:deep(.el-checkbox__label) {
  white-space: normal;
  line-height: 1.4;
}
</style>