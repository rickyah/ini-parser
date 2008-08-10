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
            iniParser.LoadFile(strEmptyINIFilePath);
            
            Assert.That(iniParser.ParsedData, Is.Not.Null);
        }

        [Test, Description("Checks correct parsing of a well formed INI file")]
        public void CheckParseGoodFileSuccess()
        {
            iniParser.LoadFile(strGoodINIFilePath);

            Assert.That(iniParser.ParsedData, Is.Not.Null);
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

            iniParser.LoadFile(strGoodINIFilePath);
            iniParser.SaveFile(fileString);

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
    }
}
