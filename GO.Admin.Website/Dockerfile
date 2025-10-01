FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY ["GO.Admin.Website/GO.Admin.Website.csproj", "GO.Admin.Website/"]

# Copy the rest of the source code
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
# Restore dependencies
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n -r linux-x64 "GO.Admin.Website/GO.Admin.Website.csproj"

COPY ["GO.Admin.Website/", "GO.Admin.Website/"]

WORKDIR /src/GO.Admin.Website

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="Asia/Bangkok"

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

ENTRYPOINT ["dotnet", "GO.Admin.Website.dll"]