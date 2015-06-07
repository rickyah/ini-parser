using System.Text;
using IniParser;
using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Parser
{
    [TestFixture]
    public class FileIniDataParserTests
    {
        [Test,  Description("Tests for Issue 18: http://code.google.com/p/ini-parser/issues/detail?id=18")]
        public void test_multiple_file_encodings()
        {
            var parser = new FileIniDataParser();

            // Encoding.Default is now the default value used in the ReadFile method, but is 
            // specified in this call for consistency with the issue report
            IniData parsedData = parser.ReadFile("./Issue18_example.ini", Encoding.UTF8);

            Assert.That(parsedData.Sections.ContainsSection("Identität"));
            Assert.That(parsedData.Sections["Identität"]["key"], Is.EqualTo("value"));
        }

        [Test, Description("Test for Issue 26: http://code.google.com/p/ini-parser/issues/detail?id=26")]
        public void allow_duplicated_sections()
        {
            FileIniDataParser parser = new FileIniDataParser();

            IniData parsedData = parser.LoadFile("Issue11_example.ini");

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }

        [Test, Description("Check on real files")]
        public void check_parses_real_test_files()
        {
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.ThrowExceptionsOnError = true;

            var iniFileData = parser.ReadFile("aircraft.cfg");

            parser.Parser.Configuration.CommentString = "//";
            iniFileData = parser.ReadFile("aircraft2.cfg");
        }
    }
}
