using System;


using IniParser;
using System.IO;
using IniParser.Model;


namespace INIFileParser.Example
{
    public class MainProgram
    {
        public static void Main()
        {
            //Create an instance of a ini file parser
            FileIniDataParser fileIniData = new FileIniDataParser();

            if (File.Exists("NewTestIniFile.ini"))
                File.Delete("NewTestIniFile.ini");

			// This is a special ini file where we use the '#' character for comment lines
			// instead of ';' so we need to change the configuration of the parser:
			fileIniData.Parser.Configuration.CommentString = "#";
			
			
            //Parse the ini file
            IniData parsedData = fileIniData.ReadFile("TestIniFile.ini");

            //Write down the contents of the ini file to the console
            Console.WriteLine("---- Printing contents of the INI file ----\n");
            Console.WriteLine(parsedData);
            Console.WriteLine();

            //Get concrete data from the ini file
            Console.WriteLine("---- Printing setMaxErrors value from GeneralConfiguration section ----");
            Console.WriteLine("setMaxErrors = " + parsedData["GeneralConfiguration"]["setMaxErrors"]);
            Console.WriteLine();

            //Modify the INI contents and save
            Console.WriteLine();
            
			// Modify the loaded ini file
            IniData modifiedParsedData = ModifyINIData(parsedData);
            
            //Write down the contents of the modified ini file to the console
            Console.WriteLine("---- Printing contents of the new INI file ----");
            Console.WriteLine(modifiedParsedData);
			Console.WriteLine();
			
            //Save to a file
            Console.WriteLine("---- Saving the new ini file to the file NewTestIniFile.ini ----");
            Console.WriteLine();
            
            fileIniData.WriteFile("NewTestIniFile.ini", modifiedParsedData);
        }

        static IniData ModifyINIData(IniData modifiedParsedData)
        {
            modifiedParsedData["GeneralConfiguration"]["setMaxErrors"] = "10";
            modifiedParsedData.Sections.AddSection("newSection");
            modifiedParsedData.Sections.GetSectionData("newSection").LeadingComments
                .Add("This is a new comment for the section");
            modifiedParsedData.Sections.GetSectionData("newSection").Keys.AddKey("myNewKey", "value");
            modifiedParsedData.Sections.GetSectionData("newSection").Keys.GetKeyData("myNewKey").Comments
            .Add("new key comment");

            return modifiedParsedData;
        }
    }
}
