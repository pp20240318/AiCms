<template>
  <div class="menus">
    <div class="page-header">
      <h1>菜单管理</h1>
      <div class="header-actions">
        <el-button @click="syncWithRoutes" :loading="syncing">
          <el-icon><Refresh /></el-icon>
          同步路由
        </el-button>
        <el-button type="primary" @click="showCreateDialog">
          <el-icon><Plus /></el-icon>
          新增菜单
        </el-button>
      </div>
    </div>
    
    <el-card>
      <el-table
        v-loading="loading"
        :data="menus"
        style="width: 100%"
        row-key="id"
        :tree-props="{ children: 'children' }"
      >
        <el-table-column prop="name" label="菜单名称" min-width="200">
          <template #default="{ row }">
            <div class="menu-name">
              <el-icon v-if="row.icon" class="menu-icon">
                <component :is="row.icon" />
              </el-icon>
              <span>{{ row.name }}</span>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="path" label="路径" min-width="200">
          <template #default="{ row }">
            <el-tag size="small" type="info">{{ row.path }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="component" label="组件" width="120">
          <template #default="{ row }">
            <el-tag size="small" v-if="row.component">{{ row.component }}</el-tag>
            <span v-else class="text-gray">-</span>
          </template>
        </el-table-column>
        <el-table-column prop="permission" label="权限" width="150">
          <template #default="{ row }">
            <el-tag size="small" type="warning" v-if="row.permission">{{ row.permission }}</el-tag>
            <span v-else class="text-gray">-</span>
          </template>
        </el-table-column>
        <el-table-column prop="sortOrder" label="排序" width="80" />
        <el-table-column label="可见性" width="80">
          <template #default="{ row }">
            <el-icon v-if="row.isVisible" class="text-green">
              <View />
            </el-icon>
            <el-icon v-else class="text-gray">
              <Hide />
            </el-icon>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-switch
              v-model="row.isActive"
              @change="toggleStatus(row)"
              :loading="row.statusLoading"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">
              <el-icon><Edit /></el-icon>
              编辑
            </el-button>
            <el-button size="small" @click="showCreateDialog(row)" v-if="!row.parentId">
              <el-icon><Plus /></el-icon>
              添加子菜单
            </el-button>
            <el-button size="small" type="danger" @click="deleteMenu(row)">
              <el-icon><Delete /></el-icon>
              删除
            </el-button>
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
          <el-input v-model="menuForm.path" placeholder="如: /admin/articles" />
        </el-form-item>
        <el-form-item label="组件">
          <el-input v-model="menuForm.component" placeholder="如: Articles" />
        </el-form-item>
        <el-form-item label="图标">
          <el-select v-model="menuForm.icon" placeholder="选择图标" clearable filterable>
            <el-option label="首页 - HomeFilled" value="HomeFilled" />
            <el-option label="文档 - Document" value="Document" />
            <el-option label="编辑 - EditPen" value="EditPen" />
            <el-option label="加号 - Plus" value="Plus" />
            <el-option label="文件夹 - FolderOpened" value="FolderOpened" />
            <el-option label="购物袋 - ShoppingBag" value="ShoppingBag" />
            <el-option label="标签 - CollectionTag" value="CollectionTag" />
            <el-option label="笔记本 - Notebook" value="Notebook" />
            <el-option label="图片 - Picture" value="Picture" />
            <el-option label="用户 - User" value="User" />
            <el-option label="头像 - Avatar" value="Avatar" />
            <el-option label="用户填充 - UserFilled" value="UserFilled" />
            <el-option label="明信片 - Postcard" value="Postcard" />
            <el-option label="设置 - Setting" value="Setting" />
            <el-option label="菜单 - Menu" value="Menu" />
            <el-option label="搜索 - Search" value="Search" />
            <el-option label="消息 - Message" value="Message" />
          </el-select>
        </el-form-item>
        <el-form-item label="权限标识">
          <el-input v-model="menuForm.permission" placeholder="如: articles:view" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="menuForm.description" placeholder="菜单功能描述" />
        </el-form-item>
        <el-form-item label="排序" prop="sortOrder">
          <el-input-number
            v-model="menuForm.sortOrder"
            :min="0"
            :max="9999"
            style="width: 200px"
          />
        </el-form-item>
        <el-row>
          <el-col :span="12">
            <el-form-item label="状态">
          <el-switch
            v-model="menuForm.isActive"
            active-text="启用"
            inactive-text="禁用"
          />
        </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="显示在菜单">
              <el-switch
                v-model="menuForm.isVisible"
                active-text="显示"
                inactive-text="隐藏"
              />
            </el-form-item>
          </el-col>
        </el-row>
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
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Edit, Delete, View, Hide, Refresh } from '@element-plus/icons-vue'
import { 
  getMenusTree, 
  createMenu, 
  updateMenu, 
  deleteMenu as deleteMenuApi, 
  toggleMenuStatus,
  type Menu, 
  type CreateMenuDto,
  type UpdateMenuDto 
} from '@/api/menus'
import { compareMenuWithRoutes, suggestMenuUpdates } from '@/utils/menuSync'
import dayjs from 'dayjs'

const router = useRouter()
const loading = ref(false)
const saving = ref(false)
const syncing = ref(false)
const menus = ref<Menu[]>([])

const dialogVisible = ref(false)
const editingMenu = ref<Menu | null>(null)
const parentMenu = ref<Menu | null>(null)
const formRef = ref()
const menuForm = reactive<CreateMenuDto>({
  name: '',
  path: '',
  icon: '',
  component: '',
  parentId: null as number | null,
  sortOrder: 0,
  isActive: true,
  isVisible: true,
  permission: '',
  description: ''
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
    menus.value = await getMenusTree()
    // 为每个菜单项添加状态加载标志
    const addStatusLoading = (items: Menu[]) => {
      items.forEach(item => {
        item.statusLoading = false
        if (item.children) {
          addStatusLoading(item.children)
        }
      })
    }
    addStatusLoading(menus.value)
  } catch (error) {
    console.error('加载菜单列表失败:', error)
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
  Object.assign(menuForm, {
    name: menu.name,
    path: menu.path,
    icon: menu.icon || '',
    component: menu.component || '',
    parentId: menu.parentId || null,
    sortOrder: menu.sortOrder,
    isActive: menu.isActive,
    isVisible: menu.isVisible,
    permission: menu.permission || '',
    description: menu.description || ''
  })
  dialogVisible.value = true
}

const resetForm = () => {
  Object.assign(menuForm, {
    name: '',
    path: '',
    icon: '',
    component: '',
    parentId: null,
    sortOrder: 0,
    isActive: true,
    isVisible: true,
    permission: '',
    description: ''
  })
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
      const updateData: UpdateMenuDto = { ...menuForm, id: editingMenu.value.id }
      await updateMenu(editingMenu.value.id, updateData)
      ElMessage.success('更新菜单成功')
    } else {
      await createMenu(menuForm)
      ElMessage.success('创建菜单成功')
    }
    
    dialogVisible.value = false
    await loadMenus()
  } catch (error) {
    console.error('保存菜单失败:', error)
    ElMessage.error('保存菜单失败')
  } finally {
    saving.value = false
  }
}

// 切换菜单状态
const toggleStatus = async (menu: Menu) => {
  menu.statusLoading = true
  try {
    await toggleMenuStatus(menu.id, menu.isActive)
    ElMessage.success(`菜单已${menu.isActive ? '启用' : '禁用'}`)
  } catch (error) {
    // 回滚状态
    menu.isActive = !menu.isActive
    console.error('切换菜单状态失败:', error)
    ElMessage.error('切换菜单状态失败')
  } finally {
    menu.statusLoading = false
  }
}

const deleteMenu = async (menu: Menu) => {
  try {
    const hasChildren = menu.children && menu.children.length > 0
    const confirmText = hasChildren 
      ? `确定要删除菜单"${menu.name}"吗？此操作会同时删除所有子菜单，且不可恢复！`
      : `确定要删除菜单"${menu.name}"吗？此操作不可恢复！`
    
    await ElMessageBox.confirm(confirmText, '警告', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await deleteMenuApi(menu.id)
    ElMessage.success('删除菜单成功')
    await loadMenus()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除菜单失败:', error)
      ElMessage.error('删除菜单失败')
    }
  }
}

// 同步路由
const syncWithRoutes = async () => {
  syncing.value = true
  try {
    // 获取所有路由
    const routes = router.getRoutes().filter(route => 
      route.path.startsWith('/admin') && 
      route.path !== '/admin' &&
      !route.meta?.hideInMenu
    )
    
    // 比较菜单和路由
    const comparison = compareMenuWithRoutes(menus.value, routes)
    const suggestions = suggestMenuUpdates(comparison)
    
    if (suggestions.length === 0) {
      ElMessage.success('菜单与路由已同步，无需更新')
      return
    }
    
    // 显示同步建议
    const suggestionText = suggestions.join('\n\n')
    await ElMessageBox.confirm(
      `检测到以下差异：\n\n${suggestionText}\n\n是否要自动同步菜单结构？`,
      '路由同步建议',
      {
        confirmButtonText: '自动同步',
        cancelButtonText: '取消',
        type: 'info',
        customClass: 'sync-dialog'
      }
    )
    
    // 这里可以实现自动同步逻辑
    ElMessage.info('自动同步功能开发中，请手动调整菜单结构')
    
  } catch (error) {
    if (error !== 'cancel') {
      console.error('同步路由失败:', error)
      ElMessage.error('同步路由失败')
    }
  } finally {
    syncing.value = false
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

.header-actions {
  display: flex;
  gap: 12px;
}

.menu-name {
  display: flex;
  align-items: center;
  gap: 8px;
}

.menu-icon {
  color: #606266;
}

.text-gray {
  color: #909399;
}

.text-green {
  color: #67c23a;
}

:deep(.el-table) {
  font-size: 14px;
}

:deep(.el-table .el-table__cell) {
  padding: 12px 0;
}

:deep(.el-dialog) {
  max-height: 90vh;
  overflow-y: auto;
}

:deep(.el-form-item__label) {
  font-weight: 500;
}

.el-tag {
  font-family: 'Monaco', 'Consolas', monospace;
}
</style>