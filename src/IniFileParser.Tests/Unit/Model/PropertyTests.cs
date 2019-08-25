using System;
using System.Collections.Generic;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class PropertyTest
    {
        [Test]
        public void check_default_values()
        {
            var property = new Property("key_name");

            Assert.That(property, Is.Not.Null);
            Assert.That(property.Key, Is.EqualTo("key_name"));
            Assert.That(property.Comments, Is.Empty);
            Assert.That(property.Value, Is.Empty);
        }

        [Test]
        public void check_create_property_with_invalid_name_throws()
        {
            Assert.Throws(typeof(ArgumentException), () => new Property(""));
        }

        [Test]
        public void check_creating_property_programatically()
        {
            var strValueTest = "Test String";
            var strKeyTest = "Mykey";
            var commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });

            //Create a key data
            Property property = new Property(strKeyTest);
            property.Value = strValueTest;
            property.Comments = commentListTest;
            
            //Assert not null and empty
            Assert.That(property, Is.Not.Null);
            Assert.That(property.Key, Is.EqualTo(strKeyTest));
            Assert.That(property.Value, Is.EqualTo(strValueTest));
            Assert.That(property.Comments, Has.Count.EqualTo(2));
            Assert.That(property.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(property.Comments[1], Is.EqualTo("testComment 2"));

        }

        [Test]
        public void check_deep_clone_property()
        {
            var strValueTest = "Test String";
            var strKeyTest = "Mykey";
            var commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });

            //Create a key data
            Property property = new Property(strKeyTest);
            property.Value = strValueTest;
            property.Comments = commentListTest;

            Property propertyCopy = property.DeepClone();

            //Assert not null and empty
            Assert.That(propertyCopy, Is.Not.Null);
            Assert.That(propertyCopy.Key, Is.EqualTo(strKeyTest));
            Assert.That(propertyCopy.Value, Is.EqualTo(strValueTest));
            Assert.That(propertyCopy.Comments, Has.Count.EqualTo(2));
            Assert.That(propertyCopy.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(propertyCopy.Comments[1], Is.EqualTo("testComment 2"));
        }

        [Test]
        public void check_merge_properties()
        {
            var properties1 = new PropertyCollection();
            properties1.Add( "key1", "value1");
            properties1.Add( "key2", "value2");
            properties1.Add( "key3", "value3");

            var properties2 = new PropertyCollection();
            properties2.Add("key1", "value11");
            properties2.Add("key4", "value4");

            properties1.Merge(properties2);

            Assert.That(properties1["key1"], Is.EqualTo("value11"));
            Assert.That(properties1["key2"], Is.EqualTo("value2"));
            Assert.That(properties1["key3"], Is.EqualTo("value3"));
            Assert.That(properties1["key4"], Is.EqualTo("value4"));
        }
    }
}
