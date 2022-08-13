#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["KTech.Notifications.Api/KTech.Notifications.Api.csproj", "KTech.Notifications.Api/"]
RUN dotnet restore "KTech.Notifications.Api/KTech.Notifications.Api.csproj"
COPY . .
WORKDIR "/src/KTech.Notifications.Api"
RUN dotnet build "KTech.Notifications.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KTech.Notifications.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KTech.Notifications.Api.dll"]