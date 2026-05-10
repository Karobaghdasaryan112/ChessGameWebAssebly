FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["webAssemblyChessGame.Services.ChessGameAPI/ChessService.API.ChessGameAPI.csproj", "webAssemblyChessGame.Services.ChessGameAPI/"]
COPY ["SharedResources/SharedResources.csproj", "SharedResources/"]
COPY ["ChessGame.Infrastructure.Persistance/ChessGame.Infrastructure.Persistance.csproj", "ChessGame.Infrastructure.Persistance/"]
COPY ["ChessGame.Core.Services/ChessGame.Core.Services.csproj", "ChessGame.Core.Services/"]
COPY ["ChessGame.Core.Domain/ChessGame.Domain.Domain.csproj", "ChessGame.Core.Domain/"]
COPY ["ChessGame.Infrastructure.Infrastructure/ChessGame.Infrastructure.Infrastructure.csproj", "ChessGame.Infrastructure.Infrastructure/"]
COPY ["ChatService.core.Services/ChatService.core.Services.csproj", "ChatService.core.Services/"]
COPY ["ChatService.core.Infrastructure/ChatService.Infrastructure.Persistance.csproj", "ChatService.core.Infrastructure/"]
RUN dotnet restore "webAssemblyChessGame.Services.ChessGameAPI/ChessService.API.ChessGameAPI.csproj"
COPY . .
WORKDIR "/src/webAssemblyChessGame.Services.ChessGameAPI"
RUN dotnet build "./ChessService.API.ChessGameAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChessService.API.ChessGameAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChessService.API.ChessGameAPI.dll"]
