using IniParser;
using IniParser.Configuration;
using IniParser.Exceptions;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Configuration
{
    [TestFixture]
    public class ConfigurationOptionsTests
    {
        [Test]
        public void check_case_insensitive_options()
        {
            var ini = @"
data1 = 1
[Section1]
Data2 = 2";

            var parser = new IniDataParser();

            var iniData = parser.Parse(ini);

            Assert.That(iniData.Sections.FindByName("Section1"), 
                Is.Not.Null);
            Assert.That(iniData.Sections.FindByName("section1"), 
                Is.Null);

            parser.Configuration.CaseInsensitive = true;

            iniData = parser.Parse(ini);

            Assert.That(iniData.Sections.FindByName("Section1"), 
                Is.Not.Null);
            Assert.That(iniData.Sections.FindByName("section1"), 
                Is.Not.Null);
            Assert.That(iniData.Sections.FindByName("Section1"), 
                Is.EqualTo(iniData.Sections.FindByName("section1")));
        }

        [Test]
        public void check_configuration_allow_keys_without_section()
        {
            var ini = @"
data1 = 1
[Section1]
data2 = 2";

            var parser = new IniDataParser();

            parser.Configuration.AllowKeysWithoutSection = true;

            var iniData = parser.Parse(ini);

            Assert.That(iniData.Global["data1"], Is.EqualTo("1"));


            parser.Configuration.AllowKeysWithoutSection = false;

            Assert.Throws(typeof(ParsingException), () => {
                parser.Parse(ini);
            });

        }

        [Test]
        public void check_duplicated_properties_behaviour()
        {
            var ini = @"[Section1]
data1 = 1
data1 = 5";

            var parser = new IniDataParser();

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.DisallowAndStopWithError;

            Assert.Throws(typeof(ParsingException), () => {
                parser.Parse(ini);
            });

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue;

            var iniData = parser.Parse(ini);

            Assert.That(iniData["Section1"]["data1"], Is.EqualTo("1"));

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;

            iniData = parser.Parse(ini);
            Assert.That(iniData["Section1"]["data1"], Is.EqualTo("5"));

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndConcatenateValues;
            parser.Configuration.ConcatenateDuplicatePropertiesString = ";;";

            iniData = parser.Parse(ini);

            Assert.That(iniData["Section1"]["data1"], Is.EqualTo("1;;5"));
        }

        [Test]
        public void check_throw_exception_on_error()
        {
            var ini = @"dfsdfdf ;bad line
data1 = 1";

            var parser = new IniDataParser();

            Assert.Throws(typeof(ParsingException), () => {
                parser.Parse(ini);
            });

            parser.Configuration.ThrowExceptionsOnError = false;

            var iniData = parser.Parse(ini);

            Assert.That(iniData, Is.Not.Null);
            Assert.That(parser.HasError, Is.True);
            Assert.That(parser.Errors, Has.Count.EqualTo(1));
            Assert.That(parser.Errors[0].GetType(), Is.EqualTo(typeof(ParsingException)));
            if (parser.Errors[0] is ParsingException parsingException) {
                Assert.That(parsingException.LineContents, Is.EqualTo("dfsdfdf ;bad line"));
                Assert.That(parsingException.LineNumber, Is.EqualTo(1));
            }
        }
    
        [Test]
        public void check_duplicate_sections()
        {
            var ini = @"[section1]
data1 = 1
[section1]
data2 = 2";

            var parser = new IniDataParser();

            Assert.Throws(typeof(ParsingException), () => parser.Parse(ini));

            parser.Configuration.AllowDuplicateSections = true;
            var iniData = parser.Parse(ini);

            Assert.That(iniData["section1"], Has.Count.EqualTo(2));
        }

        [Test]
        public void check_skip_invalid_lines()
        {
            var ini = @"[section1]
[section1]
";

            var parser = new IniDataParser();

            Assert.Throws(typeof(ParsingException), () => parser.Parse(ini));

            parser.Configuration.SkipInvalidLines = true;

            Assert.DoesNotThrow(() => parser.Parse(ini));
        }

        [Test]
        public void check_trim_properties()
        {
            var ini = @"prop1=0
   prop2 =   value2  
";

            var parser = new IniDataParser();

            var iniData = parser.Parse(ini);

            Assert.That(iniData.Global.Contains("prop1"), Is.True);
            Assert.That(iniData.Global.Contains("prop2"), Is.True);
            Assert.That(iniData.Global["prop2"], Is.EqualTo("value2"));

            parser.Configuration.TrimProperties = false;
            iniData = parser.Parse(ini);

            Assert.That(iniData.Global.Contains("prop1"), Is.True);
            Assert.That(iniData.Global.Contains("prop2"), Is.False);
            Assert.That(iniData.Global.Contains("   prop2 "), Is.True);
            Assert.That(iniData.Global["   prop2 "], Is.EqualTo("   value2  "));
        }

        [Test]
        public void check_trim_sections()
        {
            var ini = @"[   section1]";

            var parser = new IniDataParser();

            var iniData = parser.Parse(ini);

            Assert.That(iniData.Sections.Contains("section1"), Is.True);

            parser.Configuration.TrimSections = false;
            iniData = parser.Parse(ini);

            Assert.That(iniData.Sections.Contains("section1"), Is.False);
            Assert.That(iniData.Sections.Contains("   section1"), Is.True);
        }

        [Test]
        public void check_trim_comments()
        {
            var ini = @"; comment
[section1]";

            var parser = new IniDataParser();

            var iniData = parser.Parse(ini);

            Assert.That(iniData.Sections.FindByName("section1").Comments[0],
                Is.EqualTo("comment"));

            parser.Configuration.TrimComments = false;
            iniData = parser.Parse(ini);
            Assert.That(iniData.Sections.FindByName("section1").Comments[0],
                Is.EqualTo(" comment"));
        }

        [Test]
        public void check_parse_comments()
        {
            var ini = @"; comment
[section1]";

            var parser = new IniDataParser();

            var iniData = parser.Parse(ini);
            Assert.That(iniData.Sections.FindByName("section1").Comments, Has.Count.EqualTo(1));


            parser.Configuration.ParseComments = false;

            iniData = parser.Parse(ini);
            Assert.That(iniData.Sections.FindByName("section1").Comments, Has.Count.EqualTo(0));
        }
    }
}
