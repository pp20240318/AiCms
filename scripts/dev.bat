@echo off
title CMS 开发者工具
color 0B

echo ========================================
echo       CMS 开发者快速启动工具
echo ========================================
echo.

REM 检查依赖
echo [1/4] 检查环境...
where dotnet >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET SDK 未安装或未添加到PATH
    pause
    exit /b 1
)

where node >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Node.js 未安装或未添加到PATH
    pause
    exit /b 1
)

echo ✅ 环境检查通过

REM 检查前端依赖
echo [2/4] 检查前端依赖...
if not exist "%~dp0..\frontend\node_modules" (
    echo 📦 安装前端依赖...
    cd /d "%~dp0..\frontend"
    npm install
    if %errorlevel% neq 0 (
        echo ❌ 前端依赖安装失败
        pause
        exit /b 1
    )
) else (
    echo ✅ 前端依赖已存在
)

REM 清理端口
echo [3/4] 清理端口...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5001" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)

REM 启动服务
echo [4/4] 启动开发环境...
echo.
echo 🚀 启动后端API...
start "CMS-后端-开发" cmd /k "cd /d %~dp0..\backend && title CMS-后端-开发模式 && dotnet watch run --urls http://localhost:5001"

echo 等待后端启动...
timeout /t 6

echo 🎨 启动前端开发服务...
start "CMS-前端-开发" cmd /k "cd /d %~dp0..\frontend && title CMS-前端-开发模式 && npm run dev"

echo.
echo ========================================
echo ✅ 开发环境启动完成！
echo.
echo 🔧 开发模式特性：
echo - 后端热重载 (dotnet watch)
echo - 前端热更新 (Vite HMR)
echo - 自动重启服务
echo.
echo 🌐 访问地址：
echo - 后端API: http://localhost:5001
echo - 前端界面: http://localhost:5173
echo - API文档: http://localhost:5001/swagger
echo.
echo 💡 提示：修改代码后会自动重新编译
echo ========================================
echo.
pause
