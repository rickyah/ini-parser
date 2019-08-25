using NUnit.Framework;
using IniParser.Configuration;

namespace IniParser.Tests.Unit.Format
{
    [TestFixture]
    public class IniFormatterTests
    {
        IniDataParser parser;
        IniDataFormatter formatter;
        IniFormattingConfiguration formatConfig;

        [SetUp]
        public void Setup()
        {
            parser = new IniDataParser();
            formatter = new IniDataFormatter();
            formatConfig = new IniFormattingConfiguration();
        }

        [Test]
        public void check_default_formatting()
        {
            var str = @";this is a section
[section1]
key = value";


            var iniData = parser.Parse(str);

            var result = formatter.Format(iniData, formatConfig);

            Assert.That(str, Is.EqualTo(result));
        }

        [Test]
        public void check_custom_formatting()
        {
            var str = @";this is a section

[section1]


key=value


key2=value
";

            var iniData = parser.Parse(str);

            formatConfig.NumSpacesBetweenKeyAndAssigment = 0;
            formatConfig.NumSpacesBetweenAssigmentAndValue= 0;
            formatConfig.NewLineBeforeSection = true;
            formatConfig.NewLineAfterSection = true;
            formatConfig.NewLineBeforeProperty = true;
            formatConfig.NewLineAfterProperty = true;
            var result = formatter.Format(iniData, formatConfig);

            Assert.That(str, Is.EqualTo(result));
        }
    }
}

