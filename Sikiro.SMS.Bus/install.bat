@echo off
cd %~dp0
dotnet Sikiro.SMS.BUS.dll action:install
pause