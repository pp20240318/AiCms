import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: () => import('@/views/Frontend/Layout.vue'),
      meta: { requiresAuth: false },
      children: [
        {
          path: '',
          redirect: '/home'
        },
        {
          path: 'home',
          name: 'Home',
          component: () => import('@/views/Frontend/Home.vue')
        },
        {
          path: 'products',
          name: 'Products',
          component: () => import('@/views/Frontend/Products.vue')
        },
        {
          path: 'articles',
          name: 'Articles',
          component: () => import('@/views/Frontend/Articles.vue')
        },
        {
          path: 'about',
          name: 'About',
          component: () => import('@/views/Frontend/About.vue')
        },
        {
          path: 'contact',
          name: 'Contact',
          component: () => import('@/views/Frontend/Contact.vue')
        }
      ]
    }
  ]
})

export default router