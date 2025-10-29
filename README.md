# SoftwaresMarket

## Usage

```bash
docker compose up

npm i -g pnpm
pnpm dev
pnpm dev:back

# Generate frontend types
pnpm openapi

# Migrations
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add Init --project backend/Db --startup-project backend/App
dotnet ef migrations add MigrationName --project backend/Db --startup-project backend/App
dotnet ef database update MigrationName --project backend/Db --startup-project backend/App 
dotnet ef migrations remove  --project backend/Db --startup-project backend/App 

## Scripts
dotnet ef migrations script --project backend/Db --startup-project backend/App
dotnet ef migrations script FromMx ToMy --project backend/Db --startup-project backend/App

# Update deps
dotnet tool install --global dotnet-outdated-tool
dotnet outdated -u

# Tests
dotnet test backend/Tests

# Production build
pnpm i
pnpm build
dotnet publish backend/App -r=linux-x64
```

### Deploy dev

https://reestr.arodevelop.ru/

```sh
# WIP

# дропнуть и накатить базу с нуля
./reset_db_stage.sh

# сборка и деплой
./deploy_stage.sh
```

### Swagger

```
http://localhost:5000/swagger/index.html
```
