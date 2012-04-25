using IniParser;
using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue11Tests
    {
        [Test, Description("Test for Issue 11: http://code.google.com/p/ini-parser/issues/detail?id=11")]
        public void allow_duplicated_sections()
        {
            FileIniDataParser parser = new FileIniDataParser();

            IniData parsedData = parser.LoadFile("Issue11_example.ini", true);

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }
    }
}
