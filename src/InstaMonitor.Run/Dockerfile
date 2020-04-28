FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/InstaMonitor.Run/InstaMonitor.Run.csproj", "src/"]
RUN dotnet restore "src/InstaMonitor.Run.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "InstaMonitor.Run.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InstaMonitor.Run.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ENV INSTAMONITOR_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "InstaMonitor.Run.dll"]
