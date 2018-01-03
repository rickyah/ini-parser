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
            /// <summary>
            ///     Ctor.
            /// </summary>
            public LiberalTestConfiguration()
                :base(new IniScheme())
            {

				this.Scheme.SectionStartString = "<";
                this.Scheme.SectionEndString = ">";
                this.Scheme.CommentString = "#";
                this.Scheme.KeyValueAssigmentString = "=";

                this.AllowKeysWithoutSection = true;
                this.AllowDuplicateKeys = true;
                this.OverrideDuplicateKeys = true;
                this.AllowDuplicateSections = true;
                this.ThrowExceptionsOnError = false;
                this.SkipInvalidLines = true;
            }

             new LiberalTestConfiguration Clone()
             {
                 return base.Clone() as LiberalTestConfiguration;
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
[stage1]
name = Green Hill Zone
this_is_not_a_comment = ;no comment

[stage2]
# comment name
name = Marble Zone
";

        #endregion 

        [SetUp]
        public void setup()
        {
            this._parser = new IniDataParser(new LiberalTestConfiguration());
        }


        [Test]
        public void check_default_values()
        {
            var config = new IniParserConfiguration(new IniScheme());

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Scheme.CommentRegex, Is.Not.Null);
            Assert.That(config.Scheme.SectionRegex, Is.Not.Null);

        }

        [Test]
        public void simple_configuration()
        {
            var iniStr = @"[section1]
#data = 1
;data = 2";

            var config = new IniParserConfiguration(new IniScheme() );

            config.Scheme.CommentString = "#";

            this._parser = new IniDataParser(config);

            var iniData = this._parser.Parse(iniStr);

            Assert.That(iniData["section1"][";data"], Is.EqualTo("2"));

        }

        [Test]
        public void check_configuration_is_correct()
        {
            Assert.That(this._parser.Configuration, Is.InstanceOf(typeof (LiberalTestConfiguration)));
        }

        [Test]
        public void parse_not_so_good_ini_format()
        {
            var data = this._parser.Parse(this.iniFileStrNotSoGood);
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
            IniData data = this._parser.Parse(this.iniFileStr);
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
			var config = new LiberalTestConfiguration();
			var format = new IniFormattingConfiguration(config.Scheme);
			config.Scheme.CommentString = "#";
            IniData data = new IniDataParser(config).Parse(this.iniFileStr);

			Assert.That(data.ToString(new IniFormattingConfiguration(config.Scheme))
			                          .Replace(Environment.NewLine, string.Empty),
			            Is.EqualTo(this.iniFileStr.Replace(Environment.NewLine, string.Empty)));
        }

        [Test]
        public void check_new_line_confige_on_ini_writing()
        {
			var configuration = new LiberalTestConfiguration();
			IniData data = new IniDataParser(configuration).Parse(this.iniFileStr);

			var format = new IniFormattingConfiguration(configuration.Scheme);
			format.NewLineStr = "^_^";

			string newIniStr = data.ToString(format);

            Assert.That(newIniStr.Replace("^_^", string.Empty),
			Is.EqualTo(this.iniFileStr.Replace(Environment.NewLine, string.Empty)));
        }

        [Test]
        public void escape_comment_regex_special_characters()
        {
            var iniStr = @"[Section]
                \Backslash Bcomment
                Key=Value";
         
            var parser = new IniDataParser();
            parser.Configuration.Scheme.CommentString = @"\";

            parser.Parse(iniStr);
        }

        [Test]
        public void escape_section_regex_special_characters()
        {
            var iniStr = @"\section\
                ;comment
                key=value";

            var parser = new IniDataParser();
            parser.Configuration.Scheme.SectionStartString = "\\";
            parser.Configuration.Scheme.SectionEndString = "\\";

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
            IniParserConfiguration config1 = new IniParserConfiguration(new IniScheme());

            config1.AllowDuplicateKeys = true;
            config1.Scheme.CommentString = "/";

            Assert.That(config1.AllowDuplicateKeys, Is.True);
            Assert.That(config1.Scheme.CommentString, Is.EqualTo("/"));

            IniParserConfiguration config2 = config1.Clone();

            Assert.That(config2.AllowDuplicateKeys, Is.True);
            Assert.That(config2.Scheme.CommentString, Is.EqualTo("/"));

            config1.Scheme.CommentString = "#";
            Assert.That(config2.Scheme.CommentString, Is.EqualTo("/"));
        }
    }
}
