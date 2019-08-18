using System;

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
        public ParsingException(string msg)
           : this(msg, 0, string.Empty, null)
        { }

        public ParsingException(string msg, uint lineNumber)
            :this(msg, lineNumber, string.Empty, null)
        {}

        public ParsingException(string msg, Exception innerException)
            :this(msg, 0, string.Empty, innerException) 
        {}

        public ParsingException(string msg, uint lineNumber, string lineContents)
            :this(msg, lineNumber, lineContents, null)
        {}
            
        public ParsingException(string msg, uint lineNumber, string lineContents, Exception innerException)
            : base(
                string.Format(
                    "{0} while parsing line number {1} with value \'{2}\' - IniParser version: {3}", 
                    msg, 
                    lineNumber, 
                    lineContents,
                    ParsingException.AssemblyVersion),
                innerException) 
        { 
            LibVersion = ParsingException.AssemblyVersion;
            LineNumber = lineNumber;
            LineContents = lineContents;
        }

        static private Version AssemblyVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
