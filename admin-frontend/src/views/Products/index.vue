<template>
  <div class="products">
    <div class="page-header">
      <h1>产品管理</h1>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新增产品
      </el-button>
    </div>
    
    <el-card>
      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="搜索产品名称"
          @input="handleSearch"
          style="width: 300px"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>
        
        <el-select
          v-model="statusFilter"
          placeholder="状态"
          @change="handleSearch"
          style="width: 120px; margin-left: 12px"
        >
          <el-option label="全部" value="" />
          <el-option label="上架" value="active" />
          <el-option label="下架" value="inactive" />
          <el-option label="缺货" value="outOfStock" />
        </el-select>
      </div>
      
      <el-table
        v-loading="loading"
        :data="products"
        style="width: 100%"
      >
        <el-table-column label="图片" width="100">
          <template #default="{ row }">
            <el-image
              v-if="row.images && row.images.length > 0"
              :src="row.images[0]"
              style="width: 60px; height: 60px"
              fit="cover"
            />
          </template>
        </el-table-column>
        <el-table-column prop="name" label="产品名称" min-width="200" />
        <el-table-column prop="categoryName" label="分类" width="120" />
        <el-table-column label="价格" width="120">
          <template #default="{ row }">
            ¥{{ row.price }}
          </template>
        </el-table-column>
        <el-table-column prop="stock" label="库存" width="100" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag
              :type="getStatusType(row.status)"
              size="small"
            >
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="推荐" width="80">
          <template #default="{ row }">
            <el-tag v-if="row.featured" type="success" size="small">推荐</el-tag>
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
            <el-button size="small" type="danger" @click="deleteProduct(row)">删除</el-button>
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
          @size-change="loadProducts"
          @current-change="loadProducts"
        />
      </div>
    </el-card>
    
    <!-- 产品对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="editingProduct ? '编辑产品' : '新增产品'"
      width="800px"
    >
      <el-form
        ref="formRef"
        :model="productForm"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="产品名称" prop="name">
          <el-input v-model="productForm.name" placeholder="请输入产品名称" />
        </el-form-item>
        <el-form-item label="产品描述" prop="description">
          <el-input
            v-model="productForm.description"
            type="textarea"
            :rows="4"
            placeholder="请输入产品描述"
          />
        </el-form-item>
        <el-form-item label="分类" prop="categoryId">
          <el-select v-model="productForm.categoryId" placeholder="请选择分类" style="width: 100%">
            <el-option
              v-for="category in categories"
              :key="category.id"
              :label="category.name"
              :value="category.id"
            />
          </el-select>
        </el-form-item>
        <el-row :gutter="24">
          <el-col :span="12">
            <el-form-item label="售价" prop="price">
              <el-input-number
                v-model="productForm.price"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="原价" prop="originalPrice">
              <el-input-number
                v-model="productForm.originalPrice"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="库存" prop="stock">
          <el-input-number
            v-model="productForm.stock"
            :min="0"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="productForm.status">
            <el-radio label="active">上架</el-radio>
            <el-radio label="inactive">下架</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="推荐产品">
          <el-switch v-model="productForm.featured" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveProduct" :loading="saving">
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
import { getProducts, createProduct, updateProduct, deleteProduct as deleteProductApi } from '@/api/products'
import { getCategories } from '@/api/categories'
import type { Product, CreateProductData, UpdateProductData } from '@/api/products'
import type { Category } from '@/api/categories'
import dayjs from 'dayjs'

const loading = ref(false)
const saving = ref(false)
const products = ref<Product[]>([])
const categories = ref<Category[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)
const searchText = ref('')
const statusFilter = ref('')

const dialogVisible = ref(false)
const editingProduct = ref<Product | null>(null)
const formRef = ref()
const productForm = reactive({
  name: '',
  description: '',
  categoryId: null as number | null,
  price: 0,
  originalPrice: 0,
  stock: 0,
  status: 'active' as 'active' | 'inactive',
  featured: false,
  images: [] as string[],
  specifications: {}
})

const formRules = {
  name: [
    { required: true, message: '请输入产品名称', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入产品描述', trigger: 'blur' }
  ],
  categoryId: [
    { required: true, message: '请选择分类', trigger: 'change' }
  ],
  price: [
    { required: true, message: '请输入售价', trigger: 'blur' }
  ],
  originalPrice: [
    { required: true, message: '请输入原价', trigger: 'blur' }
  ],
  stock: [
    { required: true, message: '请输入库存', trigger: 'blur' }
  ]
}

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY-MM-DD HH:mm')
}

const getStatusType = (status: string) => {
  switch (status) {
    case 'active':
      return 'success'
    case 'inactive':
      return 'danger'
    case 'outOfStock':
      return 'warning'
    default:
      return ''
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'active':
      return '上架'
    case 'inactive':
      return '下架'
    case 'outOfStock':
      return '缺货'
    default:
      return status
  }
}

const loadProducts = async () => {
  loading.value = true
  try {
    const response = await getProducts({
      page: currentPage.value,
      pageSize: pageSize.value,
      search: searchText.value,
      status: statusFilter.value
    })
    products.value = response.items
    total.value = response.total
  } catch (error) {
    ElMessage.error('加载产品列表失败')
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  try {
    categories.value = await getCategories()
  } catch (error) {
    ElMessage.error('加载分类列表失败')
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadProducts()
}

const showCreateDialog = () => {
  editingProduct.value = null
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (product: Product) => {
  editingProduct.value = product
  productForm.name = product.name
  productForm.description = product.description
  productForm.categoryId = product.categoryId
  productForm.price = product.price
  productForm.originalPrice = product.originalPrice
  productForm.stock = product.stock
  productForm.status = product.status as 'active' | 'inactive'
  productForm.featured = product.featured
  productForm.images = [...product.images]
  productForm.specifications = { ...product.specifications }
  dialogVisible.value = true
}

const resetForm = () => {
  productForm.name = ''
  productForm.description = ''
  productForm.categoryId = null
  productForm.price = 0
  productForm.originalPrice = 0
  productForm.stock = 0
  productForm.status = 'active'
  productForm.featured = false
  productForm.images = []
  productForm.specifications = {}
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const saveProduct = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    saving.value = true
    
    if (editingProduct.value) {
      const updateData: UpdateProductData = {
        name: productForm.name,
        description: productForm.description,
        categoryId: productForm.categoryId!,
        price: productForm.price,
        originalPrice: productForm.originalPrice,
        stock: productForm.stock,
        status: productForm.status,
        featured: productForm.featured,
        images: productForm.images,
        specifications: productForm.specifications
      }
      await updateProduct(editingProduct.value.id, updateData)
      ElMessage.success('更新产品成功')
    } else {
      const createData: CreateProductData = {
        name: productForm.name,
        description: productForm.description,
        categoryId: productForm.categoryId!,
        price: productForm.price,
        originalPrice: productForm.originalPrice,
        stock: productForm.stock,
        status: productForm.status,
        featured: productForm.featured,
        images: productForm.images,
        specifications: productForm.specifications
      }
      await createProduct(createData)
      ElMessage.success('创建产品成功')
    }
    
    dialogVisible.value = false
    loadProducts()
  } catch (error) {
    ElMessage.error('保存产品失败')
  } finally {
    saving.value = false
  }
}

const deleteProduct = async (product: Product) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除产品"${product.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteProductApi(product.id)
    ElMessage.success('删除产品成功')
    loadProducts()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除产品失败')
    }
  }
}

onMounted(() => {
  loadProducts()
  loadCategories()
})
</script>

<style scoped>
.products {
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
  display: flex;
  align-items: center;
}

.pagination {
  margin-top: 16px;
  text-align: right;
}
</style>