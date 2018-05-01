using System.Linq;
using IniParser.Model;
using IniParser.Parser;
using NUnit.Framework;
using IniParser.Exceptions;
using IniParser.Model.Configuration;

namespace IniParser.Tests.Integration
{
    [TestFixture]
    public class ParserTests
    {

        [Test] public void parse_section()
        {
            var str = @";comment for section1
[section1]";
            var data = new IniDataParser().Parse(str);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Sections, Has.Count.EqualTo(1));
            Assert.That(data.Sections.ContainsSection("section1"), Is.True);

            var sectionData = data.Sections.GetSectionData("section1");
        }

        [Test] public void parse_property()
        {
            var str = @";comment for property
key1 = value1";
            var data = new IniDataParser().Parse(str);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.Global, Has.Count.EqualTo(1));
            Assert.That(data.Global.ContainsKey("key1"), Is.True);

            var propertyData = data.Global.GetKeyData("key1");
            Assert.That(propertyData.Value, Is.EqualTo("value1"));
        }
        
        [Test] public void parse_comments()
        {
            var str = @";comment for section1
[section1]";
            var data = new IniDataParser().Parse(str);
            Assert.That(data, Is.Not.Null);

            var sectionData = data.Sections.GetSectionData("section1");
            Assert.That(sectionData.Comments, Has.Count.EqualTo(1));
            Assert.That(sectionData.Comments.First(), Is.EqualTo("comment for section1"));
        }

        
        [Test]
        public void parse_ini_string_with_default_configuration()
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
            var parser = new IniDataParser();
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
            Assert.That(section1.Keys.GetKeyData("key1"), Is.Not.Null);
            Assert.That(section1.Keys["key1"], Is.EqualTo("value1"));
            Assert.That(section1.Keys.GetKeyData("key2"), Is.Not.Null);
            Assert.That(section1.Keys["key2"], Is.EqualTo("value5"));
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

        [Test] public void parse_ini_string_with_custom_configuration()
        {
            var parser = new IniDataParser();
            var config = parser.Configuration;

            parser.Scheme.CommentString = "#";
            parser.Scheme.SectionStartString = "<";
            parser.Scheme.SectionEndString = ">";

            IniData data = parser.Parse(iniFileStrCustom);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.Count, Is.EqualTo(2));
            var section1 = data.Sections.GetSectionData("section1");

            Assert.That(section1, Is.Not.Null);
            Assert.That(section1.SectionName, Is.EqualTo("section1"));
            Assert.That(section1.Comments, Is.Not.Empty);
            Assert.That(section1.Comments.Count, Is.EqualTo(1));

            Assert.That(section1.Keys, Is.Not.Null);
            Assert.That(section1.Keys.Count, Is.EqualTo(2));
            Assert.That(section1.Keys.GetKeyData("key1"), Is.Not.Null);
            Assert.That(section1.Keys["key1"], Is.EqualTo("value1"));
            Assert.That(section1.Keys.GetKeyData("key2"), Is.Not.Null);
            Assert.That(section1.Keys["key2"], Is.EqualTo("value5"));
        }

        [Test] public void allow_keys_with_dots()
        {
            string strTest = "[section_issue.3]\nkey.with_dots = value\n";

            IniData data = new IniDataParser().Parse(strTest);

            Assert.That(data.Sections.Count, Is.EqualTo(1));
            Assert.That(data.Sections["section_issue.3"]["key.with_dots"], Is.Not.Null);
        }

        [Test] public void allow_duplicated_keys_in_section()
        {
            string ini_duplicated_keys =
                @";comentario1
[seccion1]
;comentario 2

;valor de control
value1 = 10.6
value1 = 10";

            var parser = new IniDataParser();

            parser.Configuration.AllowDuplicateKeys = true;

            IniData data = parser.Parse(ini_duplicated_keys);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Sections.GetSectionData("seccion1").Keys.Count, Is.EqualTo(1));
            Assert.That(data.Sections.GetSectionData("seccion1").Keys["value1"], Is.EqualTo("10.6"));
        }

        [Test] public void check_using_another_leading_character_for_comments()
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
                iniData["test"].GetKeyData("connectionString").Comments[0], Is.EqualTo(" a comment"));
        }

        [Test] public void test_no_exception_is_raised_when_reading_url_like_section_names()
        {
            string data =
                @"[http://example.com/page] 
key1 = value1";

            var newData = new IniDataParser().Parse(data);

            Assert.That(newData.Sections[@"http://example.com/page"]["key1"], Is.EqualTo("value1"));
        }

        [Test]
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

        [Test] public void allow_duplicated_sections_in_section()
        {
            string data =
                @"[123_1]
key1=value1
key2=value2
[123_2]
key3 = value3
[123_1]
key4=value4";

            var parser = new IniDataParser();

            parser.Configuration.AllowDuplicateKeys = true;
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowKeysWithoutSection = true;

            var iniData = parser.Parse(data);

            Assert.That(iniData.Sections.ContainsSection("123_1"), Is.True);
            Assert.That(iniData.Sections.ContainsSection("123_2"), Is.True);
            Assert.That(iniData.Sections.GetSectionData("123_1").Keys, Has.Count.EqualTo(3));
            Assert.That(iniData["123_1"]["key4"], Is.EqualTo("value4"));

        }

        [Test]
        public void check_can_parse_special_characters_in_section_names()
        {
            string data =
                @"[{E3729302-74D1-11D3-B43A-00AA00CAD128}]
key = value";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Count, Is.EqualTo(1));
            Assert.That(iniData.Sections.ContainsSection("{E3729302-74D1-11D3-B43A-00AA00CAD128}"), Is.True);
            Assert.That(iniData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"].ContainsKey("key"), Is.True);
            Assert.That(iniData.Sections["{E3729302-74D1-11D3-B43A-00AA00CAD128}"]["key"], Is.EqualTo("value"));
        }

        [Test]
        public void allow_whitespace_in_section_names()
        {
            string data =
                @"[Web Colaboration]
key = value";

            var parser = new IniDataParser();

            IniData iniData = parser.Parse(data);

            Assert.That(iniData.Sections.Count, Is.EqualTo(1));
            Assert.That(iniData.Sections.ContainsSection("Web Colaboration"), Is.True);
            Assert.That(iniData.Sections["Web Colaboration"].ContainsKey("key"), Is.True);
            Assert.That(iniData.Sections["Web Colaboration"]["key"], Is.EqualTo("value"));
        }

        [Test]
        public void allow_skiping_unparsable_lines_disabled_by_default()
        {
            string data =
                @"win] 
key1 = value1";

            Assert.Throws<ParsingException>( () => new IniDataParser().Parse(data));
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

            Assert.That(parsedData.Sections.ContainsSection("section\\subsection"));
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

            Assert.That(parsedData.Sections.ContainsSection("section~subsection"));
            Assert.That(parsedData.Sections["section~subsection"]["key"], Is.EqualTo("value"));
        }

        [Test] public void commentchar_property_works()
        {
            string initest =
                @"#comment1
[seccion1] 
#comment 2

#control value
value1 = 10.6
value2 = 10";

            var parser = new IniDataParser();

            //Remove CommentChar property
            parser.Scheme.CommentString = "#";

            var result = parser.Parse(initest);
            Assert.That(result.Sections.GetSectionData("seccion1").Comments.Count > 0);

        }

        [Test]
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

            Assert.That(iniData.Sections.ContainsSection("2Way-Keyword-*##13559710880"));

        }

        [Test]
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
            parser.Scheme.CommentString = ";";
            var iniData = parser.Parse(iniDataString);

            Assert.That(iniData.Global.ContainsKey("value1"));

            Assert.That(iniData.Global.GetKeyData("value1").Comments, Has.Count.EqualTo(3));
            Assert.That(iniData.Global.GetKeyData("value1").Comments[2], Is.EqualTo("end"));
        }

        [Test]
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
            catch (ParsingException ex)
            {
                Assert.That(ex.LineNumber, Is.EqualTo(5));
                Assert.That(parser.HasError, Is.True);
                Assert.That(parser.Errors, Has.Count.EqualTo(1));
            }

        }

        [Test]
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
            parser.Configuration.AllowDuplicateKeys = false;

            var result = parser.Parse(iniDataString);

            Assert.That(result, Is.Null);
            Assert.That(parser.HasError, Is.True);
            Assert.That(parser.Errors, Has.Count.EqualTo(2));

        }

        [Test]
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
        
        
        [Test]
        public void check_can_parse_empty_file()
        {
            var parser = new IniDataParser();
         
            IniData parsedData = parser.Parse("");

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test]
        public void check_parsing_simple_file_succeeds()
        {
        string strGoodINIFile = @";comentario1
;comentario 2
[seccion1]

;valor de control
value1 = 10.6";

            var parser = new IniDataParser();
 
            IniData parsedData = parser.Parse(strGoodINIFile);

            Assert.That(parsedData, Is.Not.Null);
        }

        [Test]
        public void check_parsing_badly_formed_file_fails()
        {
            string strBadINIFile = @"asdfasf [seccion1] fdsafsd
   value2  =   jelou

[seccion 2]    adsfa
;
value3 = que tal estas

  [ [section   3] dsf a

 ; comentario1
 fsadfsad  ;comentario2";

             var parser = new IniDataParser();
            Assert.Throws<ParsingException>( () => parser.Parse(strBadINIFile) );
        }


        [Test]
        public void check_colliding_section_names_throws()
        {
            string strBadSectionINI = @";comentario1
[seccion1] ;comentario 2

;valor de control
value1 = 10.6

[seccion1]
value2 = 10.6";
            var parser = new IniDataParser();
            Assert.Throws<ParsingException>( () => parser.Parse(strBadSectionINI) );
        }

        [Test]
        public void check_colliding_property_names_throws()
        {
string strBadPropertiesINIFile = @";comentario1
[seccion1] ;comentario 2

;valor de control
value1 = 10.6
value1 = 10";

            var parser = new IniDataParser();
            Assert.Throws<ParsingException>( () => parser.Parse(strBadPropertiesINIFile) );

        }

    }
}
