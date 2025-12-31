@echo off

cd ..
echo Cleaning Project...
dotnet clean > NUL 2>&1
echo Building Project...
dotnet build -c Debug > NUL 2>&1

for %%f in (Tests\IntegrationWorkflows\*) do (
    echo.
    echo Testing %%~nf...
    bin\Debug\net9.0\StonebotCLI < %%~f
)

pause
