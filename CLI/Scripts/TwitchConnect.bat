@echo off
setlocal
set ASPNETCORE_ENVIRONMENT=Development
cd ..\bin\Debug\net10.0
StonebotCLI run -f="%~dp0..\..\Daemon\bin\Debug\net10.0\StonebotDaemon" && ^
StonebotCLI config load && ^
StonebotCLI twitch auth -m=refresh && ^
StonebotCLI twitch config && ^
StonebotCLI twitch run && ^
endlocal
pause
