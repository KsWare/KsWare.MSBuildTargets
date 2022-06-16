# KsWare.MSBuildTargets

**This project is no longer under active development**  
The successor for SDk projects is [KsWare.BuildTools.IncrementVersion](https://github.com/SchreinerK/KsWare.BuildTools.IncrementVersion)
<hr/>

Executes commands on build. 

With this package it is easy to build and create/publish the nuget package with auto increment version support.

You can configure your Debug build to create a pre-release package with an auto incremented "-CI#####" suffix 
and your Release build with an auto incremented patch number.

##### Version 0.x.x
 - **For test purposes only. No warranty.**
 - Call NuGet commands
 - Different build configurations
 - Hierarchical configuration (define nuget APIKey outside of project)
 - NuGet: auto increment patch version
 - NuGet: auto increment CI version [e.g. 1.0.0-CI00001]
 - NuGet: supports install.ps1/uninstall.ps1 (also in VS 2017)

[![Build status](https://ci.appveyor.com/api/projects/status/rn94sivofrvc3uvf/branch/master?svg=true)](https://ci.appveyor.com/project/SchreinerK/ksware-msbuildtargets/branch/master)
[![NuGet Badge](https://buildstats.info/nuget/KsWare.MSBuildTargets)](https://www.nuget.org/packages/KsWare.MSBuildTargets/)

##### *upcomming* Version 1.0.x
 - Public. 
 - Uses Semantic Versioning 2.0.0
 - call other commands
 - delayed signing (define key.snk outside of project)

## Usage

- Add the [KsWare.MSBuildTargets](https://www.nuget.org/packages/KsWare.MSBuildTargets/) nuget package
- optional edit your `project.nuspec`
- create/edit `KsWare.MSBuildTargets.config`
- build your project

## Batch Usage

```batch
set ProjectPath=C:\dev\YourProject\YourProject.csproj
set TargetPath=C:\dev\YourProject\bin\release\YourProject.dll
KsWare.MSBuildTargets.exe -pp "%ProjectPath%" -cn Release -pn "Any CPU" -tp "%TargetPath%"
```
### Configuration File

The PackageBuilder.config contains the commands and properties.
```xml
<?xml version="1.0" encoding="utf-8"?>
<Configuration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" FileVersion="1.0">
    <BuildConfiguration>
    <Property Name="NuGet.Pack.OutputDirectory" Value="D:\Develop\Packages" />
    <Property Name="NuGet.Push.ApiKey" Value="your-nuget-api-key-" />
    <Property Name="SN.KeyFile" Value="D:\Develop\Company.snk" />
    </BuildConfiguration>
    <BuildConfiguration Configuration="Debug">
    <Command>nuget pack $IDE.ProjectPath$ -OutputDirectory $OutputDirectory$ -version $IncrementCI$</Command>
    </BuildConfiguration>
    <BuildConfiguration Configuration="Release">
    <Command>sn -R $IDE.ProjectPath$ $SN.KeyFile$</Command>
    <Command>nuget pack $IDE.ProjectPath$ -OutputDirectory $OutputDirectory$ -version $IncrementPatch$</Command>
    <Command>nuget push $PackagePath$ -apikey $ApiKey$</Command>
    </BuildConfiguration>
</Configuration>
```
The configuration is hierarchical. That means you can define a config file with more common (or secret) properties (and commands) in a directory with higher level and project specific settings at project level.
```
\Develop\  
  KsWare.MSBuildTargets.config (Master with ApiKey)
  \Project\  
    KsWare.MSBuildTargets.config (Project specific)
```
Hierarchical property reading:  
1. in the matching configuration
2. in the empty configuration
3. in parent configuration file in the matching configuration
4. in parent configuration file in the empty configuration
5. continue with step 3 until a property has been found

Hierarchical command reading: 

As in property reading but commands read as complete block. That means, if a configuration contains commands, only this commands are used.

### Command Line Parameter

**Note:** Not yet implemented

Parameter are usually read from KsWare.MSBuildTargets.config and can also be overwritten by command line:


 - `-version Increment` will increment patch version
 - `-version IncrementCI` will increment CI version [e.g. 1.0.0-CI00001]
 - and all known NuGet.exe parameter

## Known Issues and scheduled Features

 - support YAML/JSON configuration
 - DONE include a sample KsWare.MSBuildTargets.config
 - DONE include a template nuspec
 - DONE avoid use of Build Events
 - DONE avoid use of %PATH%, instead use tool directory

## NuGet

[KsWare.MSBuildTargets](https://www.nuget.org/packages/KsWare.MSBuildTargets/) 
[![NuGet Badge](https://buildstats.info/nuget/KsWare.MSBuildTargets)](https://www.nuget.org/packages/KsWare.MSBuildTargets/)


## License

Licensed under the [MIT licence](https://raw.githubusercontent.com/KsWare/KsWare.MSBuildTargets/licence).

Thanks to [Lars Skovslund](https://github.com/LarsSkovslund) and his [Project NuGet.Package.Builder](https://github.com/LarsSkovslund/NuGet.Package.Builder) to get me an example how to use .targets and .nuspec. 