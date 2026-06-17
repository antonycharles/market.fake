FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
LABEL maintainer="Antony Charles"
WORKDIR /app

ARG GITHUB_PACKAGES_USERNAME
ARG GITHUB_PACKAGES_TOKEN

ENV GITHUB_PACKAGES_USERNAME=${GITHUB_PACKAGES_USERNAME}
ENV GITHUB_PACKAGES_TOKEN=${GITHUB_PACKAGES_TOKEN}

COPY accounts.api/*.sln ./accounts.api/
COPY accounts.api/NuGet.Config ./accounts.api/
COPY accounts.api/src/Accounts.Api/*.csproj ./accounts.api/src/Accounts.Api/
COPY accounts.api/src/Accounts.Application/*.csproj ./accounts.api/src/Accounts.Application/
COPY accounts.api/src/Accounts.Core/*.csproj ./accounts.api/src/Accounts.Core/
COPY accounts.api/src/Accounts.Infrastructure/*.csproj ./accounts.api/src/Accounts.Infrastructure/
COPY accounts.api/tests/Accounts.Api.Test/*.csproj ./accounts.api/tests/Accounts.Api.Test/
COPY accounts.api/tests/Accounts.Application.Test/*.csproj ./accounts.api/tests/Accounts.Application.Test/

RUN dotnet restore ./accounts.api/AccountsApi.sln --configfile ./accounts.api/NuGet.Config --no-cache --force --verbosity normal

COPY accounts.api/src/Accounts.Api/. ./accounts.api/src/Accounts.Api/
COPY accounts.api/src/Accounts.Application/. ./accounts.api/src/Accounts.Application/
COPY accounts.api/src/Accounts.Core/. ./accounts.api/src/Accounts.Core/
COPY accounts.api/src/Accounts.Infrastructure/. ./accounts.api/src/Accounts.Infrastructure/
COPY accounts.api/tests/Accounts.Api.Test/. ./accounts.api/tests/Accounts.Api.Test/
COPY accounts.api/tests/Accounts.Application.Test/. ./accounts.api/tests/Accounts.Application.Test/

WORKDIR /app/accounts.api/src/Accounts.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0.1
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Accounts.Api.dll"]