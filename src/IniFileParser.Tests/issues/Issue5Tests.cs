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
    public class Issue5Tests
    {
        /// <summary>
        ///     Thanks to h.eriksson@artamir.org for the issue.
        /// </summary>
        [Test, Description("Test for Issue 5: http://code.google.com/p/ini-parser/issues/detail?id=5")]
        public void correct_comment_assigment_to_keydata()
        {
            IniData inidata = new IniData();
            inidata.Sections.AddSection("TestSection");

            KeyData key = new KeyData("TestKey");
            key.Value = "TestValue";
            key.Comments.Add("This is a comment");
            inidata["TestSection"].SetKeyData(key);

            Assert.That(inidata["TestSection"].GetKeyData("TestKey").Comments[0], Is.EqualTo("This is a comment"));
        }
    }
}
