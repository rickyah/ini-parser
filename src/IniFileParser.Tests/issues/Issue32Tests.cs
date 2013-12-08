using IniParser;
using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue32Tests
    {

        [Test, Description("Test for Issue 32: https://github.com/rickyah/ini-parser/issues/32")]
        public void write_key_and_value_without_blanks()
        {
            var data = new IniData();
            data.Sections.AddSection("section1");
            data["section1"].AddKey("key1", "value1");
            data.Configuration.AssigmentSpacer = "";

            var parser = new StreamIniDataParser();
            string tempPath = System.IO.Path.GetTempFileName();
            using (var sw = new System.IO.StreamWriter(tempPath))
            {
                parser.WriteData(sw, data);
            }

            Assert.AreEqual(@"[section1]
key1=value1
", System.IO.File.ReadAllText(tempPath));
        }

    }
}
