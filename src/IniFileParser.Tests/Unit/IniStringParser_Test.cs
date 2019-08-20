using IniParser;
using NUnit.Framework;

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
            var section1 = data.Sections.GetSectionData("section1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.SectionName, Is.EqualTo("section1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));

            Assert.That(section1.Properties, Is.Not.Null);
            Assert.That(section1.Properties.Count, Is.EqualTo(2));
            Assert.That(section1.Properties.GetKeyData("key1"), Is.Not.Null);
            Assert.That(section1.Properties["key1"], Is.EqualTo("value1"));
            Assert.That(section1.Properties.GetKeyData("key2"), Is.Not.Null);
            Assert.That(section1.Properties["key2"], Is.EqualTo("value5"));
        }

        [Test]
        public void WritingTotring_Test()
        {
            IniData data = new IniData();

            data.Sections.AddSection("newSection1");
            data.Sections["newSection1"].AddKeyAndValue("newKey1", "newValue1");
            data.Sections["newSection1"].AddKeyAndValue("newKey2", "newValue5");

            string result = data.ToString();

            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Length, Is.Not.EqualTo(0));
        }
    }
}
