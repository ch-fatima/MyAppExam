FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# کپی کردن فایل‌های پروژه با حفظ نام پوشه‌ها
# دقت کنید: مقصد هم دقیقا همان مسیر اصلی است
COPY ["MyApp/ApiApp.csproj", "MyApp/"]
COPY ["MyApp/Nuget.config", "MyApp/"] 
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Core/Core.csproj", "Core/"]

# دستور Restore را با همان مسیرهای بالا اجرا کنید
RUN dotnet restore "MyApp/ApiApp.csproj" --configfile "MyApp/Nuget.config"

# حالا کل سورس را کپی کنید
COPY . .

# رفتن به پوشه MyApp برای بیلد
WORKDIR "/src/MyApp"
RUN dotnet build "ApiApp.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "ApiApp.csproj" -c Release -o /app/publish --no-restore -p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiApp.dll"]
