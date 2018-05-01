using System;
using System.Collections.Generic;
using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit
{
    [TestFixture, Category("DataModel")]
    public class PropertyTests
    {
        [Test]
        public void check_default_values()
        {
            var kd = new KeyData("key_name");

            Assert.That(kd, Is.Not.Null);
            Assert.That(kd.KeyName, Is.EqualTo("key_name"));
            Assert.That(kd.Comments, Is.Empty);
            Assert.That(kd.Value, Is.Empty);
        }

        [Test]
        public void creating_key_with_invalid_name_throws_exception()
        {
            Assert.Throws<ArgumentException>( () => new KeyData("") );
        }

        [Test]
        public void can_create_property()
        {

            var strValueTest = "Test String";
            var strKeyTest = "Mykey";
            var commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });

            //Create a key data
            KeyData kd = new KeyData(strKeyTest);
            kd.Value = strValueTest;
            kd.Comments = commentListTest;

            //Assert not null and empty
            Assert.That(kd, Is.Not.Null);
            Assert.That(kd.KeyName, Is.EqualTo(strKeyTest));
            Assert.That(kd.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Comments, Has.Count.EqualTo(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));

        }

        [Test]
        public void can_merge_keys()
        {
            var keys1 = new KeyDataCollection();
            keys1.AddKey("key1", "value1");
            keys1.AddKey("key2", "value2");
            keys1.AddKey("key3", "value3");

            var keys2 = new KeyDataCollection();
            keys2.AddKey("key1", "value11");
            keys2.AddKey("key4", "value4");

            keys1.Merge(keys2);

            Assert.That(keys1["key1"], Is.EqualTo("value11"));
            Assert.That(keys1["key2"], Is.EqualTo("value2"));
            Assert.That(keys1["key3"], Is.EqualTo("value3"));
            Assert.That(keys1["key4"], Is.EqualTo("value4"));
        }

        [Test]
        public void can_assign_comment_to_property()
        {
            IniData inidata = new IniData();
            inidata.Sections.AddSection("TestSection");

            KeyData key = new KeyData("TestKey");
            key.Value = "TestValue";
            key.Comments.Add("This is a comment");
            inidata["TestSection"].SetKeyData(key);

            Assert.That(inidata["TestSection"].GetKeyData("TestKey").Comments[0], Is.EqualTo("This is a comment"));
        }

        [Test]
        public void check_deep_clone()
        {
            var strValueTest = "Test String";
            var strKeyTest = "Mykey";
            var commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });

            //Create a key data
            KeyData kd2 = new KeyData(strKeyTest);
            kd2.Value = strValueTest;
            kd2.Comments = commentListTest;

            KeyData kd = kd2.Clone() as KeyData;

            //Assert not null and empty
            Assert.That(kd, Is.Not.Null);
            Assert.That(kd.KeyName, Is.EqualTo(strKeyTest));
            Assert.That(kd.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Comments, Has.Count.EqualTo(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));
            
            // Data was copied
            kd.Value = "t";
            Assert.That(kd2.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Value, Is.EqualTo("t"));
        }
    }
}
