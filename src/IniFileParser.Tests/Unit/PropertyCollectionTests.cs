using IniParser.Model;
using NUnit.Framework;

namespace IniParser.Tests.Unit
{
    [TestFixture, Category("DataModel")]
    public class PropertyCollectionTests
    {
        [Test]
        public void can_create_a_property_with_a_string()
        {
            var col = new KeyDataCollection();
            col.AddKey("key1");
            col.AddKey("key2", "value2");

            Assert.That(col["key1"], Is.Empty);
            Assert.That(col["key2"], Is.EqualTo("value2"));

        }
        
        [Test]
        public void can_add_property()
        {
            var col = new KeyDataCollection();
            col.AddKey("key1");

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
