using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture, Description("Test for Issue 24: http://code.google.com/p/ini-parser/issues/detail?id=24")]
    public class Issue24Tests
    {
        string data =
@"win] 
key1 = value1";

        private IniDataParser parser;
        [SetUp]
        public void Setup()
        {
            parser = new IniDataParser();
        }

        [Test, ExpectedException(typeof(ParsingException))]
        public void allow_skiping_unparsable_lines_disabled_by_default()
        {
            parser.Parse(data);
        }

        [Test]
        public void allow_skiping_unparsable_lines()
        {
            parser.Configuration.SkipInvalidLines = true;

            IniData newData = parser.Parse(data);

            Assert.That(newData.Global["key1"], Is.EqualTo("value1"));
        }

    }
}
