using System;
using System.Collections.Generic;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Model.Configuration;

namespace IniParser.Parser
{

    public class ConcatenateDuplicatedKeysIniDataParser : IniDataParser
    {
        public new ConcatenateDuplicatedKeysIniParserConfiguration Configuration
        {
            get
            {
                return (ConcatenateDuplicatedKeysIniParserConfiguration)base.Configuration;
            }
            set
            {
                base.Configuration = value;
            }
        }

        public ConcatenateDuplicatedKeysIniDataParser()
            :this(new IniScheme(), new ConcatenateDuplicatedKeysIniParserConfiguration())
        {}

        public ConcatenateDuplicatedKeysIniDataParser(IniScheme scheme, ConcatenateDuplicatedKeysIniParserConfiguration parserConfiguration)
            :base(scheme, parserConfiguration)
        {}

        protected override void HandleDuplicatedKeyInCollection(string key, string value, KeyDataCollection keyDataCollection, string sectionName)
        {
            keyDataCollection[key] += Configuration.ConcatenateSeparator + value;
        }
    }

}
