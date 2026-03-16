FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Core/Core/Core.csproj Core/Core/
COPY DataIngestorService/DataIngestorService/DataIngestorService.csproj DataIngestorService/DataIngestorService/

RUN dotnet restore DataIngestorService/DataIngestorService/DataIngestorService.csproj

COPY . .

RUN dotnet publish DataIngestorService/DataIngestorService/DataIngestorService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DataIngestorService.dll"]

