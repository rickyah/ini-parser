using System;

namespace IniParser.Exceptions
{
    /// <summary>
    /// Represents an error ococcurred while parsing data 
    /// </summary>
    public class ParsingException : Exception
    {
        public Version LibVersion {get;}
        public uint LineNumber {get;}
        public string LineContents {get;}

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
                $"{msg} while parsing line number {lineNumber} with value \'{lineContents}\'", 
                innerException) 
        { 
            LibVersion = GetAssemblyVersion();
            LineNumber = lineNumber;
            LineContents = lineContents;
        }

        private Version GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
