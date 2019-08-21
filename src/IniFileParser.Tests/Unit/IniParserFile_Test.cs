using IniParser;
using IniParser.Exceptions;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit
{
    [TestFixture, Category("File loading/saving tests")]
    public class IniParserFileTest
    {
        IniDataParser iniParser = new IniDataParser();

        [Test]
        public void CheckParseEmptyFileSuccess()
        {
            var ini = @" 
 
      ";
            IniData parsedData = iniParser.Parse(ini);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test]
        public void CheckParseGoodFileSuccess()
        {
            var ini = @";comentario1
;comentario 2
[seccion1]

;valor de control
value1 = 10.6";

            IniData parsedData = iniParser.Parse(ini);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test]
        
        public void CheckParsingFailure()
        {
            var ini = @"asdfasf [seccion1] fdsafsd
   value2  =   jelou

[seccion 2]    adsfa
;
value3 = que tal estas

  [ [section   3] dsf a 
   
 ; comentario1
 fsadfsad  ;comentario2";
            Assert.Throws(typeof(ParsingException), () => iniParser.Parse(ini));
        }

        [Test]
        public void CheckCollideSectionNames()
        {
            var ini = @";comentario1
[seccion1] ;comentario 2

;valor de control
value1 = 10.6

[seccion1]
value2 = 10.6";

            iniParser.Configuration.SkipInvalidLines = false;

            Assert.Throws(typeof(ParsingException), () => iniParser.Parse(ini));
        }

        [Test]
        public void CheckCollideKeysNames()
        {
            var ini = @";comentario1
[seccion1] ;comentario 2

;valor de control
value1 = 10.6
value1 = 10";

            Assert.Throws(typeof(ParsingException), () => iniParser.Parse(ini));
        }
    }
}
