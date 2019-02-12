using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IniParser.Exceptions
{
    /// <summary>
    /// Represents an error ococcurred while parsing data 
    /// </summary>
    public class ParsingException : Exception
    {
        public Version LibVersion {get; private set;}
        public uint LineNumber {get; private set;}
        public string LineContents {get; private set;}
        public ParsingException(string msg, uint lineNumber)
            :this(msg, lineNumber, string.Empty, null)
        {}

        public ParsingException(string msg,
                                uint lineNumber,
                                Exception innerException)
            :this(msg, lineNumber, string.Empty, innerException)
        {}

        public ParsingException(string msg,
                                uint lineNumber,
                                string lineContents)
            :this(msg, lineNumber, lineContents, null)
        {}
            
        public ParsingException(string msg,
                                uint lineNumber,
                                string lineContents,
                                Exception innerException)
            : base(string.Format("Error {0}{4} while parsing line {1}: \'{2}\' - IniParser version: {3}",
                                 msg,
                                 lineNumber,
                                 lineContents,
                                 Assembly.GetExecutingAssembly().GetName().Version,
                                 Environment.NewLine),
                                 innerException)
        { 
            LibVersion = Assembly.GetExecutingAssembly().GetName().Version;
            LineNumber = lineNumber;
            LineContents = lineContents;
        }
    }
}
