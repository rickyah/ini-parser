using System;
using System.Collections.Generic;
using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParserTests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class KeyDataTests
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

        [Test, ExpectedException(typeof(ArgumentException))]
        public void create_key_with_invalid_name()
        {
            new KeyData("");
            Assert.Fail("I shouldn't be able to create a section with an empty section name");
        }

        [Test]
        public void creating_keydata_programatically()
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
            Assert.That(kd.Comments, Has.Count(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));

        }

        [Test]
        public void check_clone_operation()
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
            Assert.That(kd.Comments, Has.Count(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));
        }
         
    }
}
