using IniParser;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture, Description("Tests for Issue 27: http://code.google.com/p/ini-parser/issues/detail?id=27")]
    public class Issue27Tests
    {
        [Test]
        public void allow_backslashes_in_sections()
        {
            string data =
@"[section\subsection]
key=value";
            IniDataParser parser = new IniDataParser();

            IniData parsedData = parser.Parse(data);

            Assert.That(parsedData.Sections.ContainsSection("section\\subsection"));
            Assert.That(parsedData.Sections["section\\subsection"]["key"], Is.EqualTo("value"));
        }
    }
}
