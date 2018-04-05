using System.IO;
using IniParser.Exceptions;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;

namespace IniParser.Tests.Unit
{
    [TestFixture, Category("File loading/saving tests")]
    public class IniParserFileTest
    {

        public IniDataParser iniParser;

        [SetUp]
        public void SetUp()
        {
            iniParser = new IniDataParser();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test, Description("Checks correct parsing of an empty INI file")]
        public void CheckParseEmptyFileSuccess()
        {
            IniData parsedData = iniParser.Parse("");

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks correct parsing of a well formed INI file")]
        public void CheckParseGoodFileSuccess()
        {
        string strGoodINIFile = @";comentario1
;comentario 2
[seccion1]

;valor de control
value1 = 10.6";

            IniData parsedData = iniParser.Parse(strGoodINIFile);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test, Description("Checks error when parsing a bad formed INI file")]
        public void CheckParsingFailure()
        {
            string strBadINIFile = @"asdfasf [seccion1] fdsafsd
   value2  =   jelou

[seccion 2]    adsfa
;
value3 = que tal estas

  [ [section   3] dsf a

 ; comentario1
 fsadfsad  ;comentario2";

            Assert.Throws<ParsingException>( () => iniParser.Parse(strBadINIFile) );
        }


        [Test, Description("Checks bad formed INI file: Two sections with same name")]
        public void CheckCollideSectionNames()
        {
            string strBadSectionINI = @";comentario1
[seccion1] ;comentario 2

;valor de control
value1 = 10.6

[seccion1]
value2 = 10.6";
            Assert.Throws<ParsingException>( () => iniParser.Parse(strBadSectionINI) );
        }

        [Test, Description("Checks bad formed INI file: Two keys in the same section with same name")]
        public void CheckCollideKeysNames()
        {
string strBadKeysINIFile = @";comentario1
[seccion1] ;comentario 2

;valor de control
value1 = 10.6
value1 = 10";
            Assert.Throws<ParsingException>( () => iniParser.Parse(strBadKeysINIFile) );

        }
    }
}
