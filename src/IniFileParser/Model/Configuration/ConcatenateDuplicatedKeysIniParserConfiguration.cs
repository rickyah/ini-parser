namespace IniParser.Model.Configuration
{
    public class ConcatenateDuplicatedKeysIniParserConfiguration : IniParserConfiguration
    {
        public new bool AllowDuplicateKeys { get { return true; } }

        public ConcatenateDuplicatedKeysIniParserConfiguration()
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
        ///     Defaults to ';'
        public string ConcatenateSeparator { get; set; }
    }

}