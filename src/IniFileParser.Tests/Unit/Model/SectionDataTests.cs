using System;

using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class SectionDataTests
    {
        [Test]
        public void check_default_values()
        {
            var sd = new SectionData("section_test");

            Assert.That(sd, Is.Not.Null);
            Assert.That(sd.SectionName, Is.EqualTo("section_test"));
            Assert.That(sd.LeadingComments, Is.Empty);
            Assert.That(sd.Keys, Is.Empty);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void create_section_with_invalid_name()
        {
            new SectionData("");
            Assert.Fail("I shouldn't be able to create a section with an empty section name");
        }

        [Test]
        public void change_section_name_with_invalid_name()
        {
            var sd = new SectionData("section_test");

            sd.SectionName = "";

            Assert.That(sd.SectionName, Is.EqualTo("section_test"));
        }

        [Test]
        public void change_section_name()
        {
            var sd = new SectionData("section_test");

            sd.SectionName = "section_test_2";

            Assert.That(sd, Is.Not.Null);
            Assert.That(sd.SectionName, Is.EqualTo("section_test_2"));
            Assert.That(sd.LeadingComments, Is.Empty);
            Assert.That(sd.Keys, Is.Empty);
        }

        [Test]
        public void add_keys_to_section()
        {
            string strKeyTest = "Mykey";
            string strValueTest = "My value";

            var sd = new SectionData("section_test");

            //Add key
            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);

            //Assign value
            sd.Keys.GetKeyData(strKeyTest).Value = strValueTest;
            Assert.That(sd.Keys.GetKeyData(strKeyTest).Value, Is.EqualTo(strValueTest));
        }

        [Test]
        public void try_adding_duplicated_keys_to_section()
        {
            string strKeyTest = "Mykey";

            var sd = new SectionData("section_test");

            //Add key
            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);

            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));

        }

        [Test]
        public void remove_key_from_section()
        {
            string strKeyTest = "Mykey";

            var sd = new SectionData("section_test");

            //Add key
            sd.Keys.AddKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);

            sd.Keys.RemoveKey(strKeyTest);
            Assert.That(sd.Keys.Count, Is.EqualTo(0));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.False);
        }

        [Test]
        public void try_removing_non_existing_key_from_section()
        {
            string strKeyTest = "Mykey";
            string strValueTest = "My value";

            var sd = new SectionData("section_test");

            //Add key
            sd.Keys.AddKey(strKeyTest);
            sd.Keys.RemoveKey("asdf");
            Assert.That(sd.Keys.Count, Is.EqualTo(1));
            Assert.That(sd.Keys.ContainsKey(strKeyTest), Is.True);
            Assert.That(sd.Keys.ContainsKey("asdf"), Is.False);
        }

        [Test]
        public void try_accessing_non_existing_key()
        {
            var sd = new SectionData("section_test");

            //Access invalid keydata
            Assert.That(sd.Keys["asdf"], Is.Null);
        }
    }
}
