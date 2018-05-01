using System;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit
{
    [TestFixture, Category("DataModel")]
    public class SectionTests
    {
        [Test] 
        public void check_default_values()
        {
            var sd = new Section("section_test");

            Assert.That(sd, Is.Not.Null);
            Assert.That(sd.SectionName, Is.EqualTo("section_test"));
            Assert.That(sd.Comments, Is.Empty);
            Assert.That(sd.Keys, Is.Empty);
        }

        [Test] 
        public void creating_section_with_invalid_name_throws_exception()
        {
            Assert.Throws<ArgumentException>(() => new Section(""));
        }

        [Test] 
        public void cannot_change_section_name_to_invalid_name()
        {
            var sd = new Section("section_test");

            sd.SectionName = "";

            Assert.That(sd.SectionName, Is.EqualTo("section_test"));
        }

        [Test] 
        public void can_change_section_name()
        {
            var sd = new Section("section_test");

            sd.SectionName = "section_test_2";

            Assert.That(sd, Is.Not.Null);
            Assert.That(sd.SectionName, Is.EqualTo("section_test_2"));
            Assert.That(sd.Comments, Is.Empty);
            Assert.That(sd.Keys, Is.Empty);
        }

        [Test]
        public void can_add_keys_to_a_section()
        {
            string strKeyTest = "Mykey";

            var sd = new Section("section_test");

            Assert.That(sd.Keys.AddKey(strKeyTest), Is.True);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);
        }

        [Test] 
        public void can_change_value_from_keys_of_a_section()
        {
            string strKeyTest = "Mykey";
            string strValueTest = "My value";

            var sd = new Section("section_test");
            
            Assert.That(sd.Keys.AddKey(strKeyTest), Is.True);
            
            sd.Keys.GetKeyData(strKeyTest).Value = strValueTest;
            Assert.That(sd.Keys.GetKeyData(strKeyTest).Value, Is.EqualTo(strValueTest));
        }

        [Test] 
        public void adding_duplicated_keys_to_section_does_nothing()
        {
            string strKeyTest = "Mykey";

            var sd = new Section("section_test");

            //Add key
            Assert.That(sd.Keys.AddKey(strKeyTest), Is.True);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);

            Assert.That(sd.Keys.AddKey(strKeyTest), Is.False);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));

        }

        [Test]
        public void can_remove_key_from_a_section()
        {
            string strKeyTest = "Mykey";

            var sd = new Section("section_test");

            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.RemoveKey(strKeyTest), Is.True);
            
            Assert.That(sd.Keys.Count, Is.EqualTo(0));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.False);
        }
        
        [Test]
        public void can_remove_unexisting_key_from_a_section()
        {
            var sd = new Section("section_test");

            Assert.That(sd.Keys.RemoveKey("test"), Is.False);
        }

        [Test]
        public void accessing_non_existing_key_returns_null()
        {
            var sd = new Section("section_test");

            //Access invalid keydata
            Assert.That(sd.Keys["asdf"], Is.Null);
        }

        [Test]
        public void can_merge_sections()
        {
            var destinySection = new Section("destiny_section");
            var newSection = new Section("new_section");

            //Add key
            destinySection.Keys.AddKey("key1", "value1");
            destinySection.Keys.AddKey("key2", "value2");

            newSection.Keys.AddKey("key2", "newvalue2");
            newSection.Keys.AddKey("key3", "value3");

            destinySection.Merge(newSection);

            Assert.That(destinySection.Keys["key1"], Is.EqualTo("value1"));
            Assert.That(destinySection.Keys["key2"], Is.EqualTo("newvalue2"));
            Assert.That(destinySection.Keys.ContainsKey("key3"));
            Assert.That(destinySection.Keys["key3"], Is.EqualTo("value3"));
        }

        [Test]
        public void can_deep_clone()
        {
            var section = new Section("ori_section");
            section.Keys.AddKey("key1", "value1");
            section.Keys.AddKey("key2", "value2");

            var copy = section.DeepClone();

            copy.Keys["key1"] = "value3";
            copy.Keys["key2"] = "value4";

            Assert.That(section.Keys["key1"], Is.EqualTo("value1"));
            Assert.That(section.Keys["key2"], Is.EqualTo("value2"));

        }
    }
}
