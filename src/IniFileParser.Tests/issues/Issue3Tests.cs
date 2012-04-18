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
    public class Issue3Tests
    {
        [Test, Description("Test for Issue 3: http://code.google.com/p/ini-parser/issues/detail?id=3")]
        public void allow_keys_with_dots()
        {
            string strTest = "[section_issue.3]\nkey.with_dots = value\n";

            IniData data = new StringIniParser().ParseString(strTest);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["section_issue.3"]["key.with_dots"], Is.Not.Null);
        }
    }
}
