using System;
using System.Text.RegularExpressions;
using IniParser.Parser;

namespace IniParser.Model.Configuration
{
    /// <summary>
    ///     Defines data for a Parser configuration object.
    /// </summary>
    ///     With a configuration object you can redefine how the parser
    ///     will detect special items in the ini file by defining new regex
    ///     (e.g. you can redefine the comment regex so it just treat text as
    ///     a comment iff the comment caracter is the first in the line)
    ///     or changing the set of characters used to define elements in    
    ///     the ini file (e.g. change the 'comment' caracter from ';' to '#')
    ///     You can also define how the parser should treat errors, or how liberal
    ///     or conservative should it be when parsing files with "strange" formats.
    public interface IIniParserConfiguration : ICloneable
    {
        /// <summary>
        ///     Regular expression used to match a comment string
        /// </summary>
        Regex CommentRegex { get; set; }

        /// <summary>
        ///     Regular expression used to match a section string
        /// </summary>
        Regex SectionRegex { get; set; }

        /// <summary>
		///     Regular expression used to match a key / value pair string
        /// </summary>
        //Regex KeyValuePairRegex { get; set; }

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
        ///     Only used if <see cref="AllowDuplicateKeys"/> is also <c>true</c> 
        ///     If set to <c>true</c> when the parser finds a duplicate key, it overrites
        ///     the previous value, so the key will always contain the value of the 
        ///     last key readed in the file
        ///     If set to <c>false</c> the first readed value is preserved, so the key will
        ///     always contain the value of the first key readed in the file
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        bool OverrideDuplicateKeys { get; set; }

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

        /// <summary>
        ///     IF set to <c>true</c>, when the parser finds an invalid line, it just skips it 
        ///     instead of throwing a <see cref="ParseException"/> or returning null when parsing
        ///     (if exceptions  are disabled because the property <seealso cref="ThrowExceptionsOnError"/>
        ///     is set to <c>false</c>).
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>
        /// </remarks>
        bool SkipInvalidLines { get; set; }


        new IIniParserConfiguration Clone();
    }
}