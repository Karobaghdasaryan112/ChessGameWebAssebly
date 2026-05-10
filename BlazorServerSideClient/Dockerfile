FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorServerSideClient/BlazorServerSideClient.csproj", "BlazorServerSideClient/"]
COPY ["ChessGame.Infrastructure.Infrastructure/ChessGame.Infrastructure.Infrastructure.csproj", "ChessGame.Infrastructure.Infrastructure/"]
COPY ["ChatService.core.Services/ChatService.core.Services.csproj", "ChatService.core.Services/"]
COPY ["ChessGame.Core.Services/ChessGame.Core.Services.csproj", "ChessGame.Core.Services/"]
COPY ["SharedResources/SharedResources.csproj", "SharedResources/"]
COPY ["ChessGame.Core.Domain/ChessGame.Domain.Domain.csproj", "ChessGame.Core.Domain/"]
RUN dotnet restore "BlazorServerSideClient/BlazorServerSideClient.csproj"
COPY . .
WORKDIR "/src/BlazorServerSideClient"
RUN dotnet build "./BlazorServerSideClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorServerSideClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorServerSideClient.dll"]
