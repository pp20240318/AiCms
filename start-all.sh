#!/bin/bash
echo "Starting CMS Full Stack Application..."
echo ""
echo "Starting Backend API..."
cd backend
dotnet run --urls "http://localhost:5001" &
BACKEND_PID=$!

echo "Waiting for backend to initialize..."
sleep 5

echo "Starting Frontend..."
cd ../frontend
npm install
npm run dev &
FRONTEND_PID=$!

echo ""
echo "Both services are running..."
echo "Backend: http://localhost:5001"
echo "Frontend: http://localhost:3000"
echo ""
echo "Press Ctrl+C to stop both services"

# Wait for both processes
wait $BACKEND_PID $FRONTEND_PID