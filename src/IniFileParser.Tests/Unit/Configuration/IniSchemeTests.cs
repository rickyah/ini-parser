using NUnit.Framework;
using IniParser.Model.Configuration;

namespace IniFileParser.Tests
{
    [TestFixture]
    public class IniSchemeTests
    {        [Test]
        public static void check_cloning()
        {
            var scheme1 = new IniScheme();
            scheme1.CommentString = "/";
            Assert.That(scheme1.CommentString, Is.EqualTo("/"));

            var scheme2 = scheme1.DeepClone();
            Assert.That(scheme2.CommentString, Is.EqualTo("/"));
        }
    }
}
