using System;
using System.IO;
using System.Text;
using IniParser.Exceptions;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Parser
{
    [TestFixture]
    public partial class ParserTests
    {
        IniData ReadAndParseIniFile(IniDataParser parser, string fileName, Encoding fileEncoding)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);

            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File does not exists at path {filePath}.");
            }

            try
            {
                // (FileAccess.Read) we want to open the ini only for reading 
                // (FileShare.ReadWrite) any other process should still have access to the ini file 
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, fileEncoding))
                    {
                        return parser.Parse(sr.ReadToEnd());
                    }
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ParsingException($"Could not parse file {filePath}", ex);
            }

        }

        [Test]
        public void check_parses_real_file()
        {
            var parser = new IniDataParser();

            IniData parsedData = ReadAndParseIniFile(parser, "real_file.ini", Encoding.ASCII);

            Assert.That(parsedData.Global[".reg (Win)"], Is.EqualTo("notepad.exe"));
        }

        [Test]
        public void check_parse_long_file()
        {
            var parser = new IniDataParser();
            parser.Scheme.CommentString = "//";

            IniData parsedData = ReadAndParseIniFile(parser, "long_file.ini", Encoding.ASCII);

            Assert.That(parsedData["contact_points"]["point.0"], 
                Is.EqualTo("1,  84.64,   0.00,  -19.05, 1600, 0, 1.8, 70,  1.8, 1.0, 0.9,  9,  8, 0, 270, 290"));
        }

        [Test]
        public void check_parse_unicode_characters()
        {
            var parser = new IniDataParser();

            IniData parsedData = ReadAndParseIniFile(parser, "unicode_chinese_example.ini", Encoding.UTF8);

            Assert.That(parsedData["HSK1.Grammar.Adverb 太"]["structure"], Is.EqualTo("太 + %adjective% + 了"));
        }
    }
}
