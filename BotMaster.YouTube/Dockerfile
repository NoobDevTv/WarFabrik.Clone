#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY ["BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj", "BotMaster.DotNetRunner/"]
COPY ["BotMaster.Core/BotMaster.Core.csproj", "BotMaster.Core/"]
COPY ["BotMaster.PluginHost/BotMaster.PluginHost.csproj", "BotMaster.PluginHost/"]
COPY ["BotMaster.PluginSystem/BotMaster.PluginSystem.csproj", "BotMaster.PluginSystem/"]
COPY ["BotMaster.YouTube/BotMaster.YouTube.csproj", "BotMaster.YouTube/"]
COPY ["BotMaster.RightsManagement/BotMaster.RightsManagement.csproj", "BotMaster.RightsManagement/"]
COPY ["BotMaster.Betterplace.MessageContract/BotMaster.Betterplace.MessageContract.csproj", "BotMaster.Betterplace.MessageContract/"]
COPY ["BotMaster.Livestream.MessageContract/BotMaster.Livestream.MessageContract.csproj", "BotMaster.Livestream.MessageContract/"]
COPY ["BotMaster.MessageContract/BotMaster.MessageContract.csproj", "BotMaster.MessageContract/"]
COPY ["BotMaster.Database/BotMaster.Database.csproj", "BotMaster.Database/"]
COPY ["10_DatabaseAdapter/BotMaster.Database.MySql/BotMaster.Database.MySql.csproj", "BotMaster.Database.MySql/"]

RUN dotnet restore "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj"
COPY . .
RUN dotnet publish "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj" -c Debug -o /app/publish /p:UseAppHost=false
RUN dotnet publish "BotMaster.YouTube/BotMaster.YouTube.csproj" -c Debug -o "/app/publish/plugins/BotMaster.YouTube"
RUN dotnet publish "10_DatabaseAdapter/BotMaster.Database.MySql/BotMaster.Database.MySql.csproj" -c Debug -o "/app/publish"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.DotNetRunner.dll", "-l", "plugins/BotMaster.YouTube/plugin.manifest.json"]