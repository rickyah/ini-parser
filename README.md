Ini File Parser
===============

A Mono compatible .Net library for managing data from a an INI file format the easy way!

Allows reading / writing INI data to and from I/O streams, file streams and/or plain strings.


The library is really simple to use; Ini data is stored in nested dictionaries, so accessing the value associated to a key in a section is straightforward:
```csharp
// Load ini file
IniData data = parser.ReadFile("TestIniFile.ini");

// Retrieve the value for the key with name 'fullscreen' inside a config section named 'ConfigSection'
// values are always retrieved as an string
string useFullScreen = data["ConfigSection"]["fullscreen"];

// Modify that very same value
data["ConfigSection"]["fullscreen"] = "true";

// save a new ini file
parser.WriteFile("NewTestIniFile.ini", data);
```

## Version 2.0
This version features a simpler way to customize the INI parsing. As the
INI file format is not really a standard, an easier way to allow the library
user to change the way INI data is parser is a must.
Now you need to pass a configuration object to an IniParser instance, which will be
used to set the behaviour of the parser. A default implementation is used
if none is passed.

Do you need to modify the default settings for the parser? That's easy, 
just override some properties in the default configuration object:
```csharp

var iniStr = @"[section1]
#data = 1
;data = 2";

var config = new DefaultIniParserConfiguration();

config.CommentChar = '#';

var parser = new IniDataParser(config);

var iniData = parser.Parse(iniStr);

Assert.That(iniData["section1"][";data"], Is.EqualTo("2"));
            
```

If you want to reuse a particular complex configuration for the parser, just create a custom
configuration class and reuse the same configuration everywhere:
```csharp
class MyTestConfiguration : DefaultIniParserConfiguration
{
  public MyTestConfiguration()
  {
    SectionStartChar = '<';
    SectionEndChar = '>';
    CommentChar = '#';
    KeyValueAssigmentChar = '=';

    AllowKeysWithoutSection = true;
    AllowDuplicateKeys = true;
    OverrideDuplicateKeys = true;
    AllowDuplicateSections = true;
    ThrowExceptionsOnError = false;
    SkipInvalidLines = true;
  }
}

var parser = new IniDataParser(new MyTestConfiguration());
        
```
Enjoy!


Copyright (c) 2008 Ricardo Amores Hernández
