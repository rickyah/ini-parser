using System;
using IniParser.Model;
using IniParser.Model.Configuration;
using IniParser.Parser;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {

        #region test data
        internal class LiberalTestConfiguration : IniParserConfiguration
        {
            public LiberalTestConfiguration()
                :base()
            {
                AllowKeysWithoutSection = true;
                AllowDuplicateKeys = true;
                OverrideDuplicateKeys = true;
                AllowDuplicateSections = true;
                ThrowExceptionsOnError = false;
                SkipInvalidLines = true;
            }
        }

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

name = Green Hill Zone
this_is_not_a_comment = ;no comment

# comment name
name = Marble Zone
";

        #endregion 

        [SetUp]
        public void setup()
        {
            _parser = new IniDataParser();
        }


        [Test]
        public void check_default_values()
        {
            var config = new IniParserConfiguration();
            var scheme = new IniScheme();
            Assert.That(config, Is.Not.Null);
            Assert.That(scheme.CommentRegex, Is.Not.Null);
            Assert.That(scheme.SectionRegex, Is.Not.Null);

        }

        [Test]
        public void simple_configuration()
        {
            var iniStr = @"[section1]
#data = 1
;data = 2";

            _parser.Scheme.CommentString = "#";

            var iniData = _parser.Parse(iniStr);

            Assert.That(iniData["section1"][";data"], Is.EqualTo("2"));

        }

        [Test]
        public void check_configuration_is_correct()
        {
            Assert.That(_parser.Configuration, Is.InstanceOf(typeof (LiberalTestConfiguration)));
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
            Assert.That(data.Global["name"], Is.EqualTo("Marble Zone"), "Should not rewrite an existing key");
        }

        [Test]
        public void parse_ini_with_new_configuration()
        {
            IniData data = _parser.Parse(iniFileStr);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Sections.Count, Is.EqualTo(2));

            Assert.That(data.Global.Count, Is.EqualTo(1));
            Assert.That(data.Global["cyberdreams"], Is.EqualTo("i have no section, and i must scream"));
            
            var section1 = data.Sections.GetSectionData("stage1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.SectionName, Is.EqualTo("stage1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));
            Assert.That(section1.Comments[0], Is.EqualTo("comment for stage1"));

            Assert.That(section1.Keys, Is.Not.Null);
            Assert.That(section1.Keys.Count, Is.EqualTo(2));
            Assert.That(section1.Keys.GetKeyData("name"), Is.Not.Null);
            Assert.That(section1.Keys["name"], Is.EqualTo("Green Hill Zone"));
            Assert.That(section1.Keys.GetKeyData("this_is_not_a_comment"), Is.Not.Null);
            Assert.That(section1.Keys["this_is_not_a_comment"], Is.EqualTo(";no comment"));
        }

        [Test]
        public void check_ini_writing()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentString = "#";
            parser.Configuration.OverwriteWith(new LiberalTestConfiguration());
            var formatConfig = new IniFormattingConfiguration();

            IniData data = parser.Parse(iniFileStr);

            var originalFile = iniFileStr.Replace(Environment.NewLine, string.Empty);
            var generatedFile = data.ToString(formatConfig).Replace(Environment.NewLine, string.Empty);
            Assert.That(originalFile, Is.EqualTo(generatedFile));
        }

        [Test]
        public void check_new_line_confige_on_ini_writing()
        {
            var parser = new IniDataParser();
            parser.Configuration.OverwriteWith(new LiberalTestConfiguration());
            IniData data = parser.Parse(iniFileStr);

            var formatConfig = new IniFormattingConfiguration();
            formatConfig.NewLineStr = "^_^";

            var originalFile = iniFileStr.Replace(Environment.NewLine, string.Empty);
            var generatedFile = data.ToString(formatConfig).Replace("^_^", string.Empty);

            Assert.That(originalFile, Is.EqualTo(generatedFile));
        }

        [Test]
        public void escape_comment_regex_special_characters()
        {
            var iniStr = @"[Section]
                \Backslash Bcomment
                Key=Value";
         
            var parser = new IniDataParser();
            parser.Scheme.CommentString = @"\";

            parser.Parse(iniStr);
        }

        [Test]
        public void escape_section_regex_special_characters()
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
            Assert.IsNotNull(iniData["noname"]);
        }

        [Test]
        public void check_cloning()
        {
            var config1 = new IniParserConfiguration();

            config1.AllowDuplicateKeys = true;
            Assert.That(config1.AllowDuplicateKeys, Is.True);

            var config2 = config1.DeepClone();
            Assert.That(config2.AllowDuplicateKeys, Is.True);

        }
    }
}
