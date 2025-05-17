FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["MqttWorkerService/MqttWorkerService.csproj", "MqttWorkerService/"]
COPY ["DataModels/DataModels.csproj", "DataModels/"]
COPY ["Database/Database.csproj", "Database/"]
RUN dotnet restore "MqttWorkerService/MqttWorkerService.csproj"
COPY . .
WORKDIR "/src/MqttWorkerService"
RUN dotnet build "MqttWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MqttWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
EXPOSE 1883
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MqttWorkerService.dll"]
