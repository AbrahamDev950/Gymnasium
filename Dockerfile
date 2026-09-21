# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar todo el proyecto
COPY . .

# Restaurar dependencias
RUN dotnet restore "Gym.csproj"

# Compilar
RUN dotnet build "Gym.csproj" -c Release --no-restore

# Publicar
RUN dotnet publish "Gym.csproj" -c Release --no-build -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copiar archivos compilados desde la etapa build
COPY --from=build /app/publish .

# Copiar carpeta Frontend (necesaria para servir static files)
COPY --from=build /src/Frontend ./Frontend

# Exponer puertos
EXPOSE 5000 5001

# Variables de entorno
ENV ASPNETCORE_URLS=http://+:5000;
ENV ASPNETCORE_ENVIRONMENT=Production

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "Gym.dll"]