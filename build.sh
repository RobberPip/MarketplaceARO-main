#!/bin/bash

echo "Build frontend"
pnpm i
pnpm build:front

echo "Build backend"
dotnet publish backend/App -r=linux-x64

echo "Archive"
cd backend/App/bin/Release/net9.0/linux-x64/publish
rm build.zip
zip -q -r build.zip . -x ".DS_Store"

echo "backend/App/bin/Release/net9.0/linux-x64/publish/build.zip"