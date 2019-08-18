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
        readonly string strGoodINIFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"INIFileGOOD.ini");
        readonly string strBadINIFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory,  @"INIfileBAD.ini");
        readonly string strEmptyINIFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"INIFileEMPTY.ini");
        readonly string strBadSectionINIFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"INIfileBADSection.ini");
        readonly string strBadKeysINIFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"INIfileBADKeys.ini");

        FileIniDataParser iniParser = new FileIniDataParser();

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
        
        public void CheckParsingFailure()
        {
            Assert.Throws(typeof(ParsingException), () => iniParser.ReadFile(strBadINIFilePath));
        }

        [Test, Description("Checks bad formed INI file: Two sections with same name")]
        public void CheckCollideSectionNames()
        {
            iniParser.Parser.Configuration.SkipInvalidLines = false;
            Assert.Throws(typeof(ParsingException), () => iniParser.ReadFile(strBadSectionINIFilePath));
        }

        [Test, Description("Checks bad formed INI file: Two keys in the same section with same name")]
        public void CheckCollideKeysNames()
        {
            Assert.Throws(typeof(ParsingException), () => iniParser.ReadFile(strBadKeysINIFilePath));
        }
    }
}
