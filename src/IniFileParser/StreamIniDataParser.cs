using System;
using System.IO;
using IniParser.Model;
using IniParser.Parser;

namespace IniParser
{

    /// <summary>
    ///     Represents an INI data parser for streams.
    /// </summary>
    public class StreamIniDataParser
    {
        /// <summary>
        ///     This instance will handle ini data parsing and writing
        /// </summary>
        public IniDataParser Parser { get; protected set; }

        /// <summary>
        ///     Ctor
        /// </summary>
        public StreamIniDataParser() : this (new IniDataParser()) {}

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="parser"></param>
        public StreamIniDataParser(IniDataParser parser)
        {
            Parser = parser;
        }
        #region Public Methods

        /// <summary>
        /// Reads data in INI format from a stream.
        /// </summary>
        /// <param name="reader">Reader stream.</param>
        /// <returns>
        ///     And <see cref="IniData"/> instance with the readed ini data parsed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reader"/> is <c>null</c>.
        /// </exception>
        public IniData ReadData(StreamReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            
            return Parser.Parse(reader.ReadToEnd());
        }

        /// <summary>
        ///     Writes the ini data to a stream.
        /// </summary>
        /// <param name="writer">A write stream where the ini data will be stored</param>
        /// <param name="iniData">An <see cref="IniData"/> instance.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="writer"/> is <c>null</c>.
        /// </exception>
        public void WriteData(StreamWriter writer, IniData iniData)
        {
            if (writer == null)
                throw new ArgumentNullException("reader");

            writer.Write(iniData.ToString());
        }

        #endregion
    }
}
