FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Core/Core/Core.csproj Core/Core/
COPY DataProccesor/DataProccesor/DataProccesor.csproj DataProccesor/DataProccesor/

RUN dotnet restore DataProccesor/DataProccesor/DataProccesor.csproj

COPY . .

RUN dotnet publish DataProccesor/DataProccesor/DataProccesor.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DataProccesor.dll"]

