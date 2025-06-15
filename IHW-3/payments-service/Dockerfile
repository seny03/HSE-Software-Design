#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081
RUN apt-get update && apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IHW-3.sln", "."]
COPY ["orders-service/OrdersService.csproj", "orders-service/"]
COPY ["payments-service/PaymentsService.csproj", "payments-service/"]
COPY ["api-gateway/ApiGateway.csproj", "api-gateway/"]
COPY ["frontend/frontend.csproj", "frontend/"]
RUN dotnet restore "IHW-3.sln"
COPY . .
WORKDIR "/src/payments-service"
RUN dotnet build "PaymentsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentsService.dll"]
