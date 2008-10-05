using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

using IniParser;

namespace IniParserTestNamespace
{
    [TestFixture, Category("Regular expression tests")]
    public class IniParserRegexTest
    {
        public StreamIniDataParser iniParser = new StreamIniDataParser();

        [TestFixtureSetUp]
        public void Init()
        {
            Console.WriteLine(iniParser.SectionRegexString);
        }

        [Test, Description("Test a regular expression for matching a comment in an INI file")]
        public void TestCommentRegex()
        {
            Console.WriteLine("Regular expresion for comments: {0}", iniParser.CommentRegexString);

            string strGoodTest1 = ";comment Test";
            string strGoodTest2 = "   ; comment Test";
            string strGoodTest3 = "   ; comment Test           ";
            string strGoodTest4 = " dfasdfasf ; comment Test ";

            Assert.That(strGoodTest1, Text.Matches(iniParser.CommentRegexString));
            Assert.That(strGoodTest2, Text.Matches(iniParser.CommentRegexString));
            Assert.That(strGoodTest3, Text.Matches(iniParser.CommentRegexString));
            Assert.That(strGoodTest4, Text.Matches(iniParser.CommentRegexString));
        }

        [Test, Description("Test a regular expression for matching a section in an INI file")]
        public void TestSectionRegex()
        {
            Console.WriteLine("Regular expresion for sections: {0}", iniParser.SectionRegexString);


            string strGoodTest1 = "[section]";
            string strGoodTest2 = "   [section]   ";
            string strGoodTest3 = "[ section ]";

            string strBadTest1 = "  bad [section]";
            string strBadTest2 = "[section] bad";


            Assert.That(strGoodTest1, Text.Matches(iniParser.SectionRegexString));
            Assert.That(strGoodTest2, Text.Matches(iniParser.SectionRegexString));
            Assert.That(strGoodTest3, Text.Matches(iniParser.SectionRegexString));

            Assert.That(strBadTest1, Text.DoesNotMatch(iniParser.SectionRegexString));
            Assert.That(strBadTest2, Text.DoesNotMatch(iniParser.SectionRegexString));
        }

        [Test, Description("Test a regular expression for matching a section in an INI file given an specific delimiter")]
        public void TestNewSectionDelimiter()
        {
            iniParser.SectionDelimiters = new char[2] { '<', '>' };
            Console.WriteLine("Regular expresion for sections: {0}", iniParser.SectionRegexString);


            string strGoodTest1 = "<section>";
            string strGoodTest2 = "   <section>   ";
            string strGoodTest3 = "< section >";

            string strBadTest1 = "  bad <section>";
            string strBadTest2 = "<section> bad";


            Assert.That(strGoodTest1, Text.Matches(iniParser.SectionRegexString));
            Assert.That(strGoodTest2, Text.Matches(iniParser.SectionRegexString));
            Assert.That(strGoodTest3, Text.Matches(iniParser.SectionRegexString));

            Assert.That(strBadTest1, Text.DoesNotMatch(iniParser.SectionRegexString));
            Assert.That(strBadTest2, Text.DoesNotMatch(iniParser.SectionRegexString));

            //Restore default delimiters
            iniParser.SectionDelimiters = new char[2] { '[', ']' };
        }

        [Test, Description("Test a regular expression for matching a key=value pair in an ini file")]
        public void KeyValuePairRegex()
        {

            Console.WriteLine("Regular expresion for key values: {0}", iniParser.KeyValuePairRegexString);

            string strGoodTest1 = "    value   =   hello   ";
            string strGoodTest2 = "   value=hello world!  ";
            string strGoodTest3 = "     value=hello world!   = hello world!  ";
            string strGoodTest4 = "value=hello world!  \" ";

            string strBadTest1 = " value value = hello world ";

            Assert.That(strGoodTest1, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest2, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest3, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest4, Text.Matches(iniParser.KeyValuePairRegexString));

            Assert.That(strBadTest1, Text.DoesNotMatch(iniParser.KeyValuePairRegexString.ToString()));
        }

        [Test, Description("Test a regular expression for matching a key in a key=value pair")]
        public void KeyRegex()
        {
            string strGoodTest1 = "    value   =   hello   ";
            string strGoodTest2 = "   value=hello world!  ";
            string strGoodTest3 = "     value=hello world!   = hello world!  ";
            string strGoodTest4 = "value=hello world!  \" ";

            string strBadTest1 = " value value = hello world ";

            Assert.That(strGoodTest1, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest2, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest3, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest4, Text.Matches(iniParser.KeyValuePairRegexString));

            Assert.That(strBadTest1, Text.DoesNotMatch(iniParser.KeyValuePairRegexString));
        }

        [Test, Description("Test a regular expression for matching a value in a key=value pair")]
        public void ValueRegex()
        {
            string strGoodTest1 = "    value   =   hello   ";
            string strGoodTest2 = "   value=hello world!  ";
            string strGoodTest3 = "     value=hello world!   = hello world!  ";
            string strGoodTest4 = "value=hello world!  \" ";

            string strBadTest1 = " value value = hello world ";

            Assert.That(strGoodTest1, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest2, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest3, Text.Matches(iniParser.KeyValuePairRegexString));
            Assert.That(strGoodTest4, Text.Matches(iniParser.KeyValuePairRegexString));

            Assert.That(strBadTest1, Text.DoesNotMatch(iniParser.KeyValuePairRegexString));
        }
    }
}
