using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Parser
{
    [TestFixture]
    public class ParserDefaultConfigurationTests
    {

        string iniFileStr =
@"
global key = global value
;comment for section1
[section1]
;comment for key1
key 1 =      value 1
key;2 = va:lu;e.5
[ section 2]
;comment for myKey1
mykey1 = value1
;comment for section2
";


        [Test]
        public void check_data_correctly_parsed()
        {
            var parser = new IniDataParser();
            IniData data = parser.Parse(iniFileStr);

            Assert.That(data, Is.Not.Null);

            // Check global section

            Assert.That(data.Global["global key"], Is.EqualTo("global value"));

            // global section is not computed when counting the number of sections in the
            // ini
            Assert.That(data.Sections.Count, Is.EqualTo(2),"Expected two (2) sections");
            var section1 = data.Sections.GetSectionData("section1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.SectionName, Is.EqualTo("section1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));

            Assert.That(section1.Keys, Is.Not.Null);
            Assert.That(section1.Keys.Count, Is.EqualTo(2));
            Assert.That(section1.Keys.GetKeyData("key 1"), Is.Not.Null);

            // Check keys / values with spaces. Leading & trailing whitespace ignored
            Assert.That(section1.Keys["key 1"], Is.EqualTo("value 1"));


            Assert.That(section1.Keys.GetKeyData("key;2"), Is.Not.Null);
            // Check special characters as part of the key/value name
            Assert.That(section1.Keys["key;2"], Is.EqualTo("va:lu;e.5"));


            //
            var section2 = data.Sections.GetSectionData("section2");
            // Bad section name
            Assert.That(section2, Is.Null);

            // Beware: leading and trailing whitespaces are ignored by default!
            section2 = data.Sections.GetSectionData("section 2");
            Assert.That(section2, Is.Not.Null);
            Assert.That(section2.SectionName, Is.EqualTo("section 2"));
            Assert.That(section2.Comments, Has.Count.EqualTo(1));

            // Check comments at the end of the section are parsed and assigned to the section
            Assert.That(section2.Keys.GetKeyData("mykey1").Comments, Is.Not.Empty);
            Assert.That(section2.Keys.GetKeyData("mykey1").Comments.Count, Is.EqualTo(1));
            Assert.That(section2.Keys.GetKeyData("mykey1").Comments[0], Is.EqualTo("comment for myKey1"));
        }

        [Test, Ignore("no writing tests")]
        public void check_ini_writing()
        {
            IniData data = new IniDataParser().Parse(iniFileStr);

            // ini file string with not-needed whitespace trimmed
            var dataAsString = data.ToString();

            // Generates a valid data file
            IniData data2 = new IniDataParser().Parse(dataAsString);


            // check all strings are equal
            Assert.That(dataAsString, Is.EqualTo(data2.ToString()));
        }
    }
}
