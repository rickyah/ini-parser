using System;
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
    public class IniParserConfiguration : ICloneable
    {
        /// <summary>
        ///     Default values used if an instance of <see cref="IniDataParser"/>
        ///     is created without specifying a configuration.
        /// </summary>
        public IniParserConfiguration(IniScheme scheme)
        {
            Scheme = scheme;

            ConcatenateDuplicateKeys = false;
            AllowKeysWithoutSection = true;
            AllowDuplicateKeys = false;
            AllowDuplicateSections = false;
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

            ThrowExceptionsOnError = ori.ThrowExceptionsOnError;
			Scheme = ori.Scheme.Clone();
        }

		public IniScheme Scheme { get; private set; }

        /// <summary>
        ///     Retrieving section / keys by name is done with a case-insensitive
        ///     search.
        /// </summary>
        /// <remarks>
        ///     Defaults to false (case sensitive search)
        /// </remarks>
        public bool CaseInsensitive{ get; set; }

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

        public bool SkipInvalidLines { get; set; }


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
			return new IniParserConfiguration(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
