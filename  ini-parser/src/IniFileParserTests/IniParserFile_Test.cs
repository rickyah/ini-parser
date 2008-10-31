using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

using IniParser;

namespace IniParserTestNamespace
{
    [TestFixture, Category("File loading/saving tests")]
    public class IniParserFileTest
    {

        public readonly string strGoodINIFilePath = @"../../INIFileGOOD.ini";
        public readonly string strBadINIFilePath = @"../../INIFileBAD.ini";
        public readonly string strEmptyINIFilePath = @"../../INIFileEmpty.ini";
        public readonly string strBadSectionINIFilePath = @"../../INIFileBADSection.ini";
        public readonly string strBadKeysINIFilePath = @"../../INIFileBADKeys.ini";

        public FileIniDataParser iniParser = new FileIniDataParser();

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test, Description("Checks existence of test INI files")]
        public void CheckTestFilesExists()
        {
            Assert.That(File.Exists(strGoodINIFilePath), "Correct INI file {0} does not exists", strGoodINIFilePath);
            Assert.That(File.Exists(strBadINIFilePath), "Invalid INI file {0} does not exists", strBadINIFilePath);
            Assert.That(File.Exists(strEmptyINIFilePath), "Empty INI file {0} does not exists", strEmptyINIFilePath);
        }

        [Test, Description("Checks correct parsing of an empty INI file")]
        public void CheckParseEmptyFileSuccess()
        {
            IniData parsedData = iniParser.LoadFile(strEmptyINIFilePath);
            
            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks correct parsing of a well formed INI file")]
        public void CheckParseGoodFileSuccess()
        {
            IniData parsedData = iniParser.LoadFile(strGoodINIFilePath);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks error when parsing a bad formed INI file")]
        [ExpectedException(typeof(ParsingException))]
        public void CheckParsingFailure()
        {
            iniParser.LoadFile(strBadINIFilePath);
        }

        [Test, Description("Checks correct saving of a file")]
        public void CheckCorrectSave()
        {
            string fileString = strGoodINIFilePath + "_test.ini";

            IniData parsedData = iniParser.LoadFile(strGoodINIFilePath);
            iniParser.SaveFile(fileString, parsedData);

            Assert.That(File.Exists(fileString));
        }

        [Test, Description("Checks bad formed INI file: Two sections with same name")]
        [ExpectedException(typeof(ParsingException))]
        public void CheckCollideSectionNames()
        {
            iniParser.LoadFile(strBadSectionINIFilePath);
        }

        [Test, Description("Checks bad formed INI file: Two keys in the same section with same name")]
        [ExpectedException(typeof(ParsingException))]
        public void CheckCollideKeysNames()
        {
            iniParser.LoadFile(strBadKeysINIFilePath);
        }

        [Test, Description("Test for Issue 2: http://code.google.com/p/ini-parser/issues/detail?id=2")]
        public void Issue2Test()
        {
            string test = "[ExampleSection]\nkey = value;value\n";

            StringIniParser strParser = new StringIniParser();

            IniData data = strParser.ParseString(test);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"], Is.Not.Null);
            Assert.That(data.Sections["ExampleSection"].Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"]["key"], Is.EqualTo("value"));


            FileIniDataParser fParser = new FileIniDataParser();
            data = fParser.LoadFile(@"../../INIFileBAD_Issue2.ini");

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"], Is.Not.Null);
            Assert.That(data.Sections["ExampleSection"].Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"]["key"], Is.EqualTo("value"));
        }
    }
}
