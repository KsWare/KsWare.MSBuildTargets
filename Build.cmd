@echo Off
SETLOCAL
set ConfigurationName=%1

if "%ConfigurationName%" == "" (
   set ConfigurationName=Debug
)

set ProjectPath=%~dp0KsWare.MSBuildTargets.csproj
set TargetPath=%~dp0bin\%ConfigurationName%\KsWare.MSBuildTargets.exe

REM Dev10 and Dev11 msbuild path
set nugetmsbuildpath="%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild"

REM Dev12 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\MSBuild\12.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\MSBuild\12.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%

REM Dev14 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\MSBuild\14.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%

REM Dev15 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%

set EnableNuGetPackageRestore=true
::TODO enable restore and fix errors
::bin\debug\nuget.exe restore
%nugetmsbuildpath% KsWare.MSBuildTargets.sln /p:Configuration="%ConfigurationName%" /p:Platform="Any CPU" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Detailed /nr:false 

:: Create package 
if "%ConfigurationName%" == "Debug" (
	bin\%ConfigurationName%\KsWare.MSBuildTargets.exe -pp "%ProjectPath%" -cn "%ConfigurationName%" -pn "Any CPU" -tp "%TargetPath%"
)
if "%ConfigurationName%" == "Release" (
	bin\release\nuget.exe pack KsWare.MSBuildTargets.nuspec
)
ENDLOCAL
pause

