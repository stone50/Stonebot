#!/bin/bash

cd ..
echo "Cleaning Project..."
dotnet clean > /dev/null 2>&1
echo "Building Project..."
dotnet build -c Debug > /dev/null 2>&1

for f in Tests/IntegrationWorkflows/*; do
    if [ -f "$f" ]; then
        echo.
        echo "Testing $(basename "$f" .*)..."
        bin/Debug/net9.0/StonebotCLI < "$f"
    fi
done
