@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
title CMS Management Tool
color 0A

:MAIN_MENU
cls
echo ========================================
echo         CMS Full Stack Management
echo ========================================
echo.
echo Select Operation:
echo.
echo 1. Start Full System (Backend + Frontend)
echo 2. Start Backend Only
echo 3. Start Frontend Only
echo 4. Stop All Services
echo 5. Quick Stop (Force)
echo 6. Check Service Status
echo 7. Clean Log Files
echo 0. Exit
echo.
echo ========================================
set /p choice="Please enter your choice (0-7): "

if "%choice%"=="1" goto START_ALL
if "%choice%"=="2" goto START_BACKEND
if "%choice%"=="3" goto START_FRONTEND
if "%choice%"=="4" goto STOP_ALL
if "%choice%"=="5" goto QUICK_STOP
if "%choice%"=="6" goto CHECK_STATUS
if "%choice%"=="7" goto CLEAN_LOGS
if "%choice%"=="0" goto EXIT
goto INVALID_CHOICE

:START_ALL
echo.
echo [Start Full System]
echo ========================================
call :CLEAN_PORTS
echo.
echo [1/3] Starting Backend API...
start "CMS-Backend-API" cmd /k "cd /d %~dp0..\backend && title CMS-Backend-API && echo Starting Backend Service... && dotnet run --urls http://localhost:5001"

echo [2/3] Waiting for backend initialization...
timeout /t 8

echo [3/3] Starting Frontend Dev Server...
start "CMS-Frontend-UI" cmd /k "cd /d %~dp0..\frontend && title CMS-Frontend-UI && echo Starting Frontend Service... && npm run dev"

echo.
echo ✅ Services started successfully!
echo.
echo 🌐 Backend API: http://localhost:5001
echo 🎨 Frontend UI: http://localhost:5173
echo.
pause
goto MAIN_MENU

:START_BACKEND
echo.
echo [启动后端服务]
echo ========================================
call :CLEAN_BACKEND_PORT
echo.
echo 启动后端API服务...
start "CMS-后端API" cmd /k "cd /d %~dp0..\backend && title CMS-后端API && echo 启动后端服务... && dotnet run --urls http://localhost:5001"
echo.
echo ✅ 后端服务已启动！
echo 🌐 后端API: http://localhost:5001
echo.
pause
goto MAIN_MENU

:START_FRONTEND
echo.
echo [启动前端服务]
echo ========================================
call :CLEAN_FRONTEND_PORT
echo.
echo 启动前端开发服务...
start "CMS-前端界面" cmd /k "cd /d %~dp0..\frontend && title CMS-前端界面 && echo 启动前端服务... && npm run dev"
echo.
echo ✅ 前端服务已启动！
echo 🎨 前端界面: http://localhost:5173
echo.
pause
goto MAIN_MENU

:STOP_ALL
echo.
echo [停止所有服务]
echo ========================================
echo.
echo 正在停止CMS服务...

tasklist | findstr "node.exe" >nul
if %errorlevel% equ 0 (
    echo ✓ 停止Node.js进程...
    taskkill /f /im node.exe >nul 2>&1
) else (
    echo ✓ 没有Node.js进程运行
)

tasklist | findstr "dotnet.exe" >nul
if %errorlevel% equ 0 (
    echo ✓ 停止.NET进程...
    taskkill /f /im dotnet.exe >nul 2>&1
) else (
    echo ✓ 没有.NET进程运行
)

call :CLEAN_PORTS

echo.
echo ✅ 所有服务已停止！
echo.
pause
goto MAIN_MENU

:QUICK_STOP
echo.
echo [快速停止]
echo ========================================
taskkill /f /im node.exe >nul 2>&1
taskkill /f /im dotnet.exe >nul 2>&1
call :CLEAN_PORTS
echo ✅ 强制停止完成！
timeout /t 2
goto MAIN_MENU

:CHECK_STATUS
echo.
echo [服务状态检查]
echo ========================================
echo.

echo 📊 进程状态:
tasklist | findstr -i "node.exe dotnet.exe" >nul
if %errorlevel% equ 0 (
    echo ✓ 发现相关进程:
    tasklist | findstr -i "node.exe dotnet.exe"
) else (
    echo ❌ 没有发现CMS相关进程
)

echo.
echo 🔌 端口状态:
netstat -ano | findstr ":5001" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (
    echo ✓ 后端端口 5001 正在监听
) else (
    echo ❌ 后端端口 5001 未使用
)

netstat -ano | findstr ":5173" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (
    echo ✓ 前端端口 5173 正在监听
) else (
    echo ❌ 前端端口 5173 未使用
)

echo.
pause
goto MAIN_MENU

:CLEAN_LOGS
echo.
echo [清理日志文件]
echo ========================================
echo.

if exist "%~dp0..\backend\*.log" (
    echo 清理后端日志文件...
    del /q "%~dp0..\backend\*.log" 2>nul
    echo ✓ 后端日志已清理
) else (
    echo ✓ 后端无日志文件
)

if exist "%~dp0..\frontend\*.log" (
    echo 清理前端日志文件...
    del /q "%~dp0..\frontend\*.log" 2>nul
    echo ✓ 前端日志已清理
) else (
    echo ✓ 前端无日志文件
)

if exist "%~dp0..\*.log" (
    echo 清理根目录日志文件...
    del /q "%~dp0..\*.log" 2>nul
    echo ✓ 根目录日志已清理
) else (
    echo ✓ 根目录无日志文件
)

echo.
echo ✅ 日志清理完成！
echo.
pause
goto MAIN_MENU

:CLEAN_PORTS
call :CLEAN_BACKEND_PORT
call :CLEAN_FRONTEND_PORT
goto :eof

:CLEAN_BACKEND_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5001" ^| findstr "LISTENING"') do (
    echo 释放后端端口 5001...
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:CLEAN_FRONTEND_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173" ^| findstr "LISTENING"') do (
    echo 释放前端端口 5173...
    taskkill /f /pid %%a >nul 2>&1
)
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000" ^| findstr "LISTENING"') do (
    echo 释放前端端口 3000...
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:INVALID_CHOICE
echo.
echo ❌ 无效选择，请重新输入！
timeout /t 2
goto MAIN_MENU

:EXIT
echo.
echo 👋 再见！
timeout /t 1
exit

endlocal
