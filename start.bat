@echo off
chcp 65001 >nul 2>&1
title CMS Management Tool
color 0A

:MENU
cls
echo ========================================
echo         CMS Management Tool
echo ========================================
echo.
echo Select Operation:
echo.
echo 1. Start Full System (Backend + Both Frontends)
echo 2. Start Backend Only
echo 3. Start Admin Frontend Only
echo 4. Start Website Frontend Only
echo 5. Start Original Frontend Only (Reference)
echo 6. Stop All Services
echo 7. Quick Stop
echo 8. Check Status
echo 0. Exit
echo.
echo ========================================
set /p choice="Enter choice (0-8): "

if "%choice%"=="1" goto START_ALL
if "%choice%"=="2" goto START_BACKEND
if "%choice%"=="3" goto START_ADMIN_FRONTEND
if "%choice%"=="4" goto START_WEBSITE_FRONTEND
if "%choice%"=="5" goto START_ORIGINAL_FRONTEND
if "%choice%"=="6" goto STOP_ALL
if "%choice%"=="7" goto QUICK_STOP
if "%choice%"=="8" goto CHECK_STATUS
if "%choice%"=="0" goto EXIT
goto INVALID

:START_ALL
echo.
echo Starting Full System...
echo ========================================

REM Clean ports first
call :CLEAN_PORTS

echo Starting Backend API...
start "CMS-Backend" cmd /k "cd backend && title CMS-Backend && dotnet run --urls http://localhost:5001"

echo Waiting 8 seconds for backend...
timeout /t 8

echo Starting Admin Frontend...
start "CMS-Admin-Frontend" cmd /k "cd admin-frontend && title CMS-Admin-Frontend && npm run dev"

echo Waiting 3 seconds...
timeout /t 3

echo Starting Website Frontend...
start "CMS-Website-Frontend" cmd /k "cd website-frontend && title CMS-Website-Frontend && npm run dev"

echo.
echo Services Started!
echo Backend: http://localhost:5001
echo Admin Frontend: http://localhost:3001
echo Website Frontend: http://localhost:3002
echo.
pause
goto MENU

:START_BACKEND
echo.
echo Starting Backend Only...
call :CLEAN_BACKEND_PORT
start "CMS-Backend" cmd /k "cd backend && title CMS-Backend && dotnet run --urls http://localhost:5001"
echo Backend started at http://localhost:5001
pause
goto MENU

:START_ADMIN_FRONTEND
echo.
echo Starting Admin Frontend Only...
call :CLEAN_ADMIN_PORT
start "CMS-Admin-Frontend" cmd /k "cd admin-frontend && title CMS-Admin-Frontend && npm run dev"
echo Admin Frontend started at http://localhost:3001
pause
goto MENU

:START_WEBSITE_FRONTEND
echo.
echo Starting Website Frontend Only...
call :CLEAN_WEBSITE_PORT
start "CMS-Website-Frontend" cmd /k "cd website-frontend && title CMS-Website-Frontend && npm run dev"
echo Website Frontend started at http://localhost:3002
pause
goto MENU

:START_ORIGINAL_FRONTEND
echo.
echo Starting Original Frontend Only (Reference)...
call :CLEAN_ORIGINAL_PORT
start "CMS-Original-Frontend" cmd /k "cd frontend && title CMS-Original-Frontend && npm run dev"
echo Original Frontend started at http://localhost:3000
pause
goto MENU

:STOP_ALL
echo.
echo Stopping All Services...
taskkill /f /im node.exe >nul 2>&1
taskkill /f /im dotnet.exe >nul 2>&1
call :CLEAN_PORTS
echo All services stopped!
pause
goto MENU

:QUICK_STOP
taskkill /f /im node.exe >nul 2>&1
taskkill /f /im dotnet.exe >nul 2>&1
call :CLEAN_PORTS
echo Quick stop completed!
timeout /t 2
goto MENU

:CHECK_STATUS
echo.
echo Service Status Check...
echo ========================================
echo.
echo Processes:
tasklist | findstr -i "node.exe dotnet.exe" 2>nul
if %errorlevel% neq 0 echo No CMS processes found

echo.
echo Ports:
netstat -ano | findstr ":5001" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (echo Port 5001: Backend Running) else (echo Port 5001: Not in use)

netstat -ano | findstr ":3001" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (echo Port 3001: Admin Frontend Running) else (echo Port 3001: Not in use)

netstat -ano | findstr ":3002" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (echo Port 3002: Website Frontend Running) else (echo Port 3002: Not in use)

netstat -ano | findstr ":3000" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (echo Port 3000: Original Frontend Running) else (echo Port 3000: Not in use)

echo.
pause
goto MENU

:CLEAN_PORTS
call :CLEAN_BACKEND_PORT
call :CLEAN_ADMIN_PORT
call :CLEAN_WEBSITE_PORT
call :CLEAN_ORIGINAL_PORT
goto :eof

:CLEAN_BACKEND_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5001" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:CLEAN_ADMIN_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3001" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:CLEAN_WEBSITE_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3002" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:CLEAN_ORIGINAL_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:INVALID
echo Invalid choice! Please try again.
timeout /t 2
goto MENU

:EXIT
echo Goodbye!
timeout /t 1
exit
