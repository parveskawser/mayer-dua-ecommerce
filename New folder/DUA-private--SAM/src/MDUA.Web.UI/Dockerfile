# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MDUA.Web.UI/MDUA.Web.UI.csproj", "MDUA.Web.UI/"]
COPY ["MDUA.Facade/MDUA.Facade.csproj", "MDUA.Facade/"]
COPY ["MDUA.DataAccess/MDUA.DataAccess.csproj", "MDUA.DataAccess/"]
COPY ["MDUA.Entities/MDUA.Entities.csproj", "MDUA.Entities/"]
COPY ["MDUA.Framework/MDUA.Framework.csproj", "MDUA.Framework/"]
RUN dotnet restore "./MDUA.Web.UI/MDUA.Web.UI.csproj"
COPY . .
WORKDIR "/src/MDUA.Web.UI"
RUN dotnet build "./MDUA.Web.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MDUA.Web.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MDUA.Web.UI.dll"]