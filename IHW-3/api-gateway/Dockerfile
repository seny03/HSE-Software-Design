FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["api-gateway/ApiGateway.csproj", "api-gateway/"]
RUN dotnet restore "api-gateway/ApiGateway.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/api-gateway"
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
