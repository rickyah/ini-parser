using System;
namespace IniParser.Configuration
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
    public class IniParserConfiguration : IDeepCloneable<IniParserConfiguration>
    {
        /// <summary>
        ///     Default values used if an instance of <see cref="IniDataParser"/>
        ///     is created without specifying a configuration.
        /// </summary>
        public IniParserConfiguration()
        {

            AllowKeysWithoutSection = true;
            AllowDuplicateKeys = false;
            AllowDuplicateSections = false;
            CaseInsensitive = false;
            ConcatenateDuplicateKeys = false;
            SkipInvalidLines = false;
            ThrowExceptionsOnError = true;
        }

        /// <summary>
        ///     Copy ctor.
        /// </summary>
        /// <param name="ori">
        ///     Original instance to be copied.
        /// </param>
        public IniParserConfiguration(IniParserConfiguration ori)
        {
            this.OverwriteWith(ori);
        }

        
        /// <summary>
        ///     If set to <c>false</c> and the <see cref="IniDataParser"/> finds a duplicate section
        ///     the parser will stop with an error.
        ///     If set to <c>true</c>, duplicated sections are allowed in the file, but only a
        ///     <see cref="Section"/> element will be created in the <see cref="IniData.Sections"/>
        ///     collection.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool AllowDuplicateSections { get; set; }
          
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
        ///     Retrieving section / keys by name is done with a case-insensitive
        ///     search.
        /// </summary>
        /// <remarks>
        ///     Defaults to false (case sensitive search)
        /// </remarks>
        public bool CaseInsensitive{ get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether duplicate keys are concatenate
        ///     together by <see cref="ConcatenateSeparator"/>.
        /// </summary>
        /// <value>
        ///     Defaults to <c>false</c>.
        /// </value>
        public bool ConcatenateDuplicateKeys { get; set; }
        
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
        ///     If <c>true</c> the <see cref="IniDataParser"/> instance will thrown an exception
        ///     if an error is found.
        ///     If <c>false</c> the parser will just stop execution and return a null value.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        public bool ThrowExceptionsOnError { get; set; }
        
        /// <summary>
        ///     Skips the processing of a line while parsing if it contains an
        ///     error instead of throwing an exception.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool SkipInvalidLines { get; set; }


        public void OverwriteWith(IniParserConfiguration ori)
        {
            AllowDuplicateKeys = ori.AllowDuplicateKeys;
            AllowDuplicateSections = ori.AllowDuplicateSections;
            AllowKeysWithoutSection = ori.AllowKeysWithoutSection;
            CaseInsensitive = ori.CaseInsensitive;
            ConcatenateDuplicateKeys = ori.ConcatenateDuplicateKeys;
            OverrideDuplicateKeys = ori.OverrideDuplicateKeys;
            SkipInvalidLines = ori.SkipInvalidLines;
            ThrowExceptionsOnError = ori.ThrowExceptionsOnError;
        }
        
        #region IDeepCloneable<T> Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public IniParserConfiguration DeepClone()
        {
			return new IniParserConfiguration(this);
        }
        #endregion
    }
}
