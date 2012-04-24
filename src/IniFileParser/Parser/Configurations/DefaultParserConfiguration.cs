namespace IniParser.Parser.Configurations
{
    public class DefaultParserConfiguration : BaseParserConfiguration
    {
        public DefaultParserConfiguration()
        {
            SectionStartChar = '[';
            SectionEndChar = ']';
            CommentChar = ';';
            KeyValueAssigmentChar = '=';

            AllowKeysWithoutSection = true;
            AllowDuplicateKeys = false;
            AllowDuplicateSections = false;
            ThrowExceptionsOnError = true;
        }

    }
}