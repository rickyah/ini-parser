using IniParser.Model;
using IniParser.Model.Formatting;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Parser
{
    [TestFixture]
    public class StreamIniDataParserTests
    {

        [Test, Description("Test for Issue 32: https://github.com/rickyah/ini-parser/issues/32")]
        public void write_key_and_value_without_blanks()
        {
            var data = new IniData();
            data.Sections.AddSection("section1");
            data["section1"].AddKey("key1", "value1");

			var iniFormatter = new IniDataFormatter();
			iniFormatter.Format.AssigmentSpacer = string.Empty;

            var parser = new StreamIniDataParser();
            string tempPath = System.IO.Path.GetTempFileName();
            using (var sw = new System.IO.StreamWriter(tempPath))
            {
				parser.WriteData(sw, data, iniFormatter);
            }

            Assert.AreEqual(@"[section1]
key1=value1
", System.IO.File.ReadAllText(tempPath));
        }

    }
}
