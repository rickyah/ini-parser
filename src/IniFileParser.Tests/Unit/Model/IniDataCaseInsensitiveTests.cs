using IniParser.Model;
using NUnit.Framework;
using IniParser.Parser;
using IniParser.Model.Configuration;

namespace IniFileParser.Tests.Unit.Model
{
    [TestFixture]
    public class IniDataCaseInsensitiveTests
    {
        [Test, Description("Test for Issue 76: https://github.com/rickyah/ini-parser/issues/76")]
        public void resolve_case_insensitive_names()
        {

            var data = new IniDataCaseInsensitive();
            var section = new Section("TestSection");
            section.Properties.AddKeyAndValue("keY1", "value1");
            section.Properties.AddKeyAndValue("KEY2", "value2");
            section.Properties.AddKeyAndValue("KeY2", "value3");

            data.Sections.Add(section);

            Assert.That(data.Sections.ContainsSection("testsection"));
            Assert.That(data.Sections.ContainsSection("TestSection"));
            Assert.That(data["TestSection"]["key1"], Is.EqualTo("value1"));
            Assert.That(data["TestSection"]["keY1"], Is.EqualTo("value1"));
            Assert.That(data["TestSection"]["KEY2"], Is.EqualTo("value3"));
            Assert.That(data["TestSection"]["KeY2"], Is.EqualTo("value3"));
            Assert.That(data["TestSection"]["key2"], Is.EqualTo("value3"));
        }

        [Test]
        public void parse_case_insensitive_names_ini_file()
        {
            string iniData = @"[TestSection]
            KEY1 = value1
            KEY2 = value2";

            var parser = new IniDataParser();
            parser.Configuration.CaseInsensitive = true;
            var data = parser.Parse(iniData);

            Assert.That(data["testsection"]["key1"], Is.EqualTo("value1"));
            Assert.That(data["testSection"]["Key2"], Is.EqualTo("value2"));

        }

        [Test, Description("Test for Issue 135: https://github.com/rickyah/ini-parser/issues/135")]
        public void resolve_case_insensitive_names_in_global()
        {

            var data = new IniDataCaseInsensitive();
            data.Global.AddKeyAndValue("keY1", "value1");
            data.Global.AddKeyAndValue("KEY2", "value2");
            data.Global["KeY2"] = "value3";

            Assert.That(data.Global["key1"], Is.EqualTo("value1"));
            Assert.That(data.Global["keY1"], Is.EqualTo("value1"));
            Assert.That(data.Global["KEY2"], Is.EqualTo("value3"));
            Assert.That(data.Global["KeY2"], Is.EqualTo("value3"));
            Assert.That(data.Global["key2"], Is.EqualTo("value3"));
        }
    }
}
