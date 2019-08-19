using NUnit.Framework;
using System;

using IniParser.Parser;
using IniParser.Model;

namespace IniFileParser.Tests.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class INIDataTests
    {
        [Test]
        public void delete_all_comments()
        {
            string iniData = @";comment1
key1 = 2
;comment2
[section1]

;a value
value1 = 10.6";

            var data = new IniDataParser().Parse(iniData);

            Assert.That(data.Global.GetKeyData("key1").Comments, Is.Not.Empty);
            Assert.That(data.Sections.GetSectionData("section1").Comments, Is.Not.Empty);
            Assert.That(data["section1"].GetKeyData("value1").Comments, Is.Not.Empty);


            data.ClearAllComments();

            Assert.That(data.Global.GetKeyData("key1").Comments, Is.Empty);
            Assert.That(data.Sections.GetSectionData("section1").Comments, Is.Empty);
            Assert.That(data["section1"].GetKeyData("value1").Comments, Is.Empty);

        }

        [Test, Description("Test for Issue 7: http://code.google.com/p/ini-parser/issues/detail?id=7")]
        public void check_add_keydata_method_using_key_and_value_strings()
        {
            var newData = new IniData();

            newData.Sections.AddSection("newSection");
            newData["newSection"].AddKeyAndValue("newKey1", "value1");

            Assert.That(newData["newSection"]["newKey1"], Is.EqualTo("value1"));
        }

      [Test, Description("Tests preconditions for TryGetKey")]
      public void TryGetKey_preconditions()
      {
        var data = new IniDataParser().Parse("");
        var separator = data.SectionKeySeparator;
        string result;
        
        // ensure that various good keys aren't rejected
        var goodKey = "";
        Assert.DoesNotThrow(() => data.TryGetKey(goodKey, out result));
        goodKey = "key";
        Assert.DoesNotThrow(() => data.TryGetKey(goodKey, out result));
        goodKey = string.Format("section{0}key", separator);
        Assert.DoesNotThrow(() => data.TryGetKey(goodKey, out result));

        // should be rejected
        var badKey = string.Format("section{0}subsection{0}key", separator);
        Assert.Throws<ArgumentException>(() => data.TryGetKey(badKey, out result));
      }

      [Test, Description("Tests retrieving data with TryGetKey")]
      public void TryGetKey_data_retrieval()
      {
        var input = @"
global = 1
[section1]
key1 = 2

[section1\subsection]
key2 = 3
";
        var data = new IniDataParser().Parse(input);
        var separator = data.SectionKeySeparator;
        string key;
        string result;

        // keys should all be retrieved
        Assert.IsTrue(data.TryGetKey("global", out result));
        Assert.AreEqual(result, "1");
        
        key = string.Format("section1{0}key1", separator);
        Assert.IsTrue(data.TryGetKey(key, out result));
        Assert.AreEqual(result, "2");
        
        key = string.Format(@"section1\subsection{0}key2", separator);
        Assert.IsTrue(data.TryGetKey(key, out result));
        Assert.AreEqual(result, "3");

        // invalid keys should fail...
        Assert.IsFalse(data.TryGetKey(null, out result));
        Assert.That(result, Is.Empty);

        Assert.IsFalse(data.TryGetKey("", out result));
        Assert.That(result, Is.Empty);
        
        Assert.IsFalse(data.TryGetKey("badglobal", out result));
        Assert.That(result, Is.Empty);
        
        key = string.Format("badsection{0}badkey", separator);
        Assert.IsFalse(data.TryGetKey(key, out result));
        Assert.That(result, Is.Empty);
      }

      // GetKey shares preconditions with TryGetKey, so tests are not duplicated
      [Test, Description("Tests retrieving data with GetKey")]
      public void GeyKey_data_retrieval()
      {
        var input = @"
global = 1
[section]
key = 2
";
        var data = new IniDataParser().Parse(input);
        var separator = data.SectionKeySeparator;
        string key;

        // should succeed
        key = "global";
        Assert.AreEqual(data.GetKey(key), "1");

        key = string.Format("section{0}key", separator);
        Assert.AreEqual(data.GetKey(key), "2");

        // should fail
        key = null;
        Assert.IsNull(data.GetKey(key));

        key = "badglobal";
        Assert.IsNull(data.GetKey(key));

        key = string.Format("badsection{0}badkey", separator);
        Assert.IsNull(data.GetKey(key));
      }

        [Test]
        public void check_deep_clone()
        {
            var input = @"
global = 1
[section]
key = 1
";
            var ori = new IniDataParser().Parse(input);

            var copy = (IniData)ori.Clone();

            copy.Global["global"] = "2";
            copy["section"]["key"] = "2";


            Assert.That(ori.Global["global"], Is.EqualTo("1"));
            Assert.That(ori["section"]["key"], Is.EqualTo("1"));


        }
    }
}

