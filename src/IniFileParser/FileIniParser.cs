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
        ///     Implements loading a file from disk.
        /// </summary>
        /// <remarks>
        ///     Expects an ASCII encoded file by default.
        /// </remarks>
        /// <param name="fileName">
        ///     Name of the file.
        /// </param>
        public IniData LoadFile(string fileName)
        {
            return LoadFile(fileName, false);
        }

        /// <summary>
        ///     Implements loading a file from disk.
        /// </summary>
        /// <remarks>
        ///     Expects an ASCII encoded file by default.
        /// </remarks>
        /// <param name="fileName">
        ///     Name of the file.
        /// </param>
        /// <param name="relaxedIniRead">
        ///     True to try reading bad formed INI files
        /// </param>
        public IniData LoadFile(string fileName, bool relaxedIniRead)
        {
            return LoadFile(fileName, relaxedIniRead, System.Text.ASCIIEncoding);
        }
        
        /// <summary>
        ///     Implements loading a file from disk.
        /// </summary>
        /// <param name="fileName">
        ///     Name of the file.
        /// </param>
        /// <param name="relaxedIniRead">
        ///     True to try reading bad formed INI files
        /// </param>
        /// <param name="fileEncoding">
        ///     File's encoding.
        /// </param>
        public IniData LoadFile(string fileName, bool relaxedIniRead, Encoding fileEncoding)
        {
            if (fileName == string.Empty)
                throw new ArgumentException("Bad filename.");

            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, fileEncoding))
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
        ///     Saves INI data to a file.
        /// </summary>
        /// <remarks>
        ///     Creats an ASCII encoded file by default.
        /// </remarks>
        /// <param name="fileName">
        ///     Name of the file.
        /// </param>
        /// <param name="parsedData">
        ///     IniData to be saved as an INI file.
        /// </param>
        public void SaveFile(string fileName, IniData parsedData)
        {
            SaveFile(fileName, parsedData, System.Text.ASCIIEncoding);
        }
                             
        /// <summary>
        ///     Saves INI data to a file.
        /// </summary>
        /// <param name="fileName">
        ///     Name of the file.
        /// </param>
        /// <param name="parsedData">
        ///     IniData to be saved as an INI file.
        /// </param>
        /// <param name="fileEncoding">
        ///     Specifies the encoding used to create the file.
        /// </param>
        public void SaveFile(string fileName, IniData parsedData, Encoding fileEncoding)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Bad filename.");

            if (parsedData == null)
                throw new ArgumentNullException("parsedData");

            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sr = new StreamWriter(fs, fileEncoding))
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
