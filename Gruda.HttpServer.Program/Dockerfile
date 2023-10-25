FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Gruda.HttpServer/Gruda.HttpServer.csproj", "Gruda.HttpServer/"]
COPY ["Gruda.HttpServer.Core/Gruda.HttpServer.Core.csproj", "Gruda.HttpServer.Core/"]
RUN dotnet restore "Gruda.HttpServer/Gruda.HttpServer.csproj"
COPY . .
WORKDIR "/src/Gruda.HttpServer"
RUN dotnet build "Gruda.HttpServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gruda.HttpServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gruda.HttpServer.dll"]
