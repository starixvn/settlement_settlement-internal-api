FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src
# COPY Settlement.InternalApi.sln .
COPY Sources/Settlement.Gaming/*.csproj ./Sources/Settlement.Gaming/
COPY Sources/Settlement.Internal/*.csproj ./Sources/Settlement.Internal/
COPY Sources/Settlement.InternalApi/*.csproj ./Sources/Settlement.InternalApi/

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore ./Sources/Settlement.InternalApi/Settlement.InternalApi.csproj

COPY Sources/. ./Sources
WORKDIR /src/Sources/Settlement.InternalApi

RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet publish -c release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

ENV COMPlus_EnableDiagnostics=0
WORKDIR /app
COPY --from=builder /app .


# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

ENTRYPOINT ["dotnet", "Settlement.InternalApi.dll"]