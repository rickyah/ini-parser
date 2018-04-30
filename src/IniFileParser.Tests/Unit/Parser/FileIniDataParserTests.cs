using System.Text;
using IniParser.Model;
using NUnit.Framework;
using IniParser.Parser;
using System;
using System.IO;

namespace IniParser.Tests.Unit.Parser
{
    [TestFixture]
    public class FileIniDataParserTests
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
            var reader = new StreamReader("Issue11_example.ini");

            IniData parsedData = parser.Parse(reader);

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }

        [Test]
        public void check_parses_real_test_files()
        {
            var parser = new IniDataParser();
            var reader = new StreamReader("aircraft.cfg");
            parser.Configuration.ThrowExceptionsOnError = true;

            var iniFileData = parser.Parse(reader);

            parser.Scheme.CommentString = "//";
            iniFileData = parser.Parse(new StreamReader("aircraft2.cfg"));
        }

        [Test]
        public void test_unicode_chinese_encoding()
        {
            var parser = new IniDataParser();
            parser.Configuration.ThrowExceptionsOnError = true;

            var iniFileData = parser.Parse(new StreamReader("unicode_chinese.ini", Encoding.UTF8));

            // If you want to write the file you must specify the encoding
            //parser.WriteFile("unicode_chinese_copy.ini", iniFileData, Encoding.UTF8);
        }
    }
}
