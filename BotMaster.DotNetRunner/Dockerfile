#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj", "BotMaster.DotNetRunner/"]
COPY ["BotMaster.Core/BotMaster.Core.csproj", "BotMaster.Core/"]
COPY ["BotMaster.PluginHost/BotMaster.PluginHost.csproj", "BotMaster.PluginHost/"]
COPY ["BotMaster.PluginSystem/BotMaster.PluginSystem.csproj", "BotMaster.PluginSystem/"]
RUN dotnet restore "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj"
COPY . .
WORKDIR "/src/BotMaster.DotNetRunner"
RUN dotnet build "BotMaster.DotNetRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BotMaster.DotNetRunner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.DotNetRunner.dll"]