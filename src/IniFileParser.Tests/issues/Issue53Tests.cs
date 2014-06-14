using NUnit.Framework;
using IniParser.Parser;

namespace IniFileParser.Tests
{
    [TestFixture]
    public class Issue53Tests
    {
        [Test,Description("Test for Issue 53 asterisks in section names https://github.com/rickyah/ini-parser/issues/53")]
        public void allow_asteriks_in_section_names()
        {

            var parser = new IniDataParser();

            var iniData = parser.Parse(iniDataString);

            Assert.That(iniData.Sections.ContainsSection("2Way-Keyword-*##13559710880"));

        }

        const string iniDataString = @"[SMSGW]
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
    }
}

