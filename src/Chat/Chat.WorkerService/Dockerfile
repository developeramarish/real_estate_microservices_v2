#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Chat/Chat.WorkerService/Chat.WorkerService.csproj", "Chat/Chat.WorkerService/"]
RUN dotnet restore "Chat/Chat.WorkerService/Chat.WorkerService.csproj"
COPY . .
WORKDIR "/src/Chat/Chat.WorkerService"
RUN dotnet build "Chat.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chat.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chat.WorkerService.dll"]