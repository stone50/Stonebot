@echo off
setlocal
set ASPNETCORE_ENVIRONMENT=Development
cd ..\bin\Debug\net10.0
StonebotCLI twitch stop
StonebotCLI stop
endlocal
pause
