#!/bin/bash

# --- Настройки ---
SQL_SCRIPT="$HOME/migrations.sql"  # Путь для скрипта, где есть права записи
DB_USER="myuser"                    # Имя пользователя базы для замены владельца
PROJECT_PATH="backend/Db"
STARTUP_PROJECT="backend/App"

# --- Проверка переменных ---
if [ -z "$SQL_SCRIPT" ] || [ -z "$DB_USER" ]; then
  echo "Ошибка: переменные SQL_SCRIPT или DB_USER не заданы."
  exit 1
fi

echo "Скрипт будет создан по пути: $SQL_SCRIPT"

# --- Проверка dotnet-ef ---
if ! command -v dotnet-ef >/dev/null 2>&1; then
  echo "dotnet-ef не найден. Устанавливаем..."
  dotnet tool install --global dotnet-ef
  export PATH="$PATH:$HOME/.dotnet/tools"
fi

# --- Генерация скрипта миграции ---
dotnet ef migrations script \
  --project "$PROJECT_PATH" \
  --startup-project "$STARTUP_PROJECT" \
  -o "$SQL_SCRIPT"

if [ $? -ne 0 ]; then
  echo "Ошибка при генерации скрипта EF."
  exit 1
fi

# --- Замена владельца в скрипте ---
if [[ "$OSTYPE" == "darwin"* ]]; then
  # macOS
  sed -i '' "s/owner to postgres/owner to \"$DB_USER\"/g" "$SQL_SCRIPT"
else
  # Linux / WSL / Git Bash
  sed -i "s/owner to postgres/owner to \"$DB_USER\"/g" "$SQL_SCRIPT"
fi

echo "Скрипт миграции успешно создан и обработан: $SQL_SCRIPT"
