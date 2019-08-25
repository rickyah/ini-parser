using IniParser;
using IniParser.Configuration;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        private IniDataParser _parser;

        string iniFileStr =
@"
cyberdreams = i have no section, and i must scream

#comment for stage1
<stage1>
name = Green Hill Zone
this_is_not_a_comment = ;no comment

<stage2>
# comment name
name = Marble Zone
";

        string iniFileStrNotSoGood =
@"
cyberdreams = i have no section, and i must scream

#comment for stage1
[stage1]
name = Green Hill Zone
this_is_not_a_comment = ;no comment

[stage2]
# comment name
name = Marble Zone
";

        [SetUp]
        public void setup()
        {
            _parser = new IniDataParser();
            _parser.Scheme.SectionStartString = "<";
            _parser.Scheme.SectionEndString = ">";
            _parser.Scheme.CommentString = "#";
            _parser.Scheme.PropertyAssigmentString = "=";

            _parser.Configuration.AllowKeysWithoutSection = true;
            _parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue;
            _parser.Configuration.AllowDuplicateSections = true;
            _parser.Configuration.ThrowExceptionsOnError = false;
            _parser.Configuration.SkipInvalidLines = true;
        }

        [Test]
        public void simple_configuration()
        {
            var iniStr = @"[section1]
#data = 1
;data = 2";

            _parser = new IniDataParser();

            _parser.Scheme.CommentString = "#";

            var iniData = _parser.Parse(iniStr);

            Assert.That(iniData["section1"][";data"], Is.EqualTo("2"));

        }

        [Test]
        public void parse_not_so_good_ini_format()
        {
            var data = _parser.Parse(iniFileStrNotSoGood);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Sections.Count, Is.EqualTo(0));
            Assert.That(data.Global.Count, Is.EqualTo(3));
            Assert.That(data.Global["cyberdreams"], Is.EqualTo("i have no section, and i must scream"));
            Assert.That(data.Global["this_is_not_a_comment"], Is.EqualTo(";no comment"));
            Assert.That(data.Global["name"], Is.EqualTo("Green Hill Zone"), "Should not rewrite an existing key");
        }

        [Test]
        public void parse_ini_with_new_configuration()
        {
            IniData data = _parser.Parse(iniFileStr);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Sections.Count, Is.EqualTo(2));

            Assert.That(data.Global.Count, Is.EqualTo(1));
            Assert.That(data.Global["cyberdreams"], Is.EqualTo("i have no section, and i must scream"));
            
            var section1 = data.Sections.FindByName("stage1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.Name, Is.EqualTo("stage1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));
            Assert.That(section1.Comments[0], Is.EqualTo("comment for stage1"));

            Assert.That(section1.Properties, Is.Not.Null);
            Assert.That(section1.Properties.Count, Is.EqualTo(2));
            Assert.That(section1.Properties.FindByKey("name"), Is.Not.Null);
            Assert.That(section1.Properties["name"], Is.EqualTo("Green Hill Zone"));
            Assert.That(section1.Properties.FindByKey("this_is_not_a_comment"), Is.Not.Null);
            Assert.That(section1.Properties["this_is_not_a_comment"], Is.EqualTo(";no comment"));
        }

        [Test, Ignore("no writing")]
        public void check_ini_writing()
        {
            //IniData data = new IniDataParser(new LiberalTestConfiguration()).Parse(iniFileStr);

            //Assert.That(
            //    data.ToString().Replace(Environment.NewLine, string.Empty), 
            //    Is.EqualTo(iniFileStr.Replace(Environment.NewLine, string.Empty)));
        }

        [Test, Ignore("no writing")]
        public void check_new_line_config_on_ini_writing()
        {
            //IniData data = new IniDataParser(new LiberalTestConfiguration()).Parse(iniFileStr);

            //data.Scheme.NewLineStr = "^_^";

            //Assert.That(
            //    data.ToString().Replace("^_^", string.Empty), 
            //    Is.EqualTo(iniFileStr.Replace(Environment.NewLine, string.Empty)));
        }

        [Test]
        public void comment_string_with_special_characters()
        {
            var iniStr = @"[Section]
                \Backslash Bcomment
                Key=Value";
         
            var parser = new IniDataParser();
            parser.Scheme.CommentString = @"\";

            parser.Parse(iniStr);
        }

        [Test]
        public void section_string_with_special_characters()
        {
            var iniStr = @"\section\
                ;comment
                key=value";

            var parser = new IniDataParser();
            parser.Scheme.SectionStartString = "\\";
            parser.Scheme.SectionEndString = "\\";

            var iniData = parser.Parse(iniStr);

            Assert.That(iniData["section"]["key"], Is.EqualTo("value"));
        }

        [Test]
        public void alway_returns_a_valid_section()
        {
            var parser = new IniDataParser();

            var iniData = parser.Parse("");
            iniData.CreateSectionsIfTheyDontExist = true;
            Assert.IsNotNull(iniData["noname"]);
        }
    }
}
