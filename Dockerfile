# ADM de Gestión Colegial · ASP.NET Core MVC (.NET 10) en Linux.
# Se construye desde la raíz de este repo:   docker build -t gc-web .
# En el stack completo lo levanta ../docker-compose.yml como servicio "web" (contenedor gc_web).

# === Compilación ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo los .csproj: si no cambian, Docker reutiliza la restauración de paquetes.
COPY GESTION_COLEGIAL.Business/GESTION_COLEGIAL.Business.csproj GESTION_COLEGIAL.Business/
COPY GESTION_COLEGIAL.Web/GESTION_COLEGIAL.Web.csproj GESTION_COLEGIAL.Web/
RUN dotnet restore GESTION_COLEGIAL.Web/GESTION_COLEGIAL.Web.csproj

COPY GESTION_COLEGIAL.Business/ GESTION_COLEGIAL.Business/
COPY GESTION_COLEGIAL.Web/ GESTION_COLEGIAL.Web/
RUN dotnet publish GESTION_COLEGIAL.Web/GESTION_COLEGIAL.Web.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# === Ejecución ===
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Zona horaria de Honduras para las fechas que calcula el servidor (si la imagen no trae tzdata, se instala).
RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && rm -rf /var/lib/apt/lists/*
ENV TZ=America/Tegucigalpa

COPY --from=build /app/publish .

# Carpetas que se montan como volumen: App_Data (graduados históricos, datos del instituto, logs)
# y las llaves que cifran la cookie de login. Deben ser del usuario sin privilegios que corre la app.
RUN mkdir -p /app/App_Data/Logs /app/keys \
    && chown -R $APP_UID:$APP_UID /app/App_Data /app/keys

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DataProtection__KeysPath=/app/keys
EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "GESTION_COLEGIAL.Web.dll"]
