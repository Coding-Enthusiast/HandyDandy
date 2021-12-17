[![.NET-CI](https://github.com/Coding-Enthusiast/HandyDandy/actions/workflows/dotnetCI.yml/badge.svg)](https://github.com/Coding-Enthusiast/HandyDandy/actions/workflows/dotnetCI.yml)
[![Build Status](https://travis-ci.com/Coding-Enthusiast/HandyDandy.svg?branch=master)](https://travis-ci.com/Coding-Enthusiast/HandyDandy)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Coding-Enthusiast/HandyDandy/blob/master/License)
[![Target](https://img.shields.io/badge/dynamic/xml?color=%23512bd4&label=target&query=%2F%2FTargetFramework%5B1%5D&url=https%3A%2F%2Fraw.githubusercontent.com%2FCoding-Enthusiast%2FHandyDandy%2Fmaster%2FSrc%2FHandyDandy%2FHandyDandy.csproj&logo=.net)](https://github.com/Coding-Enthusiast/FinderOuter/blob/master/Src/HandyDandy/HandyDandy.csproj)
[![Downloads](https://img.shields.io/github/downloads/Coding-Enthusiast/HandyDandy/total)](https://github.com/Coding-Enthusiast/HandyDandy/releases)

# Handy Dandy
Handy Dandy is a tool that helps visualize and work with data in different formats that are used in Bitcoin protocol such as private keys and mnemonics.  

This project is 100% open source and is written in C#. By using [.Net core](https://github.com/dotnet/core) and [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)
HandyDandy can run on all operating systems.  

## Quick guide
1. Select the data grouping option and the final output type from the options on top
2. Set the bits (0 or 1) manually by clicking on each button
3. When all bits are set, the result will be printed in the text-box at the bottom of the window
4. Copy the result

![Preview](/Doc/Images/MainPreview.jpg)

## Getting started
#### Step 1: Preparation
You can ignore this step at your own risk and skip to step 2.  
Since this project deals with _sensative information_ such as private keys, mnemonics, etc. the safest approach is to run it 
on a clean and [air-gapped](https://en.wikipedia.org/wiki/Air_gap_(networking)) computer. Easiest way of acheiving that is using
a live Linux:  
1. Download [Ubuntu](https://ubuntu.com/download/desktop) or any other Linux OS
2. Verify Ubuntu's iso ([link](https://ubuntu.com/tutorials/tutorial-how-to-verify-ubuntu#1-overview))
3. Follow step 2 while you are still online
4. Disconnect network cable (to remain offline)
5. Burn that ISO on a DVD or could be a USB disk 
([link](https://ubuntu.com/tutorials/try-ubuntu-before-you-install#1-getting-started))
5. Boot into Ubuntu to run HandyDandy
6. After you are done, shut down Ubuntu and remove the medium used in step 5

#### Step 2: Download and build
If you cannot or do not want to build you can go to [releases](https://github.com/Coding-Enthusiast/HandyDandy/releases) where
the ready-to-run binaires are found for 3 different x64 operating systems: Windows, Linux and MacOS. 
the other two files named `Source code.zip` and `Source code.tar.gz` are the project's source code that GitHub automatically adds
at that release version's commit.  

**To build HandyDandy:**  
If you have [Visual Studio](https://visualstudio.microsoft.com/downloads/) you can clone this repository and build the included
solution file called [HandyDandy.sln](https://github.com/Coding-Enthusiast/HandyDandy/blob/master/Src/HandyDandy.sln).  
Building is also possible through these steps using command line: 
1. Get Git: https://git-scm.com/downloads
2. Get .NET 5.0 SDK: https://dotnet.microsoft.com/download (see `TargetFramework` in
[HandyDandy.csproj](https://github.com/Coding-Enthusiast/HandyDandy/blob/master/Src/HandyDandy/HandyDandy.csproj)
for the required .net version in case readme wasn't updated)
3. Clone HandyDandy `git clone https://github.com/Coding-Enthusiast/HandyDandy.git`
4. Build using `dotnet publish -c Release -r <RID> --self-contained true` (replace `<RID>` with [RID](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)
of the operating system you want to build for. e.g. `win-x64` for x64 Windows or `linux-arm64` for Linux x64 ARM)

**Important notes:**  
- Remember to build the project using `release` [configuration](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build)
to benefit from compiler optimizations.  
- .Net applications can be published as [self contained](https://docs.microsoft.com/en-us/dotnet/core/deploying/) which will 
increase the size of the binray by including the required framework in it. That helps running the application on any computer 
(like the live Linux explained above) without needing to install .Net separately. The size can be reduced by selecting the
`Trim unused assemblies` option.  
- This project can be built on and used on any operating system, use `-r|--runtime <RUNTIME_IDENTIFIER>` to specify OS
with the correct [RID](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).  

#### Step 3: Run
If you have compiled HandyDandy as SCD or downloaded the provided binaries there is no need to download .Net Core, otherwise it
has to be [downloaded and installed](https://dotnet.microsoft.com/download) on the system that needs to run HandyDandy.  
HandyDandy can be run by using console/terminal command `dotnet HandyDandy.dll` for Linux, `dotnet HandyDandy` on MacOs and running the 
`HandyDandy.exe` on Windows.  
Linux may require providing persmissions first
([more info](https://stackoverflow.com/questions/46843863/how-to-run-net-core-console-app-on-linux)):  
1. Provide execute permissions `chmod 777 ./HandyDandy`
2. Execute application `./HandyDandy`

## Contributing
Please first check out [conventions](https://github.com/Autarkysoft/Conventions) for information about coding styles, 
versioning, making pull requests, and more.

## Donations
If You found this tool helpful consider making a donation:  
Legacy address: 1Q9swRQuwhTtjZZ2yguFWk7m7pszknkWyk  
SegWit address: bc1q3n5t9gv40ayq68nwf0yth49dt5c799wpld376s
