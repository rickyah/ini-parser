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
        public IniData LoadFile(string fileName)
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = File.Open(fileName, FileMode.Open);
                sr = new StreamReader(fs);
                return ReadData(sr);
            }
            catch ( IOException ex )
            {
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
        public void SaveFile(string fileName, IniData parsedData)
        {
            if (fileName == string.Empty)
                throw new ArgumentException("Bad filename.");

            if (parsedData == null)
                throw new ArgumentNullException("Parsed data is null");

            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = File.Create(fileName);

                sw = new StreamWriter(fs);

                WriteData(sw, parsedData);

            }
            catch ( IOException ex)
            {
                throw new ParsingException(String.Format("Could not save to file {0}", fileName), ex);
            }
            finally
            {
                if ( sw != null ) sw.Close();
                if ( fs != null ) fs.Close();
            }

        }

        #endregion

        #region Non-public members

        IniData _data = null; 

        #endregion
    }
}
