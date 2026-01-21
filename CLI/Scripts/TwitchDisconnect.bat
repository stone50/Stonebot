@echo off
setlocal enabledelayedexpansion
cd ..\bin\Debug\net10.0
echo Checking daemon status...
for /f "delims=" %%i in ('StonebotCLI status 2^>^&1') do set "status=%%i"
echo !status! | findstr /i "\"OK\"" >nul
if !errorlevel! neq 0 (
    echo Daemon not running
    echo Nothing to stop
) else (
    echo Daemon OK
    echo Checking Twitch connection...
    for /f "delims=" %%i in ('StonebotCLI twitch connected 2^>^&1') do set "connected=%%i"
    echo !connected! | findstr /i "\"IsTwitchConnected\":true" >nul
    if !errorlevel! neq 0 (
        echo Twitch not connected
    ) else (
        echo Twitch connected
        echo Stopping Twitch...
        StonebotCLI twitch stop >nul
        echo Twitch disconnected
    )
    echo Stopping daemon...
    StonebotCLI stop >nul
    echo Daemon stopped
)
echo All services processed
endlocal
pause
