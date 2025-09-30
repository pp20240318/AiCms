<template>
  <div class="tab-bar">
    <el-tabs
      v-model="activeTab"
      type="card"
      class="tabs-container"
      @tab-click="handleTabClick"
      @tab-remove="handleTabRemove"
    >
      <el-tab-pane
        v-for="tab in tabs"
        :key="tab.path"
        :label="tab.title"
        :name="tab.path"
        :closable="tab.closable"
      >
        <template #label>
          <div
            class="tab-label"
            @contextmenu.prevent="handleContextMenu($event, tab)"
          >
            <el-icon v-if="tab.icon" class="tab-icon">
              <component :is="tab.icon" />
            </el-icon>
            <span class="tab-title">{{ tab.title }}</span>
          </div>
        </template>
      </el-tab-pane>
    </el-tabs>

    <!-- 右键菜单 -->
    <div
      v-show="contextMenuVisible"
      ref="contextMenuEl"
      class="context-menu"
      :style="contextMenuStyle"
      @click="closeContextMenu"
    >
      <div class="menu-item" @click.stop="handleMenuCommand('refresh')">
        <el-icon><Refresh /></el-icon>
        <span>刷新</span>
      </div>
      <div
        class="menu-item"
        :class="{ disabled: !currentContextTab?.closable }"
        @click.stop="handleMenuCommand('close')"
      >
        <el-icon><Close /></el-icon>
        <span>关闭</span>
      </div>
      <div
        class="menu-item"
        :class="{ disabled: tabs.length <= 1 }"
        @click.stop="handleMenuCommand('closeOthers')"
      >
        <el-icon><CloseBold /></el-icon>
        <span>关闭其他</span>
      </div>
      <div
        class="menu-item"
        :class="{ disabled: !hasClosableTabs }"
        @click.stop="handleMenuCommand('closeAll')"
      >
        <el-icon><CircleClose /></el-icon>
        <span>关闭所有</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, nextTick, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTabsStore, type TabItem } from '@/stores/tabs'
import {
  Refresh,
  Close,
  CloseBold,
  CircleClose
} from '@element-plus/icons-vue'

const router = useRouter()
const tabsStore = useTabsStore()
const contextMenuEl = ref<HTMLElement>()
const currentContextTab = ref<TabItem | null>(null)
const contextMenuVisible = ref(false)
const contextMenuStyle = ref({ left: '0px', top: '0px' })

// 计算属性
const tabs = computed(() => tabsStore.tabs)
const activeTab = computed({
  get: () => tabsStore.activeTab,
  set: (value: string) => {
    tabsStore.switchTab(value)
    router.push(value)
  }
})

const hasClosableTabs = computed(() =>
  tabs.value.some(tab => tab.closable)
)

// 处理tab点击
const handleTabClick = (tab: any) => {
  const targetPath = tab.paneName as string
  tabsStore.switchTab(targetPath)
  router.push(targetPath)
}

// 处理tab关闭
const handleTabRemove = (targetPath: string) => {
  const targetTab = tabs.value.find(tab => tab.path === targetPath)
  if (targetTab?.closable) {
    tabsStore.closeTab(targetPath)

    // 如果关闭的是当前tab，路由会自动切换到新的activeTab
    if (tabsStore.activeTab && tabsStore.activeTab !== targetPath) {
      router.push(tabsStore.activeTab)
    }
  }
}

// 处理右键菜单
const handleContextMenu = (event: MouseEvent, tab: TabItem) => {
  event.preventDefault()
  currentContextTab.value = tab

  // 设置菜单位置
  contextMenuStyle.value = {
    left: event.clientX + 'px',
    top: event.clientY + 'px'
  }

  contextMenuVisible.value = true
}

// 关闭右键菜单
const closeContextMenu = () => {
  contextMenuVisible.value = false
  currentContextTab.value = null
}

// 点击外部关闭菜单
const handleClickOutside = (event: Event) => {
  if (contextMenuVisible.value && contextMenuEl.value && !contextMenuEl.value.contains(event.target as Node)) {
    closeContextMenu()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  document.addEventListener('contextmenu', closeContextMenu)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  document.removeEventListener('contextmenu', closeContextMenu)
})

// 处理菜单命令
const handleMenuCommand = (command: string) => {
  if (!currentContextTab.value) return

  const tab = currentContextTab.value

  switch (command) {
    case 'refresh':
      // 刷新当前页面
      router.go(0)
      break

    case 'close':
      if (!tab.closable) return
      handleTabRemove(tab.path)
      break

    case 'closeOthers':
      if (tabs.value.length <= 1) return
      tabsStore.closeOtherTabs(tab.path)
      if (tabsStore.activeTab !== tab.path) {
        router.push(tab.path)
      }
      break

    case 'closeAll':
      if (!hasClosableTabs.value) return
      tabsStore.closeAllTabs()
      if (tabsStore.activeTab) {
        router.push(tabsStore.activeTab)
      }
      break
  }

  closeContextMenu()
}
</script>

<style scoped>
.tab-bar {
  position: relative;
  background: #fff;
  border-bottom: 1px solid #e4e7ed;
}

.tabs-container {
  --el-tabs-header-height: 40px;
}

:deep(.el-tabs__header) {
  margin: 0;
  border-bottom: none;
}

:deep(.el-tabs__nav-wrap) {
  padding: 0 16px;
}

:deep(.el-tabs__item) {
  height: 32px;
  line-height: 32px;
  margin-right: 4px;
  border: 1px solid #d9d9d9;
  border-radius: 4px 4px 0 0;
  padding: 0 12px;
  font-size: 12px;
  color: #666;
  background: #fafafa;
  border-bottom-color: transparent;
}

:deep(.el-tabs__item:hover) {
  color: #409eff;
  border-color: #409eff;
}

:deep(.el-tabs__item.is-active) {
  color: #409eff;
  background: #fff;
  border-color: #409eff;
  border-bottom-color: #fff;
  position: relative;
  z-index: 1;
}

:deep(.el-tabs__item .el-icon-close) {
  width: 14px;
  height: 14px;
  font-size: 12px;
  margin-left: 4px;
  border-radius: 50%;
  transition: all 0.2s;
}

:deep(.el-tabs__item .el-icon-close:hover) {
  background: #c0c4cc;
  color: #fff;
}

.tab-label {
  display: flex;
  align-items: center;
  gap: 4px;
  user-select: none;
}

.tab-icon {
  font-size: 14px;
}

.tab-title {
  max-width: 120px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.context-menu {
  position: fixed;
  z-index: 9999;
  background: #fff;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  padding: 4px 0;
  min-width: 120px;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  font-size: 13px;
  color: #606266;
  cursor: pointer;
  transition: all 0.2s;
}

.menu-item:hover:not(.disabled) {
  background: #f5f7fa;
  color: #409eff;
}

.menu-item.disabled {
  color: #c0c4cc;
  cursor: not-allowed;
}

.menu-item .el-icon {
  font-size: 14px;
}
</style>