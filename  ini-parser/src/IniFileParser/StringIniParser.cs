using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace IniParser
{
    /// <summary>
    /// Represents an INI data parser for strings.
    /// </summary>
    public class StringIniParser : StreamIniDataParser
    {

        /// <summary>
        /// Parses a string containing data formatted as an INI file.
        /// </summary>
        /// <param name="dataStr">The string containing the data.</param>
        /// <param name="relaxedIniRead">
        ///     True to try reading an invalid INI file.
        /// </param>
        /// <returns>
        /// A new <see cref="IniData"/> instance with the data parsed from the string.
        /// </returns>
        public IniData ParseString(string dataStr)
        {
            return ParseString(dataStr, false);
        }

        /// <summary>
        /// Parses a string containing data formatted as an INI file.
        /// </summary>
        /// <param name="dataStr">The string containing the data.</param>
        /// <param name="relaxedIniRead">
        ///     True to try reading an invalid INI file.
        /// </param>
        /// <returns>
        /// A new <see cref="IniData"/> instance with the data parsed from the string.
        /// </returns>
        public IniData ParseString(string dataStr, bool relaxedIniRead)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(dataStr), false))
            {
                using (StreamReader s = new StreamReader(ms))
                {
                    return this.ReadData(s, relaxedIniRead);
                }
            }
        }

        /// <summary>
        /// Creates a string from the INI data.
        /// </summary>
        /// <param name="iniData">An <see cref="IniData"/> instance.</param>
        /// <returns>
        /// A formatted string with the contents of the
        /// <see cref="IniData"/> instance object.
        /// </returns>
        public string WriteString(IniData iniData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    this.WriteData(sw, iniData);
                    sw.Flush();

                    string result = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                    ms.Close();
                    sw.Close();

                    return result;
                }
            }
        }
    }
}
