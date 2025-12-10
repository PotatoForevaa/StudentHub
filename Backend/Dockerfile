FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY StudentHub.Domain/*.csproj ./StudentHub.Domain/
COPY StudentHub.Application/*.csproj ./StudentHub.Application/
COPY StudentHub.Infrastructure/*.csproj ./StudentHub.Infrastructure/
COPY StudentHub.Api/*.csproj ./StudentHub.Api/

RUN dotnet restore

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
COPY . .

RUN dotnet ef migrations bundle \
    --project ./StudentHub.Infrastructure/StudentHub.Infrastructure.csproj \
    --context AppDbContext \
    --output /src/migrate-business 

RUN dotnet ef migrations bundle \
    --project ./StudentHub.Infrastructure/StudentHub.Infrastructure.csproj \
    --context AppIdentityDbContext \
    --output /src/migrate-identity 

WORKDIR /src/StudentHub.Api

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /src/migrate-identity ./migrate-identity
COPY --from=build /src/migrate-business ./migrate-business

RUN chmod +x ./migrate-identity ./migrate-business

ENTRYPOINT ["bash", "-c", "ls ; ./migrate-business --verbose && /migrate-identity --verbose && dotnet StudentHub.Api.dll"]
