#!/bin/bash
echo "Starting CMS Backend API..."
cd backend
dotnet run --urls "http://localhost:5001"