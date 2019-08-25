using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class SectionDataCollectionTests
    {
        [Test]
        public void check_section_data_operations()
        {
            string strSectionTest = "MySection";
            string strComment = "comment";
            List<string> commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });


            //Creation
            SectionCollection sdc = new SectionCollection();
            Assert.That(sdc, Is.Empty);

            //Add sectoin
            sdc.Add(strSectionTest);
            sdc.Add(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(1));


            //Check access
            Assert.That(sdc.FindByName(strSectionTest), Is.Not.Null);
            Assert.That(sdc.FindByName(strSectionTest).Comments, Is.Empty);
            Assert.That(sdc.FindByName(strSectionTest).Properties.Count, Is.EqualTo(0));

            //Check add coments
            sdc.FindByName(strSectionTest).Comments.Add(strComment);
            Assert.That(sdc.FindByName(strSectionTest).Comments.Count, Is.EqualTo(1));
            sdc.FindByName(strSectionTest).Comments.Clear();
            sdc.FindByName(strSectionTest).Comments.AddRange(commentListTest);

            Assert.That(sdc.FindByName(strSectionTest).Comments.Count, Is.EqualTo(commentListTest.Count));


            //Remove section
            sdc.Remove("asdf");
            Assert.That(sdc.Count, Is.EqualTo(1));

            sdc.Remove(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(0));

            //Check access
            Assert.That(sdc[strSectionTest], Is.Null);
        }

        [Test]
        public void check_remove_all_keys_in_section_without_deleting_the_section()
        {
            IniData data = new IniData();
            data.Sections.Add("test");
            data.Sections.Add("test2");

            data["test"].Add("key1", "value1");
            data["test"].Add("key2", "value2");

            data["test2"].Add("key3", "value3");
            data["test2"].Add("key4", "value4");

            Assert.That(data["test"].Contains("key1"));
            Assert.That(data["test"].Contains("key2"));
            Assert.That(data["test2"].Contains("key3"));
            Assert.That(data["test2"].Contains("key4"));

            data.Sections.FindByName("test").ClearProperties();
            Assert.That(data.Sections.Contains("test"));
            Assert.That(data["test"].Contains("key1"), Is.False);
            Assert.That(data["test"].Contains("key2"), Is.False);

            data["test2"].Clear();
            Assert.That(data.Sections.Contains("test2"));
            Assert.That(data["test2"].Contains("key3"), Is.False);
            Assert.That(data["test2"].Contains("key4"), Is.False);

        }

        [Test]
        public void check_adding_sections_to_collection()
        {
            var col = new SectionCollection();

            var exampleSection = new Section("section1");
            exampleSection.Properties.Add("examplekey");
            exampleSection.Properties["examplekey"] = "examplevalue";

            col.Add(exampleSection);

            Assert.That(col["section1"], Is.Not.Null);

            // Add sections directly to the collection
            Assert.That(col.Add("section2"), Is.True);
            Assert.That(col.Add("section2"), Is.False);

            Assert.That(col["section2"], Is.Not.Null);
        }

        [Test]
        public void check_deep_clone()
        {
            var ori = new SectionCollection();
            ori.Add("section1");
            ori["section1"]["key1"] = "value1";

            var copy = ori.DeepClone();
            copy["section1"]["key1"] = "value2";

            Assert.That(ori["section1"]["key1"], Is.EqualTo("value1"));
        }
    }
}
