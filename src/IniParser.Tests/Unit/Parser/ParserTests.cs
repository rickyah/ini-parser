using IniParser;
using IniParser.Exceptions;
using IniParser.Configuration;
using NUnit.Framework;

namespace IniParser.Tests.Unit.Parser
{
    [TestFixture]
    public partial class ParserTests
    {

        string iniFileStr =
@";comment for section1
[section1]

;comment for key1
key1 = value1
key2 = value5

[section2]

;comment for myKey1
mykey1 = value1
";
   
        [Test]
        public void parse_ini_string_with_default_configuration()
        {
            var parser = new IniDataParser();
            IniData data = parser.Parse(iniFileStr);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.Count, Is.EqualTo(2));
            var section1 = data.Sections.FindByName("section1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.Name, Is.EqualTo("section1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));

            Assert.That(section1.Properties, Is.Not.Null);
            Assert.That(section1.Properties.Count, Is.EqualTo(2));
            Assert.That(section1.Properties.FindByKey("key1"), Is.Not.Null);
            Assert.That(section1.Properties["key1"], Is.EqualTo("value1"));
            Assert.That(section1.Properties.FindByKey("key2"), Is.Not.Null);
            Assert.That(section1.Properties["key2"], Is.EqualTo("value5"));
        }
         
        string iniFileStrCustom = 
@"#comment for section1
<section1>

#comment for key1
key1 = value1
key2 = value5

<section2>

#comment for myKey1
mykey1 = value1
"; 
       
        [Test]
        public void parse_ini_string_with_custom_configuration()
        {
            var parser = new IniDataParser();
			
			

            parser.Scheme.CommentString = "#";
            parser.Scheme.SectionStartString = "<";
            parser.Scheme.SectionEndString = ">";
			
            IniData data = parser.Parse(iniFileStrCustom);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.Count, Is.EqualTo(2));
            var section1 = data.Sections.FindByName("section1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.Name, Is.EqualTo("section1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));

            Assert.That(section1.Properties, Is.Not.Null);
            Assert.That(section1.Properties.Count, Is.EqualTo(2));
            Assert.That(section1.Properties.FindByKey("key1"), Is.Not.Null);
            Assert.That(section1.Properties["key1"], Is.EqualTo("value1"));
            Assert.That(section1.Properties.FindByKey("key2"), Is.Not.Null);
            Assert.That(section1.Properties["key2"], Is.EqualTo("value5"));
        }

        [Test, Description("Test for Issue 3: http://code.google.com/p/ini-parser/issues/detail?id=3")]
        public void allow_keys_with_dots()
        {
            string strTest = "[section_issue.3]\nkey.with_dots = value\n";

            IniData data = new IniDataParser().Parse(strTest);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["section_issue.3"]["key.with_dots"], Is.Not.Null);
        }

        [Test, Description("Test for Issue 4: http://code.google.com/p/ini-parser/issues/detail?id=4")]
        public void allow_duplicated_keys_in_section()
        {
            string ini_duplicated_keys =
                @";comentario1
[seccion1]
;comentario 2

;valor de control
value1 = 10.6
value1 = 10";


            var parser = new IniDataParser();

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue;

            IniData data = parser.Parse(ini_duplicated_keys);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.FindByName("seccion1").Properties.Count, Is.EqualTo(1));
            Assert.That(data.Sections.FindByName("seccion1").Properties["value1"], Is.EqualTo("10.6"));
        }

        [Test, Description("Test for Issue 9: http://code.google.com/p/ini-parser/issues/detail?id=9")]
        public void check_using_another_leading_character_for_comments()
        {
            string data = 
                @"[test]
# a comment
connectionString = Server=sqlserver.domain.com;Database=main;User ID=user;Password=password";


            var parser = new IniDataParser();
            parser.Scheme.CommentString = "#";
            IniData iniData = parser.Parse(data);

            Assert.That(
                iniData["test"]["connectionString"],
                Is.EqualTo("Server=sqlserver.domain.com;Database=main;User ID=user;Password=password"));

            Assert.That(
                iniData["test"].FindByKey("connectionString").Comments[0], Is.EqualTo("a comment"));
        }

        [Test, Description("Test for Issue 10: http://code.google.com/p/ini-parser/issues/detail?id=10")]
        public void test_no_exception_is_raised_when_reading_url_like_section_names()
        {
            string data = 
                @"[http://example.com/page] 
key1 = value1";

            var newData = new IniDataParser().Parse(data);

            Assert.That(newData.Sections[@"http://example.com/page"]["key1"], Is.EqualTo("value1"));
        }

        [Test, Description("Test for Issue 14: http://code.google.com/p/ini-parser/issues/detail?id=14")]
        public void check_can_read_keys_with_no_section()
        {
            string data =
                @"key1=value1
key2=value2
key3=value3";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Global.Count, Is.EqualTo(3));
            Assert.That(iniData.Global["key1"], Is.EqualTo("value1"));
            Assert.That(iniData.Global["key2"], Is.EqualTo("value2"));
            Assert.That(iniData.Global["key3"], Is.EqualTo("value3"));
        }

        [Test, Description("Test for Issue 15: http://code.google.com/p/ini-parser/issues/detail?id=15")]
        public void allow_duplicated_sections_in_section()
        {
            string data = 
                @"[123_1]
key1=value1
key2=value2
[123_2]
key3 = value3
[123_1]
key4=value4
key2 = value5";

            var parser = new IniDataParser();

            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            var iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Contains("123_1"), Is.True);
            Assert.That(iniData.Sections.Contains("123_2"), Is.True);
            Assert.That(iniData.Sections.FindByName("123_1").Properties, Has.Count.EqualTo(3));
            Assert.That(iniData["123_1"]["key4"], Is.EqualTo("value4"));
            Assert.That(iniData["123_1"]["key2"], Is.EqualTo("value2"));

        }

        [Test, Description("Test for Issue 17: http://code.google.com/p/ini-parser/issues/detail?id=17")]
        public void check_can_parse_special_characters_in_section_names()
        {
            string data =
                @"[{E3729302-74D1-11D3-B43A-00AA00CAD128}]
key = value";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Count, Is.EqualTo(1));
            Assert.That(iniData.Sections.Contains("{E3729302-74D1-11D3-B43A-00AA00CAD128}"), Is.True);
            Assert.That(iniData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"].Contains("key"), Is.True);
            Assert.That(iniData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"]["key"], Is.EqualTo("value"));
        }

        [Test, Description("Test for Issue 197: http://code.google.com/p/ini-parser/issues/detail?id=19")]
        public void allow_whitespace_in_section_names()
        {
            string data =
                @"[Web Colaboration]
key = value";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Count, Is.EqualTo(1));
            Assert.That(iniData.Sections.Contains("Web Colaboration"), Is.True);
            Assert.That(iniData.Sections["Web Colaboration"].Contains("key"), Is.True);
            Assert.That(iniData.Sections["Web Colaboration"]["key"], Is.EqualTo("value"));
        }

        [Test]
        public void allow_skiping_unparsable_lines_disabled_by_default()
        {
            string data =
                @"win] 
key1 = value1";


            Assert.Throws(typeof(ParsingException), () => new IniDataParser().Parse(data));
        }

        [Test]
        public void allow_skiping_unparsable_lines()
        {
            string data =
                @"win] 
key1 = value1
 = value2";

            var parser = new IniDataParser();

            parser.Configuration.SkipInvalidLines = true;

            IniData newData = parser.Parse(data);

            Assert.That(newData.Global["key1"], Is.EqualTo("value1"));
        }

        [Test]
        public void allow_backslashes_in_sections()
        {
            string data =
                @"[section\subsection]
key=value";
            IniDataParser parser = new IniDataParser();

            IniData parsedData = parser.Parse(data);

            Assert.That(parsedData.Sections.Contains("section\\subsection"));
            Assert.That(parsedData.Sections["section\\subsection"]["key"], Is.EqualTo("value"));
        }

        [Test]
        public void allow_tilde_in_sections()
        {
            string data =
                @"[section~subsection]
key=value";
            IniDataParser parser = new IniDataParser();

            IniData parsedData = parser.Parse(data);

            Assert.That(parsedData.Sections.Contains("section~subsection"));
            Assert.That(parsedData.Sections["section~subsection"]["key"], Is.EqualTo("value"));
        }

        [Test, Description("Test for Issue 43 backward compatibility https://github.com/rickyah/ini-parser/issues/32")]
        public void commentchar_property_works()
        {
            string initest =
                @"#comment1
[seccion1] 
#comment 2

#control value
value1 = 10.6
value2 = 10";
            
            var parser = new IniDataParser();

            parser.Scheme.CommentString = "#";

            var result = parser.Parse(initest);
            Assert.That(result.Sections.FindByName("seccion1").Comments.Count > 0);

        }

        [Test,Description("Test for Issue 53 asterisks in section names https://github.com/rickyah/ini-parser/issues/53")]
        public void allow_asteriks_in_section_names()
        {
            #region really long ini string for the next issue
            string iniString = @"[SMSGW]
WebAuth=No
WebMenu=Yes
WebPort=8800
ReceiveSMS=Yes
ReceiveMMS=No
ReceiveSMSCommand1=*    http://192.168.1.3:8000/getnowsms/getsms.aspx?SENDER=@@SENDER@@&FULLSMS=@@FULLSMS@@&SMSPREFIX=@@SMSPREFIX@@&RECIP=@@RECEIP@@    No  13559710880
ReceiveSMSCommand2=*    http://192.168.1.5:8002/writedb.ashx?sender=@@SENDER@@&FULLSMS=@@FULLSMS@@&RECEIPTMESSAGEID=@@RECEIPTMESSAGEID@@&MessageID=@@MessageID@@&IP=192.168.1.3&SMSCROUTE=@@SMSCROUTE@@ No
ReceiveSMSCommand3=*    http://192.168.1.88:8139/getsms.aspx?SENDER=@@SENDER@@&FULLSMS=@@FULLSMS@@&SMSPREFIX=@@SMSPREFIX@@&RECIP=@@RECEIP@@ No  13712998000
ReceiveSMSCharset=utf-8
PHPEnable=No
PHPAllowRemote=No
Modem16=COM10:
Modem15=COM9:
Modem14=COM8:
Modem13=COM7:
Modem12=COM6:
Modem11=COM5:
Modem10=COM4:
Modem9=COM3:
Modem8=COM18:
Modem7=COM17:
Modem6=COM16:
Modem5=COM15:
Modem4=COM14:
Modem3=COM13:
Modem2=COM12:
Modem1=COM11:
Modem17=COM19:
Modem18=COM20:
Modem19=COM21:
Modem20=COM22:
Modem21=COM23:
Modem22=COM24:
Modem23=COM25:
Modem24=COM26:
Modem25=COM27:
Modem26=COM28:
Modem27=COM29:
Modem28=COM30:
Modem29=COM31:
Modem30=COM32:
Modem31=COM33:
Modem32=COM34:
Modem33=COM35:
Modem34=COM36:
Modem35=COM37:
Modem36=COM38:
Modem37=COM39:
Modem38=COM40:
Modem39=COM41:
Modem40=COM42:
Modem41=COM43:
Modem42=COM44:
Modem43=COM45:
Modem44=COM46:
Modem45=COM47:
Modem46=COM48:
Modem47=COM49:
Modem48=COM50:

[Modem - COM19:]
PhoneNumber=13712998000
ReceiveSMS=Yes
ReceiveMMS=No
RoutePrefOnly=Yes
[Modem - COM20:]
PhoneNumber=13559710880
ReceiveSMS=Yes
ReceiveMMS=No
RoutePrefOnly=Yes
[2Way-Keywords]
Keyword1=*##13559710880
Keyword2=*
Keyword3=*##13712998000
NewFormat=Yes
[2Way-Keyword-*##13559710880]
Run=http://192.168.1.5:9902/getsms.aspx?SENDER=@@SENDER@@&FULLSMS=@@FULLSMS@@&SMSPREFIX=@@SMSPREFIX@@&RECIP=@@RECEIP@@
[2Way-Keyword-*]
Run=http://192.168.1.5:8002/writedb.ashx?sender=@@SENDER@@&FULLSMS=@@FULLSMS@@&RECEIPTMESSAGEID=@@RECEIPTMESSAGEID@@&MessageID=@@MessageID@@&IP=192.168.1.3&SMSCROUTE=@@SMSCROUTE@@
[2Way-Keyword-*##13712998000]
Run=http://192.168.1.88:8139/getsms.aspx?SENDER=@@SENDER@@&FULLSMS=@@FULLSMS@@&SMSPREFIX=@@SMSPREFIX@@&RECIP=@@RECEIP@@
";
            #endregion
            var parser = new IniDataParser();

            var iniData = parser.Parse(iniString);

            Assert.That(iniData.Sections.Contains("2Way-Keyword-*##13559710880"));

        }

        // Thanks https://github.com/RichardSinden for this issue
        [Test,Description("Test for Issue 66 - allow comments at end of sectionless file")]
        public void allow_comments_at_end_of_sectionless_file()
        {
            string iniDataString = @"
                        ;begin

                        ;value
                        value1 = test

                        ;end
                        ";
            var parser = new IniDataParser();

            parser.Configuration.AllowKeysWithoutSection = true;
            var iniData = parser.Parse(iniDataString);

            Assert.That(iniData.Global.Contains("value1"));

            Assert.That(iniData.Global.FindByKey("value1").Comments, Has.Count.EqualTo(3));
            Assert.That(iniData.Global.FindByKey("value1").Comments[2], Is.EqualTo("end"));
        }

        // Thanks https://github.com/RichardSinden for this issue
        [Test ,Description("Test for Issue 67 - better errors")]
        public void provides_error_data()
        {
            string iniDataString = @";begin

                            ;value
                            value1 = test
                                = test2
                            value1 = test3
                            ";

            var parser = new IniDataParser();

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
            string iniDataString = @";begin

                            ;value
                            value1 = test
                                = test2
                            value1 = test3
                            ";

            var parser = new IniDataParser();

            parser.Configuration.ThrowExceptionsOnError = false;
            parser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.DisallowAndStopWithError;

            var result = parser.Parse(iniDataString);

            Assert.That(result, Is.Not.Null);
            Assert.That(parser.HasError, Is.True);
            Assert.That(parser.Errors, Has.Count.EqualTo(2));

        }

		[Test, Description("Test for Issue 88: https://github.com/rickyah/ini-parser/issues/88")]
		public void allow_quotes_in_sections()
		{
			var parser = new IniDataParser();

			var iniDataString = @"[W101 0.5"" wc]
key = value
[W103 0.5' wc]
key2 = value2";
			IniData parsedData = parser.Parse(iniDataString);

			Assert.That(parsedData.Sections["W101 0.5\" wc"], Is.Not.Empty);
			Assert.That(parsedData.Sections["W103 0.5' wc"], Is.Not.Empty);
		}

        [Test, Description("Allow leading spaces in property values")]
        public void can_preserve_leading_space_in_value()
        {
            var parser = new IniDataParser();
            parser.Configuration.TrimValues = false;
            parser.Scheme.PropertyAssigmentString = " = ";

            var iniDataString = @"[Section 1]
key1 =  value1
key2 = value2
key3 = value3 ";
            IniData parsedData = parser.Parse(iniDataString);

            Assert.That(parsedData.Sections["Section 1"]["key1"], Is.EqualTo(" value1"));
            Assert.That(parsedData.Sections["Section 1"]["key2"], Is.EqualTo("value2"));
            Assert.That(parsedData.Sections["Section 1"]["key3"], Is.EqualTo("value3 "));
        }

        [Test, Description("Allow leading and railing space in the assignment string. Useful for reading properties with trailing space and values with leading space")]
        public void allow_leading_and_trailing_space_in_assignment_string()
        {
            var parser = new IniDataParser();
            parser.Scheme.PropertyAssigmentString = " = ";
            parser.Configuration.TrimValues = false;
            parser.Configuration.TrimProperties = false;

            var iniDataString = @"[Section 1]
key1 = value1
key2  =  value2
key3 = value3";
            IniData parsedData = parser.Parse(iniDataString);

            Assert.That(parsedData.Sections["Section 1"]["key1"], Is.EqualTo("value1"));
            Assert.That(parsedData.Sections["Section 1"]["key2 "], Is.EqualTo(" value2"));
            Assert.That(parsedData.Sections["Section 1"]["key3"], Is.EqualTo("value3"));
            Assert.That(parsedData.Sections["Section 1"]["key2"], Is.Null);
        }
    }
}
