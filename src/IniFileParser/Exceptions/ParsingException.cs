using System;

namespace IniParser.Exceptions
{
    /// <summary>
    /// Represents an error ococcurred while parsing data 
    /// </summary>
    public class ParsingException : Exception
    {
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
            : base(string.Format("Error \'{2}\' while parsing line {1}: \'{0}\'", lineValue, lineNumber, msg), innerException) 
        { 
            LineNumber = lineNumber;
            LineValue = lineValue;
        }
    }
}
