<template>
  <div class="permission-tree-selector">
    <!-- 搜索框 -->
    <el-input
      v-model="searchText"
      placeholder="搜索权限..."
      clearable
      class="search-input"
    >
      <template #prefix>
        <el-icon><Search /></el-icon>
      </template>
    </el-input>

    <!-- 权限预设模板 -->
    <div class="permission-templates">
      <span class="template-label">快速选择：</span>
      <el-button-group>
        <el-button size="small" @click="applyTemplate('admin')">管理员</el-button>
        <el-button size="small" @click="applyTemplate('editor')">编辑员</el-button>
        <el-button size="small" @click="applyTemplate('viewer')">查看员</el-button>
        <el-button size="small" @click="clearAll">清空</el-button>
      </el-button-group>
    </div>

    <!-- 权限树 -->
    <div class="permission-tree-container">
      <el-tree
        ref="treeRef"
        :data="treeData"
        :props="treeProps"
        show-checkbox
        node-key="id"
        :default-checked-keys="localSelected"
        :filter-node-method="filterNode"
        :check-strictly="false"
        @check="handleTreeCheck"
        class="permission-tree"
      >
        <template #default="{ node, data }">
          <div class="tree-node-content">
            <div class="node-main">
              <el-icon v-if="data.icon" class="node-icon">
                <component :is="data.icon" />
              </el-icon>
              <span class="node-label">{{ data.label }}</span>
              <el-tag 
                v-if="data.type === 'module'" 
                size="small" 
                type="info"
                class="node-count"
              >
                {{ getModulePermissionCount(data) }}
              </el-tag>
            </div>
            <div v-if="data.description" class="node-description">
              {{ data.description }}
            </div>
          </div>
        </template>
      </el-tree>
    </div>

    <!-- 已选权限数量统计 -->
    <div class="selection-summary">
      <el-tag type="info" size="small">
        已选择 {{ checkedPermissions.length }} 个权限
      </el-tag>
      <el-button 
        size="small" 
        type="primary" 
        link 
        @click="toggleExpandAll"
        class="expand-btn"
      >
        {{ isAllExpanded ? '收起全部' : '展开全部' }}
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import { Search, Document, User, Setting, ShoppingBag, Picture, Menu as MenuIcon } from '@element-plus/icons-vue'
import type { Permission } from '@/api/roles'

interface TreeNode {
  id: string
  label: string
  type: 'module' | 'permission'
  icon?: any
  description?: string
  permissionId?: number
  children?: TreeNode[]
}

interface Props {
  permissions: Permission[]
  modelValue: number[]
}

interface Emits {
  (e: 'update:modelValue', value: number[]): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const searchText = ref('')
const treeRef = ref()
const isAllExpanded = ref(false)

// 树组件配置
const treeProps = {
  children: 'children',
  label: 'label'
}

// 本地选择状态
const localSelected = computed<string[]>({
  get() {
    return (props.modelValue || []).map(id => `permission_${id}`)
  },
  set(value: string[]) {
    const permissionIds = value
      .filter(id => id.startsWith('permission_'))
      .map(id => parseInt(id.replace('permission_', '')))
      .filter(id => !isNaN(id))
    emit('update:modelValue', permissionIds)
  }
})

// 获取当前选中的权限
const checkedPermissions = computed(() => {
  if (!treeRef.value) return []
  const checkedKeys = treeRef.value.getCheckedKeys() || []
  return checkedKeys.filter((key: string) => key.startsWith('permission_'))
})

// 构建树形数据
const treeData = computed((): TreeNode[] => {
  const moduleMap = new Map<string, TreeNode>()
  
  // 模块图标映射
  const moduleIcons = {
    '仪表盘': Document,
    '用户管理': User,
    '角色管理': User,
    '文章管理': Document,
    '产品管理': ShoppingBag,
    '分类管理': MenuIcon,
    '轮播图管理': Picture,
    '页面管理': Document,
    '文件管理': Document,
    'SEO设置': Setting,
    '联系我们': Document,
    '会员管理': User,
    '菜单管理': MenuIcon,
    '系统设置': Setting
  }
  
  props.permissions.forEach(permission => {
    const prefix = permission.name.split(':')[0] || '其他'
    const moduleName = getModuleDisplayName(prefix)
    
    // 创建或获取模块节点
    if (!moduleMap.has(moduleName)) {
      moduleMap.set(moduleName, {
        id: `module_${prefix}`,
        label: moduleName,
        type: 'module',
        icon: moduleIcons[moduleName as keyof typeof moduleIcons],
        children: []
      })
    }
    
    const moduleNode = moduleMap.get(moduleName)!
    
    // 添加权限节点
    moduleNode.children!.push({
      id: `permission_${permission.id}`,
      label: getPermissionDisplayName(permission.name),
      type: 'permission',
      description: permission.description,
      permissionId: permission.id
    })
  })
  
  // 排序并返回
  const result = Array.from(moduleMap.values()).sort((a, b) => a.label.localeCompare(b.label))
  result.forEach(module => {
    if (module.children) {
      module.children.sort((a, b) => a.label.localeCompare(b.label))
    }
  })
  
  return result
})

// 获取模块显示名称
const getModuleDisplayName = (prefix: string): string => {
  const groupNames: { [key: string]: string } = {
    'dashboard': '仪表盘',
    'users': '用户管理',
    'roles': '角色管理',
    'articles': '文章管理',
    'products': '产品管理',
    'categories': '分类管理',
    'banners': '轮播图管理',
    'pages': '页面管理',
    'files': '文件管理',
    'seo': 'SEO设置',
    'contacts': '联系我们',
    'members': '会员管理',
    'menus': '菜单管理',
    'system': '系统设置'
  }
  return groupNames[prefix] || prefix
}

// 获取权限显示名称
const getPermissionDisplayName = (permissionName: string): string => {
  const parts = permissionName.split(':')
  if (parts.length > 1) {
    const action = parts[1]
    const actionNames: { [key: string]: string } = {
      'view': '查看',
      'list': '列表',
      'create': '创建',
      'edit': '编辑',
      'update': '更新',
      'delete': '删除',
      'manage': '管理',
      'upload': '上传',
      'download': '下载'
    }
    return actionNames[action] || action
  }
  return permissionName
}

// 获取模块权限数量
const getModulePermissionCount = (moduleNode: TreeNode): string => {
  const total = moduleNode.children?.length || 0
  if (!treeRef.value) return `${total}项`
  
  const checkedKeys = treeRef.value.getCheckedKeys() || []
  const checkedCount = moduleNode.children?.filter(child => 
    checkedKeys.includes(child.id)
  ).length || 0
  
  return `${checkedCount}/${total}`
}

// 树节点选择处理
const handleTreeCheck = () => {
  nextTick(() => {
    if (treeRef.value) {
      const checkedKeys = treeRef.value.getCheckedKeys()
      const permissionKeys = checkedKeys.filter((key: string) => key.startsWith('permission_'))
      localSelected.value = permissionKeys
    }
  })
}

// 搜索过滤
const filterNode = (value: string, data: TreeNode) => {
  if (!value) return true
  const searchLower = value.toLowerCase()
  
  // 搜索节点标签和描述
  const matchLabel = data.label.toLowerCase().includes(searchLower)
  const matchDesc = data.description?.toLowerCase().includes(searchLower)
  
  // 如果是模块节点，检查子节点是否匹配
  if (data.type === 'module' && data.children) {
    const hasMatchingChild = data.children.some(child => 
      child.label.toLowerCase().includes(searchLower) ||
      child.description?.toLowerCase().includes(searchLower)
    )
    return matchLabel || matchDesc || hasMatchingChild
  }
  
  return matchLabel || matchDesc
}

// 监听搜索文本变化
watch(searchText, (val) => {
  if (treeRef.value) {
    treeRef.value.filter(val)
  }
})

// 权限模板
const permissionTemplates = {
  admin: () => props.permissions.map(p => p.id),
  editor: () => props.permissions
    .filter(p => !p.name.includes('delete') && !p.name.includes('users:') && !p.name.includes('roles:'))
    .map(p => p.id),
  viewer: () => props.permissions
    .filter(p => p.name.includes('view') || p.name.includes('list'))
    .map(p => p.id)
}

// 应用权限模板
const applyTemplate = (template: keyof typeof permissionTemplates) => {
  const permissionIds = permissionTemplates[template]()
  emit('update:modelValue', permissionIds)
  
  // 更新树的选中状态
  nextTick(() => {
    if (treeRef.value) {
      const keys = permissionIds.map(id => `permission_${id}`)
      treeRef.value.setCheckedKeys(keys)
    }
  })
}

// 清空选择
const clearAll = () => {
  emit('update:modelValue', [])
  nextTick(() => {
    if (treeRef.value) {
      treeRef.value.setCheckedKeys([])
    }
  })
}

// 展开/收起全部
const toggleExpandAll = () => {
  if (!treeRef.value) return
  
  isAllExpanded.value = !isAllExpanded.value
  
  const expandKeys = isAllExpanded.value 
    ? treeData.value.map(node => node.id)
    : []
  
  // 展开/收起所有节点
  treeData.value.forEach(node => {
    treeRef.value.store.nodesMap[node.id].expanded = isAllExpanded.value
  })
}
</script>

<style scoped>
.permission-tree-selector {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.search-input {
  margin-bottom: 16px;
}

.permission-templates {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
  padding: 12px;
  background: #f5f7fa;
  border-radius: 4px;
  flex-shrink: 0;
}

.template-label {
  font-size: 14px;
  color: #606266;
  white-space: nowrap;
}

.permission-tree-container {
  flex: 1;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
  min-height: 300px;
}

.permission-tree {
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
  gap: 6px;
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

.node-count {
  margin-left: 8px;
  font-size: 12px;
}

.node-description {
  font-size: 12px;
  color: #909399;
  margin-top: 2px;
  line-height: 1.3;
}

.selection-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid #ebeef5;
  flex-shrink: 0;
}

.expand-btn {
  padding: 0;
  height: auto;
}

:deep(.el-tree-node__content) {
  height: auto;
  min-height: 32px;
  padding: 6px 8px;
}

:deep(.el-tree-node__content:hover) {
  background-color: #f5f7fa;
}

:deep(.el-tree-node > .el-tree-node__children) {
  padding-left: 20px;
}

:deep(.el-tree-node__expand-icon) {
  color: #c0c4cc;
}

:deep(.el-tree-node__expand-icon.expanded) {
  color: #409eff;
}

/* 模块节点样式 */
:deep(.el-tree-node[data-type="module"] > .el-tree-node__content) {
  background-color: #fafbfc;
  font-weight: 500;
}

:deep(.el-tree-node[data-type="module"] > .el-tree-node__content:hover) {
  background-color: #f0f2f5;
}
</style>
