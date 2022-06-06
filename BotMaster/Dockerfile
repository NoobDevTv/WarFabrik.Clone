#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
WORKDIR /src
COPY ["BotMaster/BotMaster.csproj", "BotMaster/"]
COPY ["BotMaster.Runtime/BotMaster.Runtime.csproj", "BotMaster.Runtime/"]
COPY ["BotMaster.Core/BotMaster.Core.csproj", "BotMaster.Core/"]
COPY ["BotMaster.PluginSystem/BotMaster.PluginSystem.csproj", "BotMaster.PluginSystem/"]
COPY ["BotMaster.PluginHost/BotMaster.PluginHost.csproj", "BotMaster.PluginHost/"]
RUN dotnet restore "BotMaster/BotMaster.csproj"
COPY . .
WORKDIR "/src/BotMaster"
RUN dotnet publish "BotMaster.csproj" --no-restore -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.dll"]