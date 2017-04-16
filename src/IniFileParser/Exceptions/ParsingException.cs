using System;
using System.Reflection;

namespace IniParser.Exceptions
{
    /// <summary>
    /// Represents an error ococcurred while parsing data
    /// </summary>
    public class ParsingException : Exception
    {
        /// <summary>
        /// Version of the IniParser library
        /// </summary>
        // Make sure to keep the "private set", even if unused. This is required for Mono 3.12.1 support!
        public Version LibVersion { get; private set; }

        /// <summary>
        /// Line number that contains unparsable data
        /// </summary>
        // Make sure to keep the "private set", even if unused. This is required for Mono 3.12.1 support!
        public int LineNumber { get; private set; }

        /// <summary>
        /// Content of the unparsable line
        /// </summary>
        // Make sure to keep the "private set", even if unused. This is required for Mono 3.12.1 support!
        public string LineValue { get; private set; }

        public ParsingException(string msg)
            : this(msg, 0, string.Empty, null)
        { }

        public ParsingException(string msg, Exception innerException)
            : this(msg, 0, string.Empty, innerException)
        { }

        public ParsingException(string msg, int lineNumber, string lineValue)
            : this(msg, lineNumber, lineValue, null)
        { }

        public ParsingException(string msg, int lineNumber, string lineValue, Exception innerException)
            : base(
                string.Format(
                    "{0} while parsing line number {1} with value \'{2}\' - IniParser version: {3}",
                    msg, lineNumber, lineValue, Assembly.GetExecutingAssembly().GetName().Version),
                    innerException)
        {
            LibVersion = Assembly.GetExecutingAssembly().GetName().Version;
            LineNumber = lineNumber;
            LineValue = lineValue;
        }
    }
}
