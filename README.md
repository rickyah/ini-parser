Ini File Parser
===============

A Mono compatible .Net open source library for managing data from an INI file format.


Allows reading / writing INI data to and from I/O streams, file streams and/or plain strings.

MIT licensed (http://opensource.org/licenses/MIT or see LICENSE.txt)

## Introduction

The library is really simple to use: ini data is stored in nested dictionaries, so accessing the value associated to a key in a section is straightforward:

```csharp
// Read an ini file fron disk
IniData data = parser.ReadFile("TestIniFile.ini");

// Retrieve the value for the key with name 'fullscreen' inside a config section named 'ConfigSection'
// values are always retrieved as an string
string useFullScreen = data["ConfigSection"]["fullscreen"];

// Modify that very same value
data["ConfigSection"]["fullscreen"] = "true";

// Persist a new ini file to disk
parser.WriteFile("NewTestIniFile.ini", data);
```

## Version 2.0
This version features a simpler way to customize the INI parsing. As the
INI file format is not really a standard it really helps to have an easier way to allow an user to change the way INI data is parser.

There are two ways to customize the INI data parsing:

- pass a configuration object to an IniParser instance, which will be
used to specify the behaviour of the parser. A default implementation is used
if none is passed.
- Create a derived class from `IniDataParser` and override some methods to a more fine-grained control about how to parse the ini data.

### Changing parser configuration
I order to change the behaviour of the ini parser, an configuration object following the `IIniParserConfiguration` interface must be passed to the constructor of an `IniDataParser` object.


Enjoy!



Copyright (c) 2008 Ricardo Amores Hernández
