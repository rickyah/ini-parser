using System.IO;
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
            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Issue18_example.ini");
            
            IniData parsedData = parser.ReadFile(filePath, Encoding.UTF8);

            Assert.That(parsedData.Sections.ContainsSection("Identität"));
            Assert.That(parsedData.Sections["Identität"]["key"], Is.EqualTo("value"));
        }

        [Test, Description("Test for Issue 26: http://code.google.com/p/ini-parser/issues/detail?id=26")]
        public void allow_duplicated_sections()
        {
            FileIniDataParser parser = new FileIniDataParser();
            
            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Issue11_example.ini");

            IniData parsedData = parser.ReadFile(filePath);

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }

        [Test, Description("Check on real files")]
        public void check_parses_real_test_files()
        {
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.ThrowExceptionsOnError = true;

            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "aircraft.cfg");
            var iniFileData = parser.ReadFile(filePath);

            parser.Parser.Scheme.CommentString = "//";
            var filePath2 = Path.Combine(TestContext.CurrentContext.TestDirectory, "aircraft2.cfg");
            iniFileData = parser.ReadFile(filePath2);
        }

        [Test, Description("Check unicode characters")]
        public void check_parse_unicode_chinese_characters()
        {
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.ThrowExceptionsOnError = true;

            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "unicode_chinese.ini");
            var iniFileData = parser.ReadFile(filePath);

            // If you want to write the file you must specify the encoding
            //parser.WriteFile("unicode_chinese_copy.ini", iniFileData, Encoding.UTF8);
        }
    }
}
