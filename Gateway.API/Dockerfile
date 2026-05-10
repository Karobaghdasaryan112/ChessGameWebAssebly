#RUNNING
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base 
#base its a name of image 
# FROM image (mcr.microsoft/dotnet/aspnet:8.0) AS base 
#we can usually use other name instead of base -- for example GatewayImage:
#so when we write this line its mean that we already setup

#- Linux
#- ASP.NET Runtime
#- dotnet runtime

USER $APP_UID

#Its a variable wich comes form docker compose file or envoirement variable or runetime configuration
#---every next command run from this user 
#not linux root user(Administrator)


WORKDIR /app
#its equivalent to --cd /app
#all the next commands execute in this folder relative
#its a work directory
#its create folder on local machine whats up from docker not in my computer



EXPOSE 8080
#its mean that this container listen on post 8080 
#generally 8080 its http and 8081 its https

EXPOSE 8081

#two expose for this container
#so container listen in 2 ports.
#8080 AS HTTP and 8081 AS HTTPS


#COMPILATION
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

#Its for compile the application 
#before FROM command its for running application
#its for compile it

ARG BUILD_CONFIGURATION=Release
#ARGUMNET
#its a build time variable and its for just pass it for dontnet run for example dotnet build -c BUILD_CONFIGURATION instead of dotnet build -c Release
WORKDIR /src
#here we pass all sdk
#so in vm now we have two foldders one to run application adn second one or compiler services
#/app
#/src
COPY ["Gateway.API/Gateway.API.csproj","Gateway.API/"]

#so here we have /src and inside of that we will have Gateway.API/Gateway.API.csproj
#{
    #src\
    #   Gateway.API\
    #       Gateway.API.csproj
#}
RUN dotnet restore Gateway.API/Gateway.API.csproj
#its for restore the application and download all dependencies 
#and nuget packages if nececarry


COPY . .
# its mean that inside of build context every file is copied into Gateway.API/ folder

WORKDIR /src/Gateway.API
#we jump on our project files becouse next command is build the application
#so we need to jump there

RUN dotnet build  "Gateway.API.csproj" -c $BUILD_CONFIGURATION -o /app/build
#so after jumping on inside the folder we can build the project
#build configuration is release becouse we set up it in the argument $BUILD_CONFIGURATION 
#and all dll or somethink else will be in the /app/build folder  



FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#about /p:UseAppHost=false
#we use dotnet command to run the application
#do we dont need specific executable host
#FROM build AS publish
#before we had a build as sdk
#so after the dotnet build we have all binaries 
#we can take this all envoirement (build)
#and call it as a publish
#and publish it 
#its finnal build all files into a /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.API.dll"]
