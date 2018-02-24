if(!(Test-Path alias:git)) { New-Alias -Name git -Value "$Env:ProgramFiles\Git\bin\git.exe" }
if(!(Test-Path alias:msbuild)) { New-Alias -Name msbuild -Value "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" }

$ConfigurationName="Debug"
$root=(Get-Item -Path ".\.." -Verbose).FullName
$ProjectPath="$root\src\KsWare.MSBuildTargets\KsWare.MSBuildTargets.csproj"
$TargetPath="$root\src\KsWare.MSBuildTargets\bin\$ConfigurationName\KsWare.MSBuildTargets.exe"

#msbuild ..\src\KsWare.MSBuildTargets.sln /p:Configuration="Debug" /p:Platform="Any CPU" /m /v:M /fl /flp:LogFile=msbuild.log /nr:false

#TODO configure better path
$nuget="$root\src\KsWare.MSBuildTargets\bin\$ConfigurationName\nuget.exe"

if($ConfigurationName -eq "Debug") {
	& $TargetPath @("-pp", "$ProjectPath", "-cn", "$ConfigurationName", "-pn", "Any CPU", "-tp", "$TargetPath")
}
if($ConfigurationName -eq "Release") {
	& $nuget @("-pack", "$ProjectPath", "-tool")
}

# If running in the console, wait for input before closing.
if ($Host.Name -eq "ConsoleHost")
{
    Write-Host "Press any key to continue..."
    $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp") > $null
}