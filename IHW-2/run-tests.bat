@echo off
echo Running tests with coverage for all services...

:: Clear
for /d %%d in (
    api-gateway\bin
    api-gateway\obj
    api-gateway\Tests\bin
    api-gateway\Tests\obj
    file-service\bin
    file-service\obj
    file-service\Tests\bin
    file-service\Tests\obj
    analysis-service\bin
    analysis-service\obj
    analysis-service\Tests\bin
    analysis-service\Tests\obj
) do (
    if exist "%%d" (
        echo Deleting %%d
        rmdir /s /q "%%d"
    )
)
rmdir /s /q test-results

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

:: File Service tests
echo Running File Service tests...
cd file-service\Tests
dotnet test --collect:"XPlat Code Coverage" --results-directory="..\..\test-results\file-service"
cd ..\..

:: Analysis Service tests
echo Running Analysis Service tests...
cd analysis-service\Tests
dotnet test --collect:"XPlat Code Coverage" --results-directory="..\..\test-results\analysis-service"
cd ..\..

echo Test runs completed successfully.
echo Coverage reports are available in test-results directory

echo Generating combined HTML coverage report...

reportgenerator ^
    -reports:"test-results\**\coverage.cobertura.xml" ^
    -targetdir:"test-results\coveragereport" ^
    -reporttypes:Html ^
    -assemblyfilters:-*Tests* ^
    -classfilters:-*Tests* ^
    -filefilters:-*\Migrations\*;-*/Migrations/*;-*Program.cs

echo HTML coverage report generated at test-results\coveragereport\index.html

:: Clear
for /d %%d in (
    api-gateway\bin
    api-gateway\obj
    api-gateway\Tests\bin
    api-gateway\Tests\obj
    file-service\bin
    file-service\obj
    file-service\Tests\bin
    file-service\Tests\obj
    analysis-service\bin
    analysis-service\obj
    analysis-service\Tests\bin
    analysis-service\Tests\obj
) do (
    if exist "%%d" (
        rmdir /s /q "%%d"
    )
)
