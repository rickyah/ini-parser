using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniFileParser.Tests.issues
{
	[TestFixture]
	public class Issue29Tests
	{
		[Test, Description("Test for Issue 29: http://code.google.com/p/ini-parser/issues/detail?id=29")]
        public void remove_all_keys_in_section_without_deleting_the_section()
		{
			IniData data = new IniData();
			data.Sections.AddSection("test");
			data.Sections.AddSection("test2");

			data["test"].AddKey("key1", "value1");
			data["test"].AddKey("key2", "value2");

			data["test2"].AddKey("key3", "value3");
			data["test2"].AddKey("key4", "value4");

			Assert.That (data["test"].ContainsKey("key1"));
			Assert.That (data["test"].ContainsKey("key2"));
			Assert.That (data["test2"].ContainsKey("key3"));
			Assert.That (data["test2"].ContainsKey("key4"));

			data.Sections.GetSectionData("test").ClearKeyData();
			Assert.That (data.Sections.ContainsSection("test"));
			Assert.That (data["test"].ContainsKey("key1"), Is.False);
			Assert.That (data["test"].ContainsKey("key2"), Is.False);

			data["test2"].RemoveAllKeys();
			Assert.That (data.Sections.ContainsSection("test2"));
			Assert.That (data["test2"].ContainsKey("key3"), Is.False);
			Assert.That (data["test2"].ContainsKey("key4"), Is.False);

		}

	}
}

