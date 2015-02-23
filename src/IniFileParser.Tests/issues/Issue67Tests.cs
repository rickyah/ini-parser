using IniParser.Parser;
using NUnit.Framework;
using IniParser.Exceptions;

namespace IniFileParser.Tests.issues
{

    [TestFixture]
    public class Issue67Tests
    {
        IniDataParser parser;
        [SetUp]
        public void Setup()
        {
            parser = new IniDataParser();
        }
        // Thanks https://github.com/RichardSinden for this issue
        [Test ,Description("Test for Issue 67 - better errors")]
        public void provides_error_data()
        {

            parser.Configuration.ThrowExceptionsOnError = true;

            try
            {
               parser.Parse(iniDataString);
            }
            catch(ParsingException ex)
            {
                Assert.That(ex.LineNumber, Is.EqualTo(5));
                Assert.That(parser.HasError, Is.True);
                Assert.That(parser.Errors, Has.Count.EqualTo(1));
            }
     
        }

        [Test ,Description("Test for Issue 67 - better errors")]
        public void provides_a_list_of_errors()
        {
            parser.Configuration.ThrowExceptionsOnError = false;
            parser.Configuration.AllowDuplicateKeys = false;

            var result = parser.Parse(iniDataString);

            Assert.That(result, Is.Null);
            Assert.That(parser.HasError, Is.True);
            Assert.That(parser.Errors, Has.Count.EqualTo(2));

        }
        const string iniDataString = @";begin

                            ;value
                            value1 = test
                                = test2
                            value1 = test3
                            ";
    }
}
