using System;
using System.Collections.Generic;
using System.Text;
using IniParser;
using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue9Tests
    {
        [Test, Description("Test for Issue 9: http://code.google.com/p/ini-parser/issues/detail?id=9")]
        public void check_using_another_leading_character_for_comments()
        {
            string data = 
@"[test]
# a comment
connectionString = Server=sqlserver.domain.com;Database=main;User ID=user;Password=password";


            StringIniParser parser = new StringIniParser();
            parser.Parser.Configuration.CommentChar = '#';
            IniData iniData = parser.ParseString(data);

            Assert.That(
                iniData["test"]["connectionString"],
                Is.EqualTo("Server=sqlserver.domain.com;Database=main;User ID=user;Password=password"));

            Assert.That(
                iniData["test"].GetKeyData("connectionString").Comments[0], Is.EqualTo(" a comment"));
        }

    }
}
