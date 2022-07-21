#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Web Apps/Services/Execution/External/HttpClient/HttpClientAPI.csproj", "src/Web Apps/Services/Execution/External/HttpClient/"]
COPY ["src/Lib/SJConvert/SJ.Convert.csproj", "src/Lib/SJConvert/"]
COPY ["src/EFPLib/Common/Common.csproj", "src/EFPLib/Common/"]
RUN dotnet restore "src/Web Apps/Services/Execution/External/HttpClient/HttpClientAPI.csproj"
COPY . .
WORKDIR "/src/src/Web Apps/Services/Execution/External/HttpClient"
RUN dotnet build "HttpClientAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HttpClientAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HttpClientAPI.dll"]