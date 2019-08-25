using IniParser;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Configuration
{
    [TestFixture]
    public class SchemeOptionTests
    {
        [Test]
        public void check_comment_string()
        {
            IniData iniData;
            var parser = new IniDataParser();

            parser.Scheme.CommentString = "";
            Assert.That(parser.Scheme.CommentString, Is.EqualTo(";"));
            parser.Scheme.CommentString = null;
            Assert.That(parser.Scheme.CommentString, Is.EqualTo(";"));

            var ini1 = @"; comment
key1=1";
            iniData = parser.Parse(ini1);
            Assert.That(iniData.Global.FindByKey("key1").Comments, Has.Count.EqualTo(1));
            Assert.That(iniData.Global.FindByKey("key1").Comments[0], Is.EqualTo("comment"));

            parser.Scheme.CommentString = "#";
            var ini2 = @"# comment
key1=1";

            iniData = parser.Parse(ini2);
            Assert.That(iniData.Global.FindByKey("key1").Comments, Has.Count.EqualTo(1));
            Assert.That(iniData.Global.FindByKey("key1").Comments[0], Is.EqualTo("comment"));

            parser.Scheme.CommentString = "##";

            var ini3 = @"##a comment
#key1=1";

            iniData = parser.Parse(ini3);
            Assert.That(iniData.Global.Contains("#key1"), Is.True);

            Assert.That(iniData.Global.FindByKey("#key1").Comments, Has.Count.EqualTo(1));
            Assert.That(iniData.Global.FindByKey("#key1").Comments[0], Is.EqualTo("a comment"));
        }

        [Test]
        public void check_section_string()
        {
            IniData iniData;
            var parser = new IniDataParser();

            parser.Scheme.SectionStartString = "";
            parser.Scheme.SectionEndString = "";
            Assert.That(parser.Scheme.SectionStartString, Is.EqualTo("["));
            Assert.That(parser.Scheme.SectionEndString, Is.EqualTo("]"));
            parser.Scheme.SectionStartString = null;
            parser.Scheme.SectionEndString = null;
            Assert.That(parser.Scheme.SectionStartString, Is.EqualTo("["));
            Assert.That(parser.Scheme.SectionEndString, Is.EqualTo("]"));

            var ini1 = @"[section]";
            iniData = parser.Parse(ini1);

            Assert.That(iniData.Sections.Contains("section"), Is.True);

            var ini2 = @"<section>";
            parser.Scheme.SectionStartString = "<";
            parser.Scheme.SectionEndString = ">";
            iniData = parser.Parse(ini2);

            Assert.That(iniData.Sections.Contains("section"), Is.True);

            var ini3 = @"<section]";
            parser.Scheme.SectionStartString = "<";
            parser.Scheme.SectionEndString = "]";
            iniData = parser.Parse(ini3);
            Assert.That(iniData.Sections.Contains("section"), Is.True);

            var ini4 = @"<<section>>
<key> = <value>";

            parser.Scheme.SectionStartString = "<<";
            parser.Scheme.SectionEndString = ">>";
            iniData = parser.Parse(ini4);
            Assert.That(iniData.Sections.Contains("section"), Is.True);
            Assert.That(iniData["section"].Contains("<key>"), Is.True);
        }

        [Test]
        public void check_property_assignment_string()
        {
            IniData iniData;
            var parser = new IniDataParser();
            parser.Configuration.AllowKeysWithoutSection = true;

            parser.Scheme.PropertyAssigmentString = "";
            Assert.That(parser.Scheme.PropertyAssigmentString, Is.EqualTo("="));
            parser.Scheme.PropertyAssigmentString = null;
            Assert.That(parser.Scheme.PropertyAssigmentString, Is.EqualTo("="));

            var ini1 = @"key1 = 1";
            iniData = parser.Parse(ini1);

            Assert.That(iniData.Global["key1"], Is.EqualTo("1"));

            var ini2 = @"key1 <== 1";
            parser.Scheme.PropertyAssigmentString = "<==";
            iniData = parser.Parse(ini2);
        }
    }
}
