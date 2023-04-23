#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src

COPY . .

#RUN dotnet restore "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj"

#RUN dotnet publish "BotMaster.DotNetRunner/BotMaster.DotNetRunner.csproj" -c Debug -o /app/publish /p:UseAppHost=false --no-restore
RUN dotnet publish "BotMaster.Web.AdministrationUi/BotMaster.Web.AdministrationUi.csproj" -c Debug -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotMaster.Web.AdministrationUi.dll", "-l", "plugins/BotMaster.AdministrationUi/plugin.manifest.json"]
#ENTRYPOINT ["dotnet", "BotMaster.DotNetRunner.dll", "-l", "plugins/BotMaster.AdministrationUi/plugin.manifest.json"]