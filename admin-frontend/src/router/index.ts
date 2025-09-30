import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { checkRoutePermission } from '@/utils/permission'
import { ElMessage } from 'element-plus'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      redirect: '/dashboard'
    },
    {
      path: '/dashboard',
      redirect: '/admin/dashboard'
    },
    {
      path: '/admin',
      component: () => import('@/views/Layout/index.vue'),
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          redirect: '/admin/dashboard'
        },
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: () => import('@/views/Dashboard/index.vue'),
          meta: { permission: 'dashboard.view' }
        },
        {
          path: 'users',
          name: 'Users',
          component: () => import('@/views/Users/index.vue'),
          meta: { permission: 'user.view' }
        },
        {
          path: 'roles',
          name: 'Roles',
          component: () => import('@/views/Roles/index.vue'),
          meta: { permission: 'role.view' }
        },
        {
          path: 'members',
          name: 'Members',
          component: () => import('@/views/Members/index.vue'),
          meta: { permission: 'member.view' }
        },
        {
          path: 'articles',
          name: 'Articles',
          component: () => import('@/views/Articles/index.vue'),
          meta: { permission: 'article.view' }
        },
        {
          path: 'articles/create',
          name: 'ArticleCreate',
          component: () => import('@/views/Articles/Create.vue'),
          meta: { permission: 'article.create' }
        },
        {
          path: 'articles/:id/edit',
          name: 'ArticleEdit',
          component: () => import('@/views/Articles/Edit.vue'),
          meta: { permission: 'article.edit' }
        },
        {
          path: 'products',
          name: 'Products',
          component: () => import('@/views/Products/index.vue'),
          meta: { permission: 'product.view' }
        },
        {
          path: 'article-categories',
          name: 'ArticleCategories',
          component: () => import('@/views/ArticleCategories/index.vue'),
          meta: { permission: 'article.view' }
        },
        {
          path: 'product-categories',
          name: 'ProductCategories',
          component: () => import('@/views/ProductCategories/index.vue'),
          meta: { permission: 'product.view' }
        },
        {
          path: 'menus',
          name: 'Menus',
          component: () => import('@/views/Menus/index.vue'),
          meta: { permission: 'menu.view' }
        },
        {
          path: 'banners',
          name: 'Banners',
          component: () => import('@/views/Banners/index.vue'),
          meta: { permission: 'banner.view' }
        },
        {
          path: 'seo',
          name: 'SeoSettings',
          component: () => import('@/views/Seo/index.vue'),
          meta: { permission: 'seo.view' }
        },
        {
          path: 'contacts',
          name: 'Contacts',
          component: () => import('@/views/Contacts/index.vue'),
          meta: { permission: 'contact.view' }
        },
        {
          path: 'pages',
          name: 'Pages',
          component: () => import('@/views/Pages/index.vue'),
          meta: { permission: 'page.view' }
        },
        {
          path: 'categories',
          redirect: '/admin/article-categories'
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

    // 检查权限
    if (!checkRoutePermission(to)) {
      ElMessage.error('您没有访问该页面的权限')
      next('/admin/dashboard') // 跳转到仪表盘
      return
    }
  } else {
    // 如果是登录页且用户已登录，跳转到管理后台
    if (to.path === '/login' && userStore.isLoggedIn) {
      next('/admin/dashboard')
      return
    }
  }

  next()
})

export default router