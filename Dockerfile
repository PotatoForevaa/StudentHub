FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY StudentHub.Domain/*.csproj ./StudentHub.Domain/
COPY StudentHub.Application/*.csproj ./StudentHub.Application/
COPY StudentHub.Infrastructure/*.csproj ./StudentHub.Infrastructure/
COPY StudentHub.Web/*.csproj ./StudentHub.Web/

RUN dotnet restore

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
COPY . .
RUN dotnet ef migrations bundle \
    --project StudentHub.Infrastructure \
    --startup-project StudentHub.Web \
    --output /src/migrate \
    --configuration Release


WORKDIR /src/StudentHub.Web

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /src/migrate ./migrate

ENTRYPOINT ["bash", "-c", "./migrate && dotnet StudentHub.Web.dll"]
