<template>
  <div class="menu-permission-selector">
    <!-- 搜索和操作 -->
    <div class="selector-header">
      <el-input
        v-model="searchText"
        placeholder="搜索菜单或权限..."
        clearable
        class="search-input"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>
      
      <div class="header-actions">
        <el-button-group>
          <el-button size="small" @click="expandAll">全部展开</el-button>
          <el-button size="small" @click="collapseAll">全部收起</el-button>
        </el-button-group>
        
        <el-dropdown trigger="click" @command="handleBatchAction">
          <el-button size="small" type="primary">
            批量操作<el-icon class="el-icon--right"><arrow-down /></el-icon>
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="selectAll">选择全部</el-dropdown-item>
              <el-dropdown-item command="selectVisible">仅选择可见菜单</el-dropdown-item>
              <el-dropdown-item command="selectViewOnly">仅查看权限</el-dropdown-item>
              <el-dropdown-item command="selectManageOnly">仅管理权限</el-dropdown-item>
              <el-dropdown-item command="clear" divided>清空选择</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </div>

    <!-- 菜单权限树 -->
    <div class="menu-tree-container">
      <el-tree
        ref="treeRef"
        :data="menuTreeData"
        :props="treeProps"
        show-checkbox
        node-key="id"
        :default-checked-keys="selectedPermissions"
        :filter-node-method="filterNode"
        :check-strictly="false"
        @check="handleTreeCheck"
        class="menu-permission-tree"
      >
        <template #default="{ node, data }">
          <div class="tree-node-content">
            <div class="node-main">
              <!-- 菜单图标 -->
              <el-icon v-if="data.icon" class="node-icon">
                <component :is="getIconComponent(data.icon)" />
              </el-icon>
              
              <!-- 节点标签 -->
              <span class="node-label">{{ data.label }}</span>
              
              <!-- 节点类型标识 -->
              <el-tag 
                :type="getNodeTypeColor(data.type)" 
                size="small" 
                class="node-type-tag"
              >
                {{ getNodeTypeLabel(data.type) }}
              </el-tag>
              
              <!-- 路径显示 -->
              <span v-if="data.path" class="node-path">{{ data.path }}</span>
            </div>
            
            <!-- 描述信息 -->
            <div v-if="data.description" class="node-description">
              {{ data.description }}
            </div>
            
            <!-- 权限操作 -->
            <div v-if="data.type === 'menu' && data.permissions" class="menu-permissions">
              <el-tag
                v-for="permission in data.permissions"
                :key="permission.code"
                size="small"
                :type="isPermissionSelected(permission.code) ? 'success' : 'info'"
                class="permission-tag"
              >
                {{ permission.label }}
              </el-tag>
            </div>
          </div>
        </template>
      </el-tree>
    </div>

    <!-- 选择统计 -->
    <div class="selection-summary">
      <div class="summary-stats">
        <el-tag type="primary" size="small">
          菜单: {{ selectedMenuCount }}/{{ totalMenuCount }}
        </el-tag>
        <el-tag type="success" size="small">
          权限: {{ selectedPermissionCount }}/{{ totalPermissionCount }}
        </el-tag>
      </div>
      
      <el-button 
        type="primary" 
        size="small" 
        link 
        @click="showSelectedDetails"
      >
        查看已选权限
      </el-button>
    </div>

    <!-- 已选权限详情对话框 -->
    <el-dialog
      v-model="detailsVisible"
      title="已选择的权限"
      width="600px"
    >
      <div class="selected-details">
        <div v-for="group in selectedPermissionGroups" :key="group.menuName" class="permission-group">
          <div class="group-header">
            <el-icon class="group-icon">
              <component :is="getIconComponent(group.menuIcon)" />
            </el-icon>
            <span class="group-title">{{ group.menuName }}</span>
            <el-tag size="small" type="info">{{ group.permissions.length }}项</el-tag>
          </div>
          <div class="group-permissions">
            <el-tag
              v-for="permission in group.permissions"
              :key="permission"
              size="small"
              type="success"
            >
              {{ permission }}
            </el-tag>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import { 
  Search, 
  ArrowDown,
  HomeFilled,
  Document,
  User,
  Setting,
  ShoppingBag,
  Picture,
  Menu as MenuIcon,
  FolderOpened,
  EditPen,
  Plus,
  CollectionTag,
  Notebook,
  Postcard
} from '@element-plus/icons-vue'

interface MenuPermission {
  code: string
  label: string
  description?: string
}

interface MenuTreeNode {
  id: string
  label: string
  type: 'menu' | 'permission'
  path?: string
  icon?: string
  description?: string
  permissions?: MenuPermission[]
  children?: MenuTreeNode[]
}

interface Props {
  menus: any[] // 菜单数据
  modelValue: string[] // 选中的权限代码
}

interface Emits {
  (e: 'update:modelValue', value: string[]): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const searchText = ref('')
const treeRef = ref()
const detailsVisible = ref(false)

// 树组件配置
const treeProps = {
  children: 'children',
  label: 'label'
}

// 图标组件映射
const iconComponents = {
  HomeFilled,
  Document,
  User,
  Setting,
  ShoppingBag,
  Picture,
  Menu: MenuIcon,
  FolderOpened,
  EditPen,
  Plus,
  CollectionTag,
  Notebook,
  Postcard
}

// 获取图标组件
const getIconComponent = (iconName: string) => {
  return iconComponents[iconName as keyof typeof iconComponents] || Document
}

// 选中的权限
const selectedPermissions = computed<string[]>({
  get() {
    return props.modelValue || []
  },
  set(value: string[]) {
    emit('update:modelValue', value)
  }
})

// 构建菜单权限树数据
const menuTreeData = computed((): MenuTreeNode[] => {
  return buildMenuTree(props.menus)
})

// 构建菜单树
const buildMenuTree = (menus: any[]): MenuTreeNode[] => {
  return menus.map(menu => {
    const menuNode: MenuTreeNode = {
      id: `menu_${menu.id}`,
      label: menu.name,
      type: 'menu',
      path: menu.path,
      icon: menu.icon,
      description: menu.description,
      permissions: generateMenuPermissions(menu),
      children: []
    }

    // 添加权限子节点
    if (menuNode.permissions) {
      menuNode.children = menuNode.permissions.map(permission => ({
        id: permission.code,
        label: permission.label,
        type: 'permission' as const,
        description: permission.description
      }))
    }

    // 添加子菜单
    if (menu.children && menu.children.length > 0) {
      const childMenus = buildMenuTree(menu.children)
      menuNode.children = [...(menuNode.children || []), ...childMenus]
    }

    return menuNode
  })
}

// 为菜单生成标准权限
const generateMenuPermissions = (menu: any): MenuPermission[] => {
  const permissions: MenuPermission[] = []
  const baseName = menu.name.replace('管理', '')
  
  // 根据菜单路径生成权限模块名
  const getModuleName = (path: string): string => {
    const cleanPath = path.replace('/admin/', '')
    // 映射路径到后端权限模块名
    const pathMappings: Record<string, string> = {
      'dashboard': 'dashboard',
      'articles': 'article',
      'article-categories': 'article',
      'products': 'product', 
      'product-categories': 'product',
      'banners': 'banner',
      'pages': 'page',
      'users': 'user',
      'roles': 'role',
      'members': 'member',
      'menus': 'menu',
      'seo': 'seo',
      'contacts': 'contact'
    }
    return pathMappings[cleanPath] || cleanPath
  }

  const moduleName = getModuleName(menu.path)
  
  // 基础权限
  permissions.push({
    code: `${moduleName}.view`,
    label: `查看${baseName}`,
    description: `可以访问${menu.name}页面`
  })

  // 根据菜单类型添加其他权限
  if (menu.path.includes('articles') || menu.path.includes('products') || 
      menu.path.includes('banners') || menu.path.includes('pages')) {
    permissions.push(
      {
        code: `${moduleName}.create`,
        label: `创建${baseName}`,
        description: `可以创建新的${baseName}`
      },
      {
        code: `${moduleName}.edit`,
        label: `编辑${baseName}`,
        description: `可以编辑${baseName}`
      },
      {
        code: `${moduleName}.delete`,
        label: `删除${baseName}`,
        description: `可以删除${baseName}`
      }
    )
  }

  if (menu.path.includes('users') || menu.path.includes('roles') || menu.path.includes('members')) {
    permissions.push(
      {
        code: `${moduleName}.manage`,
        label: `管理${baseName}`,
        description: `完全管理${baseName}`
      }
    )
  }

  return permissions
}

// 节点类型相关
const getNodeTypeLabel = (type: string): string => {
  const labels = {
    menu: '菜单',
    permission: '权限'
  }
  return labels[type as keyof typeof labels] || type
}

const getNodeTypeColor = (type: string): string => {
  const colors = {
    menu: 'primary',
    permission: 'success'
  }
  return colors[type as keyof typeof colors] || 'info'
}

// 统计信息
const totalMenuCount = computed(() => {
  const countMenus = (nodes: MenuTreeNode[]): number => {
    return nodes.reduce((count, node) => {
      if (node.type === 'menu') {
        count++
        if (node.children) {
          count += countMenus(node.children.filter(child => child.type === 'menu'))
        }
      }
      return count
    }, 0)
  }
  return countMenus(menuTreeData.value)
})

const selectedMenuCount = computed(() => {
  const checkedKeys = treeRef.value?.getCheckedKeys() || []
  return checkedKeys.filter((key: string) => key.startsWith('menu_')).length
})

const totalPermissionCount = computed(() => {
  const countPermissions = (nodes: MenuTreeNode[]): number => {
    return nodes.reduce((count, node) => {
      if (node.type === 'permission') {
        count++
      }
      if (node.children) {
        count += countPermissions(node.children)
      }
      return count
    }, 0)
  }
  return countPermissions(menuTreeData.value)
})

const selectedPermissionCount = computed(() => {
  const checkedKeys = treeRef.value?.getCheckedKeys() || []
  return checkedKeys.filter((key: string) => !key.startsWith('menu_')).length
})

// 判断权限是否被选中
const isPermissionSelected = (permissionCode: string): boolean => {
  return selectedPermissions.value.includes(permissionCode)
}

// 已选权限分组
const selectedPermissionGroups = computed(() => {
  const groups: Array<{
    menuName: string
    menuIcon?: string
    permissions: string[]
  }> = []
  
  // TODO: 实现分组逻辑
  return groups
})

// 树节点选择处理
const handleTreeCheck = () => {
  nextTick(() => {
    if (treeRef.value) {
      const checkedKeys = treeRef.value.getCheckedKeys()
      const permissionKeys = checkedKeys.filter((key: string) => !key.startsWith('menu_'))
      selectedPermissions.value = permissionKeys
    }
  })
}

// 搜索过滤
const filterNode = (value: string, data: MenuTreeNode): boolean => {
  if (!value) return true
  const searchLower = value.toLowerCase()
  return data.label.toLowerCase().includes(searchLower) ||
         data.description?.toLowerCase().includes(searchLower) ||
         data.path?.toLowerCase().includes(searchLower)
}

// 监听搜索
watch(searchText, (val) => {
  if (treeRef.value) {
    treeRef.value.filter(val)
  }
})

// 展开/收起操作
const expandAll = () => {
  const allKeys = getAllNodeKeys(menuTreeData.value)
  allKeys.forEach(key => {
    const node = treeRef.value?.store.nodesMap[key]
    if (node) node.expanded = true
  })
}

const collapseAll = () => {
  const allKeys = getAllNodeKeys(menuTreeData.value)
  allKeys.forEach(key => {
    const node = treeRef.value?.store.nodesMap[key]
    if (node) node.expanded = false
  })
}

const getAllNodeKeys = (nodes: MenuTreeNode[]): string[] => {
  const keys: string[] = []
  nodes.forEach(node => {
    keys.push(node.id)
    if (node.children) {
      keys.push(...getAllNodeKeys(node.children))
    }
  })
  return keys
}

// 批量操作
const handleBatchAction = (command: string) => {
  switch (command) {
    case 'selectAll':
      selectedPermissions.value = getAllPermissionKeys()
      break
    case 'selectVisible':
      // TODO: 实现选择可见菜单
      break
    case 'selectViewOnly':
      selectedPermissions.value = getAllPermissionKeys().filter(key => key.includes('.view'))
      break
    case 'selectManageOnly':
      selectedPermissions.value = getAllPermissionKeys().filter(key => 
        key.includes('.manage') || key.includes('.edit') || key.includes('.delete')
      )
      break
    case 'clear':
      selectedPermissions.value = []
      break
  }
  
  nextTick(() => {
    if (treeRef.value) {
      treeRef.value.setCheckedKeys(selectedPermissions.value)
    }
  })
}

const getAllPermissionKeys = (): string[] => {
  const keys: string[] = []
  const collectKeys = (nodes: MenuTreeNode[]) => {
    nodes.forEach(node => {
      if (node.type === 'permission') {
        keys.push(node.id)
      }
      if (node.children) {
        collectKeys(node.children)
      }
    })
  }
  collectKeys(menuTreeData.value)
  return keys
}

// 显示选择详情
const showSelectedDetails = () => {
  detailsVisible.value = true
}
</script>

<style scoped>
.menu-permission-selector {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.selector-header {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
  align-items: center;
}

.search-input {
  flex: 1;
}

.header-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.menu-tree-container {
  flex: 1;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
  min-height: 400px;
}

.menu-permission-tree {
  height: 100%;
  overflow-y: auto;
  padding: 8px;
}

.tree-node-content {
  flex: 1;
  min-width: 0;
}

.node-main {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
}

.node-icon {
  color: #606266;
  font-size: 16px;
}

.node-label {
  font-size: 14px;
  color: #303133;
  font-weight: 500;
}

.node-type-tag {
  font-size: 12px;
}

.node-path {
  font-size: 12px;
  color: #909399;
  font-family: 'Monaco', 'Consolas', monospace;
  background: #f5f7fa;
  padding: 2px 6px;
  border-radius: 2px;
}

.node-description {
  font-size: 12px;
  color: #909399;
  margin-bottom: 4px;
  line-height: 1.3;
}

.menu-permissions {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  margin-top: 4px;
}

.permission-tag {
  font-size: 12px;
  cursor: pointer;
}

.selection-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid #ebeef5;
}

.summary-stats {
  display: flex;
  gap: 8px;
}

.selected-details {
  max-height: 400px;
  overflow-y: auto;
}

.permission-group {
  margin-bottom: 16px;
}

.permission-group:last-child {
  margin-bottom: 0;
}

.group-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
  padding-bottom: 6px;
  border-bottom: 1px solid #ebeef5;
}

.group-icon {
  color: #409eff;
}

.group-title {
  font-weight: 500;
  color: #303133;
}

.group-permissions {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

:deep(.el-tree-node__content) {
  height: auto;
  min-height: 36px;
  padding: 6px 8px;
}

:deep(.el-tree-node__content:hover) {
  background-color: #f5f7fa;
}

:deep(.el-tree-node[data-type="menu"] > .el-tree-node__content) {
  background-color: #fafbfc;
  border-left: 3px solid #409eff;
}

:deep(.el-tree-node[data-type="permission"] > .el-tree-node__content) {
  margin-left: 12px;
  background-color: #f9f9f9;
}
</style>
