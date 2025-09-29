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
echo 1. Start Full System
echo 2. Start Backend Only  
echo 3. Start Frontend Only
echo 4. Stop All Services
echo 5. Quick Stop
echo 6. Check Status
echo 0. Exit
echo.
echo ========================================
set /p choice="Enter choice (0-6): "

if "%choice%"=="1" goto START_ALL
if "%choice%"=="2" goto START_BACKEND
if "%choice%"=="3" goto START_FRONTEND
if "%choice%"=="4" goto STOP_ALL
if "%choice%"=="5" goto QUICK_STOP
if "%choice%"=="6" goto CHECK_STATUS
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

echo Starting Frontend...
start "CMS-Frontend" cmd /k "cd frontend && title CMS-Frontend && npm run dev"

echo.
echo Services Started!
echo Backend: http://localhost:5001
echo Frontend: http://localhost:5173
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

:START_FRONTEND
echo.
echo Starting Frontend Only...
call :CLEAN_FRONTEND_PORT
start "CMS-Frontend" cmd /k "cd frontend && title CMS-Frontend && npm run dev"
echo Frontend started at http://localhost:5173
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

netstat -ano | findstr ":5173" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (echo Port 5173: Frontend Running) else (echo Port 5173: Not in use)

echo.
pause
goto MENU

:CLEAN_PORTS
call :CLEAN_BACKEND_PORT
call :CLEAN_FRONTEND_PORT
goto :eof

:CLEAN_BACKEND_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5001" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
goto :eof

:CLEAN_FRONTEND_PORT
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173" ^| findstr "LISTENING"') do (
    taskkill /f /pid %%a >nul 2>&1
)
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000" ^| findstr "LISTENING"') do (
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
