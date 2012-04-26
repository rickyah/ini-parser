using IniParser;
using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue26Tests
    {
        [Test, Description("Test for Issue 26: http://code.google.com/p/ini-parser/issues/detail?id=26")]
        public void allow_duplicated_sections()
        {
            FileIniDataParser parser = new FileIniDataParser();

            IniData parsedData = parser.LoadFile("Issue11_example.ini");

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }
    }
}
