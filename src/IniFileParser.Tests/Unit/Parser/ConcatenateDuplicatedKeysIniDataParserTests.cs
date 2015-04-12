using System;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Parser
{
    [TestFixture]
    public class ConcatenateDuplicatedKeysIniDataParserTests
    {
        string iniFileStr =
            @"
key1 = hello
key1 = world
[section1]
;comment for key1
key1 = hello
key1 = world
";


        [Test]
        public void test_duplicated_keys_are_concatenated_in_same_section()
        {
            var iniData = new ConcatenateDuplicatedKeysIniDataParser().Parse(iniFileStr);

            Assert.That(iniData.Global["key1"], Is.EqualTo("hello;world"));
            Assert.That(iniData["section1"]["key1"], Is.EqualTo("hello;world"));
        }
    }

}
