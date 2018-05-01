using System.Text;
using IniParser.Model;
using NUnit.Framework;
using IniParser.Parser;
using System;
using System.IO;

namespace IniParser.Tests.Integration
{
    [TestFixture]
    public class ParsingConfigurationTests
    {
        [Test]
        public void test_utf8_encoding()
        {
            var parser = new IniDataParser();
            string data = @"[Identität]
key = value";

            IniData parsedData = parser.Parse(data);

            Assert.That(parsedData.Sections.ContainsSection("Identität"));
            Assert.That(parsedData.Sections["Identität"]["key"], Is.EqualTo("value"));
        }

        [Test]
        public void allow_duplicated_sections()
        {
            var parser = new IniDataParser();
            var reader = new StreamReader("TestIniFiles/BigIniFile.ini");

            IniData parsedData = parser.Parse(reader);

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }

        [Test]
        public void check_parses_real_test_files()
        {
            var parser = new IniDataParser();
            var reader = new StreamReader("TestIniFiles/aircraft.cfg");
            parser.Configuration.ThrowExceptionsOnError = true;

            var iniFileData = parser.Parse(reader);

            parser.Scheme.CommentString = "//";
            iniFileData = parser.Parse(new StreamReader("TestIniFiles/aircraft2.cfg"));
        }

        [Test]
        public void test_unicode_chinese_encoding()
        {
            var parser = new IniDataParser();
            parser.Configuration.ThrowExceptionsOnError = true;

            var iniFileData = parser.Parse(new StreamReader("TestIniFiles/unicode_chinese.ini", Encoding.UTF8));

            Assert.That(iniFileData["HSK1.Grammar.Adverb 太"]["structure"], Is.EqualTo("太 + %adjective% + 了"));
        }
    }
}
