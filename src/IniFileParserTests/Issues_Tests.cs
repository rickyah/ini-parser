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
            Assert.That(data.Sections.Count, Is.EqualTo(2));
            Assert.That(data.Sections.GetSectionData("seccion1").Keys.Count, Is.EqualTo(2));

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

        [Test, Description("Test for Issue 9: http://code.google.com/p/ini-parser/issues/detail?id=9")]
        public void Issue9_Tests()
        {

            string data = @"[test]
connectionString = Server=sqlserver.domain.com;Database=main;User ID=user;Password=password";

            StringIniParser parser = new StringIniParser();
            parser.CommentDelimiter = '#';
            IniData iniData = parser.ParseString(data);

            Assert.That(
                iniData["test"]["connectionString"],
                Is.EqualTo("Server=sqlserver.domain.com;Database=main;User ID=user;Password=password"));
        }

        [Test, Description("Test for Issue 10: http://code.google.com/p/ini-parser/issues/detail?id=10")]
        public void Issue10_Tests()
        {
            string data = @"[http://example.com/page] 
key1 = value1";
 
            IniData newData = new StringIniParser().ParseString(data);

            Assert.That(newData.Sections[@"http://example.com/page"]["key1"], Is.EqualTo("value1"));
        }

        [Test, Description("Test for Issue 11: http://code.google.com/p/ini-parser/issues/detail?id=11")]
        public void Issue11_Tests()
        {
            FileIniDataParser parser = new FileIniDataParser();
            
            IniData parsedData = parser.LoadFile("Issue11_example.ini",true);

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }

        [Test, Description("Test for Issue 15: http://code.google.com/p/ini-parser/issues/detail?id=15")]
        public void Issue15_Tests()
        {
            string data = @"[123_1]
key1=value1
key2=value2
[123_2]
key3 = value3
[123_1]
key4=value4";
            IniData iniData = new StringIniParser().ParseString(data, true);

            Assert.That(iniData.Sections.ContainsSection("123_1"), Is.True);
            Assert.That(iniData.Sections.ContainsSection("123_2"), Is.True);
            Assert.That(iniData["123_1"]["key4"], Is.EqualTo("value4"));

        }

		[Test, Description("Test for Issue 17: http://code.google.com/p/ini-parser/issues/detail?id=17")]
        public void Issue17_Tests()
		{
            FileIniDataParser parser = new FileIniDataParser();
            
            IniData parsedData = parser.LoadFile("Issue17_example.ini");
			
			Assert.That(parsedData.Sections.Count, Is.EqualTo(1));
			Assert.That(parsedData.Sections.ContainsSection("{E3729302-74D1-11D3-B43A-00AA00CAD128}"), Is.True);
			Assert.That(parsedData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"].ContainsKey("key"), Is.True);
			Assert.That(parsedData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"]["key"], Is.EqualTo("value"));
		}
		
        #endregion
    }
}
