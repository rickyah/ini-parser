using System;
using System.Text.RegularExpressions;
using IniParser.Parser;

namespace IniParser.Model.Configuration
{
    /// <summary>
    ///     Configuration data used for a <see cref="IniDataParser"/> class instance.
    /// </summary>
    /// 
    /// This class allows changing the behaviour of a <see cref="IniDataParser"/> instance.
    /// The <see cref="IniDataParser"/> class exposes an instance of this class via 
    /// <see cref="IniDataParser.Configuration"/>
    public class BaseIniParserConfiguration : IIniParserConfiguration
    {
        #region Initialization
        /// <summary>
        ///     Ctor.
        /// </summary>
        public BaseIniParserConfiguration() { }

        /// <summary>
        ///     Copy ctor.
        /// </summary>
        /// <param name="ori">
        ///     Original instance to be copied.
        /// </param>
        public BaseIniParserConfiguration(IIniParserConfiguration ori)
        {
            AllowDuplicateKeys = ori.AllowDuplicateKeys;
            OverrideDuplicateKeys = ori.OverrideDuplicateKeys;
            AllowDuplicateSections = ori.AllowDuplicateSections;
            AllowKeysWithoutSection = ori.AllowKeysWithoutSection;

            SectionStartChar = ori.SectionStartChar;
            SectionEndChar = ori.SectionEndChar;
            CommentChar = ori.CommentChar;
            ThrowExceptionsOnError = ori.ThrowExceptionsOnError;

            // Regex values should recreate themselves.
        }
        #endregion

        #region State
        /// <summary>
        ///     Regular expression for matching a comment string
        /// </summary>
        public Regex CommentRegex { get; set; }

        /// <summary>
        ///     Regular expression for matching a section string
        /// </summary>
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
        ///     Sets the char that defines the start of a comment.
        ///     A comment spans from the comment character to the end of the line.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '#'
        /// </remarks>
        public char CommentChar
        {
            get { return _commentChar; }
            set
            {
                CommentRegex = new Regex(value + _strCommentRegex);
                _commentChar = value;
            }
        }

        /// <summary>
        ///     Sets the char that defines a value assigned to a key
        /// </summary>
        /// <remarks>
        ///     Defaults to character '='
        /// </remarks>
        public char KeyValueAssigmentChar { get; set; }

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
        ///     Only used if <see cref="IIniParserConfiguration.AllowDuplicateKeys"/> is also <c>true</c> 
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

        public bool SkipInvalidLines { get; set; }

        #endregion

        #region Fields
        private char _sectionStartChar;
        private char _sectionEndChar;
        private char _commentChar;
        #endregion

        #region Constants
        protected const string _strCommentRegex = @".*";
        protected const string _strSectionRegexStart = @"^(\s*?)";
        protected const string _strSectionRegexMiddle = @"{1}\s*[_\{\}\#\+\;\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s\\\~]+\s*";
        protected const string _strSectionRegexEnd = @"(\s*?)$";
        protected const string _strKeyRegex = @"^(\s*[_\.\d\w]*\s*)";
        protected const string _strValueRegex = @"([\s\d\w\W\.]*)$";
        protected const string _strSpecialRegexChars = @"[\^$.|?*+()";
        #endregion

        #region Helpers
        private void RecreateSectionRegex(char value)
        {
            if (char.IsControl(value)
                || char.IsWhiteSpace(value)
                || value == CommentChar
                || value == KeyValueAssigmentChar)
                throw new Exception(string.Format("Invalid character for section delimiter: '{0}",
                                                              value));

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

        public override bool Equals(object obj)
        {
            var copyObj = obj as BaseIniParserConfiguration;
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
        public IIniParserConfiguration Clone()
		{
			return this.MemberwiseClone() as IIniParserConfiguration;
        }
        #endregion

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
