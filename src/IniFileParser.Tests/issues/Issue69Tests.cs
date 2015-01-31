using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue69Tests
    {
        private string iniFileStr =
@"[MySection]
File=a.txt
[MySection]
File=b.txt
[MySection]
File=c.txt
[MySection]
File=d.txt
";

        [Test, Description("Test setting ConcatenateDuplicateKeys for Issue 69: https://github.com/rickyah/ini-parser/issues/69")]
        public void allow_concatenate_duplicate_keys()
        {
            var parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            parser.Configuration.ConcatenateDuplicateKeys = true;
            IniData data = parser.Parse(iniFileStr);
            Assert.That(data["MySection"]["File"] == "a.txt;b.txt;c.txt;d.txt");
        }

        [Test, Description("Test setting ConcatenateSeparator for Issue 69: https://github.com/rickyah/ini-parser/issues/69")]
        public void allow_set_concatenate_separator()
        {
            var parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            parser.Configuration.ConcatenateDuplicateKeys = true;
            parser.Configuration.ConcatenateSeparator = "+";
            IniData data = parser.Parse(iniFileStr);
            Assert.That(data["MySection"]["File"] == "a.txt+b.txt+c.txt+d.txt");
        }
    }
}