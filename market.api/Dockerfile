FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
LABEL maintainer="Antony Charles"
WORKDIR /app

ARG GITHUB_PACKAGES_USERNAME
ARG GITHUB_PACKAGES_TOKEN

ENV GITHUB_PACKAGES_USERNAME=${GITHUB_PACKAGES_USERNAME}
ENV GITHUB_PACKAGES_TOKEN=${GITHUB_PACKAGES_TOKEN}

COPY market.api/*.sln ./market.api/
COPY market.api/NuGet.Config ./market.api/
COPY market.api/src/Market.Api/*.csproj ./market.api/src/Market.Api/
COPY market.api/src/Market.Application/*.csproj ./market.api/src/Market.Application/
COPY market.api/src/Market.Domain/*.csproj ./market.api/src/Market.Domain/
COPY market.api/src/Market.Infrastructure/*.csproj ./market.api/src/Market.Infrastructure/

RUN dotnet restore ./market.api/MarketApi.sln --configfile ./market.api/NuGet.Config --no-cache --force --verbosity normal

COPY market.api/src/Market.Api/. ./market.api/src/Market.Api/
COPY market.api/src/Market.Application/. ./market.api/src/Market.Application/
COPY market.api/src/Market.Domain/. ./market.api/src/Market.Domain/
COPY market.api/src/Market.Infrastructure/. ./market.api/src/Market.Infrastructure/

WORKDIR /app/market.api/src/Market.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0.1
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Market.Api.dll"]
