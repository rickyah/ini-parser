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
        IniDataParser _parser;
        IniParserConfiguration _permisiveConfiguration;
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


        [SetUp]
        public void setup()
        {
            _permisiveConfiguration = new IniParserConfiguration();
            _permisiveConfiguration.AllowKeysWithoutSection = true;
            _permisiveConfiguration.AllowDuplicateKeys = true;
            _permisiveConfiguration.OverrideDuplicateKeys = true;
            _permisiveConfiguration.AllowDuplicateSections = true;
            _permisiveConfiguration.ThrowExceptionsOnError = false;
            _permisiveConfiguration.SkipInvalidLines = true;
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
        public void parse_not_so_good_ini_format()
        {
                string iniFileStrNotSoGood =
@"
cyberdreams = i have no section, and i must scream

#comment for stage1

name = Green Hill Zone
this_is_not_a_comment = ;no comment

# comment name
name = Marble Zone
";
            _parser.Scheme.CommentString = "#";
            _parser.Configuration.AllowDuplicateKeys = true;
            _parser.Configuration.OverrideDuplicateKeys = true;

            var data = _parser.Parse(iniFileStrNotSoGood);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Sections.Count, Is.EqualTo(0));
            Assert.That(data.Global.Count, Is.EqualTo(3));
            Assert.That(data.Global["cyberdreams"], Is.EqualTo("i have no section, and i must scream"));
            Assert.That(data.Global["this_is_not_a_comment"], Is.EqualTo(";no comment"));
            Assert.That(data.Global["name"], Is.EqualTo("Marble Zone"), "Value of an existing key was overwritten!");
        }

        [Test]
        public void parse_ini_with_new_configuration()
        {
            var iniDataStr =
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
            _parser.Scheme.SectionStartString = "<";
            _parser.Scheme.SectionEndString = ">";
            _parser.Scheme.CommentString = "#";
            IniData data = _parser.Parse(iniDataStr);
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

        [Test, Ignore("Testing file writing does not belong here")]
        public void check_ini_writing()
        {
            //var parser = new IniDataParser();
            //parser.Scheme.CommentString = "#";
            //parser.Configuration.OverwriteWith(_permisiveConfiguration);
            //var formatConfig = new IniFormattingConfiguration();

            //IniData data = parser.Parse(iniFileStr);

            //var originalFile = iniFileStr.Replace(Environment.NewLine, string.Empty);
            //var generatedFile = data.ToString(formatConfig).Replace(Environment.NewLine, string.Empty);
            //Assert.That(originalFile, Is.EqualTo(generatedFile));
        }

        [Test, Ignore("Testing file writing does not belong here")]
        public void check_new_line_confige_on_ini_writing()
        {
            //var parser = new IniDataParser();
            //parser.Configuration.OverwriteWith(_permisiveConfiguration);
            //IniData data = parser.Parse(iniFileStr);

            //var formatConfig = new IniFormattingConfiguration();
            //formatConfig.NewLineStr = "^_^";

            //var originalFile = iniFileStr.Replace(Environment.NewLine, string.Empty);
            //var generatedFile = data.ToString(formatConfig).Replace("^_^", string.Empty);

            //Assert.That(originalFile, Is.EqualTo(generatedFile));
        }

        [Test, Ignore("Testing file writing does not belong here")]
        public void alway_returns_a_valid_section()
        {
            //var parser = new IniDataParser();

            //var iniData = parser.Parse("");
            //Assert.IsNotNull(iniData["noname"]);
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
