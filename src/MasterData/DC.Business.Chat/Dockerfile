#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MasterData/DC.Business.Chat/DC.Business.Chat.csproj", "DC.Business.Chat/"]
RUN dotnet restore "DC.Business.Chat/DC.Business.Chat.csproj"
COPY . .
WORKDIR "/src/MasterData/DC.Business.Chat"
RUN dotnet build "DC.Business.Chat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DC.Business.Chat.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DC.Business.Chat.dll"]