namespace IniParser.Model.Configuration
{
    public class ConcatenateDuplicatedKeysIniParserConfiguration : IniParserConfiguration
    {
        public new bool AllowDuplicateKeys { get { return true; } }

        public ConcatenateDuplicatedKeysIniParserConfiguration()
            :this(new IniScheme())
        {}

        public ConcatenateDuplicatedKeysIniParserConfiguration(IniScheme schema)
            :base(schema)
        {
            ConcatenateSeparator = ";";
        }

        public ConcatenateDuplicatedKeysIniParserConfiguration(ConcatenateDuplicatedKeysIniParserConfiguration ori)
            :base(ori)
        {
            ConcatenateSeparator = ori.ConcatenateSeparator;
        }

        /// <summary>
        ///     Gets or sets the string used to concatenate duplicated keys.
        /// </summary>
        ///     Defaults to ';'.
        /// </value>
        public string ConcatenateSeparator { get; set; }
    }

}