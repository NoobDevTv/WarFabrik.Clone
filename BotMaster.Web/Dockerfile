#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BotMaster.Web/BotMaster.Web.csproj", "BotMaster.Web/"]
COPY ["BotMaster.Runtime/BotMaster.Runtime.csproj", "BotMaster.Runtime/"]
COPY ["WarFabrik.Clone/WarFabrik.Clone.csproj", "WarFabrik.Clone/"]
COPY ["BotMaster.Core/BotMaster.Core.csproj", "BotMaster.Core/"]
COPY ["NoobDevBot.Telegram/NoobDevBot.Telegram.csproj", "NoobDevBot.Telegram/"]
COPY ["BotMaster.Betterplace/BotMaster.Betterplace.csproj", "BotMaster.Betterplace/"]
RUN dotnet restore "BotMaster.Web/BotMaster.Web.csproj"
COPY . .
WORKDIR "/src/BotMaster.Web"
RUN dotnet build "BotMaster.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BotMaster.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.Web.dll"]