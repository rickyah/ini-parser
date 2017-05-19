using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using IniParser.Exceptions;

namespace IniFileParser.Tests
{

	[TestFixture]
	public class ReferenceParserTests
	{

		string iniFileStr =
	@";comment for section1
[section1]

;comment for key1
key1 = value1
key2 = ${section1.key1}

[section2]

;comment for myKey1
mykey1 = ${section1.key1}
mykey2 = ${section1.key2}
";
		
		[Test]
		public void parse_ini_string_with_variables()
		{
			var parser = new IniDataParser();
			parser.Configuration.VariableSubstitution = true;
			IniData data = parser.Parse(iniFileStr);

			Assert.That(data, Is.Not.Null);
			Assert.That(data.Sections.Count, Is.EqualTo(2));

			var section1 = data.Sections.GetSectionData("section1");

			Assert.That(section1, Is.Not.Null);
			Assert.That(section1.SectionName, Is.EqualTo("section1"));
			Assert.That(section1.Comments, Is.Not.Empty);
			Assert.That(section1.Comments.Count, Is.EqualTo(1));

			Assert.That(section1.Keys, Is.Not.Null);
			Assert.That(section1.Keys.Count, Is.EqualTo(2));
			Assert.That(data.GetKey("section1.key1"), Is.Not.Null);
			Assert.That(data.GetKey("section1.key1"), Is.EqualTo("value1"));
			Assert.That(data.GetKey("section1.key2"), Is.Not.Null);
			Assert.That(data.GetKey("section1.key2"), Is.EqualTo("value1"));

			var section2 = data.Sections.GetSectionData("section2");

			Assert.That(data.GetKey("section2.mykey1"), Is.Not.Null);
			Assert.That(data.GetKey("section2.mykey1"), Is.EqualTo("value1"));
			Assert.That(data.GetKey("section2.mykey2"), Is.Not.Null);
			Assert.That(data.GetKey("section2.mykey2"), Is.EqualTo("value1"));
		}


	}
}
