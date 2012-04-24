using System.Text.RegularExpressions;

namespace IniParser.Parser
{
    /// <summary>
    ///     Defines data for a Parser configuration object.
    ///     
    /// </summary>
    public interface IParserConfiguration
    {

        /// <summary>
        ///     Regular expression for matching a comment string
        /// </summary>
        Regex CommentRegex { get; set; }

        /// <summary>
        ///     Regular expression for matching a section string
        /// </summary>
        Regex SectionRegex { get; set; }

        /// <summary>
        ///     Regular expression for matching a key / value pair string
        /// </summary>
        Regex KeyValuePairRegex { get; set; }


        /// <summary>
        ///     Sets the char that defines the start of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '['
        /// </remarks>
        char SectionStartChar { get; set; }

        /// <summary>
        ///     Sets the char that defines the end of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character ']'
        /// </remarks>
        char SectionEndChar { get; set; }

        /// <summary>
        ///     Sets the char that defines the start of a comment.
        ///     A comment spans from the comment character to the end of the line.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '#'
        /// </remarks>
        char CommentChar { get; set; }

        /// <summary>
        ///     Sets the char that defines a value assigned to a key
        /// </summary>
        /// <remarks>
        ///     Defaults to character '='
        /// </remarks>
        char KeyValueAssigmentChar { get; set; }

        /// <summary>
        ///     Allows having keys in the file that don't belong to any section.
        ///     i.e. allows defining keys before defining a section.
        ///     If set to <c>false</c> and keys without a section are defined, 
        ///     the <see cref="IniDataParser"/> will stop with an error.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        bool AllowKeysWithoutSection { get; set; }

        /// <summary>
        ///     If set to <c>false</c> and the <see cref="IniDataParser"/> finds duplicate keys in a
        ///     section the parser will stop with an error.
        ///     If set to <c>true</c>, duplicated keys are allowed in the file. The value
        ///     of the duplicate key will be the last value asigned to the key in the file.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        bool AllowDuplicateKeys { get; set; }

        /// <summary>
        ///     If <c>true</c> the <see cref="IniDataParser"/> instance will thrown an exception
        ///     if an error is found. 
        ///     If <c>false</c> the parser will just stop execution and return a null value.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        bool ThrowExceptionsOnError { get; set; }

        /// <summary>
        ///     If set to <c>false</c> and the <see cref="IniDataParser"/> finds a duplicate section
        ///     the parser will stop with an error.
        ///     If set to <c>true</c>, duplicated sections are allowed in the file, but only a 
        ///     <see cref="SectionData"/> element will be created in the <see cref="IniData.Sections"/>
        ///     collection.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        bool AllowDuplicateSections { get; set; }
    }
}