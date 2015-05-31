using NUnit.Framework;
using System;

using IniParser.Parser;
using IniParser.Model;

namespace IniFileParser.Tests.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class INIDataTests
    {
        [Test]
        public void delete_all_comments()
        {
            string iniData = @";comment1
key1 = 2
;comment2
[section1]

;a value
value1 = 10.6";

            var data = new IniDataParser().Parse(iniData);

            Assert.That(data.Global.GetKeyData("key1").Comments, Is.Not.Empty);
            Assert.That(data.Sections.GetSectionData("section1").Comments, Is.Not.Empty);
            Assert.That(data["section1"].GetKeyData("value1").Comments, Is.Not.Empty);


            data.ClearAllComments();

            Assert.That(data.Global.GetKeyData("key1").Comments, Is.Empty);
            Assert.That(data.Sections.GetSectionData("section1").Comments, Is.Empty);
            Assert.That(data["section1"].GetKeyData("value1").Comments, Is.Empty);

        }

        [Test, Description("Test for Issue 7: http://code.google.com/p/ini-parser/issues/detail?id=7")]
        public void check_add_keydata_method_using_key_and_value_strings()
        {
            var newData = new IniData();

            newData.Sections.AddSection("newSection");
            newData["newSection"].AddKey("newKey1", "value1");

            Assert.That(newData["newSection"]["newKey1"], Is.EqualTo("value1"));
        }
    }
}

