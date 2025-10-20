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
    --project ./StudentHub.Infrastructure/StudentHub.Infrastructure.csproj \
    --startup-project ./StudentHub.Web/StudentHub.Web.csproj \
    --context AppDbContext \
    --output /src/migrate-business \
    --configuration Release \

RUN dotnet ef migrations bundle \
    --project ./StudentHub.Infrastructure/StudentHub.Infrastructure.csproj \
    --startup-project ./StudentHub.Web/StudentHub.Web.csproj \
    --context AppIdentityDbContext \
    --output /src/migrate-identity \
    --configuration Release \

WORKDIR /src/StudentHub.Web

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /src/migrate-identity ./migrate-identity
COPY --from=build /src/migrate-business ./migrate-business

ENTRYPOINT ["bash", "-c", "ls ; ./migrate-business && /migrate-identity && dotnet StudentHub.Web.dll"]
