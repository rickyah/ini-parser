# INI File Parser

A .NET, Mono and Unity3d compatible(*) library for reading/writing INI data from IO streams, file streams, and strings written in C#.

Also implements merging operations, both for complete ini files, sections, or even just a subset of the keys contained by the files.


(*) This library is 100% .NET code and does not have any dependencies on Windows API calls in order to be portable.

[![Build Status](https://travis-ci.org/rickyah/ini-parser.png?branch=master)](https://travis-ci.org/rickyah/ini-parser)


Get the latest version: https://github.com/rickyah/ini-parser/releases/latest
Install it with NuGet: https://www.nuget.org/packages/ini-parser/

## Version 2.0
Since the INI format isn't really a "standard", version 2 introduces a simpler way to customize INI parsing:

 * Pass a configuration object to an `IniParser`, specifying the behaviour of the parser. A default implementation is used if none is provided.
 
 * Derive from `IniDataParser` and override the fine-grained parsing methods.


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

All code examples expect the following using clauses:

```csharp
using IniParser;
using IniParser.Model;
```

INI data is stored in nested dictionaries, so accessing the value associated to a key in a section is straightforward. Load the data using one of the provided methods.

```csharp
var parser = new FileIniDataParser();
IniData data = parser.ReadFile("Configuration.ini");
```

Retrieve the value for a key inside of a named section. Values are always retrieved as `string`s.

```csharp
string useFullScreenStr = data["UI"]["fullscreen"];
// useFullScreenStr contains "true"
bool useFullScreen = bool.Parse(useFullScreenStr);
```

Modify the value in the dictionary, not the value retrieved, and save to a new file or overwrite.

```csharp
data["UI"]["fullscreen"] = "true";
parser.WriteFile("Configuration.ini", data);
```

Head to the [wiki](https://github.com/rickyah/ini-parser/wiki) for more usage examples, or [check out the code of the example project](https://github.com/rickyah/ini-parser/blob/development/src/INIFileParser.Example/Program.cs)


## Merging ini files
Merging ini files is a one-method operation:

```csharp

   var parser = new IniParser.Parser.IniDataParser();

   IniData config = parser.Parse(File.ReadAllText("global_config.ini"));
   IniData user_config = parser.Parse(File.ReadAllText("user_config.ini"));
   config.Merge(user_config);

   // config now contains that data from both ini files, and the values of
   // the keys and sections are overwritten with the values of the keys and
   // sections that also existed in the user config file
```

Keep in mind that you can merge individual sections if you like:

```csharp
config["user_settings"].Merge(user_config["user_settings"]);
```

## Comments

The library allows modifying the comments from an ini file. 
However note than writing the file back to disk, the comments will be rearranged so 
comments are written before the element they refer to.

To query, add or remove comments, access the property `Comments` available both in `SectionData` and `KeyData` models.

```csharp
var listOfCommentsForSection = config.["user_settings"].Comments;
var listOfCommentsForKey = config["user_settings"].GetKeyData("resolution").Comments;
```

## Unity3D
You can easily use this library in your Unity3D projects. Just drop either the code or the DLL inside your project's Assets folder and you're ready to go!

ini-parser is actually being used in [ProjectPrefs](http://u3d.as/content/garrafote/project-prefs/5so) a free add-on available in the Unity Assets Store that allows you to set custom preferences for your project. I'm not affiliated with this project: Kudos to Garrafote for making this add-on.

## Contributing

Do you have an idea to improve this library, or did you happen to run into a bug? Please share your idea or the bug you found in the [issues page](https://github.com/rickyah/ini-parser/issues), or even better: feel free to fork and [contribute](https://github.com/rickyah/ini-parser/wiki/Contributing) to this project with a Pull Request.
