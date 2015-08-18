using System;
using System.Text.RegularExpressions;
using IniParser.Parser;

namespace IniParser.Model.Configuration
{
    public class ConcatenateDuplicatedKeysIniParserConfiguration : IniParserConfiguration
    {
        public new bool AllowDuplicateKeys { get {return true; }}
        public ConcatenateDuplicatedKeysIniParserConfiguration()
            :base()
        {
            this.ConcatenateSeparator = ";";
        }

        public ConcatenateDuplicatedKeysIniParserConfiguration(ConcatenateDuplicatedKeysIniParserConfiguration ori)
            :base(ori)
        {
            this.ConcatenateSeparator = ori.ConcatenateSeparator;
        }

        /// <summary>
        ///     Gets or sets the string used to concatenate duplicated keys.
        /// </summary>
        ///     Defaults to ';'.
        /// </value>
        public string ConcatenateSeparator { get; set; }
    }

}