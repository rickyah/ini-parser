using System;
using IniParser;
using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue6Tests
    {
        /// <summary>
        ///     Thanks to h.eriksson@artamir.org for the issue.
        /// </summary>
        [Test, Description("Test for Issue 6: http://code.google.com/p/ini-parser/issues/detail?id=6")]
        public void check_that_comment_char_is_not_stored_as_the_key()
        {
            string data = "[data]" + Environment.NewLine + "key = value;";

            IniData inidata = new StringIniParser().ParseString(data);

            Assert.That(inidata["data"]["key"], Is.EqualTo("value;"));
        }
    }
}
