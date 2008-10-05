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
        public IniData ParseString(string dataStr)
        {
            StreamReader s =
                new StreamReader(new MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(dataStr), false));
            return this.ReadData(s);
        }

        public string WriteString(IniData iniData)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter( ms );
            this.WriteData(sw, iniData);
            sw.Flush();

            string result = System.Text.Encoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            sw.Close();

            return result;
        }
    }
}
