using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public void Issue2Test()
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


        #endregion
    }
}
