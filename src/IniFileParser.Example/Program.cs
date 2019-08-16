using System;

using System.IO;
using IniParser.Model;
using IniParser.Parser;

namespace IniFileParser.Example
{
    public class MainProgram
    {
        public static void Main()
        {   
            var testIniFileName = "TestIniFile.ini";
            var newTestIniFileName = "NewTestIniFile.ini";

            //Create an instance of a ini file parser
            var parser = new IniDataParser();

            if (File.Exists(newTestIniFileName))
                File.Delete(newTestIniFileName);

            // This is a special ini file where we use the '#' character for comment lines
            // instead of ';' so we need to change the configuration of the parser:
            parser.Configuration.CommentString = "#";

            // Here we'll be storing the contents of the ini file we are about to read:
            IniData parsedData;

            // Read and parse the ini file
            using (FileStream fs = File.Open(testIniFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
                {
                    parsedData = parser.Parse(sr.ReadToEnd());
                }
            }

            // Write down the contents of the ini file to the console
            Console.WriteLine("---- Printing contents of the INI file ----\n");
            Console.WriteLine(parsedData);
            Console.WriteLine();

            // Get concrete data from the ini file
            Console.WriteLine("---- Printing setMaxErrors value from GeneralConfiguration section ----");
            Console.WriteLine("setMaxErrors = " + parsedData["GeneralConfiguration"]["setMaxErrors"]);
            Console.WriteLine();

            // Modify the INI contents and save
            Console.WriteLine();

            // Modify the loaded ini file
            parsedData["GeneralConfiguration"]["setMaxErrors"] = "10";
            parsedData.Sections.AddSection("newSection");
            parsedData.Sections.GetSectionData("newSection").Comments
                .Add("This is a new comment for the section");
            parsedData.Sections.GetSectionData("newSection").Keys.AddKey("myNewKey", "value");
            parsedData.Sections.GetSectionData("newSection").Keys.GetKeyData("myNewKey").Comments
            .Add("new key comment");

            // Write down the contents of the modified ini file to the console
            Console.WriteLine("---- Printing contents of the new INI file ----");
            Console.WriteLine(parsedData);
			Console.WriteLine();
        }
    }
}
