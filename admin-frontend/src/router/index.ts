import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { useTabsStore } from '@/stores/tabs'
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
          meta: {
            permission: 'dashboard.view',
            title: '仪表盘',
            icon: 'Dashboard'
          }
        },
        {
          path: 'users',
          name: 'Users',
          component: () => import('@/views/Users/index.vue'),
          meta: {
            permission: 'user.view',
            title: '用户管理',
            icon: 'User'
          }
        },
        {
          path: 'roles',
          name: 'Roles',
          component: () => import('@/views/Roles/index.vue'),
          meta: {
            permission: 'role.view',
            title: '角色管理',
            icon: 'UserFilled'
          }
        },
        {
          path: 'members',
          name: 'Members',
          component: () => import('@/views/Members/index.vue'),
          meta: {
            permission: 'member.view',
            title: '会员管理',
            icon: 'Avatar'
          }
        },
        {
          path: 'articles',
          name: 'Articles',
          component: () => import('@/views/Articles/index.vue'),
          meta: {
            permission: 'article.view',
            title: '文章管理',
            icon: 'Document'
          }
        },
        {
          path: 'articles/create',
          name: 'ArticleCreate',
          component: () => import('@/views/Articles/Create.vue'),
          meta: {
            permission: 'article.create',
            title: '新建文章',
            icon: 'EditPen'
          }
        },
        {
          path: 'articles/:id/edit',
          name: 'ArticleEdit',
          component: () => import('@/views/Articles/Edit.vue'),
          meta: {
            permission: 'article.edit',
            title: '编辑文章',
            icon: 'Edit'
          }
        },
        {
          path: 'products',
          name: 'Products',
          component: () => import('@/views/Products/index.vue'),
          meta: {
            permission: 'product.view',
            title: '产品管理',
            icon: 'Goods'
          }
        },
        {
          path: 'article-categories',
          name: 'ArticleCategories',
          component: () => import('@/views/ArticleCategories/index.vue'),
          meta: {
            permission: 'article.view',
            title: '文章分类',
            icon: 'Folder'
          }
        },
        {
          path: 'product-categories',
          name: 'ProductCategories',
          component: () => import('@/views/ProductCategories/index.vue'),
          meta: {
            permission: 'product.view',
            title: '产品分类',
            icon: 'FolderOpened'
          }
        },
        {
          path: 'menus',
          name: 'Menus',
          component: () => import('@/views/Menus/index.vue'),
          meta: {
            permission: 'menu.view',
            title: '菜单管理',
            icon: 'Menu'
          }
        },
        {
          path: 'banners',
          name: 'Banners',
          component: () => import('@/views/Banners/index.vue'),
          meta: {
            permission: 'banner.view',
            title: '轮播图管理',
            icon: 'Picture'
          }
        },
        {
          path: 'seo',
          name: 'SeoSettings',
          component: () => import('@/views/Seo/index.vue'),
          meta: {
            permission: 'seo.view',
            title: 'SEO设置',
            icon: 'Search'
          }
        },
        {
          path: 'contacts',
          name: 'Contacts',
          component: () => import('@/views/Contacts/index.vue'),
          meta: {
            permission: 'contact.view',
            title: '联系信息',
            icon: 'ChatDotRound'
          }
        },
        {
          path: 'pages',
          name: 'Pages',
          component: () => import('@/views/Pages/index.vue'),
          meta: {
            permission: 'page.view',
            title: '页面管理',
            icon: 'Files'
          }
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

// 路由后置守卫 - 添加Tab管理
router.afterEach((to) => {
  // 只在管理后台页面添加tab
  if (to.path.startsWith('/admin/') && to.path !== '/admin') {
    const tabsStore = useTabsStore()
    tabsStore.addTab(to)
  }
})

export default router