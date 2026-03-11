# ЭТАП 1: Сборка (Используем SDK образ!)
# Здесь есть инструменты для build и publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файл проекта
COPY ["Catcher.csproj", "./"]
RUN dotnet restore "Catcher.csproj"

# Копируем остальной код и собираем
COPY . .
RUN dotnet publish "Catcher.csproj" -c Release -o /app/publish

# ЭТАП 2: Запуск (Используем Runtime образ)
# Здесь только то, что нужно для работы приложения (он легче)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Catcher.dll"]