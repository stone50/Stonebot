@echo off
setlocal

set ASPNETCORE_ENVIRONMENT=Development

cd ..\bin\Debug\net10.0

StonebotCLI run -f="%~dp0..\..\Daemon\bin\Debug\net10.0\StonebotDaemon" && ^
echo. && ^
StonebotCLI config load && ^
echo. && ^
StonebotCLI twitch auth && ^
echo. && ^
StonebotCLI twitch config && ^
echo. && ^
StonebotCLI twitch run && ^
echo.

endlocal
pause