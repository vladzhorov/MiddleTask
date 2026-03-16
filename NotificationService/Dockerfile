FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Core/Core/Core.csproj Core/Core/
COPY NotificationService/NotificationService.csproj NotificationService/

RUN dotnet restore NotificationService/NotificationService.csproj

COPY . .

RUN dotnet publish NotificationService/NotificationService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.dll"]

