@echo off
setlocal enabledelayedexpansion
set ASPNETCORE_ENVIRONMENT=Development
cd ..\bin\Debug\net10.0
echo Checking daemon status...
for /f "delims=" %%i in ('StonebotCLI status 2^>^&1') do set "status=%%i"
echo !status! | findstr /i "\"OK\"" >nul
if !errorlevel! neq 0 (
    echo Daemon not running
    echo Starting daemon...
    StonebotCLI run -f="%~dp0..\..\Daemon\bin\Debug\net10.0\StonebotDaemon" >nul
    echo Loading config...
    StonebotCLI config load >nul
    echo Refreshing Twitch auth...
    StonebotCLI twitch auth -m=refresh >nul
    echo Configuring Twitch client...
    StonebotCLI twitch config >nul
    echo Connecting Twitch...
    StonebotCLI twitch run >nul
) else (
    echo Daemon OK
    echo Checking authorization...
    for /f "delims=" %%i in ('StonebotCLI twitch authorized 2^>^&1') do set "authorized=%%i"
    echo !authorized! | findstr /i "\"IsTwitchAuthorized\":true" >nul
    if !errorlevel! neq 0 (
        echo Not authorized
        echo Refreshing Twitch auth...
        StonebotCLI twitch auth -m=refresh >nul
        echo Configuring Twitch client...
        StonebotCLI twitch config >nul
        echo Connecting Twitch...
        StonebotCLI twitch run >nul
    ) else (
        echo Authorized OK
        echo Checking configuration...
        for /f "delims=" %%i in ('StonebotCLI twitch configured 2^>^&1') do set "configured=%%i"
        echo !configured! | findstr /i "\"IsTwitchClientConfigured\":true" >nul
        if !errorlevel! neq 0 (
            echo Not configured
            echo Configuring Twitch client...
            StonebotCLI twitch config >nul
            echo Connecting Twitch...
            StonebotCLI twitch run >nul
        ) else (
            echo Configured OK
            echo Checking connection...
            for /f "delims=" %%i in ('StonebotCLI twitch connected 2^>^&1') do set "connected=%%i"
            echo !connected! | findstr /i "\"IsTwitchConnected\":true" >nul
            if !errorlevel! neq 0 (
                echo Not connected
                echo Connecting Twitch...
                StonebotCLI twitch run >nul
            ) else (
                echo Connected OK
            )
        )
    )
)
echo Twitch ready
endlocal
pause
