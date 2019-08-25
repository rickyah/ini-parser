using System;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class SectionDataTests
    {
        [Test]
        public void check_default_values()
        {
            var sd = new Section("section_test");

            Assert.That(sd, Is.Not.Null);
            Assert.That(sd.Name, Is.EqualTo("section_test"));
            Assert.That(sd.Comments, Is.Empty);
            Assert.That(sd.Properties, Is.Empty);
        }

        [Test]
        public void check_create_section_with_invalid_name_throws()
        {
            Assert.Throws(typeof(ArgumentException), () => new Section(""));
        }

        [Test]
        public void check_cannot_change_section_name_with_invalid_name()
        {
            var sd = new Section("section_test");

            sd.Name = "";

            Assert.That(sd.Name, Is.EqualTo("section_test"));
        }

        [Test]
        public void check_change_section_name()
        {
            var sd = new Section("section_test");

            sd.Name = "section_test_2";

            Assert.That(sd, Is.Not.Null);
            Assert.That(sd.Name, Is.EqualTo("section_test_2"));
            Assert.That(sd.Comments, Is.Empty);
            Assert.That(sd.Properties, Is.Empty);
        }

        [Test]
        public void check_adding_keys_to_section()
        {
            string strKeyTest = "Mykey";
            string strValueTest = "My value";

            var sd = new Section("section_test");

            //Add key
            sd.Properties.Add(strKeyTest);
            Assert.That(sd.Properties.Count, Is.EqualTo(1));
            Assert.That(sd.Properties.Contains(strKeyTest), Is.True);

            //Assign value
            sd.Properties.FindByKey(strKeyTest).Value = strValueTest;
            Assert.That(sd.Properties.FindByKey(strKeyTest).Value, Is.EqualTo(strValueTest));
        }

        [Test]
        public void try_adding_duplicated_keys_to_section()
        {
            string strKeyTest = "Mykey";

            var sd = new Section("section_test");

            //Add key
            sd.Properties.Add(strKeyTest);
            Assert.That(sd.Properties.Count, Is.EqualTo(1));
            Assert.That(sd.Properties.Contains(strKeyTest), Is.True);

            sd.Properties.Add(strKeyTest);
            Assert.That(sd.Properties.Count, Is.EqualTo(1));

        }

        [Test]
        public void check_removing_key_from_section()
        {
            string strKeyTest = "Mykey";

            var sd = new Section("section_test");

            //Add key
            sd.Properties.Add(strKeyTest);
            Assert.That(sd.Properties.Count, Is.EqualTo(1));
            Assert.That(sd.Properties.Contains(strKeyTest), Is.True);

            sd.Properties.Remove(strKeyTest);
            Assert.That(sd.Properties.Count, Is.EqualTo(0));
            Assert.That(sd.Properties.Contains(strKeyTest), Is.False);
        }

        [Test]
        public void try_removing_non_existing_key_from_section()
        {
            string strKeyTest = "Mykey";

            var sd = new Section("section_test");

            //Add key
            sd.Properties.Add(strKeyTest);
            sd.Properties.Remove("asdf");
            Assert.That(sd.Properties.Count, Is.EqualTo(1));
            Assert.That(sd.Properties.Contains(strKeyTest), Is.True);
            Assert.That(sd.Properties.Contains("asdf"), Is.False);
        }

        [Test]
        public void try_accessing_non_existing_key()
        {
            var sd = new Section("section_test");

            //Access invalid keydata
            Assert.That(sd.Properties["asdf"], Is.Null);
        }

        [Test]
        public void check_merging_sections()
        {
            var destinySection = new Section("destiny_section");
            var newSection = new Section("new_section");

            //Add key
            destinySection.Properties.Add("key1", "value1");
            destinySection.Properties.Add("key2", "value2");

            newSection.Properties.Add("key2", "newvalue2");
            newSection.Properties.Add("key3", "value3");

            destinySection.Merge(newSection);

            Assert.That(destinySection.Properties["key1"], Is.EqualTo("value1"));
            Assert.That(destinySection.Properties["key2"], Is.EqualTo("newvalue2"));
            Assert.That(destinySection.Properties.Contains("key3"));
            Assert.That(destinySection.Properties["key3"], Is.EqualTo("value3"));
        }

        [Test]
        public void check_deep_clone()
        {
            var section = new Section("ori_section");
            section.Properties.Add("key1", "value1");
            section.Properties.Add("key2", "value2");

            var copy = section.DeepClone();

            copy.Properties["key1"] = "value3";
            copy.Properties["key2"] = "value4";

            Assert.That(section.Properties["key1"], Is.EqualTo("value1"));
            Assert.That(section.Properties["key2"], Is.EqualTo("value2"));

        }
    }
}
