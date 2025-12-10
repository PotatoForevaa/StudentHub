FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY StudentHub.Domain/*.csproj ./StudentHub.Domain/
COPY StudentHub.Application/*.csproj ./StudentHub.Application/
COPY StudentHub.Infrastructure/*.csproj ./StudentHub.Infrastructure/
COPY StudentHub.Api/*.csproj ./StudentHub.Api/

RUN dotnet restore

COPY . .

WORKDIR /src/StudentHub.Api

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT dotnet StudentHub.Api.dll
