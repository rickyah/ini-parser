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
        #region Public Methods

        /// <summary>
        /// Implements loading a file from disk.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public IniData LoadFile(string fileName)
        {
            return LoadFile(fileName, false);
        }

        /// <summary>
        /// Implements loading a file from disk.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="relaxedIniRead">True to try reading bad formed INI files</param>
        public IniData LoadFile(string fileName, bool relaxedIniRead)
        {
            if (fileName == string.Empty)
                throw new ArgumentException("Bad filename.");

            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        return this.ReadData(sr, relaxedIniRead);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ParsingException(String.Format("Could not parse file {0}", fileName), ex);
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

            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sr = new StreamWriter(fs))
                    {
                        this.WriteData(sr, parsedData);
                    }
                }

            }
            catch (IOException ex)
            {
                throw new ParsingException(String.Format("Could not save to file {0}", fileName), ex);
            }

        }

        #endregion

        #region Non-public members

        IniData _data = null;

        #endregion
    }
}
