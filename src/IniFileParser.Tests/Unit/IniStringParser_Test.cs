using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using IniParser;

namespace IniFileParser.Tests.Unit
{
    [TestFixture, Category("String parsing/writing tests")]
    public class IniStringParser_Test
    {

        private string iniFileStr = 
@";comment for section1
[section1]
;comment for key1
key1 = value1
key2 = value5
[section2]
;comment for myKey1 
mykey1 = value1 
";
        [Test]
        public void parse_ini_string_with_default_configuration()
        {
            var parser = new IniDataParser();
            IniData data = parser.Parse(iniFileStr);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.Count, Is.EqualTo(2));
            var section1= data.Sections.GetSectionData("section1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.SectionName, Is.EqualTo("section1"));
            Assert.That(section1.LeadingComments, Is.Not.Empty);
            Assert.That(section1.LeadingComments.Count, Is.EqualTo(1));

            Assert.That(section1.Keys, Is.Not.Null);
            Assert.That(section1.Keys.Count, Is.EqualTo(2));
            Assert.That(section1.Keys.GetKeyData("key1"), Is.Not.Null);
            Assert.That(section1.Keys["key1"], Is.EqualTo("value1"));
            Assert.That(section1.Keys.GetKeyData("key2"), Is.Not.Null);
            Assert.That(section1.Keys["key2"], Is.EqualTo("value5"));
        }

        [Test]
        public void WritingTotring_Test()
        {
            StringIniParser parser = new StringIniParser();
            IniData data = new IniData();

            data.Sections.AddSection("newSection1");
            data.Sections["newSection1"].AddKey("newKey1", "newValue1");
            data.Sections["newSection1"].AddKey("newKey2", "newValue5");

            string result = parser.WriteString(data);

            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Length, Is.Not.EqualTo(0));

        }

    }
}