using IniParser;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture, Description("Tests for Issue 28: http://code.google.com/p/ini-parser/issues/detail?id=28")]
    public class Issue28Tests
    {
        [Test]
        public void allow_tilde_in_sections()
        {
            string data =
@"[section~subsection]
key=value";
            IniDataParser parser = new IniDataParser();

            IniData parsedData = parser.Parse(data);

            Assert.That(parsedData.Sections.ContainsSection("section~subsection"));
            Assert.That(parsedData.Sections["section~subsection"]["key"], Is.EqualTo("value"));
        }
    }
}
