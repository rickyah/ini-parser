using System;


using IniParser;
using System.IO;
using IniParser.Model;


namespace ExampleProject
{
    public class MainProgram
    {
        public static void Main()
        {
            //Create an instance of a ini file parser
            FileIniDataParser fileIniData = new FileIniDataParser();

            if (File.Exists("NewTestIniFile.ini"))
                File.Delete("NewTestIniFile.ini");

            //Parse the ini file
            IniData parsedData = fileIniData.LoadFile("TestIniFile.ini");

            //Write down the contents of the ini file to the console
            Console.WriteLine("---- Printing contents of the INI file ----\n");
            Console.WriteLine(parsedData.ToString());

            //Get concrete data from the ini file
            Console.WriteLine("---- Printing contents concrete data from the INI file ----");
            Console.WriteLine("setMaxErrors = " + parsedData["GeneralConfiguration"]["setMaxErrors"]);
            Console.WriteLine();

            //Modify the INI contents and save
            Console.WriteLine();
            //Write down the contents of the modified ini file to the console
            Console.WriteLine("---- Printing contents of the new INI file ----\n");
            IniData modifiedParsedData = ModifyINIData(parsedData);
            Console.WriteLine(modifiedParsedData.ToString());

            //Save to a file
            fileIniData.WriteFile("NewTestIniFile.ini", modifiedParsedData);
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

            return modifiedParsedData;
        }
    }
}
