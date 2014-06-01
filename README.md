# INI File Parser

A Mono-compatible(*) .NET library for reading/writing INI data from IO streams, file streams, and strings written in C#.

Also implements merging operations, both for complete ini files, sections, or even just a subset of the keys contained by the files.


(*) This library is 100% .NET code and does not have any dependencies on Windows API calls in order to be portable.

[![Build Status](https://travis-ci.org/rickyah/ini-parser.png?branch=master)](https://travis-ci.org/rickyah/ini-parser)

## Installation

The library is published to [NuGet](https://www.nuget.org/packages/ini-parser/) and can be installed on the command-line from the directory containing your solution.

```bat
> nuget install ini-parser
```

Or, from the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) in Visual Studio

```powershell
PM> Install-Package ini-parser
```

If you are using Visual Studio, you can download the [NuGet Package Manager](http://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c) extension that will allow adding the NuGet dependency for your project.

If you use MonoDevelop / Xamarin Studio, you can install the [MonoDevelop NuGet AddIn](https://github.com/mrward/monodevelop-nuget-addin) to also be able to add this library as dependency from the IDE.

## Getting Started

INI data is stored in nested dictionaries, so accessing the value associated to a key in a section is straightforward. Load the data using one of the provided methods.

```csharp
var data = parser.ReadFile("Configuration.ini");
```

Retrieve the value for a key inside of a named section. Values are always retrieved as `string`s.

```csharp
var useFullScreen = data["UI"]["fullscreen"];
```

Modify the value in the dictionary, not the value retrieved, and save to a new file or overwrite.

```csharp
data["UI"]["fullscreen"] = "true";
parser.WriteFile("Configuration.ini", data);
```

See the [wiki](https://github.com/rickyah/ini-parser/wiki) for more usage examples.


## Merging ini files
Merging ini files is a one-method operation:

``csharp

   var parser = new IniParser.Parser.IniDataParser();

   IniData config = parser.Parse("global_config.ini");        
   IniData user_config = parser.Parse("user_config.ini");

   
   config.Merge(user_config);

   // config now contains that data from both ini files, and the values of
   // the keys and sections are overwritten with the values of the keys and
   // sections that also existed in the user config file
```

Keep in mind that you merge individual sections if you like:

```csharp
config["user_settings"].Merge(user_config["user_settings"]);
```

## Contributing

Do you have an idea to improve this library, or did you happen to run into a bug. Please share your idea or the bug you found in the issues page, or even better: feel free to fork and [contribute](https://github.com/rickyah/ini-parser/wiki/Contributing) to this project!

## Version 2.0!
Since the INI format isn't really a "standard", this version introduces a simpler way to customize INI parsing:

 * Pass a configuration object to an `IniParser`, specifying the behaviour of the parser. A default implementation is used if none is provided.
 
 * Derive from `IniDataParser` and override the fine-grained parsing methods.

