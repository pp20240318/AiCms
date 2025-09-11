# 🚀 CMS系统启动指南

## 📁 项目结构

```
My_Ai_Cms/
├── backend/          # 后端 .NET 8 Web API
├── frontend/         # 前端 Vue 3 + TypeScript
├── README.md         # 详细文档
└── start.md          # 快速启动指南
```

## ⚡ 快速启动

### 1. 启动后端服务

```bash
# 进入后端目录
cd backend

# 还原依赖包
dotnet restore

# 启动后端服务
dotnet run
```

后端服务将运行在：`http://localhost:5001`

### 2. 启动前端服务

**新开一个终端窗口**

```bash
# 进入前端目录
cd frontend

# 安装依赖（首次运行）
npm install

# 启动前端服务
npm run dev
```

前端服务将运行在：`http://localhost:5173`

## 🔑 默认登录信息

- **用户名**: `admin`
- **密码**: `admin123`

## 📝 开发说明

- 后端API接口地址：`http://localhost:5001/api`
- 前端管理界面：`http://localhost:5173`
- 数据库文件：`backend/cms.db` (SQLite)

## 🛠️ 开发工具推荐

- **后端**: Visual Studio Code + C# 扩展
- **前端**: Visual Studio Code + Vetur/Volar 扩展
- **API测试**: Postman 或 Swagger UI (`http://localhost:5001/swagger`)

---

🎉 现在你的CMS系统已经统一到 `My_Ai_Cms` 目录下了！