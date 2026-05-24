# Stage 1: Build the Blazor WebAssembly application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Market.Web/Market.Web.csproj", "Market.Web/"]
RUN dotnet restore "Market.Web/Market.Web.csproj"
COPY src/Market.Web/. Market.Web/
WORKDIR "/src/Market.Web"
RUN dotnet build "Market.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Market.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .         
COPY nginx.conf /etc/nginx/conf.d/default.conf     
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]