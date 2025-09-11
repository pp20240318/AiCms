<template>
  <div class="menus">
    <div class="page-header">
      <h1>菜单管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增菜单
      </el-button>
    </div>
    
    <el-card>
      <el-table
        v-loading="loading"
        :data="menus"
        style="width: 100%"
        row-key="id"
        :tree-props="{ children: 'children' }"
      >
        <el-table-column prop="name" label="菜单名称" min-width="200" />
        <el-table-column prop="path" label="路径" min-width="200" />
        <el-table-column prop="icon" label="图标" width="100">
          <template #default="{ row }">
            <el-icon v-if="row.icon"><component :is="row.icon" /></el-icon>
          </template>
        </el-table-column>
        <el-table-column prop="sortOrder" label="排序" width="100" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">编辑</el-button>
            <el-button size="small" @click="showCreateDialog(row)">添加子菜单</el-button>
            <el-button size="small" type="danger" @click="deleteMenu(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 创建/编辑菜单对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingMenu ? '编辑菜单' : '新增菜单'"
      width="500px"
    >
      <el-form
        ref="formRef"
        :model="menuForm"
        :rules="formRules"
        label-width="80px"
      >
        <el-form-item label="父菜单">
          <el-tree-select
            v-model="menuForm.parentId"
            :data="menuOptions"
            :props="{ label: 'name', value: 'id', children: 'children' }"
            placeholder="请选择父菜单（可选）"
            check-strictly
            clearable
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="menuForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        <el-form-item label="路径" prop="path">
          <el-input v-model="menuForm.path" placeholder="请输入菜单路径" />
        </el-form-item>
        <el-form-item label="图标">
          <el-input v-model="menuForm.icon" placeholder="请输入图标名称" />
        </el-form-item>
        <el-form-item label="排序" prop="sortOrder">
          <el-input-number
            v-model="menuForm.sortOrder"
            :min="0"
            :max="9999"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="状态" v-if="editingMenu">
          <el-switch
            v-model="menuForm.isActive"
            active-text="启用"
            inactive-text="禁用"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveMenu" :loading="saving">
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
import dayjs from 'dayjs'

// 这里应该导入菜单的API，但由于没有实际的API，我们使用模拟数据
interface Menu {
  id: number
  name: string
  path: string
  icon?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
  createdAt: string
  children?: Menu[]
}

const loading = ref(false)
const saving = ref(false)
const menus = ref<Menu[]>([
  {
    id: 1,
    name: '仪表盘',
    path: '/dashboard',
    icon: 'HomeFilled',
    sortOrder: 1,
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z'
  },
  {
    id: 2,
    name: '内容管理',
    path: '/content',
    icon: 'Document',
    sortOrder: 2,
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    children: [
      {
        id: 3,
        name: '文章管理',
        path: '/articles',
        icon: 'DocumentCopy',
        parentId: 2,
        sortOrder: 1,
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z'
      },
      {
        id: 4,
        name: '产品管理',
        path: '/products',
        icon: 'ShoppingBag',
        parentId: 2,
        sortOrder: 2,
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z'
      }
    ]
  },
  {
    id: 5,
    name: '系统管理',
    path: '/system',
    icon: 'Setting',
    sortOrder: 3,
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    children: [
      {
        id: 6,
        name: '用户管理',
        path: '/users',
        icon: 'User',
        parentId: 5,
        sortOrder: 1,
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z'
      },
      {
        id: 7,
        name: '角色管理',
        path: '/roles',
        icon: 'UserFilled',
        parentId: 5,
        sortOrder: 2,
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z'
      }
    ]
  }
])

const dialogVisible = ref(false)
const editingMenu = ref<Menu | null>(null)
const parentMenu = ref<Menu | null>(null)
const formRef = ref()
const menuForm = reactive({
  name: '',
  path: '',
  icon: '',
  parentId: null as number | null,
  sortOrder: 0,
  isActive: true
})

const formRules = {
  name: [
    { required: true, message: '请输入菜单名称', trigger: 'blur' }
  ],
  path: [
    { required: true, message: '请输入菜单路径', trigger: 'blur' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序值', trigger: 'blur' }
  ]
}

const menuOptions = computed(() => {
  const options = [...menus.value]
  if (editingMenu.value) {
    return filterMenus(options, editingMenu.value.id)
  }
  return options
})

const filterMenus = (menus: Menu[], excludeId: number): Menu[] => {
  return menus.filter(menu => {
    if (menu.id === excludeId) return false
    if (menu.children) {
      menu.children = filterMenus(menu.children, excludeId)
    }
    return true
  })
}

const loadMenus = async () => {
  loading.value = true
  try {
    // 这里应该调用API获取菜单数据
    // menus.value = await getMenusTree()
  } catch (error) {
    ElMessage.error('加载菜单列表失败')
  } finally {
    loading.value = false
  }
}

const showCreateDialog = (parent?: Menu) => {
  editingMenu.value = null
  parentMenu.value = parent || null
  resetForm()
  menuForm.parentId = parent?.id || null
  dialogVisible.value = true
}

const showEditDialog = (menu: Menu) => {
  editingMenu.value = menu
  parentMenu.value = null
  menuForm.name = menu.name
  menuForm.path = menu.path
  menuForm.icon = menu.icon || ''
  menuForm.parentId = menu.parentId || null
  menuForm.sortOrder = menu.sortOrder
  menuForm.isActive = menu.isActive
  dialogVisible.value = true
}

const resetForm = () => {
  menuForm.name = ''
  menuForm.path = ''
  menuForm.icon = ''
  menuForm.parentId = null
  menuForm.sortOrder = 0
  menuForm.isActive = true
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const saveMenu = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    
    if (editingMenu.value) {
      // await updateMenu(editingMenu.value.id, menuForm)
      ElMessage.success('更新菜单成功')
    } else {
      // await createMenu(menuForm)
      ElMessage.success('创建菜单成功')
    }
    
    dialogVisible.value = false
    loadMenus()
  } catch (error) {
    ElMessage.error('保存菜单失败')
  } finally {
    saving.value = false
  }
}

const deleteMenu = async (menu: Menu) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除菜单"${menu.name}"吗？此操作会同时删除所有子菜单，且不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // await deleteMenuApi(menu.id)
    ElMessage.success('删除菜单成功')
    loadMenus()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除菜单失败')
    }
  }
}

onMounted(() => {
  loadMenus()
})
</script>

<style scoped>
.menus {
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