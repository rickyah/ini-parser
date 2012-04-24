using System.Collections.Generic;
using IniParser.Model;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace IniFileParserTests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class SectionDataCollectionTests
    {
        [Test]
        public void check_section_data_operations()
        {
            string strSectionTest = "MySection";
            string strComment = "comment";
            List<string> commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });


            //Creation
            SectionDataCollection sdc = new SectionDataCollection();
            Assert.That(sdc, Is.Empty);

            //Add sectoin
            sdc.AddSection(strSectionTest);
            sdc.AddSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(1));


            //Check access
            Assert.That(sdc.GetSectionData(strSectionTest), Is.Not.Null);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments, Is.Empty);
            Assert.That(sdc.GetSectionData(strSectionTest).Keys.Count, Is.EqualTo(0));

            //Check add coments
            sdc.GetSectionData(strSectionTest).Comments.Add(strComment);
            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(1));
            sdc.GetSectionData(strSectionTest).Comments = commentListTest;

            Assert.That(sdc.GetSectionData(strSectionTest).Comments.Count, Is.EqualTo(commentListTest.Count));


            //Remove section
            sdc.RemoveSection("asdf");
            Assert.That(sdc.Count, Is.EqualTo(1));

            sdc.RemoveSection(strSectionTest);
            Assert.That(sdc.Count, Is.EqualTo(0));

            //Check access
            Assert.That(sdc[strSectionTest], Is.Null);
        }
    }
}
