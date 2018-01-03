using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Parser
{
    [TestFixture]
    public class StringIniDataParserTests
    {
        [Test, Description("Test for Issue 2: http://code.google.com/p/ini-parser/issues/detail?id=2")]
        public void not_reproduced_error_tests()
        {
            string test = "[ExampleSection]\nkey = value;value\n";

            IniDataParser strParser = new IniDataParser();

            IniData data = strParser.Parse(test);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"], Is.Not.Null);
            Assert.That(data.Sections["ExampleSection"].Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"]["key"], Is.EqualTo("value;value"));
        }

        /// <summary>
        ///     Thanks to h.eriksson@artamir.org for the issue.
        /// </summary>
        [Test, Description("Test for Issue 6: http://code.google.com/p/ini-parser/issues/detail?id=6")]
        public void check_that_comment_char_is_not_stored_as_the_key()
        {
            string data = "[data]" + System.Environment.NewLine + "key = value;";

            IniData inidata = new IniDataParser().Parse(data);

            Assert.That(inidata["data"]["key"], Is.EqualTo("value;"));
        }
    }
}
