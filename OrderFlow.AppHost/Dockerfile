FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5900
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderFlow.AppHost/OrderFlow.AppHost.csproj", "OrderFlow.AppHost/"]
RUN dotnet restore "OrderFlow.AppHost/OrderFlow.AppHost.csproj"
COPY . .
WORKDIR "/src/OrderFlow.AppHost"
RUN dotnet build "OrderFlow.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderFlow.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderFlow.AppHost.dll"]
