#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Trading.Bot/Trading.Bot.csproj", "Trading.Bot/"]
RUN dotnet restore "Trading.Bot/Trading.Bot.csproj"
COPY . .
WORKDIR "/src/Trading.Bot"
RUN dotnet build "Trading.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trading.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trading.Bot.dll"]