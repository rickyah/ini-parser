using System;
using System.Collections.Generic;
using IniParser.Model;
using IniParser.Model.Configuration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class IniDataConfigurationTests
    {
        [Test]
        public void check_default_values()
        {
            var config = new DefaultIniParserConfiguration();

            Assert.That(config, Is.Not.Null);
            Assert.That(config.CommentRegex, Is.Not.Null);
            Assert.That(config.SectionRegex, Is.Not.Null);
            
        }

        [Test]
        public void check_cloning()
        {
            IIniParserConfiguration config1 = new DefaultIniParserConfiguration();

            config1.AllowDuplicateKeys = true;
            config1.CommentString = "/";

			Assert.That(config1.AllowDuplicateKeys, Is.True);
			Assert.That(config1.CommentString, Is.EqualTo("/"));

			IIniParserConfiguration config2 = config1.Clone();

            Assert.That(config2.AllowDuplicateKeys, Is.True);
            Assert.That(config2.CommentString, Is.EqualTo("/"));

            config1.CommentString = "#";
            Assert.That(config2.CommentString, Is.EqualTo("/"));
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
