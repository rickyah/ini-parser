using System;
using System.Text.RegularExpressions;
using IniParser.Parser;

namespace IniParser.Model.Configuration
{
    [Obsolete("Kept for backward compatibility, just use IniParserConfiguration class")]
    public class DefaultIniParserConfiguration : ConcatenateDuplicatedKeysIniParserConfiguration {}

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
    public class IniParserConfiguration : ICloneable
    {
        #region Initialization
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
        public IniParserConfiguration()
        {
            CommentString = ";";
            SectionStartChar = '[';
            SectionEndChar = ']';
            KeyValueAssigmentChar = '=';
            AssigmentSpacer = " ";
            NewLineStr = Environment.NewLine;
            ConcatenateDuplicateKeys = false;
            AllowKeysWithoutSection = true;
            AllowDuplicateKeys = false;
            AllowDuplicateSections = false;
            AllowCreateSectionsOnFly = true;
            ThrowExceptionsOnError = true;
            SkipInvalidLines = false;
        }

        /// <summary>
        ///     Copy ctor.
        /// </summary>
        /// <param name="ori">
        ///     Original instance to be copied.
        /// </param>
        public IniParserConfiguration(IniParserConfiguration ori)
        {
            AllowDuplicateKeys = ori.AllowDuplicateKeys;
            OverrideDuplicateKeys = ori.OverrideDuplicateKeys;
            AllowDuplicateSections = ori.AllowDuplicateSections;
            AllowKeysWithoutSection = ori.AllowKeysWithoutSection;
            AllowCreateSectionsOnFly = ori.AllowCreateSectionsOnFly;

            SectionStartChar = ori.SectionStartChar;
            SectionEndChar = ori.SectionEndChar;
            CommentString = ori.CommentString;
            ThrowExceptionsOnError = ori.ThrowExceptionsOnError;

          // Regex values should recreate themselves.
        }
        #endregion

        #region IniParserConfiguration

        public Regex CommentRegex { get; set; }

        public Regex SectionRegex { get; set; }

        /// <summary>
        ///     Sets the char that defines the start of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '['
        /// </remarks>
        public char SectionStartChar
        {
            get { return _sectionStartChar; }
            set
            {
                _sectionStartChar = value;
                RecreateSectionRegex(_sectionStartChar);
            }
        }

        /// <summary>
        ///     Sets the char that defines the end of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character ']'
        /// </remarks>
        public char SectionEndChar
        {
            get { return _sectionEndChar; }
            set
            {
                _sectionEndChar = value;
                RecreateSectionRegex(_sectionEndChar);
            }
        }

        /// <summary>
        ///     Retrieving section / keys by name is done with a case-insensitive
        ///     search.
        /// </summary>
        /// <remarks>
        ///     Defaults to false (case sensitive search)
        /// </remarks>
        public bool CaseInsensitive{ get; set; }

        /// <summary>
        ///     Sets the char that defines the start of a comment.
        ///     A comment spans from the comment character to the end of the line.
        /// </summary>
        /// <remarks>
        ///     Defaults to character ';'
        /// </remarks>
        [Obsolete("Please use the CommentString property")]
        public char CommentChar
        {
            get { return CommentString[0]; }
            set { CommentString = value.ToString(); }
        }

        /// <summary>
        ///     Sets the string that defines the start of a comment.
        ///     A comment spans from the mirst matching comment string
        ///     to the end of the line.
        /// </summary>
        /// <remarks>
        ///     Defaults to string ";"
        /// </remarks>
        public string CommentString
        {

            get { return _commentString ?? string.Empty; }
            set
            {
                // Sanitarize special characters for a regex
                foreach (var specialChar in _strSpecialRegexChars)
                {
                    value = value.Replace(new String(specialChar, 1), @"\" + specialChar);
                }

                CommentRegex = new Regex(string.Format(_strCommentRegex, value));
                _commentString = value;
            }
        }

        /// <summary>
        ///     Gets or sets the string to use as new line string when formating an IniData structure using a
        ///     IIniDataFormatter. Parsing an ini-file accepts any new line character (Unix/windows)
        /// </summary>
        /// <remarks>
        ///     This allows to write a file with unix new line characters on windows (and vice versa)
        /// </remarks>
        /// <value>Defaults to value Environment.NewLine</value>
        public string NewLineStr
        {
            get; set;
        }
        /// <summary>
        ///     Sets the char that defines a value assigned to a key
        /// </summary>
        /// <remarks>
        ///     Defaults to character '='
        /// </remarks>
        public char KeyValueAssigmentChar { get; set; }

        /// <summary>
        ///     Sets the string around KeyValuesAssignmentChar
        /// </summary>
        /// <remarks>
        ///     Defaults to string ' '
        /// </remarks>
        public string AssigmentSpacer { get; set; }

        /// <summary>
        ///     Allows having keys in the file that don't belong to any section.
        ///     i.e. allows defining keys before defining a section.
        ///     If set to <c>false</c> and keys without a section are defined,
        ///     the <see cref="IniDataParser"/> will stop with an error.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        public bool AllowKeysWithoutSection { get; set; }
			
        /// <summary>
        ///     If set to <c>false</c> and the <see cref="IniDataParser"/> finds duplicate keys in a
        ///     section the parser will stop with an error.
        ///     If set to <c>true</c>, duplicated keys are allowed in the file. The value
        ///     of the duplicate key will be the last value asigned to the key in the file.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool AllowDuplicateKeys { get; set; }

        /// <summary>
        ///     Only used if <see cref="IniParserConfiguration.AllowDuplicateKeys"/> is also <c>true</c>
        ///     If set to <c>true</c> when the parser finds a duplicate key, it overrites
        ///     the previous value, so the key will always contain the value of the
        ///     last key readed in the file
        ///     If set to <c>false</c> the first readed value is preserved, so the key will
        ///     always contain the value of the first key readed in the file
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool OverrideDuplicateKeys { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether duplicate keys are concatenate
        ///     together by <see cref="ConcatenateSeparator"/>.
        /// </summary>
        /// <value>
        ///     Defaults to <c>false</c>.
        /// </value>
        public bool ConcatenateDuplicateKeys { get; set; }

        /// <summary>
        ///     If <c>true</c> the <see cref="IniDataParser"/> instance will thrown an exception
        ///     if an error is found.
        ///     If <c>false</c> the parser will just stop execution and return a null value.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        public bool ThrowExceptionsOnError { get; set; }

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
        public bool AllowDuplicateSections { get; set; }

        /// <summary>
        ///     If set to <c>false</c>, the <see cref="IniDataParser"/> stop with a error if you try
        ///     to access a section that was not created previously and the parser will stop with an error.
        ///     If set to <c>true</c>, inexistents sections are created, always returning a valid
        ///     <see cref="SectionData"/> element.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool AllowCreateSectionsOnFly { get; set; }

        public bool SkipInvalidLines { get; set; }

        #endregion

        #region Fields
        private char _sectionStartChar;
        private char _sectionEndChar;
        private string _commentString;
        #endregion

        #region Constants
        protected const string _strCommentRegex = @"^{0}(.*)";
        protected const string _strSectionRegexStart = @"^(\s*?)";
        protected const string _strSectionRegexMiddle = @"{1}\s*[\p{L}\p{P}\p{M}_\""\'\{\}\#\+\;\*\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s\\\~]+\s*";
        protected const string _strSectionRegexEnd = @"(\s*?)$";
        protected const string _strKeyRegex = @"^(\s*[_\.\d\w]*\s*)";
        protected const string _strValueRegex = @"([\s\d\w\W\.]*)$";
        protected const string _strSpecialRegexChars = @"[]\^$.|?*+()";
        #endregion

        #region Helpers
        private void RecreateSectionRegex(char value)
        {
            if (char.IsControl(value)
                || char.IsWhiteSpace(value)
                || CommentString.Contains(new string(new [] {value}))
                || value == KeyValueAssigmentChar)
                throw new Exception(string.Format("Invalid character for section delimiter: '{0}", value));

            string builtRegexString = _strSectionRegexStart;

            if (_strSpecialRegexChars.Contains(new string(_sectionStartChar, 1)))
                builtRegexString += "\\" + _sectionStartChar;
            else builtRegexString += _sectionStartChar;

            builtRegexString += _strSectionRegexMiddle;

            if (_strSpecialRegexChars.Contains(new string(_sectionEndChar, 1)))
                builtRegexString += "\\" + _sectionEndChar;
            else
                builtRegexString += _sectionEndChar;

            builtRegexString += _strSectionRegexEnd;

            SectionRegex = new Regex(builtRegexString);
        }
        #endregion

        public override int GetHashCode()
        {
            var hash = 27;
            foreach (var property in GetType().GetProperties())
            {
                hash = (hash * 7) + property.GetValue(this, null).GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            var copyObj = obj as IniParserConfiguration;
            if (copyObj == null) return false;

            var oriType = this.GetType();
            try
            {
                foreach (var property in oriType.GetProperties())
                {
                    if (property.GetValue(copyObj, null).Equals(property.GetValue(this, null)))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IniParserConfiguration Clone()
        {
            return this.MemberwiseClone() as IniParserConfiguration;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
