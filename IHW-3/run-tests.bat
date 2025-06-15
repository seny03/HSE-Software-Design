@echo off
echo Running tests with coverage for all services...

:: Clear
for /d %%d in (
    api-gateway\bin
    api-gateway\obj
    api-gateway\Tests\bin
    api-gateway\Tests\obj
    orders-service\bin
    orders-service\obj
    orders-service\Tests\bin
    orders-service\Tests\obj
    payments-service\bin
    payments-service\obj
    payments-service\Tests\bin
    payments-service\Tests\obj
) do (
    if exist "%%d" (
        echo Deleting %%d
        rmdir /s /q "%%d"
    )
)
rmdir /s /q test-results 2>nul

:: Create a directory for combined results
mkdir test-results 2>nul

:: Restore NuGet packages first
echo Restoring NuGet packages...
dotnet restore

:: API Gateway tests
echo Running API Gateway Service tests...
cd api-gateway\Tests
dotnet test --collect:"XPlat Code Coverage" --results-directory="..\..\test-results\api-gateway"
cd ..\..

:: Orders Service tests
echo Running Orders Service tests...
cd orders-service\Tests
dotnet test --collect:"XPlat Code Coverage" --results-directory="..\..\test-results\orders-service"
cd ..\..

:: Payments Service tests
echo Running Payments Service tests...
cd payments-service\Tests
dotnet test --collect:"XPlat Code Coverage" --results-directory="..\..\test-results\payments-service"
cd ..\..

echo Test runs completed successfully.
echo Coverage reports are available in test-results directory

echo Generating combined HTML coverage report...

:: Check if reportgenerator is installed
where reportgenerator >nul 2>nul
if %errorlevel% neq 0 (
    echo Installing reportgenerator...
    dotnet tool install -g dotnet-reportgenerator-globaltool
)

reportgenerator ^
    -reports:"test-results\**\coverage.cobertura.xml" ^
    -targetdir:"test-results\coveragereport" ^
    -reporttypes:Html ^
    -assemblyfilters:-*Tests* ^
    -classfilters:-*Tests* ^
    -filefilters:-*\Migrations\*;-*/Migrations/*;-*Program.cs

echo HTML coverage report generated at test-results\coveragereport\index.html 