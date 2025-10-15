import request from './index'

export interface Menu {
  id: number
  name: string
  path: string
  icon?: string
  component?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
  isVisible: boolean
  permission?: string
  description?: string
  createdAt: string
  updatedAt?: string
  children?: Menu[]
  // 前端状态管理属性
  statusLoading?: boolean
}

export interface CreateMenuDto {
  name: string
  path: string
  icon?: string
  component?: string
  parentId?: number
  sortOrder: number
  isActive: boolean
  isVisible: boolean
  permission?: string
  description?: string
}

export interface UpdateMenuDto extends CreateMenuDto {
  id: number
}

// 获取菜单树
export const getMenusTree = async (): Promise<Menu[]> => {
  // 暂时返回完整的菜单数据，稍后可以连接到后端API
  return getAllSystemMenus()
}

// 获取所有系统菜单（基于路由自动生成）
export const getAllSystemMenus = (): Menu[] => {
  return [
    {
      id: 1,
      name: '仪表盘',
      path: '/admin/dashboard',
      icon: 'HomeFilled',
      component: 'Dashboard',
      sortOrder: 1,
      isActive: true,
      isVisible: true,
      permission: 'dashboard.view',
      description: '系统概览和统计数据',
      createdAt: '2024-01-01T00:00:00Z'
    },
    {
      id: 2,
      name: '内容管理',
      path: '/admin/content',
      icon: 'Document',
      sortOrder: 2,
      isActive: true,
      isVisible: true,
      permission: 'content.view',
      description: '内容相关管理功能',
      createdAt: '2024-01-01T00:00:00Z',
      children: [
        {
          id: 21,
          name: '文章管理',
          path: '/admin/articles',
          icon: 'EditPen',
          component: 'Articles',
          parentId: 2,
          sortOrder: 1,
          isActive: true,
          isVisible: true,
          permission: 'article.view',
          description: '管理网站文章内容',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 22,
          name: '新增文章',
          path: '/admin/articles/create',
          icon: 'Plus',
          component: 'ArticleCreate',
          parentId: 2,
          sortOrder: 2,
          isActive: true,
          isVisible: false, // 隐藏在菜单中，但路由存在
          permission: 'article.create',
          description: '创建新文章',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 23,
          name: '文章分类',
          path: '/admin/article-categories',
          icon: 'FolderOpened',
          component: 'ArticleCategories',
          parentId: 2,
          sortOrder: 3,
          isActive: true,
          isVisible: true,
          permission: 'article.view',
          description: '管理文章分类',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 24,
          name: '产品管理',
          path: '/admin/products',
          icon: 'ShoppingBag',
          component: 'Products',
          parentId: 2,
          sortOrder: 4,
          isActive: true,
          isVisible: true,
          permission: 'product.view',
          description: '管理产品信息',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 25,
          name: '产品分类',
          path: '/admin/product-categories',
          icon: 'CollectionTag',
          component: 'ProductCategories',
          parentId: 2,
          sortOrder: 5,
          isActive: true,
          isVisible: true,
          permission: 'product.view',
          description: '管理产品分类',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 26,
          name: '页面管理',
          path: '/admin/pages',
          icon: 'Notebook',
          component: 'Pages',
          parentId: 2,
          sortOrder: 6,
          isActive: true,
          isVisible: true,
          permission: 'page.view',
          description: '管理静态页面',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 27,
          name: '轮播图管理',
          path: '/admin/banners',
          icon: 'Picture',
          component: 'Banners',
          parentId: 2,
          sortOrder: 7,
          isActive: true,
          isVisible: true,
          permission: 'banner.view',
          description: '管理首页轮播图',
          createdAt: '2024-01-01T00:00:00Z'
        }
      ]
    },
    {
      id: 3,
      name: '用户管理',
      path: '/admin/users-management',
      icon: 'User',
      sortOrder: 3,
      isActive: true,
      isVisible: true,
      permission: 'user.view',
      description: '用户和权限管理',
      createdAt: '2024-01-01T00:00:00Z',
      children: [
        {
          id: 31,
          name: '管理员',
          path: '/admin/users',
          icon: 'Avatar',
          component: 'Users',
          parentId: 3,
          sortOrder: 1,
          isActive: true,
          isVisible: true,
          permission: 'user.view',
          description: '管理系统管理员',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 32,
          name: '角色管理',
          path: '/admin/roles',
          icon: 'UserFilled',
          component: 'Roles',
          parentId: 3,
          sortOrder: 2,
          isActive: true,
          isVisible: true,
          permission: 'role.view',
          description: '管理用户角色和权限',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 33,
          name: '会员管理',
          path: '/admin/members',
          icon: 'Postcard',
          component: 'Members',
          parentId: 3,
          sortOrder: 3,
          isActive: true,
          isVisible: true,
          permission: 'member.view',
          description: '管理网站会员',
          createdAt: '2024-01-01T00:00:00Z'
        }
      ]
    },
    {
      id: 4,
      name: '系统设置',
      path: '/admin/system',
      icon: 'Setting',
      sortOrder: 4,
      isActive: true,
      isVisible: true,
      permission: 'system.manage',
      description: '系统配置和设置',
      createdAt: '2024-01-01T00:00:00Z',
      children: [
        {
          id: 41,
          name: '菜单管理',
          path: '/admin/menus',
          icon: 'Menu',
          component: 'Menus',
          parentId: 4,
          sortOrder: 1,
          isActive: true,
          isVisible: true,
          permission: 'menu.view',
          description: '管理系统菜单结构',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 42,
          name: 'SEO设置',
          path: '/admin/seo',
          icon: 'Search',
          component: 'SeoSettings',
          parentId: 4,
          sortOrder: 2,
          isActive: true,
          isVisible: true,
          permission: 'seo.view',
          description: '搜索引擎优化设置',
          createdAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 43,
          name: '留言管理',
          path: '/admin/contacts',
          icon: 'Message',
          component: 'Contacts',
          parentId: 4,
          sortOrder: 3,
          isActive: true,
          isVisible: true,
          permission: 'contact.view',
          description: '管理用户留言和反馈',
          createdAt: '2024-01-01T00:00:00Z'
        }
      ]
    }
  ]
}

// 创建菜单
export const createMenu = (data: CreateMenuDto): Promise<Menu> => {
  return request.post('/admin/menus', data)
}

// 更新菜单
export const updateMenu = (id: number, data: UpdateMenuDto): Promise<Menu> => {
  return request.put(`/admin/menus/${id}`, data)
}

// 删除菜单
export const deleteMenu = (id: number): Promise<void> => {
  return request.delete(`/admin/menus/${id}`)
}

// 更新菜单排序
export const updateMenuSort = (menuIds: number[]): Promise<void> => {
  return request.post('/admin/menus/sort', { menuIds })
}

// 切换菜单状态
export const toggleMenuStatus = (id: number, isActive: boolean): Promise<void> => {
  return request.patch(`/admin/menus/${id}/status`, { isActive })
}

// 获取可见菜单（用于侧边栏显示）
export const getVisibleMenus = (): Menu[] => {
  const allMenus = getAllSystemMenus()
  return allMenus
}
