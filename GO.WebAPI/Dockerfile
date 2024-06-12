FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ["GO.Core/GO.Core.csproj", "GO.Core/"]
COPY ["GO.Models/GO.Models.csproj", "GO.Models/"]
COPY ["GO.WebAPI/GO.WebAPI.csproj", "GO.WebAPI/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n "GO.WebAPI/GO.WebAPI.csproj"

COPY ["GO.Core/", "GO.Core/"]
COPY ["GO.Models/", "GO.Models/"]
COPY ["GO.WebAPI/", "GO.WebAPI/"]

WORKDIR /src/GO.WebAPI

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-slim AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "GO.WebAPI.dll"]
