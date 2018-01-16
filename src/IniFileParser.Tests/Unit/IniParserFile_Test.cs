using System.IO;
using IniParser.Exceptions;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit
{
    [TestFixture, Category("File loading/saving tests")]
    public class IniParserFileTest
    {

        public readonly string strGoodINIFilePath = "INIfileGOOD.ini";
        public readonly string strBadINIFilePath = "INIfileBAD.ini";
        public readonly string strEmptyINIFilePath = "INIfileEMPTY.ini";
        public readonly string strBadSectionINIFilePath = "INIfileBADSection.ini";
        public readonly string strBadKeysINIFilePath = "INIfileBADKeys.ini";

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
            Assert.That(File.Exists(this.strGoodINIFilePath), "Correct INI file {0} does not exists", this.strGoodINIFilePath);
            Assert.That(File.Exists(this.strBadINIFilePath), "Invalid INI file {0} does not exists", this.strBadINIFilePath);
            Assert.That(File.Exists(this.strEmptyINIFilePath), "Empty INI file {0} does not exists", this.strEmptyINIFilePath);
        }

        [Test, Description("Checks correct parsing of an empty INI file")]
        public void CheckParseEmptyFileSuccess()
        {
            IniData parsedData = this.iniParser.ReadFile(this.strEmptyINIFilePath);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks correct parsing of a well formed INI file")]
        public void CheckParseGoodFileSuccess()
        {
            IniData parsedData = this.iniParser.ReadFile(this.strGoodINIFilePath);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks error when parsing a bad formed INI file")]
        public void CheckParsingFailure()
        {
            Assert.Throws<ParsingException>(() => this.iniParser.ReadFile(this.strBadINIFilePath));
        }

        [Test, Description("Checks correct saving of a file")]
        public void CheckCorrectSave()
        {
            string fileString = this.strGoodINIFilePath + "_test.ini";

            IniData parsedData = this.iniParser.ReadFile(this.strGoodINIFilePath);
            this.iniParser.WriteFile(fileString, parsedData);

            Assert.That(File.Exists(fileString));
        }

        [Test, Description("Checks bad formed INI file: Two sections with same name")]
        public void CheckCollideSectionNames()
        {
            Assert.Throws<ParsingException>(() => this.iniParser.ReadFile(this.strBadSectionINIFilePath));
        }

        [Test, Description("Checks bad formed INI file: Two keys in the same section with same name")]
        public void CheckCollideKeysNames()
        {
            Assert.Throws<ParsingException>(() => this.iniParser.ReadFile(this.strBadKeysINIFilePath));
        }
    }
}
