using System.Collections.Generic;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit
{
    [TestFixture, Category("DataModel")]
    public class SectionCollectionTests
    {
        SectionCollection sdc;

        [SetUp]
        public void Setup()
        { 
            sdc = new SectionCollection(); 
        }
        
        [Test]
        public void can_add_a_section_to_collection_just_with_a_name()
        {
            string strSectionTest = "MySection";

            Assert.That(sdc.AddSection(strSectionTest), Is.True);
            Assert.That(sdc.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void can_add_a_section_to_collection()
        {
            var section = new Section("MySection");

            Assert.That(sdc.Add(section), Is.True);
            Assert.That(sdc.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void cannot_add_same_section_twice_to_collection()
        {
            string strSectionTest = "MySection";

            Assert.That(sdc.AddSection(strSectionTest), Is.True);
            Assert.That(sdc.AddSection(strSectionTest), Is.False);
            Assert.That(sdc.Count, Is.EqualTo(1));
        }

        [Test]
        public void can_access_section()
        {
            string strSectionTest = "MySection";
        
            
            sdc.AddSection(strSectionTest);
            
            //Check access
            Assert.That(sdc.GetSectionData(strSectionTest), Is.Not.Null);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments, Is.Empty);
            Assert.That(sdc.GetSectionData(strSectionTest).Keys.Count, Is.EqualTo(0));
        }

        [Test]
        public void can_add_comments()
        {
            string strSectionTest = "MySection";
            string strComment = "comment";
            var commentListTest = new List<string> { "testComment 1", "testComment 2" };

            Assert.That(sdc.AddSection(strSectionTest), Is.True);
            
            sdc.GetSectionData(strSectionTest).Comments.Add(strComment);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(1));

            sdc.GetSectionData(strSectionTest).Comments.Clear();
            sdc.GetSectionData(strSectionTest).Comments.AddRange(commentListTest);

            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(commentListTest.Count));
        }

        [Test]
        public void can_remove_sections()
        { 
            string strSectionTest = "MySection";

            sdc.AddSection(strSectionTest);
            sdc.RemoveSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(0));
            Assert.That(sdc[strSectionTest], Is.Null);
        }

        [Test]
        public void removing_non_existing_section_is_ok()
        {
            Assert.That(sdc.RemoveSection("asdf"), Is.False);
        }
        
        [Test]
        public void can_remove_all_properties_from_a_section()
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
        public void can_deep_clone()
        {
            var ori = new SectionCollection();
            ori.AddSection("section1");
            ori["section1"]["key1"] = "value1";

            var copy = (SectionCollection)ori.Clone();
            copy["section1"]["key1"] = "value2";

            Assert.That(ori["section1"]["key1"], Is.EqualTo("value1"));
        }
    }
}
