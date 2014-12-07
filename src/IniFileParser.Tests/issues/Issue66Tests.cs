using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{

    [TestFixture]
    public class Issue66Tests
    {
        // Thanks https://github.com/RichardSinden for this issue
        [Test,Description("Test for Issue 66 - allow comments at end of sectionless file")]
        public void allow_comments_at_end_of_sectionless_file()
        {
            var parser = new IniDataParser();

            parser.Configuration.AllowKeysWithoutSection = true;
            var iniData = parser.Parse(iniDataString);

            Assert.That(iniData.Global.ContainsKey("value1"));

            Assert.That(iniData.Global.GetKeyData("value1").Comments, Has.Count.EqualTo(3));
            Assert.That(iniData.Global.GetKeyData("value1").Comments[2], Is.EqualTo("end"));
        }

        const string iniDataString = @"
                            ;begin

                            ;value
                            value1 = test

                            ;end
                            ";
    }
}
