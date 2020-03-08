FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/InstaMonitor.csproj", "src/"]
RUN dotnet restore "src/InstaMonitor.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "InstaMonitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InstaMonitor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ENV INSTAMONITOR_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "InstaMonitor.dll"]
