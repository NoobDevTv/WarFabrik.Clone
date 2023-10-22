#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY ["BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj", "BotMaster.DotNetRunner/"]
COPY ["BotMaster.PluginHost/BotMaster.PluginHost.csproj", "BotMaster.PluginHost/"]
COPY ["BotMaster.RightsManagement/BotMaster.RightsManagement.csproj", "BotMaster.RightsManagement/"]
COPY ["BotMaster.Commandos/BotMaster.Commandos.csproj", "BotMaster.Commandos/"]
COPY ["BotMaster.PluginSystem/BotMaster.PluginSystem.csproj", "BotMaster.PluginSystem/"]
COPY ["BotMaster.MessageContract/BotMaster.MessageContract.csproj", "BotMaster.MessageContract/"]

COPY ["BotMaster.Database/BotMaster.Database.csproj", "BotMaster.Database/"]
COPY ["10_DatabaseAdapter/BotMaster.Database.MySql/BotMaster.Database.MySql.csproj", "BotMaster.Database.MySql/"]

RUN dotnet restore "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj"
COPY . .
RUN dotnet publish "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj" -c Debug -o /app/publish /p:UseAppHost=false
RUN dotnet publish "BotMaster.DbMigrator.Plugin/BotMaster.DbMigrator.Plugin.csproj" -c Debug -o "/app/publish/plugins/BotMaster.DbMigrator.Plugin"
RUN dotnet publish "10_DatabaseAdapter/BotMaster.Database.MySql/BotMaster.Database.MySql.csproj" -c Debug -o "/app/publish"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.DotNetRunner.dll", "-l", "plugins/BotMaster.DbMigrator.Plugin/plugin.manifest.json"]