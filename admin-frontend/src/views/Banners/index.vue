<template>
  <div class="banners">
    <div class="page-header">
      <h1>轮播图管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增轮播图
      </el-button>
    </div>
    
    <el-card>
      <el-table
        v-loading="loading"
        :data="banners"
        style="width: 100%"
      >
        <el-table-column label="图片" width="120">
          <template #default="{ row }">
            <el-image
              :src="row.imageUrl"
              style="width: 80px; height: 45px"
              fit="cover"
            />
          </template>
        </el-table-column>
        <el-table-column prop="title" label="标题" min-width="200" />
        <el-table-column prop="description" label="描述" min-width="200" />
        <el-table-column prop="linkUrl" label="链接" min-width="200">
          <template #default="{ row }">
            <el-link :href="row.linkUrl" target="_blank" v-if="row.linkUrl">{{ row.linkUrl }}</el-link>
            <span v-else>-</span>
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
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150">
          <template #default="{ row }">
            <el-button size="small" @click="showEditDialog(row)">编辑</el-button>
            <el-button size="small" type="danger" @click="deleteBanner(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 创建/编辑轮播图对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingBanner ? '编辑轮播图' : '新增轮播图'"
      width="600px"
    >
      <el-form
        ref="formRef"
        :model="bannerForm"
        :rules="formRules"
        label-width="80px"
      >
        <el-form-item label="图片" prop="imageUrl">
          <el-upload
            class="banner-uploader"
            action="#"
            :show-file-list="false"
            :before-upload="beforeUpload"
            :http-request="handleUpload"
          >
            <img v-if="bannerForm.imageUrl" :src="bannerForm.imageUrl" class="banner-image" />
            <el-icon v-else class="banner-uploader-icon"><Plus /></el-icon>
          </el-upload>
          <div class="upload-tip">推荐尺寸：1200x400px</div>
        </el-form-item>
        <el-form-item label="标题" prop="title">
          <el-input v-model="bannerForm.title" placeholder="请输入轮播图标题" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input
            v-model="bannerForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入轮播图描述（可选）"
          />
        </el-form-item>
        <el-form-item label="链接">
          <el-input v-model="bannerForm.linkUrl" placeholder="请输入跳转链接（可选）" />
        </el-form-item>
        <el-form-item label="排序" prop="sortOrder">
          <el-input-number
            v-model="bannerForm.sortOrder"
            :min="0"
            :max="9999"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="状态" v-if="editingBanner">
          <el-switch
            v-model="bannerForm.isActive"
            active-text="启用"
            inactive-text="禁用"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveBanner" :loading="saving">
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
import dayjs from 'dayjs'
import { 
  getBanners, 
  createBanner, 
  updateBanner, 
  deleteBanner as deleteBannerApi, 
  toggleBannerStatus,
  type Banner, 
  type CreateBannerRequest 
} from '@/api/banners'
import { uploadImage } from '@/api/upload'

const loading = ref(false)
const saving = ref(false)
const banners = ref<Banner[]>([])

const dialogVisible = ref(false)
const editingBanner = ref<Banner | null>(null)
const formRef = ref()
const bannerForm = reactive<CreateBannerRequest>({
  title: '',
  description: '',
  imageUrl: '',
  linkUrl: '',
  sortOrder: 0,
  isActive: true
})

const formRules = {
  title: [
    { required: true, message: '请输入轮播图标题', trigger: 'blur' }
  ],
  imageUrl: [
    { required: true, message: '请上传轮播图', trigger: 'change' }
  ],
  sortOrder: [
    { required: true, message: '请输入排序值', trigger: 'blur' }
  ]
}

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

const loadBanners = async () => {
  loading.value = true
  try {
    const result = await getBanners()
    banners.value = result.items
  } catch (error) {
    console.error('加载轮播图列表失败:', error)
    ElMessage.error('加载轮播图列表失败')
  } finally {
    loading.value = false
  }
}

const showCreateDialog = () => {
  editingBanner.value = null
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (banner: Banner) => {
  editingBanner.value = banner
  bannerForm.title = banner.title
  bannerForm.description = banner.description || ''
  bannerForm.imageUrl = banner.imageUrl
  bannerForm.linkUrl = banner.linkUrl || ''
  bannerForm.sortOrder = banner.sortOrder
  bannerForm.isActive = banner.isActive
  dialogVisible.value = true
}

const resetForm = () => {
  bannerForm.title = ''
  bannerForm.description = ''
  bannerForm.imageUrl = ''
  bannerForm.linkUrl = ''
  bannerForm.sortOrder = 0
  bannerForm.isActive = true
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const beforeUpload = (file: File) => {
  const isImage = file.type.startsWith('image/')
  const isLt5M = file.size / 1024 / 1024 < 5

  if (!isImage) {
    ElMessage.error('只能上传图片文件!')
    return false
  }
  if (!isLt5M) {
    ElMessage.error('图片大小不能超过 5MB!')
    return false
  }
  return true
}

const handleUpload = async (options: any) => {
  try {
    const response = await uploadImage(options.file)
    bannerForm.imageUrl = response.url
    ElMessage.success('图片上传成功')
  } catch (error) {
    console.error('图片上传失败:', error)
    ElMessage.error('图片上传失败')
  }
}

const saveBanner = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    
    if (editingBanner.value) {
      await updateBanner(editingBanner.value.id, bannerForm)
      ElMessage.success('更新轮播图成功')
    } else {
      await createBanner(bannerForm)
      ElMessage.success('创建轮播图成功')
    }
    
    dialogVisible.value = false
    await loadBanners()
  } catch (error) {
    console.error('保存轮播图失败:', error)
    ElMessage.error('保存轮播图失败')
  } finally {
    saving.value = false
  }
}

const deleteBanner = async (banner: Banner) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除轮播图"${banner.title}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteBannerApi(banner.id)
    ElMessage.success('删除轮播图成功')
    await loadBanners()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除轮播图失败')
    }
  }
}

onMounted(() => {
  loadBanners()
})
</script>

<style scoped>
.banners {
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

.banner-uploader {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: border-color 0.2s;
}

.banner-uploader:hover {
  border-color: #409eff;
}

.banner-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 300px;
  height: 100px;
  text-align: center;
  line-height: 100px;
}

.banner-image {
  width: 300px;
  height: 100px;
  display: block;
  object-fit: cover;
}

.upload-tip {
  font-size: 12px;
  color: #999;
  margin-top: 8px;
}
</style>