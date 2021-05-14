@echo off
cls

REM Allow FAKE to run on .NET Core 3.x
set DOTNET_ROLL_FORWARD=Major

dotnet tool restore
if "%~1"=="" (dotnet fake run eng\build.fsx) else (dotnet fake run eng\%*.fsx)
