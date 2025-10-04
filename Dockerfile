# ---------- Stage 1: Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем решение и проекты по слоям
COPY *.sln .
COPY StudentHub.Domain/*.csproj ./StudentHub.Domain/
COPY StudentHub.Application/*.csproj ./StudentHub.Application/
COPY StudentHub.Infrastructure/*.csproj ./StudentHub.Infrastructure/
COPY StudentHub.Web/*.csproj ./StudentHub.Web/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем все исходники
COPY . .

# Переходим в слой Web для публикации
WORKDIR /src/StudentHub.Web

# Публикуем в Release в папку /app/publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Stage 2: Final ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем только опубликованные файлы
COPY --from=build /app/publish .

# Точка входа
ENTRYPOINT ["dotnet", "StudentHub.Web.dll"]
