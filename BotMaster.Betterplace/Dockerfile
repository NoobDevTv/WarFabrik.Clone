#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
COPY ["BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj", "BotMaster.DotNetRunner/"]
COPY ["BotMaster.Core/BotMaster.Core.csproj", "BotMaster.Core/"]
COPY ["BotMaster.PluginHost/BotMaster.PluginHost.csproj", "BotMaster.PluginHost/"]
COPY ["BotMaster.PluginSystem/BotMaster.PluginSystem.csproj", "BotMaster.PluginSystem/"]
COPY ["BotMaster.Betterplace/BotMaster.Betterplace.csproj", "BotMaster.Betterplace/"]
COPY ["BotMaster.RightsManagement/BotMaster.RightsManagement.csproj", "BotMaster.RightsManagement/"]
COPY ["BotMaster.Betterplace.MessageContract/BotMaster.Betterplace.MessageContract.csproj", "BotMaster.Betterplace.MessageContract/"]
COPY ["BotMaster.Livestream.MessageContract/BotMaster.Livestream.MessageContract.csproj", "BotMaster.Livestream.MessageContract/"]
COPY ["BotMaster.MessageContract/BotMaster.MessageContract.csproj", "BotMaster.MessageContract/"]
COPY ["BotMaster.Database/BotMaster.Database.csproj", "BotMaster.Database/"]
COPY ["10_DatabaseAdapter/BotMaster.Database.MySql/BotMaster.Database.MySql.csproj", "BotMaster.Database.MySql/"]

RUN dotnet restore "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj"
COPY . .
RUN dotnet publish "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj" -c Debug -o /app/publish /p:UseAppHost=false
RUN dotnet publish "BotMaster.Betterplace/BotMaster.Betterplace.csproj" -c Debug -o "/app/publish/plugins/BotMaster.Betterplace"
RUN dotnet publish "10_DatabaseAdapter/BotMaster.Database.MySql/BotMaster.Database.MySql.csproj" -c Debug -o "/app/publish" /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.DotNetRunner.dll", "-l", "plugins/BotMaster.Betterplace/plugin.manifest.json"]