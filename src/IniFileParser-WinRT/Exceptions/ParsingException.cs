using System;
using System.Reflection;
using IniParser.Extensions;

namespace IniParser.Exceptions
{
    /// <summary>
    /// Represents an error ococcurred while parsing data 
    /// </summary>
    public class ParsingException : Exception
    {
        public Version LibVersion {get; private set;}
        public int LineNumber {get; private set;}
        public string LineValue {get; private set;}

        public ParsingException(string msg)
            :this(msg, 0, string.Empty, null) 
        {}

        public ParsingException(string msg, Exception innerException)
            :this(msg, 0, string.Empty, innerException) 
        {}

        public ParsingException(string msg, int lineNumber, string lineValue)
            :this(msg, lineNumber, lineValue, null)
        {}
            
        public ParsingException(string msg, int lineNumber, string lineValue, Exception innerException)
            : base(
                string.Format(
                    "{0} while parsing line number {1} with value \'{2}\' - IniParser version: {3}", 
                    msg, lineNumber, lineValue, typeof(ParsingException).GetTypeInfo().Assembly.GetVersion()),
                innerException) 
        { 
            LibVersion = new Version(typeof(ParsingException).GetTypeInfo().Assembly.GetVersion());
            LineNumber = lineNumber;
            LineValue = lineValue;
        }
    }
}
