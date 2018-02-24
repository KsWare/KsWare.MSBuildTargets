@echo Off
SETLOCAL
REM set current path to repository root
%~d0
cd %~dp0..
echo %CD%

set ConfigurationName=%1

if "%ConfigurationName%" == "" (
   set ConfigurationName=Debug
)

set ProjectPath="%CD%\src\KsWare.MSBuildTargets\KsWare.MSBuildTargets.csproj"
set TargetPath="%CD%\src\KsWare.MSBuildTargets\bin\%ConfigurationName%\KsWare.MSBuildTargets.exe"

REM Dev15 msbuild path
::TODO use vswhere.exe 
set msbuildtmp="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set path=%PATH%;%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin

::TODO configure better path
set nuget="%~dp0src\KsWare.MSBuildTargets\bin\%ConfigurationName%\nuget.exe"

set EnableNuGetPackageRestore=true
::TODO %nuget% restore src

:: TODO the fucking shit would't work in this cmd-file!
msbuild src\KsWare.MSBuildTargets.sln /p:Configuration="%ConfigurationName%" /p:Platform="Any CPU" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Detailed /nr:false
pause
REM Create package 
if "%ConfigurationName%" == "Debug" (
	%TargetPath% -pp %ProjectPath% -cn "%ConfigurationName%" -pn "Any CPU" -tp %TargetPath%
)
if "%ConfigurationName%" == "Release" (
	%nuget% pack %ProjectPath% -tool
)
ENDLOCAL
pause

