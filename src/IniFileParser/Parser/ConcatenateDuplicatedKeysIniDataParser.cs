using System;
using System.Collections.Generic;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Model.Configuration;

namespace IniParser.Parser
{

    public class ConcatenateDuplicatedKeysIniDataParser : IniDataParser
    {
        public string ConcatenateSeparator {get; private set;}

        public ConcatenateDuplicatedKeysIniDataParser(string concatenateSeparator = ";")
            :this(new DefaultIniParserConfiguration(), concatenateSeparator)
        {
        }

        public ConcatenateDuplicatedKeysIniDataParser(IIniParserConfiguration parserConfiguration, string concatenateSeparator = ";")
            :base(parserConfiguration)
        {
            Configuration.AllowDuplicateKeys = true;
            ConcatenateSeparator = concatenateSeparator;
        }

        protected override void HandleDuplicatedKeyInCollection(string key, string value, KeyDataCollection keyDataCollection, string sectionName)
        {
            keyDataCollection[key] += ConcatenateSeparator + value;
        }
    }

}
