FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SferumNet/SferumNet.csproj", "SferumNet/"]
COPY ["SferumNet.DbModels/SferumNet.DbModels.csproj", "SferumNet.DbModels/"]
RUN dotnet restore "SferumNet/SferumNet.csproj"
COPY . .
WORKDIR "/src/SferumNet"
RUN dotnet build "SferumNet.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SferumNet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SferumNet.dll"]
