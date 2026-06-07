FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MyApp/ApiApp.csproj", "ApiApp/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["InfraStructure/InfraStructure.csproj", "InfraStructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["AllTests/AllTests.csproj", "AllTests/"]

RUN dotnet restore "MyApp/ApiApp.csproj"

COPY . .

WORKDIR "/src/ApiApp"
RUN dotnet publish "ApiApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiApp.csproj" -c Release -o /app/publish --no-restore -p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet","ApiApp.dll"]