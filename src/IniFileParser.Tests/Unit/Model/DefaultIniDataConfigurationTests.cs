using System;
using System.Collections.Generic;
using IniParser.Model;
using IniParser.Model.Configuration;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Model
{

	// TODO: Move to configuration tests
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class IniDataConfigurationTests
    {
        [Test]
        public void check_default_values()
        {
			var config = new IniParserConfiguration(new IniScheme());

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Scheme.CommentRegex, Is.Not.Null);
            Assert.That(config.Scheme.SectionRegex, Is.Not.Null);
            
        }

		//Todo: Move to configuration tests
        [Test]
        public void check_cloning()
        {
			IniParserConfiguration config1 = new IniParserConfiguration(new IniScheme());

            config1.AllowDuplicateKeys = true;
            config1.Scheme.CommentString = "/";

			Assert.That(config1.AllowDuplicateKeys, Is.True);
            Assert.That(config1.Scheme.CommentString, Is.EqualTo("/"));

			IniParserConfiguration config2 = config1.Clone();

            Assert.That(config2.AllowDuplicateKeys, Is.True);
            Assert.That(config2.Scheme.CommentString, Is.EqualTo("/"));

            config1.Scheme.CommentString = "#";
            Assert.That(config2.Scheme.CommentString, Is.EqualTo("/"));
        }
º
        [Test]
        public void create_key_with_invalid_name()
        {
            Assert.Throws<ArgumentException>(() => new KeyData(""));
        }

		// todo: move to KeyData tests
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
            Assert.That(kd.Comments, Has.Count.EqualTo(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));

        }

		// todo: move to KeyData tests
		[Test]
		public void check_clone_copies_data()
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


            kd.Value = "t";
            Assert.That(kd2.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Value, Is.EqualTo("t"));

        }
         
    }
}
