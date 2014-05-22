using System;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Parser
{
    [TestFixture]
    public class MergeMultipleFilesTest
    {
        string iniFileStrA =
@"
g = 1
[s0]
a = 2
[s1]
a = 3
b = 4
";

        string iniFileStrB = 
@"
g = 11
j = a
[s0]
a = 22
b = 44

[s2]
c = 55
";
        
        [Test]
        public void merge_multiple_inis()
        {
            var parser = new IniDataParser();
            parser.Configuration.OverrideDuplicateKeys = true;
            parser.Configuration.AllowDuplicateKeys = true;
            parser.Configuration.AllowDuplicateSections = true;

            IniData data = parser.Parse(iniFileStrA);

            Assert.That(data, Is.Not.Null);

            {
                // first file
                Assert.That(data.Global["g"], Is.EqualTo("1"));
                Assert.That(data.Sections.Count, Is.EqualTo(2), "Expected two (2) sections");

                var s0 = data.Sections.GetSectionData("s0");

                Assert.That(s0, Is.Not.Null);
                Assert.That(s0.SectionName, Is.EqualTo("s0"));
                Assert.That(s0.Keys["a"], Is.EqualTo("2"));

                var s1 = data.Sections.GetSectionData("s1");

                Assert.That(s1, Is.Not.Null);
                Assert.That(s1.SectionName, Is.EqualTo("s1"));
                Assert.That(s1.Keys["a"], Is.EqualTo("3"));
                Assert.That(s1.Keys["b"], Is.EqualTo("4"));
            }

            parser.ParseInto(iniFileStrB, data);

            {
                // merged files
                Assert.That(data.Global["g"], Is.EqualTo("11"));
                Assert.That(data.Global["j"], Is.EqualTo("a"));
                Assert.That(data.Sections.Count, Is.EqualTo(3), "Expected two (3) sections");

                var s0 = data.Sections.GetSectionData("s0");

                Assert.That(s0, Is.Not.Null);
                Assert.That(s0.SectionName, Is.EqualTo("s0"));
                Assert.That(s0.Keys["a"], Is.EqualTo("22"));
                Assert.That(s0.Keys["b"], Is.EqualTo("44"));

                var s1 = data.Sections.GetSectionData("s1");

                Assert.That(s1, Is.Not.Null);
                Assert.That(s1.SectionName, Is.EqualTo("s1"));
                Assert.That(s1.Keys["a"], Is.EqualTo("3"));
                Assert.That(s1.Keys["b"], Is.EqualTo("4"));

                var s2 = data.Sections.GetSectionData("s2");

                Assert.That(s2, Is.Not.Null);
                Assert.That(s2.SectionName, Is.EqualTo("s2"));
                Assert.That(s2.Keys["c"], Is.EqualTo("55"));
            }



        }
    }
}
