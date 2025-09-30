<template>
  <div class="rich-editor">
    <div class="editor-toolbar">
      <div class="toolbar-section">
        <el-button-group>
          <el-tooltip content="上传图片" placement="top">
            <el-button size="small" @click="handleImageUpload">
              <el-icon><Picture /></el-icon>
            </el-button>
          </el-tooltip>
          <el-tooltip content="插入链接" placement="top">
            <el-button size="small" @click="showLinkDialog">
              <el-icon><Link /></el-icon>
            </el-button>
          </el-tooltip>
          <el-tooltip content="全屏编辑" placement="top">
            <el-button size="small" @click="toggleFullscreen">
              <el-icon><FullScreen /></el-icon>
            </el-button>
          </el-tooltip>
        </el-button-group>
      </div>
    </div>
    
    <div :class="['editor-container', { 'fullscreen': isFullscreen }]">
      <QuillEditor
        ref="quillRef"
        v-model:content="content"
        :options="editorOptions"
        :placeholder="placeholder"
        @update:content="handleContentChange"
        content-type="html"
        class="quill-editor"
      />
    </div>

    <!-- 图片上传 -->
    <input
      ref="imageInput"
      type="file"
      accept="image/*"
      style="display: none"
      @change="uploadImage"
    />

    <!-- 链接插入对话框 -->
    <el-dialog
      v-model="linkDialogVisible"
      title="插入链接"
      width="400px"
    >
      <el-form :model="linkForm" label-width="80px">
        <el-form-item label="链接文字">
          <el-input v-model="linkForm.text" placeholder="请输入链接文字" />
        </el-form-item>
        <el-form-item label="链接地址">
          <el-input v-model="linkForm.url" placeholder="请输入链接地址" />
        </el-form-item>
        <el-form-item label="打开方式">
          <el-radio-group v-model="linkForm.target">
            <el-radio value="_self">当前窗口</el-radio>
            <el-radio value="_blank">新窗口</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="linkDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="insertLink">确认</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { QuillEditor } from '@vueup/vue-quill'
import { ElMessage } from 'element-plus'
import { Picture, Link, FullScreen } from '@element-plus/icons-vue'
import { uploadImage } from '@/api/upload'
import '@vueup/vue-quill/dist/vue-quill.snow.css'

interface Props {
  modelValue?: string
  placeholder?: string
  height?: string
  disabled?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  placeholder: '请输入内容...',
  height: '400px',
  disabled: false
})

const emit = defineEmits<Emits>()

const quillRef = ref()
const imageInput = ref<HTMLInputElement>()
const isFullscreen = ref(false)
const linkDialogVisible = ref(false)

// 编辑器内容
const content = computed({
  get: () => props.modelValue,
  set: (value: string) => {
    emit('update:modelValue', value)
    emit('change', value)
  }
})

// 链接表单
const linkForm = reactive({
  text: '',
  url: '',
  target: '_self'
})

// 编辑器配置
const editorOptions = computed(() => ({
  theme: 'snow',
  placeholder: props.placeholder,
  readOnly: props.disabled,
  modules: {
    toolbar: [
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      [{ 'size': ['small', false, 'large', 'huge'] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ 'color': [] }, { 'background': [] }],
      [{ 'font': [] }],
      [{ 'align': [] }],
      [{ 'list': 'ordered' }, { 'list': 'bullet' }],
      [{ 'indent': '-1' }, { 'indent': '+1' }],
      ['blockquote', 'code-block'],
      ['clean']
    ],
    history: {
      delay: 2000,
      maxStack: 500,
      userOnly: true
    }
  }
}))

// 处理内容变化
const handleContentChange = (newContent: string) => {
  content.value = newContent
}

// 处理图片上传
const handleImageUpload = () => {
  if (imageInput.value) {
    imageInput.value.click()
  }
}

// 上传图片
const uploadImage = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (!file) return

  // 检查文件类型
  if (!file.type.startsWith('image/')) {
    ElMessage.error('请选择图片文件')
    return
  }

  // 检查文件大小 (5MB)
  if (file.size > 5 * 1024 * 1024) {
    ElMessage.error('图片大小不能超过5MB')
    return
  }

  try {
    ElMessage.info('正在上传图片...')
    const response = await uploadImage(file)
    const imageUrl = response.url
    insertImageToEditor(imageUrl)
    ElMessage.success('图片上传成功')
  } catch (error) {
    console.error('图片上传失败:', error)
    ElMessage.error('图片上传失败，请重试')
  } finally {
    // 清空 input
    if (target) {
      target.value = ''
    }
  }
}

// 插入图片到编辑器
const insertImageToEditor = (imageUrl: string) => {
  const quill = quillRef.value?.getQuill()
  if (quill) {
    const range = quill.getSelection(true)
    quill.insertEmbed(range?.index || 0, 'image', imageUrl)
    quill.setSelection((range?.index || 0) + 1)
  }
}

// 显示链接对话框
const showLinkDialog = () => {
  const quill = quillRef.value?.getQuill()
  if (quill) {
    const range = quill.getSelection()
    if (range && range.length > 0) {
      linkForm.text = quill.getText(range.index, range.length)
    } else {
      linkForm.text = ''
    }
  }
  linkForm.url = ''
  linkForm.target = '_self'
  linkDialogVisible.value = true
}

// 插入链接
const insertLink = () => {
  if (!linkForm.text || !linkForm.url) {
    ElMessage.warning('请填写链接文字和链接地址')
    return
  }

  const quill = quillRef.value?.getQuill()
  if (quill) {
    const range = quill.getSelection(true)
    
    if (range && range.length > 0) {
      // 如果有选中文本，为选中文本添加链接
      quill.format('link', linkForm.url)
    } else {
      // 如果没有选中文本，插入新的链接
      quill.insertText(range?.index || 0, linkForm.text)
      quill.setSelection((range?.index || 0), linkForm.text.length)
      quill.format('link', linkForm.url)
      quill.setSelection((range?.index || 0) + linkForm.text.length)
    }
  }

  linkDialogVisible.value = false
}

// 切换全屏
const toggleFullscreen = () => {
  isFullscreen.value = !isFullscreen.value
  
  if (isFullscreen.value) {
    document.body.style.overflow = 'hidden'
  } else {
    document.body.style.overflow = ''
  }
}

// 按键事件处理
const handleKeydown = (event: KeyboardEvent) => {
  // ESC 键退出全屏
  if (event.key === 'Escape' && isFullscreen.value) {
    toggleFullscreen()
  }
}

onMounted(() => {
  document.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown)
  // 清理全屏状态
  if (isFullscreen.value) {
    document.body.style.overflow = ''
  }
})
</script>

<style scoped>
.rich-editor {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  background: #fff;
}

.editor-toolbar {
  padding: 8px 12px;
  border-bottom: 1px solid #ebeef5;
  background: #fafafa;
  border-radius: 4px 4px 0 0;
}

.toolbar-section {
  display: flex;
  align-items: center;
  gap: 8px;
}

.editor-container {
  position: relative;
  height: v-bind(height);
}

.editor-container.fullscreen {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
  height: 100vh;
  background: #fff;
  border-radius: 0;
}

.fullscreen .rich-editor {
  height: 100vh;
  border: none;
  border-radius: 0;
}

.fullscreen .editor-container {
  height: calc(100vh - 60px);
}

:deep(.quill-editor) {
  height: 100%;
}

:deep(.ql-container) {
  height: calc(100% - 42px);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif;
}

:deep(.ql-editor) {
  padding: 12px 15px;
  line-height: 1.6;
  font-size: 14px;
}

:deep(.ql-editor.ql-blank::before) {
  font-style: normal;
  color: #c0c4cc;
}

:deep(.ql-snow .ql-tooltip) {
  z-index: 10000;
}

/* 工具栏样式 */
:deep(.ql-toolbar) {
  border: none;
  border-bottom: 1px solid #ebeef5;
  padding: 8px 12px;
}

:deep(.ql-toolbar .ql-formats) {
  margin-right: 12px;
}

:deep(.ql-snow .ql-tooltip::before) {
  content: '请输入链接地址:';
}

:deep(.ql-snow .ql-tooltip[data-mode="link"]::before) {
  content: '请输入链接地址:';
}

:deep(.ql-snow .ql-tooltip[data-mode="video"]::before) {
  content: '请输入视频地址:';
}
</style>
