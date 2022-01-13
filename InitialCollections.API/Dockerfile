#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["InitialCollections.API/InitialCollections.API.csproj", "InitialCollections.API/"]
RUN dotnet restore "InitialCollections.API/InitialCollections.API.csproj"
COPY . .
WORKDIR "/src/InitialCollections.API"
RUN dotnet build "InitialCollections.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InitialCollections.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InitialCollections.API.dll"]