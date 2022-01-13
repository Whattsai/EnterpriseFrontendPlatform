#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CreateCollections.API/CreateCollections.API.csproj", "CreateCollections.API/"]
RUN dotnet restore "CreateCollections.API/CreateCollections.API.csproj"
COPY . .
WORKDIR "/src/CreateCollections.API"
RUN dotnet build "CreateCollections.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreateCollections.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreateCollections.API.dll"]