FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/User.Api/User.Api.csproj", "src/User.Api/"]
COPY ["src/User.Application/User.Application.csproj", "src/User.Application/"]
COPY ["src/User.Core/User.Core.csproj", "src/User.Core/"]
COPY ["src/User.Infrastructure/User.Infrastructure.csproj", "src/User.Infrastructure/"]
RUN dotnet restore "src/User.Api/User.Api.csproj"
COPY . .
WORKDIR "/src/src/User.Api"
RUN dotnet build "User.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "User.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Api.dll"]
