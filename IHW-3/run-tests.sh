#!/bin/bash

echo "Running tests with coverage for all services..."

echo "Cleaning up previous build artifacts..."
rm -rf api-gateway/bin
rm -rf api-gateway/obj
rm -rf api-gateway/Tests/bin
rm -rf api-gateway/Tests/obj
rm -rf orders-service/bin
rm -rf orders-service/obj
rm -rf orders-service/Tests/bin
rm -rf orders-service/Tests/obj
rm -rf payments-service/bin
rm -rf payments-service/obj
rm -rf payments-service/Tests/bin
rm -rf payments-service/Tests/obj

mkdir -p test-results

echo "Restoring NuGet packages..."
dotnet restore

# Run API Gateway tests
echo "Running API Gateway Service tests..."
dotnet test api-gateway/Tests/ApiGateway.Tests.csproj --collect:"XPlat Code Coverage" --results-directory:test-results/api-gateway

# Run Orders Service tests
echo "Running Orders Service tests..."
dotnet test orders-service/Tests/OrdersService.Tests.csproj --collect:"XPlat Code Coverage" --results-directory:test-results/orders-service

# Run Payments Service tests
echo "Running Payments Service tests..."
dotnet test payments-service/Tests/PaymentsService.Tests.csproj --collect:"XPlat Code Coverage" --results-directory:test-results/payments-service

echo "Test runs completed successfully."
echo "Coverage reports are available in test-results directory"

# Generate combined HTML coverage report
echo "Generating combined HTML coverage report..."
reportgenerator -reports:test-results/**/coverage.cobertura.xml -targetdir:test-results/coveragereport -reporttypes:Html -assemblyfilters:-*Tests* -classfilters:-*Tests* -filefilters:-*\\Migrations\\*\;-*/Migrations/*\;-*Program.cs

echo "HTML coverage report generated at test-results/coveragereport/index.html" 