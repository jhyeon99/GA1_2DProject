@echo off
echo ===================================================
echo  [2D Side-View Adventure Game] Project Initializer
echo ===================================================
echo.

echo [1/2] Restoring dotnet tools (Husky)...
call dotnet tool restore
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] dotnet tool restore failed!
    echo Please check if .NET SDK is installed on your computer.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Installing Husky Git hooks...
call dotnet husky install
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Husky installation failed!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ===================================================
echo  Setup completed successfully!
echo  Git hooks are now active.
echo ===================================================
echo.
pause