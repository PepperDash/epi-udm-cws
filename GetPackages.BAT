@Echo OFF
echo Downloading packages...
echo.

REM Install NuGet packages (Essentials + Biamp)
echo Installing packages from NuGet...
nuget install .\packages.config -OutputDirectory .\packages -excludeVersion -PreRelease

REM Download Cisco v2.6.0 from GitHub (not available on NuGet)
echo.
echo Downloading Cisco codec v2.6.0 from GitHub...
if not exist "packages\epi-videoCodec-ciscoExtended" mkdir "packages\epi-videoCodec-ciscoExtended"
curl -L -o "packages\epi-videoCodec-ciscoExtended\epi-videoCodec-ciscoExtended.4Series.2.6.0.cplz" https://github.com/PepperDash/epi-videoCodec-ciscoExtended/releases/download/v2.6.0/epi-videoCodec-ciscoExtended.4Series.2.6.0.cplz

echo.
echo ========================================
echo Package download complete!
echo ========================================
pause
