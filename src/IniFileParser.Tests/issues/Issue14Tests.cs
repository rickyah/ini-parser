using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue14Tests
    {
        [Test, Description("Test for Issue 14: http://code.google.com/p/ini-parser/issues/detail?id=14")]
        public void check_can_read_keys_with_no_section()
        {
            string data =
@"key1=value1
key2=value2
key3=value3";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Global.Count, Is.EqualTo(3));
            Assert.That(iniData.Global["key1"], Is.EqualTo("value1"));
            Assert.That(iniData.Global["key2"], Is.EqualTo("value2"));
            Assert.That(iniData.Global["key3"], Is.EqualTo("value3"));
        }
    }
}
