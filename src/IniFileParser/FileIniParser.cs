using System;
using System.IO;
using System.Text;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Parser;

namespace IniParser
{
    /// <summary>
    ///     Represents an INI data parser for files.
    /// </summary>
    public class FileIniDataParser : StreamIniDataParser
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        public FileIniDataParser() {}

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="parser"></param>
        public FileIniDataParser(IniDataParser parser) : base(parser)
        {
            Parser = parser;
        }

        #region Deprecated methods

        [Obsolete("Please use ReadFile method instead of this one as is more semantically accurate")]
        public IniData LoadFile(string filePath)
        {
            return ReadFile(filePath);
        }

        [Obsolete("Please use ReadFile method instead of this one as is more semantically accurate")]
        public IniData LoadFile(string filePath, Encoding fileEncoding)
        {
            return ReadFile(filePath, fileEncoding);
        }
        #endregion

        /// <summary>
        ///     Implements reading ini data from a file.
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="Encoding.Default"/> codification for the file.
        /// </remarks>
        /// <param name="filePath">
        ///     Path to the file
        /// </param>
        public IniData ReadFile(string filePath)
        {
            return ReadFile(filePath, Encoding.ASCII);
        }

        /// <summary>
        ///     Implements reading ini data from a file.
        /// </summary>
        /// <param name="filePath">
        ///     Path to the file
        /// </param>
        /// <param name="fileEncoding">
        ///     File's encoding.
        /// </param>
        public IniData ReadFile(string filePath, Encoding fileEncoding)
        {
            if (filePath == string.Empty)
                throw new ArgumentException("Bad filename.");

            try
            {
                // (FileAccess.Read) we want to open the ini only for reading 
                // (FileShare.ReadWrite) any other process should still have access to the ini file 
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, fileEncoding))
                    {
                        return ReadData(sr);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ParsingException(String.Format("Could not parse file {0}", filePath), ex);
            }

        }

        /// <summary>
        ///     Saves INI data to a file.
        /// </summary>
        /// <remarks>
        ///     Creats an ASCII encoded file by default.
        /// </remarks>
        /// <param name="filePath">
        ///     Path to the file.
        /// </param>
        /// <param name="parsedData">
        ///     IniData to be saved as an INI file.
        /// </param>
        [Obsolete("Please use WriteFile method instead of this one as is more semantically accurate")]
        public void SaveFile(string filePath, IniData parsedData)
        {
            WriteFile(filePath, parsedData, Encoding.UTF8);
        }
                             
        /// <summary>
        ///     Writes INI data to a text file.
        /// </summary>
        /// <param name="filePath">
        ///     Path to the file.
        /// </param>
        /// <param name="parsedData">
        ///     IniData to be saved as an INI file.
        /// </param>
        /// <param name="fileEncoding">
        ///     Specifies the encoding used to create the file.
        /// </param>
        public void WriteFile(string filePath, IniData parsedData, Encoding fileEncoding = null)
        {
            // The default value can't be assigned as a default parameter value because it is not
            // a constant expression.
			if (fileEncoding == null)
				fileEncoding = Encoding.UTF8;

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Bad filename.");

            if (parsedData == null)
                throw new ArgumentNullException("parsedData");

            using (FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sr = new StreamWriter(fs, fileEncoding))
                {
                    WriteData(sr, parsedData);
                }
            }
        }
    }
}
