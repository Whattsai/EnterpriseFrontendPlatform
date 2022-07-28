#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Setting Apps/ManagementCollections.API/ManagementCollections.API.csproj", "src/Setting Apps/ManagementCollections.API/"]
RUN dotnet restore "src/Setting Apps/ManagementCollections.API/ManagementCollections.API.csproj"
COPY . .
WORKDIR "/src/src/Setting Apps/ManagementCollections.API"
RUN dotnet build "ManagementCollections.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ManagementCollections.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManagementCollections.API.dll"]