@echo off
setlocal
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\preview.ps1" %*
set "preview_exit=%errorlevel%"
if not "%preview_exit%"=="0" pause
exit /b %preview_exit%
