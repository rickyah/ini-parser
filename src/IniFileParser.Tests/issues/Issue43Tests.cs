using IniParser.Parser;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue43Tests
    {
        private string initest =
@"#comment1
[seccion1] 
#comment 2

#control value
value1 = 10.6
value2 = 10";

        [Test, Description("Test for Issue 43 backward compatibility https://github.com/rickyah/ini-parser/issues/32")]
        public void commentchar_property_works()
        {
            var parser = new IniDataParser();

            parser.Configuration.CommentChar = '#';

            var result = parser.Parse(initest);
            Assert.That(result.Sections.GetSectionData("seccion1").Comments.Count > 0);

        }
    }

}
