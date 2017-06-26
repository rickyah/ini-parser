using NUnit.Framework;
using System.Linq;
using System.IO;
using IniParser.Parser;
using IniParser.Model.Configuration;
using System.Collections.Generic;

namespace Unit.Parser
{
    [TestFixture]
    public class ScannerTests
    {
        Scanner scanner;
        IniScheme scheme;

        [SetUp] public void Setup()
        {
            scanner = new Scanner();
            scheme = new IniScheme();
        }

        [Test] public void tokenize_empty_file()
        {
            string testIniFile = "";
            var tokens = TokenizeString(testIniFile, scheme);

            Assert.That(tokens, Is.Empty);
        }

        [Test] public void tokenize_comments_one_char()
        {
            string testIniFile =
    @"#section 1 comment
  [section1]
#first key comment
keyNumber = 1
keyString = hello world # second key comment
keyBool = true

#endfile comment";

            scheme.CommentString = "#";
            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.COMMENT);

            Assert.That(tokens.Count, Is.EqualTo(4));

            Assert.That(tokens[0].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[0].literal, Is.EqualTo("section 1 comment"));
            Assert.That(tokens[0].line, Is.EqualTo(1));

            Assert.That(tokens[1].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[1].literal, Is.EqualTo("first key comment"));
            Assert.That(tokens[1].line, Is.EqualTo(3));

            Assert.That(tokens[2].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[2].literal, Is.EqualTo(" second key comment"));
            Assert.That(tokens[2].line, Is.EqualTo(5));

            Assert.That(tokens[3].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[3].literal, Is.EqualTo("endfile comment"));
            Assert.That(tokens[3].line, Is.EqualTo(8));
        }

        [Test] public void tokenize_comments_multi_char()
        {
            string testIniFile =
@"##section 1 comment
[section1]
##   first key comment
#keyNumber = 1

keyString = hello world # not a comment";

            scheme.CommentString = "##";
            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.COMMENT);

            Assert.That(tokens.Count, Is.EqualTo(2));

            Assert.That(tokens[0].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[0].literal, Is.EqualTo("section 1 comment"));
            Assert.That(tokens[0].line, Is.EqualTo(1));

            Assert.That(tokens[1].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[1].literal, Is.EqualTo("   first key comment"));
            Assert.That(tokens[1].line, Is.EqualTo(3));
        }

        [Test] public void tokenize_sections_one_char()
        {
            string testIniFile =
@"#section 1 comment
[section1]
#   first key comment
#keyNumber = 1

keyString = hello world # not a comment";

            scheme.CommentString = "#";
            scheme.SectionStartString = "[";
            scheme.SectionEndString = "]";

            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.SECTION_OPEN,
                                           Token.TokenType.SECTION_CLOSE);

            Assert.That(tokens.Count, Is.EqualTo(2));

            Assert.That(tokens[0].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[0].literal, Is.Null);
            Assert.That(tokens[0].line, Is.EqualTo(2));

            Assert.That(tokens[1].type, Is.EqualTo(Token.TokenType.SECTION_CLOSE));
            Assert.That(tokens[1].literal, Is.Null);
            Assert.That(tokens[1].line, Is.EqualTo(2));

        }

        [Test] public void tokenize_sections_multi_char()
        {
            string testIniFile =
@"##section 1 comment
[<section1<]
##   first key comment
#keyNumber = 1

keyString = hello world # not a comment";

            scheme.SectionStartString = "[<";
            scheme.SectionEndString = "<]";

            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.SECTION_OPEN,
                                           Token.TokenType.SECTION_CLOSE);

            Assert.That(tokens.Count, Is.EqualTo(2));

            Assert.That(tokens[0].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[0].literal, Is.Null);
            Assert.That(tokens[0].line, Is.EqualTo(2));

            Assert.That(tokens[1].type, Is.EqualTo(Token.TokenType.SECTION_CLOSE));
            Assert.That(tokens[1].literal, Is.Null);
            Assert.That(tokens[1].line, Is.EqualTo(2));

        }

        [Test] public void tokenize_bad_formed_sections()
        {
            string testIniFile =
@"[section1]
[section 2]
[nosection
[section3]";

            scheme.CommentString = "#";
            scheme.SectionStartString = "[";
            scheme.SectionEndString = "]";

            var tokens = TokenizeString(testIniFile, scheme);

            Assert.That(tokens[0].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[1].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[2].type, Is.EqualTo(Token.TokenType.SECTION_CLOSE));
            Assert.That(tokens[3].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[4].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[5].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[6].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[7].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[8].type, Is.EqualTo(Token.TokenType.SECTION_CLOSE));
            Assert.That(tokens[9].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[10].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[11].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[12].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[13].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[14].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[15].type, Is.EqualTo(Token.TokenType.SECTION_CLOSE));
        }

        [Test] public void tokenize_whitespace()
        {
            string testIniFile =
            @"##section 1 comment

[<section1<]
##   first key comment
#keyNumber = 1


keyString = hello world # not a comment

";
            scheme.CommentString = "##";
            var allTokens = TokenizeString(testIniFile, scheme);
            var wsTokens = FilterByTokenType(allTokens,
                                             Token.TokenType.WHITESPACE);

            Assert.That(wsTokens.Count, Is.EqualTo(15));

            var nlTokens = FilterByTokenType(allTokens,
                                             Token.TokenType.NEWLINE);

            Assert.That(nlTokens.Count, Is.EqualTo(9));
            for (int i = 0; i < nlTokens.Count; ++i)
            {
                Assert.That(nlTokens[i].line, Is.EqualTo(i+1));
            }
        }

        [Test] public void tokenize_properties_one_char()
        {
                    string testIniFile =
    @"#section 1 comment
  [section1]
#first key comment
keyNumber = 1
keyString => hello world # second key comment
keyBool = true

#endfile comment";

            scheme.CommentString = "#";
            scheme.PropertyDelimiterString = "=";
            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.PROPERTY_DELIMITER);

            Assert.That(tokens.Count, Is.EqualTo(3));

            Assert.That(tokens[0].line, Is.EqualTo(4));
            Assert.That(tokens[1].line, Is.EqualTo(5));
            Assert.That(tokens[2].line, Is.EqualTo(6));

        }

        [Test] public void tokenize_properties_multi_char()
        {
                    string testIniFile =
    @"#section 1 comment
  [section1]
#first key comment
keyNumber => 1
keyString => hello world # second key comment
keyBool = true

#endfile comment";

            scheme.CommentString = "#";
            scheme.PropertyDelimiterString = "=>";
            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.PROPERTY_DELIMITER);

            Assert.That(tokens.Count, Is.EqualTo(2));

            Assert.That(tokens[0].line, Is.EqualTo(4));
            Assert.That(tokens[1].line, Is.EqualTo(5));

        }

        [Test]
        public void test_correct_new_lines()
        {
            string testIniFile =
@"


";
            scheme.CommentString = "#";

            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.NEWLINE);

            Assert.That(tokens.Count, Is.EqualTo(3));

        }

        [Test] public void test_no_new_lines()
        {
            var testIniFile = @"#comment";

            var tokens = FilterByTokenType(TokenizeString(testIniFile, scheme),
                                           Token.TokenType.NEWLINE);

            Assert.That(tokens.Count, Is.EqualTo(0));
        }

        [Test] public void test_full_tokenization_on_valid_ini()
        {
            string testIniFile =
@"#section 1 comment
   [section 1]     #  comment section
#first key comment
keyString = hello world # second key comment

  key Bool=  true";

            scheme.CommentString = "#";
            scheme.PropertyDelimiterString = "=";
            var tokens = TokenizeString(testIniFile, scheme);

            int idx = 0;
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.SECTION_OPEN));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.SECTION_CLOSE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.PROPERTY_DELIMITER));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.COMMENT));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.NEWLINE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.PROPERTY_DELIMITER));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.WHITESPACE));
            Assert.That(tokens[idx++].type, Is.EqualTo(Token.TokenType.VALUE));

            Assert.That(idx, Is.GreaterThanOrEqualTo(tokens.Count));
        }

        #region Helpers
        List<Token> FilterByTokenType(List<Token> list, params Token.TokenType[] types)
        {
            return list.Where(t => types.Contains(t.type)).ToList();
        }

        List<Token> TokenizeString(string testIniFile, IniScheme scheme)
        {
            return scanner.Tokenize(new StringReader(testIniFile), scheme);
        }
        #endregion

    }
}
