#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 6001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Web Apps/Test/HR/HR.Test.csproj", "src/Web Apps/Test/HR/"]
RUN dotnet restore "src/Web Apps/Test/HR/HR.Test.csproj"
COPY . .
WORKDIR "/src/src/Web Apps/Test/HR"
RUN dotnet build "HR.Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HR.Test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HR.Test.dll"]