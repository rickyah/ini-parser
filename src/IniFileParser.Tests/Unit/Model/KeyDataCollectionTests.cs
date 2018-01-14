using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class KeyDataCollectionTests
    {
        [Test]
        public void test()
        {
            var col = new KeyDataCollection();
            col.AddKey("key1");

            Assert.That(col["key1"], Is.Empty);


            col.AddKey("key2", "value2");

            Assert.That(col["key2"], Is.EqualTo("value2"));

            var keyData = new KeyData("key3");
            keyData.Value = "value3";
            col.AddKey(keyData);

            Assert.That(col["key3"], Is.EqualTo("value3"));
        }

        [Test]
        public void check_deep_clone()
        {
            var ori = new KeyDataCollection();

            ori.AddKey("key1", "value1");

            var copy = (KeyDataCollection)ori.Clone();

            copy["key1"] = "Value2";

            Assert.That(ori["key1"], Is.EqualTo("value1"));

        }
    }
}
