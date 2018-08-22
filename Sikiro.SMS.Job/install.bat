@echo off
cd %~dp0
dotnet Sikiro.SMS.Job.dll action:install
pause