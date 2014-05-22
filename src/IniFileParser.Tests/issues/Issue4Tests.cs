using System;
using System.Collections.Generic;
using System.Text;
using IniParser;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
    [TestFixture]
    public class Issue4Tests
    {

        private string ini_duplicated_keys =
@";comentario1
[seccion1]
;comentario 2

;valor de control
value1 = 10.6
value1 = 10";


        [Test, Description("Test for Issue 4: http://code.google.com/p/ini-parser/issues/detail?id=4")]
        public void allow_duplicated_keys_in_section()
        {
            var parser = new IniDataParser();

            parser.Configuration.AllowDuplicateKeys = true;

            IniData data = parser.Parse(ini_duplicated_keys);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.GetSectionData("seccion1").Keys.Count, Is.EqualTo(1));
            Assert.That(data.Sections.GetSectionData("seccion1").Keys["value1"], Is.EqualTo("10.6"));
        }
    }
}
