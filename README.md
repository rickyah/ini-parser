Ini File Parser
===============

.Net library for managing data from a an INI file format the easy way!

Allows reading / writing INI data to and from I/O streams, file streams and/or plain strings.


The library is really simple to use. 
Ini data is stored in nested dictionaries, so accessing the value associated to a key in a section is straightforward:
```csharp
// Load ini file
IniData data = parser.LoadFile("TestIniFile.ini");

// Retrieve the value for the key with name 'fullscreen' inside a config section named 'ConfigSection'
// values are always retrieved as an string
string useFullScreen = data["ConfigSection"]["fullscreen"];

// Modify that very same value
data["ConfigSection"]["fullscreen"] = "true";

// save a new ini file
parser.SaveFile("NewTestIniFile.ini", data);
```

Enjoy!


Copyright (c) 2008 Ricardo Amores Hernández
