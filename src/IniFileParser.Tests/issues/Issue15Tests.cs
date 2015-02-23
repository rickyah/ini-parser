using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue15Tests
    {
        [Test, Description("Test for Issue 15: http://code.google.com/p/ini-parser/issues/detail?id=15")]
        public void allow_duplicated_sections_in_section()
        {
            string data = 
@"[123_1]
key1=value1
key2=value2
[123_2]
key3 = value3
[123_1]
key4=value4";

            var parser = new IniDataParser();

            parser.Configuration.AllowDuplicateKeys = true;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            var iniData = parser.Parse(data);

            Assert.That(iniData.Sections.ContainsSection("123_1"), Is.True);
            Assert.That(iniData.Sections.ContainsSection("123_2"), Is.True);
            Assert.That(iniData.Sections.GetSectionData("123_1").Keys, Has.Count.EqualTo(3));
            Assert.That(iniData["123_1"]["key4"], Is.EqualTo("value4"));

        }
    }
}
