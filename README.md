# CMS 管理系统

一个现代化的内容管理系统，采用前后端分离架构，支持用户管理、权限控制、内容管理等功能。

## 技术栈

### 后端 (MyCms.Api)
- **.NET 8** - 后端框架
- **Entity Framework Core** - ORM框架
- **SQLite** - 数据库（开发环境）
- **JWT** - 身份认证
- **BCrypt** - 密码加密

### 前端 (cms-admin)
- **Vue 3** - 前端框架
- **TypeScript** - 类型检查
- **Element Plus** - UI组件库
- **Vue Router** - 路由管理
- **Pinia** - 状态管理
- **Axios** - HTTP客户端
- **Quill** - 富文本编辑器

## 功能特性

### 🔐 权限管理
- 用户管理（增删改查）
- 角色管理（支持权限分配）
- 权限管理（模块化权限控制）
- JWT认证授权
- 密码安全策略

### 📋 系统管理
- 菜单管理（树形结构）
- 系统配置
- 操作日志
- 定时任务

### 📝 内容管理
- **文章管理**
  - 文章分类（多级分类）
  - 文章发布/草稿
  - 富文本编辑
  - SEO优化设置
  
- **产品管理**
  - 产品分类（多级分类）
  - 产品信息管理
  - 图片上传
  - 库存管理
  - 价格管理

### 🖼️ 媒体管理
- 轮播图管理
- 文件上传
- 图片预览
- 媒体库管理

## 项目结构

```
├── backend/                   # 后端API项目
│   ├── Controllers/           # 控制器
│   ├── Models/               # 实体模型
│   ├── Services/             # 业务服务
│   ├── Data/                 # 数据访问层
│   ├── DTOs/                 # 数据传输对象
│   └── Extensions/           # 扩展方法
│
├── frontend/                 # 前端管理界面
│   ├── src/
│   │   ├── api/              # API接口
│   │   ├── components/       # 公共组件
│   │   ├── layout/           # 布局组件
│   │   ├── router/           # 路由配置
│   │   ├── stores/           # 状态管理
│   │   ├── utils/            # 工具函数
│   │   └── views/            # 页面组件
│   │       ├── system/       # 系统管理页面
│   │       └── content/      # 内容管理页面
│   └── public/               # 静态资源
│
├── start-all.bat             # Windows一键启动脚本
├── start-all.sh              # Linux/Mac一键启动脚本
├── start-backend.bat         # Windows后端启动脚本
├── start-frontend.bat        # Windows前端启动脚本
└── README.md                 # 项目说明文档
```

## 快速开始

### 🚀 一键启动（推荐）

**Windows用户：**
```bash
# 双击运行或在终端执行
start-all.bat
```

**Linux/Mac用户：**
```bash
# 在终端执行
./start-all.sh
```

### 📝 分别启动

**后端启动：**
```bash
# Windows
start-backend.bat

# Linux/Mac
./start-backend.sh
```

**前端启动：**
```bash
# Windows  
start-frontend.bat

# Linux/Mac
./start-frontend.sh
```

### 🔧 手动启动

**后端启动：**
1. 确保已安装 .NET 8 SDK
2. 进入后端目录：`cd backend`
3. 运行项目：`dotnet run --urls "http://localhost:5001"`

**前端启动：**
1. 确保已安装 Node.js 16+
2. 进入前端目录：`cd frontend`
3. 安装依赖：`npm install`
4. 启动开发服务器：`npm run dev`

### 🌐 访问地址
- **后端API**: http://localhost:5001
- **前端界面**: http://localhost:3000

## 默认账户

- **用户名**: admin
- **密码**: admin123

## 数据库

### 初始数据
系统启动时会自动创建数据库并初始化以下数据：
- 管理员账户
- 基础角色和权限
- 默认菜单结构

### 主要表结构
- `Users` - 用户表
- `Roles` - 角色表  
- `Permissions` - 权限表
- `UserRoles` - 用户角色关联表
- `RolePermissions` - 角色权限关联表
- `Menus` - 菜单表
- `Articles` - 文章表
- `ArticleCategories` - 文章分类表
- `Products` - 产品表
- `ProductCategories` - 产品分类表
- `Banners` - 轮播图表
- `UploadedFiles` - 文件上传表
- `ScheduledTasks` - 定时任务表

## 功能截图

### 登录界面
- 简洁的登录界面
- 支持用户名/邮箱登录
- 记住登录状态

### 仪表盘
- 数据统计卡片
- 快捷操作入口
- 最新动态展示

### 用户管理
- 用户列表展示
- 新增/编辑用户
- 角色分配
- 状态管理

### 内容管理
- 文章管理（富文本编辑）
- 产品管理（图片上传）
- 分类管理（树形结构）
- SEO设置

### 轮播图管理
- 图片上传预览
- 链接设置
- 有效期控制
- 排序管理

## 开发计划

- [ ] 文件上传功能完善
- [ ] 邮件通知功能
- [ ] 数据导入导出
- [ ] 系统日志查看
- [ ] 在线用户管理
- [ ] 多语言支持
- [ ] 主题切换
- [ ] 移动端适配

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request！

---

🚀 这是一个功能完整的CMS管理系统，适合用于学习和实际项目开发！