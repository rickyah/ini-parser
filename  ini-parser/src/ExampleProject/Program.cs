using System;
using System.Collections.Generic;
using System.Text;

using IniParser;
using System.IO;

namespace ExampleProject
{
    public class MainProgram
    {
        public static void Main()
        {
            //Create an instance of a ini file parser
            IniParser.FileIniDataParser parser = new FileIniDataParser();

            if (File.Exists("NewTestIniFile.ini"))
                File.Delete("NewTestIniFile.ini");

            //Parse the ini file
            IniData parsedData = parser.LoadFile("TestIniFile.ini");

            //Write down the contents of the ini file to the console
            Console.WriteLine("---- Printing contents of the INI file ----\n");
            Console.WriteLine(ElaboratedParsing(parsedData, parser));

            //Get concrete data from the ini file
            Console.WriteLine("---- Printing contents concrete data from the INI file ----");
            Console.WriteLine("setMaxErrors = " + parsedData["GeneralConfiguration"]["setMaxErrors"]);
            Console.WriteLine();

            //Modify the INI contents and save
            Console.WriteLine();
            //Write down the contents of the modified ini file to the console
            Console.WriteLine("---- Printing contents of the new INI file ----\n");
            IniData modifiedParsedData = ModifyINIData(parsedData);
            Console.WriteLine(ElaboratedParsing(modifiedParsedData, parser));

            //Save to a file
            parser.SaveFile("NewTestIniFile.ini", modifiedParsedData);
        }

        private static string ElaboratedParsing(IniData parsedData, FileIniDataParser parser)
        {
            StringBuilder sb = new StringBuilder();

            //Process data: print contents of the file into screen
            foreach (SectionData sectionData in parsedData.Sections)
            {
                //Print comments for current section
                foreach (string sectionComment in sectionData.Comments)
                    sb.AppendLine(parser.CommentDelimiter + sectionComment);
                
                //Print section's name
                sb.AppendLine(
                    parser.SectionDelimiters[0] + sectionData.SectionName + parser.SectionDelimiters[1]);
                
                sb.AppendLine();

                //Print section's key-value pairs with it's comments
                foreach (KeyData keyData in sectionData.Keys)
                {
                    //Print comments for current key
                    foreach (string keyComment in keyData.Comments)
                        sb.AppendLine(parser.CommentDelimiter + keyComment);

                    //Print key and value
                    sb.AppendLine(keyData.KeyName + " " + parser.KeyValueDelimiter + " " + keyData.Value);

                    sb.AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static IniData ModifyINIData(IniData modifiedParsedData)
        {
            modifiedParsedData["GeneralConfiguration"]["setMaxErrors"] = "10";
            modifiedParsedData.Sections.AddSection("newSection");
            modifiedParsedData.Sections.GetSectionData("newSection").Comments
                .Add("This is a new comment for the section");
            modifiedParsedData.Sections.GetSectionData("newSection").Keys.AddKey("myNewKey", "value");
            modifiedParsedData.Sections.GetSectionData("newSection").Keys.GetKeyData("myNewKey").Comments
            .Add("new key comment");

            return modifiedParsedData; modifiedParsedData;
        }
    }
}
