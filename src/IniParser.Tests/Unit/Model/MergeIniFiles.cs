using System;
using IniParser;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Model
{
    [TestFixture]
    public class MergeIniFiles
    {
        [Test]
        public void merge_programatically_created_ini_files()
        {
			var iniData = new IniData();
			iniData.Global.Add("UseSeparateRepositoryForAssets", true.ToString());

			iniData.Sections.Add("MainRepository");
			iniData["MainRepository"]["Type"] = "git";
			iniData["MainRepository"]["RelativePath"] = ".";

			Assert.That(iniData["MainRepository"].Contains("Type"));
			Assert.That(iniData["MainRepository"].Contains("RelativePath"));

			iniData.Sections.Add("AssetsRepository");
			iniData["AssetsRepository"]["Type"] = "svn";
			iniData["AssetsRepository"]["RelativePath"] = "./Assets";

			Assert.That(iniData["AssetsRepository"].Contains("Type"));
			Assert.That(iniData["AssetsRepository"].Contains("RelativePath"));

			Console.WriteLine(iniData.ToString());
        }

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
        public void merge_parsed_ini_files()
        {
            var parser = new IniDataParser();

            IniData dataA = parser.Parse(iniFileStrA);

            IniData dataB = parser.Parse(iniFileStrB);

            dataA.Merge(dataB);

            {
                // merged files
                Assert.That(dataA.Global["g"], Is.EqualTo("11"));
                Assert.That(dataA.Global["j"], Is.EqualTo("a"));
                Assert.That(dataA.Sections.Count, Is.EqualTo(3), "Expected two (3) sections");

                var s0 = dataA.Sections.FindByName("s0");

                Assert.That(s0, Is.Not.Null);
                Assert.That(s0.Name, Is.EqualTo("s0"));
                Assert.That(s0.Properties["a"], Is.EqualTo("22"));
                Assert.That(s0.Properties["b"], Is.EqualTo("44"));

                var s1 = dataA.Sections.FindByName("s1");

                Assert.That(s1, Is.Not.Null);
                Assert.That(s1.Name, Is.EqualTo("s1"));
                Assert.That(s1.Properties["a"], Is.EqualTo("3"));
                Assert.That(s1.Properties["b"], Is.EqualTo("4"));

                var s2 = dataA.Sections.FindByName("s2");

                Assert.That(s2, Is.Not.Null);
                Assert.That(s2.Name, Is.EqualTo("s2"));
                Assert.That(s2.Properties["c"], Is.EqualTo("55"));
            }

        }

    }
}
