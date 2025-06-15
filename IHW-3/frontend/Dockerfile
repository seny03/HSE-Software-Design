# Stage 1: Build the Blazor app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["frontend/frontend.csproj", "frontend/"]
RUN dotnet restore "frontend/frontend.csproj"
COPY . .
WORKDIR "/src/frontend"
RUN dotnet build "frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "frontend.csproj" -c Release -o /app/publish

# Stage 2: Serve the app using Nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"] 