#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RuleCollections.API/RuleCollections.API.csproj", "RuleCollections.API/"]
RUN dotnet restore "RuleCollections.API/RuleCollections.API.csproj"
COPY . .
WORKDIR "/src/RuleCollections.API"
RUN dotnet build "RuleCollections.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RuleCollections.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuleCollections.API.dll"]