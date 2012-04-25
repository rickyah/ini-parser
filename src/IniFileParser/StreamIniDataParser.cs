using System;
using System.IO;
using IniParser.Model;
using IniParser.Parser;

namespace IniParser
{
    public class StreamIniDataParser : StreamIniData
    {
        
    }

    /// <summary>
    /// Represents an INI data parser for streams.
    /// </summary>
    public class StreamIniData
    {       
        public IniDataParser Parser { get; protected set; }

        public StreamIniData() : this (new IniDataParser())
        {
            
        }


        public StreamIniData(IniDataParser parser)
        {
            Parser = parser;
        }
        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamIniDataParser"/> class.
        /// </summary>
        /// <summary>
        /// <para>Reads data in INI format from a stream.</para> 
        /// </summary>
        /// <param name="reader">Reader stream.</param>
        /// <returns></returns>
        public IniData ReadData(StreamReader reader)
        {
     
            if (reader == null)
                throw new ArgumentNullException("reader");
            
            return Parser.Parse(reader.ReadToEnd());
        }

        /// <summary>
        /// Writes the ini data to a stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="iniData">An <see cref="IniData"/> instance .</param>
        public void WriteData(StreamWriter writer, IniData iniData)
        {

            SectionDataCollection sdc = iniData.Sections;

            writer.Write(iniData.ToString());

            return;
        }

        #endregion
    }
}
