using System.Collections.Generic;
using IniParser.Model;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Model
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
            SectionDataCollection sdc = new SectionDataCollection();
            Assert.That(sdc, Is.Empty);

            //Add sectoin
            sdc.AddSection(strSectionTest);
            sdc.AddSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(1));


            //Check access
            Assert.That(sdc.GetSectionData(strSectionTest), Is.Not.Null);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments, Is.Empty);
            Assert.That(sdc.GetSectionData(strSectionTest).Keys.Count, Is.EqualTo(0));

            //Check add coments
            sdc.GetSectionData(strSectionTest).Comments.Add(strComment);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(1));
            sdc.GetSectionData(strSectionTest).Comments.Clear();
            sdc.GetSectionData(strSectionTest).Comments.AddRange(commentListTest);

            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(commentListTest.Count));


            //Remove section
            sdc.RemoveSection("asdf");
            Assert.That(sdc.Count, Is.EqualTo(1));

            sdc.RemoveSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(0));

            //Check access
            Assert.That(sdc[strSectionTest], Is.Null);
        }

        [Test, Description("Test for Issue 29: http://code.google.com/p/ini-parser/issues/detail?id=29")]
        public void remove_all_keys_in_section_without_deleting_the_section()
        {
            IniData data = new IniData();
            data.Sections.AddSection("test");
            data.Sections.AddSection("test2");

            data["test"].AddKey("key1", "value1");
            data["test"].AddKey("key2", "value2");

            data["test2"].AddKey("key3", "value3");
            data["test2"].AddKey("key4", "value4");

            Assert.That(data["test"].ContainsKey("key1"));
            Assert.That(data["test"].ContainsKey("key2"));
            Assert.That(data["test2"].ContainsKey("key3"));
            Assert.That(data["test2"].ContainsKey("key4"));

            data.Sections.GetSectionData("test").ClearKeyData();
            Assert.That(data.Sections.ContainsSection("test"));
            Assert.That(data["test"].ContainsKey("key1"), Is.False);
            Assert.That(data["test"].ContainsKey("key2"), Is.False);

            data["test2"].RemoveAllKeys();
            Assert.That(data.Sections.ContainsSection("test2"));
            Assert.That(data["test2"].ContainsKey("key3"), Is.False);
            Assert.That(data["test2"].ContainsKey("key4"), Is.False);

        }

        [Test]
        public void check_adding_sections_to_collection()
        {
            var col = new SectionDataCollection();

            var exampleSection = new SectionData("section1");
            exampleSection.Keys.AddKey("examplekey");
            exampleSection.Keys["examplekey"] = "examplevalue";

            col.Add(exampleSection);

            Assert.That(col["section1"], Is.Not.Null);

            // Add sections directly to the collection
            Assert.That(col.AddSection("section2"), Is.True);
            Assert.That(col.AddSection("section2"), Is.False);

            Assert.That(col["section2"], Is.Not.Null);
        }

        [Test]
        public void check_deep_clone()
        {
            var ori = new SectionDataCollection();
            ori.AddSection("section1");
            ori["section1"]["key1"] = "value1";

            var copy = (SectionDataCollection)ori.Clone();
            copy["section1"]["key1"] = "value2";

            Assert.That(ori["section1"]["key1"], Is.EqualTo("value1"));
        }
    }
}
