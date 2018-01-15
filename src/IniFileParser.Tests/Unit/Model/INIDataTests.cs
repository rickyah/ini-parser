using System;
using IniParser.Model;
using IniParser.Model.Configuration;

// TODO change namespaces and keep consistency (see Unit Test explorer)
namespace IniParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class INIDataTests
    {
        [Test]
        public void delete_all_comments()
        {
            string iniData = @";comment1
key1 = 2
;comment2
[section1]

;a value
value1 = 10.6";

            var data = new IniDataParser().Parse(iniData);

            Assert.That(data.Global.GetKeyData("key1").Comments, Is.Not.Empty);
            Assert.That(data.Sections.GetSectionData("section1").Comments, Is.Not.Empty);
            Assert.That(data["section1"].GetKeyData("value1").Comments, Is.Not.Empty);


            data.ClearAllComments();

            Assert.That(data.Global.GetKeyData("key1").Comments, Is.Empty);
            Assert.That(data.Sections.GetSectionData("section1").Comments, Is.Empty);
            Assert.That(data["section1"].GetKeyData("value1").Comments, Is.Empty);

        }

        [Test, Description("Test for Issue 7: http://code.google.com/p/ini-parser/issues/detail?id=7")]
        public void check_add_keydata_method_using_key_and_value_strings()
        {
            var newData = new IniData();

            newData.Sections.AddSection("newSection");
            newData["newSection"].AddKey("newKey1", "value1");

            Assert.That(newData["newSection"]["newKey1"], Is.EqualTo("value1"));
        }

        [Test, Description("Test for Issue 76: https://github.com/rickyah/ini-parser/issues/76")]
        public void resolve_case_insensitive_names()
        {

            var data = new IniDataCaseInsensitive();
            var section = new SectionData("TestSection");
            section.Keys.AddKey("keY1", "value1");
            section.Keys.AddKey("KEY2", "value2");
            section.Keys.AddKey("KeY2", "value3");

            data.Sections.Add(section);

            Assert.That(data.Sections.ContainsSection("testsection"));
            Assert.That(data.Sections.ContainsSection("TestSection"));
            Assert.That(data["TestSection"]["key1"], Is.EqualTo("value1"));
            Assert.That(data["TestSection"]["keY1"], Is.EqualTo("value1"));
            Assert.That(data["TestSection"]["KEY2"], Is.EqualTo("value3"));
            Assert.That(data["TestSection"]["KeY2"], Is.EqualTo("value3"));
            Assert.That(data["TestSection"]["key2"], Is.EqualTo("value3"));
        }

        [Test]
        public void parse_case_insensitive_names_ini_file()
        {
            string iniData = @"[TestSection]
            KEY1 = value1
            KEY2 = value2";

            var config = new IniParserConfiguration(new IniScheme() );
            config.CaseInsensitive = true;
            var data = new IniDataParser(config).Parse(iniData);

            Assert.That(data["testsection"]["key1"], Is.EqualTo("value1"));
            Assert.That(data["testSection"]["Key2"], Is.EqualTo("value2"));

        }

        [Test, Description("Test for Issue 135: https://github.com/rickyah/ini-parser/issues/135")]
        public void resolve_case_insensitive_names_in_global()
        {

            var data = new IniDataCaseInsensitive();
            data.Global.AddKey("keY1", "value1");
            data.Global.AddKey("KEY2", "value2");
            data.Global["KeY2"] = "value3";

            Assert.That(data.Global["key1"], Is.EqualTo("value1"));
            Assert.That(data.Global["keY1"], Is.EqualTo("value1"));
            Assert.That(data.Global["KEY2"], Is.EqualTo("value3"));
            Assert.That(data.Global["KeY2"], Is.EqualTo("value3"));
            Assert.That(data.Global["key2"], Is.EqualTo("value3"));
        }

        [Test]
        public void check_deep_clone()
        {
            var input = @"
global = 1
[section]
key = 1
";
            var ori = new IniDataParser().Parse(input);

            var copy = (IniData)ori.Clone();

            copy.Global["global"] = "2";
            copy["section"]["key"] = "2";


            Assert.That(ori.Global["global"], Is.EqualTo("1"));
            Assert.That(ori["section"]["key"], Is.EqualTo("1"));


        }

        [Test]
        public void merge_programatically_created_ini_files()
        {
            var iniData = new IniData();
            iniData.Global.AddKey("UseSeparateRepositoryForAssets", true.ToString());

            iniData.Sections.AddSection("MainRepository");
            iniData["MainRepository"]["Type"] = "git";
            iniData["MainRepository"]["RelativePath"] = ".";

            Assert.That(iniData["MainRepository"].ContainsKey("Type"));
            Assert.That(iniData["MainRepository"].ContainsKey("RelativePath"));

            iniData.Sections.AddSection("AssetsRepository");
            iniData["AssetsRepository"]["Type"] = "svn";
            iniData["AssetsRepository"]["RelativePath"] = "./Assets";

            Assert.That(iniData["AssetsRepository"].ContainsKey("Type"));
            Assert.That(iniData["AssetsRepository"].ContainsKey("RelativePath"));

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

                var s0 = dataA.Sections.GetSectionData("s0");

                Assert.That(s0, Is.Not.Null);
                Assert.That(s0.SectionName, Is.EqualTo("s0"));
                Assert.That(s0.Keys["a"], Is.EqualTo("22"));
                Assert.That(s0.Keys["b"], Is.EqualTo("44"));

                var s1 = dataA.Sections.GetSectionData("s1");

                Assert.That(s1, Is.Not.Null);
                Assert.That(s1.SectionName, Is.EqualTo("s1"));
                Assert.That(s1.Keys["a"], Is.EqualTo("3"));
                Assert.That(s1.Keys["b"], Is.EqualTo("4"));

                var s2 = dataA.Sections.GetSectionData("s2");

                Assert.That(s2, Is.Not.Null);
                Assert.That(s2.SectionName, Is.EqualTo("s2"));
                Assert.That(s2.Keys["c"], Is.EqualTo("55"));
            }

        }

    }
}

