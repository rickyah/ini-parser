using System;
using IniParser;

namespace IniParser.Example
{
    public class MainProgram
    {
        public static void Main()
        {   
            var testIniFile = @"#This section provides the general configuration of the application
[GeneralConfiguration] 

#Update rate in msecs
setUpdate = 100

#Maximun errors before quit
setMaxErrors = 2

#Users allowed to access the system
#format: user = pass
[Users]
ricky = rickypass
patty = pattypass ";

            //Create an instance of a ini file parser
            var parser = new IniDataParser();

            // This is a special ini file where we use the '#' character for comment lines
            // instead of ';' so we need to change the configuration of the parser:
            parser.Scheme.CommentString = "#";

            // Here we'll be storing the contents of the ini file we are about to read:
            IniData parsedData = parser.Parse(testIniFile);

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
            parsedData.Sections.Add("newSection");
            parsedData.Sections.FindByName("newSection").Comments
                .Add("This is a new comment for the section");
            parsedData.Sections.FindByName("newSection").Properties.Add("myNewKey", "value");
            parsedData.Sections.FindByName("newSection").Properties.FindByKey("myNewKey").Comments
            .Add("new key comment");

            // Write down the contents of the modified ini file to the console
            Console.WriteLine("---- Printing contents of the new INI file ----");
            Console.WriteLine(parsedData);
			Console.WriteLine();
        }
    }
}
