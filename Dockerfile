# api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/MovieApi
RUN dotnet restore MovieApi.sln
RUN dotnet publish MovieApi.sln -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
# Escucha en 8080 para que compose lo enlace
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
# (opcional redis si lo usas)
# ENV Redis__ConnectionString=redis:6379
EXPOSE 8080
ENTRYPOINT ["dotnet", "MovieApi.dll"]
