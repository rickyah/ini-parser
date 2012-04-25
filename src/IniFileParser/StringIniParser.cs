using System;
using IniParser.Model;

namespace IniParser
{
    [Obsolete("Use StringIniData class instead")]
    public class StringIniParser : StringIniData
    {
    }

    /// <summary>
    ///     Represents an INI data parser for strings.
    /// </summary>
    public class StringIniData : StreamIniData
    {
        /// <summary>
        /// Parses a string containing data formatted as an INI file.
        /// </summary>
        /// <param name="dataStr">The string containing the data.</param>
        /// <returns>
        /// A new <see cref="IniData"/> instance with the data parsed from the string.
        /// </returns>
        public IniData ParseString(string dataStr)
        {
            return Parser.Parse(dataStr);
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
            iniData.Configuration = Parser.Configuration;

            return iniData.ToString();
        }
    }
}
