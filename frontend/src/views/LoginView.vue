<template>
  <div class="login-container">
    <div class="login-box">
      <div class="login-header">
        <h1>CMS管理系统</h1>
        <p>欢迎使用内容管理系统</p>
      </div>
      
      <el-form 
        ref="formRef" 
        :model="loginForm" 
        :rules="formRules" 
        class="login-form"
        @keyup.enter="handleLogin"
      >
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            placeholder="用户名"
            size="large"
            :prefix-icon="User"
          />
        </el-form-item>
        
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            type="password"
            placeholder="密码"
            size="large"
            :prefix-icon="Lock"
            show-password
          />
        </el-form-item>
        
        <el-form-item>
          <el-button
            type="primary"
            size="large"
            class="login-button"
            :loading="loading"
            @click="handleLogin"
          >
            登录
          </el-button>
        </el-form-item>
      </el-form>
      
      <div class="login-tips">
        <p>默认账号: admin / admin123</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'

const router = useRouter()
const userStore = useUserStore()

const loading = ref(false)
const formRef = ref()

const loginForm = reactive({
  username: 'admin',
  password: 'admin123'
})

const formRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' }
  ]
}

const handleLogin = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    loading.value = true
    
    console.log('开始登录...')
    await userStore.login({
      username: loginForm.username,
      password: loginForm.password
    })
    
    console.log('登录成功，准备跳转...')
    console.log('用户登录状态:', userStore.isLoggedIn)
    
    // 使用 nextTick 确保状态更新后再跳转
    await new Promise(resolve => setTimeout(resolve, 100))
    
    // 先尝试跳转到简化仪表盘测试
    try {
      await router.push('/simple-dashboard')
      console.log('Vue Router跳转到简化仪表盘完成')
    } catch (routerError) {
      console.error('Vue Router跳转失败:', routerError)
      // 如果Vue Router失败，使用原生跳转
      console.log('使用原生跳转...')
      window.location.href = '/simple-dashboard'
    }
  } catch (error) {
    console.error('登录失败:', error)
    // 错误处理已在store中完成
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-container {
  height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-box {
  background: white;
  border-radius: 10px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  padding: 40px;
  width: 400px;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h1 {
  color: #333;
  margin: 0 0 10px 0;
  font-size: 28px;
}

.login-header p {
  color: #666;
  margin: 0;
  font-size: 14px;
}

.login-form {
  margin-top: 30px;
}

.login-button {
  width: 100%;
}

.login-tips {
  margin-top: 20px;
  text-align: center;
}

.login-tips p {
  color: #999;
  font-size: 12px;
  margin: 0;
}
</style>