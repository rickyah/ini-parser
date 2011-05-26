using System;

using IniParser;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;


namespace IniParserTestNamespace
{
    [TestFixture]
    public class Issues_Test
    {
        public readonly string strBadSectionINIFilePath = @"../../INIFileBADSection.ini";
        public readonly string strBadKeysINIFilePath = @"../../INIFileBADKeys.ini";

        #region Test Members

        //TODO: Add fields used in the tests

        #endregion

        #region TestFixture SetUp/TearDown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //TODO: Add test fixture set up code
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            //TODO: Add test fixture tear down code
        }

        #endregion

        #region  Test SetUp/TearDown

        [SetUp]
        public void SetUp()
        {
            //TODO: Add test set up code
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: Add test tear down code
        }

        #endregion

        #region Tests

        [Test, Description("Test for Issue 2: http://code.google.com/p/ini-parser/issues/detail?id=2")]
        public void Issue2_Test3()
        {
            string test = "[ExampleSection]\nkey = value;value\n";

            StringIniParser strParser = new StringIniParser();

            IniData data = strParser.ParseString(test);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"], Is.Not.Null);
            Assert.That(data.Sections["ExampleSection"].Count, Is.EqualTo(1));
            Assert.That(data.Sections["ExampleSection"]["key"], Is.EqualTo("value"));

        }

        [Test, Description("Test for Issue 3: http://code.google.com/p/ini-parser/issues/detail?id=3")]
        public void Issue3_Tests()
        {
            string strTest = "[section_issue.3]\nkey.with_dots = value\n";

            IniData data = new StringIniParser().ParseString(strTest);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["section_issue.3"]["key.with_dots"], Is.Not.Null);

        }

        [Test, Description("Test for Issue 4: http://code.google.com/p/ini-parser/issues/detail?id=4")]
        public void Issue4_Tests()
        {
            FileIniDataParser fileParser = new FileIniDataParser();

            IniData data = fileParser.LoadFile(strBadSectionINIFilePath, true);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections.GetSectionData("seccion1").Keys.Count, Is.EqualTo(1));

            data = fileParser.LoadFile(strBadKeysINIFilePath, true);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.GetSectionData("seccion1").Keys.Count, Is.EqualTo(1));
        }


        /// <summary>
        ///     Thanks to h.eriksson@artamir.org for the issue.
        /// </summary>
        [Test, Description("Test for Issue 5: http://code.google.com/p/ini-parser/issues/detail?id=5")]
        public void Issue5_Tests()
        {
            
            IniData inidata = new IniData();
            inidata.Sections.AddSection("TestSection");

            KeyData key = new KeyData("TestKey");
            key.Value = "TestValue";
            key.Comments.Add("This is a comment");
            inidata["TestSection"].SetKeyData(key);

            Assert.That(inidata["TestSection"].GetKeyData("TestKey").Comments[0], Is.EqualTo("This is a comment"));
        }


        /// <summary>
        ///     Thanks to h.eriksson@artamir.org for the issue.
        /// </summary>
        [Test, Description("Test for Issue 6: http://code.google.com/p/ini-parser/issues/detail?id=6")]
        public void Issue6_Tests()
        {
            string data = "[data]" + Environment.NewLine +"key = value;";

            IniData inidata = new StringIniParser().ParseString(data);

            Assert.That(inidata["data"]["key"],Is.EqualTo("value") );
        }

        /// <summary>
        ///     Thanks to  for the issue.
        /// </summary>
        [Test, Description("Test for Issue 7: http://code.google.com/p/ini-parser/issues/detail?id=7")]
        public void Issue7_Tests_1()
        {

            IniData newData = new IniData();

            newData.Sections.AddSection("newSection");
            newData["newSection"].AddKey("newKey1", "value1");

            Assert.That(newData["newSection"]["newKey1"], Is.EqualTo("value1"));
        }

        /// <summary>
        ///     Thanks to  for the issue.
        /// </summary>
        [Test, Description("Test for Issue 7: http://code.google.com/p/ini-parser/issues/detail?id=7")]
        public void Issue7_Tests_2()
        {
            IniData newData = new IniData();

            newData.Sections.AddSection("newSection");
            newData["newSection"].AddKey("newKey1");
            newData["newSection"]["newKey1"] = "value1";

            Assert.That(newData["newSection"]["newKey1"], Is.EqualTo("value1"));
        }

        [Test, Description("Test for Issue 10: http://code.google.com/p/ini-parser/issues/detail?id=10")]
        public void Issue10_Tests()
        {
            string data = @"[http://example.com/page] 
key1 = value1";
 
            var parser = new StringIniParser();
            var newData = parser.ParseString(data);

            Assert.That(newData.Sections[@"http://example.com/page"]["key1"], Is.EqualTo("value1"));
        }


        #endregion
    }
}
