using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IniParser
{
    /// <summary>
    /// Represents an INI data parser for files.
    /// </summary>
    public class FileIniDataParser : StreamIniDataParser
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIniDataParser"/> class.
        /// </summary>
        public FileIniDataParser()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implements loading a file from disk.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadFile(string fileName)
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = File.Open(fileName, FileMode.Open);
                sr = new StreamReader(fs);
                ParsedData = ReadData(sr);
            }
            catch ( IOException ex )
            {
                ParsedData = null;
                throw new ParsingException(String.Format("Could not parse file {0}", fileName), ex);
            }
            finally
            {
               if ( sr != null ) sr.Close();
               if ( fs != null ) fs.Close();
            }
        }

        /// <summary>
        /// Implements saving a file from disk.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveFile(string fileName)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = File.Create(fileName);

                sw = new StreamWriter(fs);

                WriteData(sw, ParsedData);

            }
            catch ( IOException ex)
            {
                ParsedData = null;
                throw new ParsingException(String.Format("Could not save to file {0}", fileName), ex);
            }
            finally
            {
                if ( sw != null ) sw.Close();
                if ( fs != null ) fs.Close();
            }

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the parsed data.
        /// </summary>
        /// <value>
        /// An object with the parsed data or <c>null</c> 
        /// if either no data parsing was done, or was terminated with errors.</value>
        public IniData ParsedData
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        #region Non-public members

        IniData _data = null; 

        #endregion
    }
}
