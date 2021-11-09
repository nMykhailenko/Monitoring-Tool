FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringTool.API/MonitoringTool.API.csproj", "MonitoringTool.API/"]
RUN dotnet restore "MonitoringTool.API/MonitoringTool.API.csproj"

COPY . .
WORKDIR "/src/MonitoringTool.API"
RUN dotnet build "MonitoringTool.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringTool.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringTool.API.dll"]