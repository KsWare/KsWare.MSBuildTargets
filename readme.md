﻿# KsWare.MSBuildTargets

Executes commands on build. 

##### Version 0.1.x
 - **For test purposes only. No compatibility warranty.**
 - Call NuGet commands
 - Different build configurations
 - Hierarchical configuration (define nuget APIKey outside of project)
 - NuGet: auto increment patch version
 - NuGet: auto increment CI version [e.g. 1.0.0-CI00001]
 - NuGet: supports install.ps1/uninstall.ps1 (also in VS 2017)

##### *upcomming* Version 1.0.x
 - Public. 
 - Uses Semantic Versioning 2.0.0
 - call other commands
 - delayed signing (define key.snk outside of project)

## Usage

- Add the KsWare.MSBuildTargets nuget package
- optional edit your project.nuspec
- create/edit PackageBuilder.config
- build your project

## Batch Usage

    set ProjectPath=C:\dev\YourProject\YourProject.csproj
    set TargetPath=C:\dev\YourProject\bin\release\YourProject.dll
    KsWare.MSBuildTargets.exe -pp "%ProjectPath%" -cn Release -pn "Any CPU" -tp "%TargetPath%"

### Configuration File

The PackageBuilder.config contains the commands and properties.

    <?xml version="1.0" encoding="utf-8"?>
    <Configuration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" FileVersion="1.0">
      <BuildConfiguration>
        <Property Name="NuGet.Pack.OutputDirectory" Value="D:\Develop\Packages" />
        <Property Name="NuGet.Push.ApiKey" Value="your-nuget-api-key-" />
        <Property Name="SN.KeyFile" Value="D:\Develop\Company.snk" />
      </BuildConfiguration>
      <BuildConfiguration Configuration="Debug">
        <Command>nuget pack $IDE.ProjectPath$ -OutputDirectory $OutputDirectory$ -suffix $IncrementCI$</Command>
      </BuildConfiguration>
      <BuildConfiguration Configuration="Release">
        <Command>sn -R $IDE.ProjectPath$ $SN.KeyFile$</Command>
        <Command>nuget pack $IDE.ProjectPath$ -OutputDirectory $OutputDirectory$ -version $IncrementPatch$</Command>
        <Command>nuget push $PackagePath$ -apikey $ApiKey$</Command>
      </BuildConfiguration>
    </Configuration>

The configuration is hierarchical. That means you can define a config file with more common (or secret) properties (and commands) in a directory with higher level and project specific settings at project level.

    \Develop\  
     PackageBuilder.config (Master with ApiKey)
      \Project\  
       PackageBuilder.config (Project specific)

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
 - `-suffix IncrementCI` will increment CI version [e.g. 1.0.0-CI00001]
 - and all known NuGet.exe parameter

## Known Issues and scheduled Features

 - DONE include a sample KsWare.MSBuildTargets.config
 - DONE include a template nuspec
 - DONE avoid use of Build Events
 - DONE avoid use of %PATH%, instead use tool directory

## License

Licensed under the [MIT licence](https://raw.githubusercontent.com/KsWare/KsWare.MSBuildTargets/licence).

Thanks to [Lars Skovslund](https://github.com/LarsSkovslund) and his [Project NuGet.Package.Builder](https://github.com/LarsSkovslund/NuGet.Package.Builder) to get me an example how to use .targets and .nuspec. 