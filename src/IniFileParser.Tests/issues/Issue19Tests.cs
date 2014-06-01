using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue19Tests
    {
        [Test, Description("Test for Issue 197: http://code.google.com/p/ini-parser/issues/detail?id=19")]
        public void allow_whitespace_in_section_names()
        {
            string data =
@"[Web Colaboration]
key = value";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Count, Is.EqualTo(1));
            Assert.That(iniData.Sections.ContainsSection("Web Colaboration"), Is.True);
            Assert.That(iniData.Sections["Web Colaboration"].ContainsKey("key"), Is.True);
            Assert.That(iniData.Sections["Web Colaboration"]["key"], Is.EqualTo("value"));
        }
    }
}
