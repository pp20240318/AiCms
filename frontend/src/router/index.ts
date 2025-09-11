import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/dashboard'
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/test',
      name: 'Test',
      component: () => import('@/views/Test.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/simple-dashboard',
      name: 'SimpleDashboard',
      component: () => import('@/views/SimpleDashboard.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      component: () => import('@/views/Layout/index.vue'),
      meta: { requiresAuth: true },
      children: [
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: () => import('@/views/Dashboard/index.vue')
        },
        {
          path: 'users',
          name: 'Users',
          component: () => import('@/views/Users/index.vue')
        },
        {
          path: 'roles',
          name: 'Roles',
          component: () => import('@/views/Roles/index.vue')
        },
        {
          path: 'articles',
          name: 'Articles',
          component: () => import('@/views/Articles/index.vue')
        },
        {
          path: 'products',
          name: 'Products',
          component: () => import('@/views/Products/index.vue')
        },
        {
          path: 'categories',
          name: 'Categories',
          component: () => import('@/views/Categories/index.vue')
        },
        {
          path: 'menus',
          name: 'Menus',
          component: () => import('@/views/Menus/index.vue')
        },
        {
          path: 'banners',
          name: 'Banners',
          component: () => import('@/views/Banners/index.vue')
        }
      ]
    }
  ]
})

// 路由守卫
router.beforeEach((to, from, next) => {
  const userStore = useUserStore()
  
  // 检查路由是否需要认证
  if (to.meta.requiresAuth !== false) {
    // 如果用户未登录，跳转到登录页
    if (!userStore.isLoggedIn) {
      next('/login')
      return
    }
  } else {
    // 如果是登录页且用户已登录，跳转到仪表盘
    if (to.path === '/login' && userStore.isLoggedIn) {
      next('/dashboard')
      return
    }
  }
  
  next()
})

export default router