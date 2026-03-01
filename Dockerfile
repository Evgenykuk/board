# Используем стабильную комбинацию: SDK 7.0 на Debian Slim
FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
WORKDIR /app

# Копируем только .csproj-файлы сначала
COPY src/BoardService.Api/BoardService.Api.csproj ./src/BoardService.Api/
COPY src/BoardService.Domain/BoardService.Domain.csproj ./src/BoardService.Domain/
COPY src/BoardService.Application/BoardService.Application.csproj ./src/BoardService.Application/
COPY src/BoardService.Infrastructure/BoardService.Infrastructure.csproj ./src/BoardService.Infrastructure/
COPY src/BoardService.Shared/BoardService.Shared.csproj ./src/BoardService.Shared/

# Восстанавливаем зависимости
RUN dotnet restore src/BoardService.Api/BoardService.Api.csproj

# Копируем всё остальное
COPY . .

# Публикуем
RUN dotnet publish src/BoardService.Api/BoardService.Api.csproj -c Release -o out

# Финальный образ — тоже slim
FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "BoardService.Api.dll"]
