FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["BotMaster/BotMaster.csproj", "BotMaster/"]
COPY ["WarFabrik.Clone/WarFabrik.Clone.csproj", "WarFabrik.Clone/"]
COPY ["NoobDevBot.Telegram/NoobDevBot.Telegram.csproj", "NoobDevBot.Telegram/"]
RUN dotnet restore "BotMaster/BotMaster.csproj"
COPY . .
WORKDIR "/src/BotMaster"
RUN dotnet build "BotMaster.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BotMaster.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BotMaster.dll"]