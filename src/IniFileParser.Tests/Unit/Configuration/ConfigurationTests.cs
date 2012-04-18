using System;
using IniParser.Model;
using IniParser.Model.Configuration;
using IniParser.Parser;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.Unit.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        internal class LiberalTestConfiguration : DefaultIniParserConfiguration
        {
            /// <summary>
            ///     Ctor.
            /// </summary>
            public LiberalTestConfiguration()
            {
                SectionStartChar = '<';
                SectionEndChar = '>';
                CommentChar = '#';
                KeyValueAssigmentChar = '=';

                AllowKeysWithoutSection = true;
                AllowDuplicateKeys = true;
                OverrideDuplicateKeys = true;
                AllowDuplicateSections = true;
                ThrowExceptionsOnError = false;
                SkipInvalidLines = true;
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

        string iniFileReallyBad =
@"
{no section}
key # = wops!
= value
";

        [SetUp]
        public void setup()
        {
            _parser = new IniDataParser(new LiberalTestConfiguration());
        }

        [Test]
        public void check_configuration_is_correct()
        {
            Assert.That(_parser.Configuration, Is.InstanceOfType(typeof (LiberalTestConfiguration)));
            Assert.That(_parser.Parse(iniFileStr).Configuration, Is.InstanceOfType(typeof(LiberalTestConfiguration)));
        }

        [Test]
        public void parser_really_bad_ini_format()
        {
            Assert.That(_parser.Parse(iniFileReallyBad), Is.Null);    
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
            IniData data = new IniDataParser(new LiberalTestConfiguration()).Parse(iniFileStr);

            Assert.That(
                data.ToString().Replace(Environment.NewLine, string.Empty), 
                Is.EqualTo(iniFileStr.Replace(Environment.NewLine, string.Empty)));
        }
    }
}
