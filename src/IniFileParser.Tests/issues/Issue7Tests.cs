using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue7Tests
    {
        /// <summary>
        ///     Thanks to  for the issue.
        /// </summary>
        [Test, Description("Test for Issue 7: http://code.google.com/p/ini-parser/issues/detail?id=7")]
        public void check_add_keydata_method_using_key_and_value_strings()
        {

            IniData newData = new IniData();

            newData.Sections.AddSection("newSection");
            newData["newSection"].AddKey("newKey1", "value1");

            Assert.That(newData["newSection"]["newKey1"], Is.EqualTo("value1"));
        }
    }
}
