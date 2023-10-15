FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.101-bullseye-slim AS build
WORKDIR /src
COPY ["ThermalLog.csproj", "./"]
RUN dotnet restore "ThermalLog.csproj"
COPY . .
WORKDIR /app/log
WORKDIR "/src/."
RUN dotnet build "ThermalLog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ThermalLog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "ThermalLog.dll"]
