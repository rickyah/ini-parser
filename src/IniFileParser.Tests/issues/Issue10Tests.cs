using IniParser;
using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue10Tests
    {

        [Test, Description("Test for Issue 10: http://code.google.com/p/ini-parser/issues/detail?id=10")]
        public void test_no_exception_is_raised_when_reading_url_like_section_names()
        {
            string data = 
@"[http://example.com/page] 
key1 = value1";

            IniData newData = new StringIniParser().ParseString(data);

            Assert.That(newData.Sections[@"http://example.com/page"]["key1"], Is.EqualTo("value1"));
        }

    }
}
