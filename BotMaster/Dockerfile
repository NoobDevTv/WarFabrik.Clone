#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BotMaster/BotMaster.csproj", "BotMaster/"]
COPY ["WarFabrik.Clone/WarFabrik.Clone.csproj", "WarFabrik.Clone/"]
COPY ["NoobDevBot.Telegram/NoobDevBot.Telegram.csproj", "NoobDevBot.Telegram/"]
COPY ["BotMaster.Betterplace/BotMaster.Betterplace.csproj", "BotMaster.Betterplace/"]
COPY ["BotMaster.Core/BotMaster.Core.csproj", "BotMaster.Core/"]
RUN dotnet restore "BotMaster/BotMaster.csproj"
COPY . .
WORKDIR "/src/BotMaster"
RUN dotnet build "BotMaster.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BotMaster.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.dll"]