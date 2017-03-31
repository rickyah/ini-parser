using System.IO;
using IniParser.Exceptions;
using IniParser.Model;
using NUnit.Framework;
using IniParser;

namespace IniFileParser.Tests.Unit
{
    [TestFixture, Category("File loading/saving tests")]
    public class IniParserFileTest
    {

        public readonly string strGoodINIFilePath = @"../../INIFileGOOD.ini";
        public readonly string strBadINIFilePath = @"../../INIfileBAD.ini";
        public readonly string strEmptyINIFilePath = @"../../INIFileEMPTY.ini";
        public readonly string strBadSectionINIFilePath = @"../../INIFileBADSection.ini";
        public readonly string strBadKeysINIFilePath = @"../../INIfileBADKeys.ini";

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
            IniData parsedData = iniParser.ReadFile(strEmptyINIFilePath);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks correct parsing of a well formed INI file")]
        public void CheckParseGoodFileSuccess()
        {
            IniData parsedData = iniParser.ReadFile(strGoodINIFilePath);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks error when parsing a bad formed INI file")]
        [ExpectedException(typeof(ParsingException))]
        public void CheckParsingFailure()
        {
            iniParser.ReadFile(strBadINIFilePath);
        }

        [Test, Description("Checks correct saving of a file")]
        public void CheckCorrectSave()
        {
            string fileString = strGoodINIFilePath + "_test.ini";

            IniData parsedData = iniParser.ReadFile(strGoodINIFilePath);
            iniParser.WriteFile(fileString, parsedData);

            Assert.That(File.Exists(fileString));
        }

        [Test, Description("Checks bad formed INI file: Two sections with same name")]
        [ExpectedException(typeof(ParsingException))]
        public void CheckCollideSectionNames()
        {
            iniParser.ReadFile(strBadSectionINIFilePath);
        }

        [Test, Description("Checks bad formed INI file: Two keys in the same section with same name")]
        [ExpectedException(typeof(ParsingException))]
        public void CheckCollideKeysNames()
        {
            iniParser.ReadFile(strBadKeysINIFilePath);
        }
    }
}
