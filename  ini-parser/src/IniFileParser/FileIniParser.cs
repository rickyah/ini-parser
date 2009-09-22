using System;
using System.IO;

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
                        return ReadData(sr, relaxedIniRead);
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
        /// <param name="parsedData">IniData to be saved as an INI file.</param>
        public void SaveFile(string fileName, IniData parsedData)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Bad filename.");

            if (parsedData == null)
                throw new ArgumentNullException("parsedData");

            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sr = new StreamWriter(fs))
                    {
                        WriteData(sr, parsedData);
                    }
                }

            }
            catch (IOException ex)
            {
                throw new ParsingException(String.Format("Could not save data to file {0}", fileName), ex);
            }

        }

        #endregion
    }
}
