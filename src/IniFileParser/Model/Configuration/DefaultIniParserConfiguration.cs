using IniParser.Parser;

namespace IniParser.Model.Configuration
{
    /// <summary>
    ///     Default values used if an instance of <see cref="IniDataParser"/>
    ///     is created without specifying a configuration.
    /// </summary>
    /// <remarks>
    ///     By default the various delimiters for the data are setted:
    ///     <para>';' for one-line comments</para>
    ///     <para>'[' ']' for delimiting a section</para>
    ///     <para>'=' for linking key / value pairs</para>
    ///     <example>
    ///         An example of well formed data with the default values:
    ///         <para>
    ///         ;section comment<br/>
    ///         [section] ; section comment<br/>
    ///         <br/>
    ///         ; key comment<br/>
    ///         key = value ;key comment<br/>
    ///         <br/>
    ///         ;key2 comment<br/>
    ///         key2 = value<br/>
    ///         </para>
    ///     </example>
    /// </remarks>
    public class DefaultIniParserConfiguration : BaseIniParserConfiguration
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        public DefaultIniParserConfiguration()
        {
            SectionStartChar = '[';
            SectionEndChar = ']';
            CommentChar = ';';
            KeyValueAssigmentChar = '=';

            AllowKeysWithoutSection = true;
            AllowDuplicateKeys = false;
            AllowDuplicateSections = false;
            ThrowExceptionsOnError = true;
            SkipInvalidLines = false;
        }
    }
}