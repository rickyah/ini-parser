using System;
using System.Collections.Generic;
using System.Text;

namespace IniParser
{
    /// <summary>
    /// Represents an error ococcurred while parsing data 
    /// </summary>
    public class ParsingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        public ParsingException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="msg">The message describing the exception cause.</param>
        public ParsingException(string msg)
            : base(msg) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="msg">The message describing the exception cause.</param>
        /// <param name="innerException">An inner exception.</param>
        public ParsingException(string msg, Exception innerException)
            : base(msg, innerException) { }
    }
}
