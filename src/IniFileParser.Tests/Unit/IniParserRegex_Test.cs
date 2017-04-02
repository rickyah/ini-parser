using System;
using NUnit.Framework;
using IniParser;

namespace IniFileParser.Tests.Unit
{
    [TestFixture, Category("Regular expression tests")]
    public class IniParserRegexTest
    {
        public StreamIniDataParser iniParser = new StreamIniDataParser();

        [TestFixtureSetUp]
        public void Init()
        {
            Console.WriteLine(iniParser.Parser.Configuration.SectionRegex);
        }

        [Test, Description("Test a regular expression for matching a comment in an INI file")]
        public void TestCommentRegex()
        {
            Console.WriteLine("Regular expresion for comments: {0}", iniParser.Parser.Configuration.CommentRegex);

            string strGoodTest1 = ";comment Test";
            string strGoodTest2 = "   ; comment Test";
            string strGoodTest3 = "   ; comment Test           ";
            string strGoodTest4 = " dfasdfasf ; comment Test ";

            Assert.That(strGoodTest1, Is.StringMatching(iniParser.Parser.Configuration.CommentRegex.ToString()));
            Assert.That(strGoodTest2, !Is.StringMatching(iniParser.Parser.Configuration.CommentRegex.ToString()));
            Assert.That(strGoodTest3, !Is.StringMatching(iniParser.Parser.Configuration.CommentRegex.ToString()));
            Assert.That(strGoodTest4, !Is.StringMatching(iniParser.Parser.Configuration.CommentRegex.ToString()));
        }

        [Test, Description("Test a regular expression for matching a section in an INI file")]
        public void TestSectionRegex()
        {
            Console.WriteLine("Regular expresion for sections: {0}", iniParser.Parser.Configuration.SectionRegex);


            string strGoodTest1 = "[section]";
            string strGoodTest2 = "   [sec-tion]   ";
            string strGoodTest3 = "[ .section ]";
            string strGoodTest4 = "[ s_ection ]";

            string strBadTest1 = "  bad [section]";
            string strBadTest2 = "[section] bad";

            Assert.That(strGoodTest1, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strGoodTest2, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strGoodTest3, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strGoodTest4, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));

            Assert.That(strBadTest1, Is.Not.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strBadTest2, Is.Not.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
        }

        [Test, Description("Test a regular expression for matching a section in an INI file given an specific delimiter")]
        public void TestNewSectionDelimiter()
        {
            iniParser.Parser.Configuration.SectionStartChar = '<';
            iniParser.Parser.Configuration.SectionEndChar = '>';
            Console.WriteLine("Regular expresion for sections: {0}", iniParser.Parser.Configuration.SectionRegex.ToString());


            string strGoodTest1 = "<section>";
            string strGoodTest2 = "   <section>   ";
            string strGoodTest3 = "< section >";

            string strBadTest1 = "  bad <section>";
            string strBadTest2 = "<section> bad";


            Assert.That(strGoodTest1, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strGoodTest2, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strGoodTest3, Is.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));

            Assert.That(strBadTest1, Is.Not.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));
            Assert.That(strBadTest2, Is.Not.StringMatching(iniParser.Parser.Configuration.SectionRegex.ToString()));

            //Restore default delimiters
            iniParser.Parser.Configuration.SectionStartChar = '[';
            iniParser.Parser.Configuration.SectionEndChar = ']';
        }
    }
}
