using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue17Tests
    {
        [Test, Description("Test for Issue 17: http://code.google.com/p/ini-parser/issues/detail?id=17")]
        public void check_can_parse_special_characters_in_section_names()
        {
            string data =
@"[{E3729302-74D1-11D3-B43A-00AA00CAD128}]
key = value";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Count, Is.EqualTo(1));
            Assert.That(iniData.Sections.ContainsSection("{E3729302-74D1-11D3-B43A-00AA00CAD128}"), Is.True);
            Assert.That(iniData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"].ContainsKey("key"), Is.True);
            Assert.That(iniData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"]["key"], Is.EqualTo("value"));
        }
    }
}
