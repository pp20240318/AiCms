@echo off
echo Starting CMS Full Stack Application...
echo.
echo Starting Backend API...
start cmd /k "cd backend && dotnet run --urls http://localhost:5001"

echo Waiting for backend to initialize...
timeout /t 5

echo Starting Frontend...
start cmd /k "cd frontend && npm install && npm run dev"

echo.
echo Both services are starting...
echo Backend: http://localhost:5001
echo Frontend: http://localhost:3000
echo.
pause