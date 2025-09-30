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
        <el-table-column label="权限" min-width="300">
          <template #default="{ row }">
            <div class="permissions-display">
              <el-tag
                v-for="(permission, index) in row.permissions.slice(0, 3)"
                :key="permission.id"
                type="info"
                size="small"
                class="permission-tag"
              >
                {{ getPermissionDisplayName(permission.name) }}
              </el-tag>
              <el-popover
                v-if="row.permissions.length > 3"
                placement="top"
                :width="400"
                trigger="hover"
              >
                <template #reference>
                  <el-tag type="info" size="small" class="permission-tag more-tag">
                    +{{ row.permissions.length - 3 }}个权限
                  </el-tag>
                </template>
                <div class="permission-popover">
                  <div class="permission-groups-display">
                    <div 
                      v-for="(group, groupName) in groupPermissionsByModule(row.permissions)"
                      :key="groupName"
                      class="permission-group-display"
                    >
                      <div class="group-name">{{ groupName }}</div>
                      <div class="group-permissions">
                        <el-tag
                          v-for="permission in group"
                          :key="permission.id"
                          size="small"
                          type="success"
                          class="permission-tag-small"
                        >
                          {{ getPermissionDisplayName(permission.name) }}
                        </el-tag>
                      </div>
                    </div>
                  </div>
                </div>
              </el-popover>
            </div>
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
      width="900px"
      top="5vh"
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
        <el-form-item label="权限" prop="permissionCodes">
          <div class="permission-selector-container">
            <div class="selector-header">
              <span class="selector-title">菜单权限</span>
            </div>
            
            <MenuPermissionSelector
              :menus="systemMenus"
              v-model="roleForm.permissionCodes"
            />
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
import { ref, onMounted, reactive, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getRoles, createRole, updateRole, deleteRole as deleteRoleApi, getPermissions } from '@/api/roles'
import type { Role, Permission, CreateRoleData, UpdateRoleData } from '@/api/roles'
import MenuPermissionSelector from '@/components/MenuPermissionSelector.vue'
import { getAllSystemMenus } from '@/api/menus'
import dayjs from 'dayjs'

const loading = ref(false)
const saving = ref(false)
const roles = ref<Role[]>([])
const permissions = ref<Permission[]>([])
const systemMenus = ref<any[]>([])

const dialogVisible = ref(false)
const editingRole = ref<Role | null>(null)
const formRef = ref()
const roleForm = reactive({
  name: '',
  description: '',
  permissionCodes: [] as string[]
})

const formRules = {
  name: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 20, message: '角色名称长度为2-20个字符', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入角色描述', trigger: 'blur' }
  ],
  permissionCodes: [
    { required: true, message: '请选择权限', trigger: 'change' }
  ]
}

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

// 获取权限显示名称（去掉模块前缀）
const getPermissionDisplayName = (permissionName: string) => {
  const parts = permissionName.split(':')
  if (parts.length > 1) {
    const action = parts[1]
    const actionNames: Record<string, string> = {
      view: '查看',
      list: '列表',
      create: '创建',
      edit: '编辑',
      update: '更新',
      delete: '删除',
      manage: '管理'
    }
    return actionNames[action] || action
  }
  return permissionName
}

// 按模块分组权限（用于表格展示）
const groupPermissionsByModule = (rolePermissions: Permission[] = []) => {
  const groups: Record<string, Permission[]> = {}
  rolePermissions.forEach(permission => {
    if (!permission?.name) return
    const prefix = permission.name.split(':')[0] || '其他'
    const groupName = getGroupDisplayName(prefix)
    if (!groups[groupName]) {
      groups[groupName] = []
    }
    groups[groupName].push(permission)
  })
  return groups
}

const getGroupDisplayName = (prefix: string): string => {
  const groupNames: Record<string, string> = {
    dashboard: '仪表盘',
    users: '用户管理',
    roles: '角色管理',
    articles: '文章管理',
    products: '产品管理',
    categories: '分类管理',
    banners: '轮播图管理',
    pages: '页面管理',
    files: '文件管理',
    seo: 'SEO设置',
    contacts: '联系我们',
    members: '会员管理',
    menus: '菜单管理',
    system: '系统设置'
  }
  return groupNames[prefix] || prefix
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
  roleForm.permissionCodes = role.permissions?.map(p => p.name) || []
  dialogVisible.value = true
}

const resetForm = () => {
  roleForm.name = ''
  roleForm.description = ''
  roleForm.permissionCodes = []
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const permissionCodeToId = computed(() => {
  const map = new Map<string, number>()
  permissions.value.forEach(permission => {
    if (permission.name && permission.id) {
      map.set(permission.name, permission.id)
    }
  })
  return map
})

const saveRole = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    const selectedPermissionIds = roleForm.permissionCodes
      .map(code => permissionCodeToId.value.get(code))
      .filter((id): id is number => typeof id === 'number')
    if (selectedPermissionIds.length === 0) {
      ElMessage.error('请选择有效的权限')
      return
    }
    
    if (editingRole.value) {
      const updateData: UpdateRoleData = {
        name: roleForm.name,
        description: roleForm.description,
        permissionIds: selectedPermissionIds
      }
      await updateRole(editingRole.value.id, updateData)
      ElMessage.success('更新角色成功')
    } else {
      const createData: CreateRoleData = {
        name: roleForm.name,
        description: roleForm.description,
        permissionIds: selectedPermissionIds
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

// 加载系统菜单
const loadSystemMenus = () => {
  systemMenus.value = getAllSystemMenus()
}

onMounted(() => {
  loadRoles()
  loadPermissions()
  loadSystemMenus()
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

.permission-selector-container {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
}

.selector-header {
  padding: 12px 16px;
  background: #fafafa;
  border-bottom: 1px solid #ebeef5;
}

.selector-title {
  font-weight: 500;
  color: #303133;
}

</style>