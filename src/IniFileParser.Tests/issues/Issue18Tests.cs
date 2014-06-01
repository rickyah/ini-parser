using System.Text;
using IniParser;
using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture, Description("Tests for Issue 18: http://code.google.com/p/ini-parser/issues/detail?id=18")]
    public class Issue18Tests
    {
        [Test]
        public void test_multiple_file_encodings()
        {
            var parser = new FileIniDataParser();

            // Encoding.Default is now the default value used in the ReadFile method, but is 
            // specified in this call for consistency with the issue report
            IniData parsedData = parser.ReadFile("./Issue18_example.ini", Encoding.UTF8);

            Assert.That(parsedData.Sections.ContainsSection("Identität"));
            Assert.That(parsedData.Sections["Identität"]["key"], Is.EqualTo("value"));
        }
    }
}
