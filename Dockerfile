FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["NuGet.config", "./"]

COPY ["src/FCG.Users.Api/FCG.Users.Api.csproj", "src/FCG.Users.Api/"]
COPY ["src/FCG.Users.Application/FCG.Users.Application.csproj", "src/FCG.Users.Application/"]
COPY ["src/FCG.Users.Domain/FCG.Users.Domain.csproj", "src/FCG.Users.Domain/"]
COPY ["src/FCG.Users.Infrastructure/FCG.Users.Infrastructure.csproj", "src/FCG.Users.Infrastructure/"]

RUN dotnet restore "src/FCG.Users.Api/FCG.Users.Api.csproj" \
    --configfile "/src/NuGet.config"

COPY . .

RUN dotnet publish "src/FCG.Users.Api/FCG.Users.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG.Users.Api.dll"]